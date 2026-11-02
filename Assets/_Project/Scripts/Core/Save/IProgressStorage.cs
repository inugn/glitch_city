namespace _Project.Scripts.Core.Save
{
    public interface IProgressStorage
    {
        bool TryLoadMeta(out MetaProgressSaveData data);
        bool TrySaveMeta(MetaProgressSaveData data);
    }
}