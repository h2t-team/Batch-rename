using RenameLib;
using System;

namespace AllUpperRule
{
    public class AllUpperRule : IRenameRule
    {
        public string Rename(string src)
        {
            return src.ToUpper();
        }
    }
}
