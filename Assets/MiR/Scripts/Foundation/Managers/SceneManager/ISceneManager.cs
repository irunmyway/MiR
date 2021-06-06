using System;

namespace Foundation
{
    public interface ISceneManager
    {
        ObserverList<IOnBeginSceneLoad> OnBeginSceneLoad { get; }
        ObserverList<IOnCurrentSceneUnload> OnCurrentSceneUnload { get; }
        ObserverList<IOnSceneLoadProgress> OnSceneLoadProgress { get; }
        ObserverList<IOnEndSceneLoad> OnEndSceneLoad { get; }

        void LoadSceneAsync(string sceneName, Action preinitScene = null);
        void LoadScenesAsync(string[] sceneNames, Action<int> preinitScene = null, Action afterLoad = null);
    }
}
