using System;
using System.IO;
using UnityEngine;

namespace _Project.Scripts.Core.Save
{
    public sealed class JsonFileProgressStorage : IProgressStorage
    {
        private const string FileName = "meta_progress.json";

        private static string GetPath()
            => Path.Combine(Application.persistentDataPath, FileName);

        public bool TryLoadMeta(out MetaProgressSaveData data)
        {
            data = null;

            var path = GetPath();
            if (!File.Exists(path))
                return false;

            try
            {
                var json = File.ReadAllText(path);
                if (string.IsNullOrWhiteSpace(json))
                    return false;

                data = JsonUtility.FromJson<MetaProgressSaveData>(json);
                return data != null;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[Save] Failed to load meta progress. Path='{path}'. Error: {e.Message}");
                TryBackupCorrupted(path);
                return false;
            }
        }

        public bool TrySaveMeta(MetaProgressSaveData data)
        {
            if (data == null) return false;

            var path = GetPath();
            var tmpPath = path + ".tmp";

            try
            {
                var dir = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                var json = JsonUtility.ToJson(data, prettyPrint: true);

                File.WriteAllText(tmpPath, json);

                // Atomic-ish replace
                if (File.Exists(path))
                    File.Delete(path);

                File.Move(tmpPath, path);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[Save] Failed to save meta progress. Path='{path}'. Error: {e.Message}");

                try
                {
                    if (File.Exists(tmpPath))
                        File.Delete(tmpPath);
                }
                catch { /* ignore */ }

                return false;
            }
        }

        private static void TryBackupCorrupted(string path)
        {
            try
            {
                var bad = path + ".bad";
                if (File.Exists(bad))
                    File.Delete(bad);
                File.Move(path, bad);
            }
            catch { /* ignore */ }
        }
    }
}
