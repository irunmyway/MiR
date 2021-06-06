using System;
using System.IO;
using UnityEngine;

namespace Foundation
{
    [RequireComponent(typeof(GuidComponent))]
    public sealed class Saveable : MonoBehaviour
    {
        const string TransformComponentID = "<T>";
        const string RigidBodyComponentID = "<RB>";
        const string EndID = "<>";

        public GuidComponent GuidComponent { get; private set; }
        public String FactoryId;

        void Awake()
        {
            GuidComponent = GetComponent<GuidComponent>();
        }

        public bool Load(uint formatVersion, BinaryReader reader)
        {
            for (;;) {
                string componentID = reader.ReadString();
                switch (componentID) {
                    case EndID:
                        return true;

                    case TransformComponentID: {
                        // position
                        float x = reader.ReadSingle();
                        float y = reader.ReadSingle();
                        float z = reader.ReadSingle();
                        transform.localPosition = new Vector3(x, y, z);
                        // rotation
                        x = reader.ReadSingle();
                        y = reader.ReadSingle();
                        z = reader.ReadSingle();
                        float w = reader.ReadSingle();
                        transform.localRotation = new Quaternion(x, y, z, w);
                        // scale
                        x = reader.ReadSingle();
                        y = reader.ReadSingle();
                        z = reader.ReadSingle();
                        transform.localScale = new Vector3(x, y, z);
                        break;
                    }

                    case RigidBodyComponentID: {
                        var rigidBody = GetComponent<Rigidbody>();
                        // velocity
                        float x = reader.ReadSingle();
                        float y = reader.ReadSingle();
                        float z = reader.ReadSingle();
                        rigidBody.velocity = new Vector3(x, y, z);
                        break;
                    }

                    default: {
                        Type type = Type.GetType(componentID);
                        if (type == null) {
                            DebugOnly.Error($"Unknown type \"{componentID}\"");
                            return false;
                        } else if (!typeof(ISaveableComponent).IsAssignableFrom(type)) {
                            DebugOnly.Error($"Type \"{componentID}\" does not implement ISaveableComponent.");
                            return false;
                        } else {
                            ISaveableComponent c = (ISaveableComponent)GetComponent(type);
                            if (c == null) {
                                DebugOnly.Error($"No component \"{componentID}\".");
                                return false;
                            }
                            if (!c.Load(formatVersion, reader))
                                return false;
                        }
                        break;
                    }
                }
            }
        }

        public bool Save(BinaryWriter writer)
        {
            foreach (var component in GetComponents<Component>()) {
                switch (component) {
                    case Transform t:
                        writer.Write(TransformComponentID);
                        // position
                        writer.Write(t.localPosition.x);
                        writer.Write(t.localPosition.y);
                        writer.Write(t.localPosition.z);
                        // rotation
                        writer.Write(t.localRotation.x);
                        writer.Write(t.localRotation.y);
                        writer.Write(t.localRotation.z);
                        writer.Write(t.localRotation.w);
                        // scale
                        writer.Write(t.localScale.x);
                        writer.Write(t.localScale.y);
                        writer.Write(t.localScale.z);
                        break;

                    case Rigidbody rb:
                        writer.Write(RigidBodyComponentID);
                        // velocity
                        writer.Write(rb.velocity.x);
                        writer.Write(rb.velocity.y);
                        writer.Write(rb.velocity.z);
                        break;

                    case ISaveableComponent c:
                        writer.Write(component.GetType().AssemblyQualifiedName);
                        if (!c.Save(writer))
                            return false;
                        break;
                }
            }

            writer.Write(EndID);
            return true;
        }
    }
}
