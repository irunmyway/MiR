using System.Collections.Generic;

namespace Foundation
{
    public interface ILoadSaveManager
    {
        ObserverList<IOnLoadSaveSlotsChanged> OnLoadSaveSlotsChanged { get; }

        List<SaveSlot> GetSlots();

        bool LoadAsync(SaveSlot slot);

        bool Delete(SaveSlot slot);

        bool SaveNew(string name);
        bool SaveOverwrite(SaveSlot slot, string newName = null);
    }
}
