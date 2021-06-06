using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Foundation
{
    public sealed class SceneManager : AbstractService<ISceneManager>, ISceneManager
    {
        public ObserverList<IOnBeginSceneLoad> OnBeginSceneLoad { get; } = new ObserverList<IOnBeginSceneLoad>();
        public ObserverList<IOnCurrentSceneUnload> OnCurrentSceneUnload { get; } = new ObserverList<IOnCurrentSceneUnload>();
        public ObserverList<IOnSceneLoadProgress> OnSceneLoadProgress { get; } = new ObserverList<IOnSceneLoadProgress>();
        public ObserverList<IOnEndSceneLoad> OnEndSceneLoad { get; } = new ObserverList<IOnEndSceneLoad>();

        public void LoadSceneAsync(string sceneName, Action preinitScene = null)
        {
            StartCoroutine(LoadSceneCoroutine(sceneName, preinitScene));
        }

        IEnumerator LoadSceneCoroutine(string sceneName, Action preinitScene)
        {
            foreach (var observer in OnBeginSceneLoad.Enumerate()) {
                var task = observer.Do();
                yield return new WaitUntil(() => task.IsCompleted);
            }

            foreach (var observer in OnCurrentSceneUnload.Enumerate())
                observer.Do();

            var operation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
            operation.allowSceneActivation = false;
            while (!operation.isDone) {
                yield return null;
                foreach (var observer in OnSceneLoadProgress.Enumerate())
                    observer.Do(operation.progress);

                if (operation.progress >= 0.9f && !operation.allowSceneActivation) {
                    operation.allowSceneActivation = true;
                    preinitScene?.Invoke();
                }
            }

            foreach (var observer in OnEndSceneLoad.Enumerate()) {
                var task = observer.Do();
                yield return new WaitUntil(() => task.IsCompleted);
            }
        }

        public void LoadScenesAsync(string[] sceneNames, Action<int> preinitScene = null, Action afterLoad = null)
        {
            StartCoroutine(LoadScenesCoroutine(sceneNames, preinitScene, afterLoad));
        }

        IEnumerator LoadScenesCoroutine(string[] sceneNames, Action<int> preinitScene, Action afterLoad)
        {
            foreach (var observer in OnBeginSceneLoad.Enumerate()) {
                var task = observer.Do();
                yield return new WaitUntil(() => task.IsCompleted);
            }

            foreach (var observer in OnCurrentSceneUnload.Enumerate())
                observer.Do();

            for (int sceneIndex = 0; sceneIndex < sceneNames.Length; sceneIndex++) {
                var operation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneNames[sceneIndex],
                    (sceneIndex == 0 ? LoadSceneMode.Single : LoadSceneMode.Additive));
                operation.allowSceneActivation = false;
                while (!operation.isDone) {
                    yield return null;
                    foreach (var observer in OnSceneLoadProgress.Enumerate())
                        observer.Do((sceneIndex + operation.progress) / sceneNames.Length);

                    if (operation.progress >= 0.9f && !operation.allowSceneActivation) {
                        operation.allowSceneActivation = true;
                        preinitScene?.Invoke(sceneIndex);
                    }
                }
            }

            afterLoad?.Invoke();

            foreach (var observer in OnEndSceneLoad.Enumerate()) {
                var task = observer.Do();
                yield return new WaitUntil(() => task.IsCompleted);
            }
        }
    }
}
