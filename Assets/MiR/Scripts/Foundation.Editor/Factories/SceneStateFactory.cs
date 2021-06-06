using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.Events;
using System;
using System.Collections.Generic;
using Zenject;

namespace Foundation.Editor
{
    public static class SceneStateFactory
    {
        [MenuItem("GameObject/OTUS/Scene State", false, 10)]
        static void CreateSceneState(MenuCommand menuCommand)
        {
            var go = new GameObject("SceneState");
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);

            var state = go.AddComponent<SceneState>();

            var context = go.AddComponent<GameObjectContext>();
            context.Installers = new List<MonoInstaller> { state };

            Undo.RegisterCreatedObjectUndo(go, "Create scene state");
        }

        [MenuItem("GameObject/OTUS/UI State", false, 10)]
        static void CreateUIState(MenuCommand menuCommand)
        {
            var go = new GameObject("UIState");
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);

            var state = go.AddComponent<SceneState>();

            var canvasGO = new GameObject("Canvas");
            canvasGO.transform.SetParent(go.transform, false);

            var canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.sortingLayerName = "UI";
            canvas.planeDistance = 0;

            var scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.referenceResolution = new Vector2(1280, 720);
            scaler.matchWidthOrHeight = 1.0f;

            var raycaster = canvasGO.AddComponent<GraphicRaycaster>();

            var canvasGroup = canvasGO.AddComponent<CanvasGroup>();

            var context = go.AddComponent<GameObjectContext>();
            context.Installers = new List<MonoInstaller> { state };

            var camera = go.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.Depth;
            camera.orthographic = true;
            camera.orthographicSize = 1;
            camera.nearClipPlane = -0.01f;
            camera.farClipPlane = 0.01f;
            camera.useOcclusionCulling = false;
            camera.depth = 0;

            canvas.worldCamera = camera;

            var controller = canvasGO.AddComponent<CanvasController>();
            controller.Canvas = canvas;
            controller.CanvasGroup = canvasGroup;
            controller.EditorCamera = camera;

            Undo.RegisterCreatedObjectUndo(go, "Create UI state");
        }
    }
}
