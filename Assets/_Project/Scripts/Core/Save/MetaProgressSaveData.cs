using System;
using System.Collections.Generic;

namespace _Project.Scripts.Core.Save
{
    [Serializable]
    public sealed class MetaProgressSaveData
    {
        public int SchemaVersion = 1;

        public int PlayerLevel = 1;
        public int CurrentXp = 0;

        public List<string> UnlockedWeapons = new List<string>(16);

        public long LastUpdatedUnixMs;
    }
}