namespace RenameLib
{
    public class AddSuffixRule : IRenameRule
    {
        public string Suffix { set; get; }
        public AddSuffixRule(string suffix)
        {
            Suffix = suffix;
        }
        public string Rename(string src)
        {
            return src + Suffix;
        }
    }
}
