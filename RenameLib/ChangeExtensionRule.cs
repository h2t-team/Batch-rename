namespace RenameLib
{
    public class ChangeExtensionRule : IRenameRule
    {
        public string Extension { set; get; }
        public ChangeExtensionRule(string ext)
        {
            Extension = ext;
        }
        public string Rename(string src)
        {
            int lastindex = src.LastIndexOf('.');
            string newName = src.Substring(0, lastindex + 1) + Extension;
            return newName;
        }
    }
}
