using RenameLib;
using System;

namespace AddSuffixRule
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
