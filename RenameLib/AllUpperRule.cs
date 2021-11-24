namespace RenameLib
{
    public class AllUpperRule : IRenameRule
    {
        public string Rename(string src)
        {
            return src.ToUpper();
        }
    }
}
