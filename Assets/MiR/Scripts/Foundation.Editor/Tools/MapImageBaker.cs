using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Foundation.Editor
{
    public class MapImageBaker
    {
        [MenuItem("OTUS/Bake map image")]
        static void Bake()
        {
            var cameras = Object.FindObjectsOfType<Camera>();
            foreach (var camera in cameras) {
                if (camera.name == "FullMapCamera") {
                    /*
                    var currentRT = RenderTexture.active;
                    RenderTexture.active = camera.targetTexture;

                    camera.Render();

                    Texture2D image = new Texture2D(camera.targetTexture.width, camera.targetTexture.height);
                    image.ReadPixels(new Rect(0, 0, camera.targetTexture.width, camera.targetTexture.height), 0, 0);
                    image.Apply();

                    RenderTexture.active = currentRT;

                    byte[] pngData = image.EncodeToPNG();
                    File.WriteAllBytes(Application.dataPath + "/FullMapImage.png", image.EncodeToPNG());
                    */

                    Vector3[] corners = new Vector3[4];
                    //camera.CalculateFrustumCorners(new Rect(0, 0, 1, 1), camera.farClipPlane, Camera.MonoOrStereoscopicEye.Mono, corners);

                    foreach (var corner in corners) {
                        var go = new GameObject("G");
                        go.transform.position = camera.transform.TransformVector(corner);
                    }
                }
            }
        }
    }
}
