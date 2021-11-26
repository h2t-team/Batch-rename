namespace RenameLib
{
    public class TrimRule : IRenameRule
    {
        public string Rename(string src)
        {
            return src.Trim();
        }
    }
}
