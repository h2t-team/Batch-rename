using RenameLib;
using System;

namespace TrimRule
{
    public class TrimRule : IRenameRule
    {
        public string Rename(string src)
        {
            return src.Trim();
        }
    }
}
