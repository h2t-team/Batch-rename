using System;
using System.Collections.Generic;

namespace RenameLib
{
    public interface IRenameRule
    {
        List<string> Rename(List<string> src);
    }
}
