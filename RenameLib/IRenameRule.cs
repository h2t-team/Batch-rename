using System;

namespace RenameLib
{
    public interface IRenameRule
    {
        string Rename(string src);
    }
}
