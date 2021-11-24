using RenameLib;
using System;

namespace PascalCaseRule
{
    public class PascalCaseRule : IRenameRule
    {
        public string Rename(string src)
        {
            string newName = char.ToUpper(src[0]) + src.Substring(1);
            char[] tokens = { ' ', '-', '_' };
            foreach (char token in tokens)
            {
                int index = newName.IndexOf(token);
                while (index != -1)
                {
                    newName = newName.Substring(0, index) + char.ToUpper(newName[index + 1]) + newName.Substring(index + 2);
                    index = newName.IndexOf(token);
                }
            }
            return newName;
        }
    }
}
