namespace RenameLib
{
    public class AddPrefixRule : IRenameRule
    {
        public string Prefix { set; get; }
        public AddPrefixRule(string prefix)
        {
            Prefix = prefix;
        }
        public string Rename(string src)
        {
            return Prefix + src;
        }
    }
}
