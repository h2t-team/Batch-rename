using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchRename.DataTypes
{
    public abstract class RuleUI
    { 
        public string TYPE { get; set; }
        public string Display { set; get; }
    }

    public class AddPrefixRuleUI : RuleUI
    {
        public AddPrefixRuleUI(string prefix)
        {
            TYPE = "Add Prefix";
            Display = $"Add Prefix: {prefix}";
        }
    }
    public class AddSuffixRuleUI : RuleUI
    {
        public AddSuffixRuleUI(string suffix)
        {
            TYPE = "AddSuffix";
            Display = $"Add Suffix: {suffix}";
        }
    }
    public class AllLowerRuleUI : RuleUI
    {
        public AllLowerRuleUI()
        {
            TYPE = "AllLower";
            Display = "All Lower Case";
        }
    }
    public class AllUpperRuleUI : RuleUI
    {
        public AllUpperRuleUI()
        {
            TYPE = "AllUpper";
            Display = "All Upper Case";
        }
    }
    public class ChangeExtRuleUI : RuleUI
    {
        public ChangeExtRuleUI(string ext)
        {
            TYPE = "ChangeExtension";
            Display = $"Change Extension: {ext}";
        }
    }
    public class PascalCaseRuleUI : RuleUI
    {
        public PascalCaseRuleUI()
        {
            TYPE = "PascalCase";
            Display = "Pascal Case";
        }
    }
    public class ReplaceRuleUI : RuleUI
    {
        public ReplaceRuleUI(List<string> needles, string replacer)
        {
            TYPE = "Replace";
            for (int i =0; i < needles.Count(); i++)
                needles[i] = $"\"{needles[i]}\"";
            Display = $"Replace: [{string.Join(", ",needles)}] => \"{replacer}\"";
        }
    }
    public class TrimRuleUI : RuleUI
    {
        public TrimRuleUI()
        {
            TYPE = "Trim";
            Display = "Trim";
        }
    }
}
