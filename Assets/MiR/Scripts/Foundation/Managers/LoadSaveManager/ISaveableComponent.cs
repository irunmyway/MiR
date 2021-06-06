using System.IO;

namespace Foundation
{
    public interface ISaveableComponent
    {
        bool Load(uint formatVersion, BinaryReader reader);
        bool Save(BinaryWriter writer);
    }
}
