namespace Foundation
{
    public sealed class SaveSlot
    {
        public readonly string Name;
        internal readonly string File;

        internal SaveSlot(string file, string name)
        {
            File = file;
            Name = name;
        }
    }
}
