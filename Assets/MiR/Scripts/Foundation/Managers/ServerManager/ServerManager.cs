using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Foundation
{
    public sealed class ServerManager : AbstractService<ServerManager>
    {
        public string ServerHost;
        public string SessionToken;

        public ObserverList<IOnServerError> OnServerError { get; } = new ObserverList<IOnServerError>();

        public override void Start()
        {
            string userId = PlayerPrefs.GetString("UserID");
            string secret = PlayerPrefs.GetString("UserSecret");

            if (string.IsNullOrEmpty(userId)) {
                userId = Guid.NewGuid().ToString();

                StringBuilder builder = new StringBuilder();
                string symbols = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ[]{}-=_+|/<>.,!@#$%^&*()";
                for (int i = 0; i < 64; i++) {
                    char ch = symbols[UnityEngine.Random.Range(0, symbols.Length)];
                    builder.Append(ch);
                }

                secret = builder.ToString();

                PlayerPrefs.SetString("UserID", userId);
                PlayerPrefs.SetString("UserSecret", secret);
            }

            var req = new AuthenticationRequest();
            req.UserID = userId;
            req.Secret = secret;
            SendRequest("game", req, (AuthenticationResponse response) => {
                    if (response != null) {
                        DebugOnly.Message($"Response: {response.name}");
                        SessionToken = response.token;
                    }
                });
        }

        public void SendRequest<REQ, RES>(string controller, REQ request, Action<RES> callback)
            where REQ : class
            where RES : class
        {
            StartCoroutine(RequestCoroutine<REQ, RES>(ServerHost + controller, request, callback));
        }

        IEnumerator RequestCoroutine<REQ, RES>(string url, REQ request, Action<RES> callback)
            where REQ : class
            where RES : class
        {
            for (;;) {
                string json = JsonUtility.ToJson(request);
                DebugOnly.Message(json);

                UnityWebRequest web = UnityWebRequest.Put(url, json);
                web.SetRequestHeader("Content-Type", "application/json");
                yield return web.SendWebRequest();

                if (web.result == UnityWebRequest.Result.Success) {
                    string jsonResponse = Encoding.UTF8.GetString(web.downloadHandler.data);
                    DebugOnly.Message(jsonResponse);
                    callback?.Invoke(JsonUtility.FromJson<RES>(jsonResponse));
                    yield break;
                }

                DebugOnly.Error(web.error);

                List<Task<bool>> tasks = new List<Task<bool>>();
                foreach (var it in OnServerError.Enumerate())
                    tasks.Add(it.ShouldRetry());

                for (;;) {
                    bool done = true;
                    foreach (var task in tasks) {
                        if (!task.IsCompleted) {
                            done = false;
                            break;
                        }
                    }

                    if (done)
                        break;

                    yield return null;
                }

                bool shouldRetry = false;
                foreach (var task in tasks)
                    shouldRetry = task.Result || shouldRetry;

                if (!shouldRetry) {
                    callback?.Invoke(null);
                    yield break;
                }
            }
        }
    }
}
