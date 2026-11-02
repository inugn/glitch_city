using UnityEngine;

namespace _Project.Scripts.Core.Save
{
    public sealed class MetaProgressPersistence : IGameService
    {
        private readonly MetaProgress meta;
        private readonly IProgressStorage storage;
        private readonly EventBus bus;

        public MetaProgressPersistence(MetaProgress meta, IProgressStorage storage, EventBus bus)
        {
            this.meta = meta;
            this.storage = storage;
            this.bus = bus;

            if (bus != null)
                bus.RunEnded += SaveIfDirty;
        }

        public void Load()
        {
            /*
            if (storage.TryLoadMeta(out var data))
            {
                meta.ApplySnapshot(data);
                Debug.Log("[Save] MetaProgress loaded.");
            }
            else
            {
                Debug.Log("[Save] No save found. Using defaults.");
            }
            */
        }

        public void SaveIfDirty()
        {
            /*
            if (meta == null || storage == null) return;
            if (!meta.IsDirty) return;

            var data = meta.CreateSnapshot();
            if (storage.TrySaveMeta(data))
            {
                meta.ClearDirty();
                Debug.Log("[Save] MetaProgress saved.");
            }
            */
        }
    }
}