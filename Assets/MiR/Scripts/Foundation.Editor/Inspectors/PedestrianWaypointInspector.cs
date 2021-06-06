using UnityEngine;
using UnityEditor;
using System;
using System.Text.RegularExpressions;

namespace Foundation.Editor
{
    [CustomEditor(typeof(PedestrianWaypoint))]
    [CanEditMultipleObjects]
    public sealed class PedestrianWaypointInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Set Next")) {
                foreach (var obj in targets) {
                    String nextName;
                    var waypoint = (PedestrianWaypoint)obj;
                    if (waypoint.name == "Waypoint")
                        nextName = "Waypoint (1)";
                    else {
                        int nextNumber = int.Parse(Regex.Match(waypoint.name, @"\d+").Value) + 1;
                        nextName = $"Waypoint ({nextNumber})";
                    }

                    foreach (var child in waypoint.transform.parent.GetComponentsInChildren<PedestrianWaypoint>()) {
                        if (child.name == nextName) {
                            Undo.RecordObject(waypoint, "Set next waypoint");
                            waypoint.Next = child;
                            PrefabUtility.RecordPrefabInstancePropertyModifications(target);
                            EditorUtility.SetDirty(target);
                            break;
                        }
                    }
                }
            }
        }
    }
}
