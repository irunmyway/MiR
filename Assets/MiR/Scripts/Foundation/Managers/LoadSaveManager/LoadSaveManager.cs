using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Foundation
{
    public sealed class LoadSaveManager : AbstractService<ILoadSaveManager>, ILoadSaveManager
    {
        const string FileExtension = ".sav";

        const uint SaveFileID = 0x1A564153;     // SAV\x1A
        const uint FormatVersion = 1;

        const byte EndOfObjectList = 0;
        const byte ActiveObject = 1;
        const byte InactiveObject = 2;

        public ObserverList<IOnLoadSaveSlotsChanged> OnLoadSaveSlotsChanged { get; } = new ObserverList<IOnLoadSaveSlotsChanged>();

        [Inject] DiContainer container = default;
        [Inject] ISceneManager sceneManager = default;

        string GetSavesPath()
        {
            return Path.Combine(Application.persistentDataPath, "Saves");
        }

        public List<SaveSlot> GetSlots()
        {
            var result = new List<SaveSlot>();

            var directoryPath = GetSavesPath();
            if (!Directory.Exists(directoryPath))
                return result;

            var files = Directory.GetFiles(directoryPath);
            foreach (var file in files) {
                if (!file.EndsWith(FileExtension))
                    continue;

                string filePath = Path.Combine(directoryPath, file);
                try {
                    using (var reader = new BinaryReader(File.OpenRead(filePath))) {
                        if (reader.ReadUInt32() != SaveFileID)
                            continue;

                        string name = reader.ReadString();
                        result.Add(new SaveSlot(filePath, name));
                    }
                } catch (IOException e) {
                    Debug.LogException(e);
                }
            }

            return result;
        }

        public bool Delete(SaveSlot slot)
        {
            try {
                File.Delete(slot.File);
            } catch (IOException e) {
                Debug.LogException(e);
                return false;
            }

            foreach (var it in OnLoadSaveSlotsChanged.Enumerate())
                it.Do();

            return true;
        }

        public bool SaveNew(string name)
        {
            var directoryPath = GetSavesPath();

            string fileName = Path.Combine(directoryPath, $"{name}{FileExtension}");
            if (!File.Exists(fileName))
                return Save(fileName, name);

            for (int counter = 1; ; ++counter) {
                fileName = Path.Combine(directoryPath, $"{name} ({counter}){FileExtension}");
                if (!File.Exists(fileName))
                    return Save(fileName, name);
            }
        }

        public bool SaveOverwrite(SaveSlot slot, string newName = null)
        {
            return Save(slot.File, (newName != null ? newName : slot.Name));
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Сохранение

        bool Save(BinaryWriter writer, GameObject obj)
        {
            if (obj.TryGetComponent<Saveable>(out var saveable)) {
                writer.Write((byte)(obj.activeSelf ? ActiveObject : InactiveObject));
                writer.Write(saveable.FactoryId);
                writer.Write(saveable.GuidComponent.GetGuid().ToString());
                if (!saveable.Save(writer))
                    return false;
            }

            var t = obj.transform;
            int n = t.childCount;
            for (int i = 0; i < n; i++) {
                var child = t.GetChild(i);
                if (!Save(writer, child.gameObject))
                    return false;
            }

            return true;
        }

        bool Save(string fileName, string saveName)
        {
            var memoryStream = new MemoryStream();
            var writer = new BinaryWriter(memoryStream);

            writer.Write(SaveFileID);
            writer.Write(saveName);
            writer.Write(FormatVersion);

            int sceneCount = UnityEngine.SceneManagement.SceneManager.sceneCount;
            writer.Write(sceneCount);

            for (int i = 0; i < sceneCount; i++) {
                Scene scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
                writer.Write(scene.name);
            }

            for (int i = 0; i < sceneCount; i++) {
                Scene scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
                var objects = scene.GetRootGameObjects();
                foreach (var obj in objects) {
                    if (!Save(writer, obj))
                        return false;
                }
                writer.Write(EndOfObjectList);
            }

            try {
                Directory.CreateDirectory(Path.GetDirectoryName(fileName));
                using (var file = File.OpenWrite(fileName))
                    memoryStream.WriteTo(file);
            } catch (IOException e) {
                Debug.LogException(e);
                return false;
            }

            foreach (var it in OnLoadSaveSlotsChanged.Enumerate())
                it.Do();

            return true;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Загрузка

        public bool LoadAsync(SaveSlot slot)
        {
            uint formatVersion;

            try {
                string[] sceneNames;

                var reader = new BinaryReader(File.OpenRead(slot.File));
                int sceneCount;

                try {
                    if (reader.ReadUInt32() != SaveFileID) {
                        DebugOnly.Error("Invalid save file ID.");
                        return false;
                    }

                    reader.ReadString(); // skip name
                    formatVersion = reader.ReadUInt32();
                    sceneCount = reader.ReadInt32();

                    sceneNames = new string[sceneCount];
                    for (int i = 0; i < sceneCount; i++)
                        sceneNames[i] = reader.ReadString();
                } catch (Exception e) {
                    reader.Dispose();
                    Debug.LogException(e);
                    return false;
                }

                sceneManager.LoadScenesAsync(sceneNames, null, () => {
                        try {
                            for (int i = 0; i < sceneCount; i++) {
                                for (;;) {
                                    byte objectState = reader.ReadByte();
                                    if (objectState == EndOfObjectList)
                                        break;

                                    if (objectState != ActiveObject && objectState != InactiveObject)
                                        DebugOnly.Error($"Invalid object state.");
                                    else {
                                        string factoryId = reader.ReadString();
                                        Guid guid = new Guid(reader.ReadString());

                                        var go = GuidManager.ResolveGuid(guid);
                                        if (go == null) {
                                            if (string.IsNullOrEmpty(factoryId))
                                                DebugOnly.Error($"Object with guid \"{guid}\" not found.");
                                            else {
                                                // FIXME
                                                var factory = (IRawFactory)container.ResolveId(typeof(IRawFactory), factoryId);
                                                if (factory == null)
                                                    DebugOnly.Error($"Can't find factory \"{factoryId}\" for object \"{guid}\".");
                                                else
                                                    go = ((Component)factory.CreateRaw()).gameObject;
                                            }
                                        }

                                        if (go != null) {
                                            go.SetActive(objectState == ActiveObject);

                                            var saveable = go.GetComponent<Saveable>();
                                            if (saveable == null)
                                                DebugOnly.Error($"Object with guid \"{guid}\" does not have Saveable component.");
                                            else
                                                saveable.Load(formatVersion, reader);
                                        }
                                    }
                                }
                            }
                        } finally {
                            reader.Dispose();
                        }
                    });

                return true;
            } catch (IOException e) {
                Debug.LogException(e);
                return false;
            }
        }
    }
}
