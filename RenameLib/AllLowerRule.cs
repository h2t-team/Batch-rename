namespace RenameLib
{
    public class AllLowerRule : IRenameRule
    {
        public string Rename(string src)
        {
            return src.ToLower();
        }
    }
}
