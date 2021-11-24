using RenameLib;
using System;

namespace AllLowerRule
{
    public class AllLowerRule : IRenameRule
    {
        public string Rename(string src)
        {
            return src.ToLower();
        }
    }
}
