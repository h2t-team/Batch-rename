using RenameLib;
using System;
using System.Collections.Generic;

namespace ReplaceRule
{
    public class ReplaceRule : IRenameRule
    {
        public List<string> Needles { get; set; }
        public string Replacer { get; set; }
        public ReplaceRule(List<string> needles, string replacer)
        {
            this.Needles = needles;
            this.Replacer = replacer;
        }
        public string Rename(string src)
        {
            string newName = src;

            foreach (var needle in Needles)
            {
                newName = newName.Replace(needle, Replacer);
            }

            return newName;
        }
    }
}
