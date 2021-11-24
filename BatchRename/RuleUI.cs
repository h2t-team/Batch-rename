using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchRename
{
    public abstract class RuleUI
    {
        public string Display { set; get; }
    }

    public class AddPrefixRuleUI : RuleUI
    {
        public AddPrefixRuleUI(string prefix)
        {
            Display = $"Add Prefix: {prefix}";
        }
    }
    public class AddSuffixRuleUI : RuleUI
    {
        public AddSuffixRuleUI(string suffix)
        {
            Display = $"Add Suffix: {suffix}";
        }
    }
    public class AllLowerRuleUI : RuleUI
    {
        public AllLowerRuleUI()
        {
            Display = "All Lower Case";
        }
    }
    public class AllUpperRuleUI : RuleUI
    {
        public AllUpperRuleUI()
        {
            Display = "All Upper Case";
        }
    }
    public class ChangeExtRuleUI : RuleUI
    {
        public ChangeExtRuleUI(string ext)
        {
            Display = $"Change Extension: {ext}";
        }
    }
    public class PascalCaseRuleUI : RuleUI
    {
        public PascalCaseRuleUI()
        {
            Display = "Pascal Case";
        }
    }
    public class ReplaceRuleUI : RuleUI
    {
        public ReplaceRuleUI(List<string> needles, string replacer)
        {
            for(int i =0; i < needles.Count(); i++)
                needles[i] = $"\"{needles[i]}\"";
            Display = $"Replace: [{string.Join(", ",needles)}] => \"{replacer}\"";
        }
    }
    public class TrimRuleUI : RuleUI
    {
        public TrimRuleUI()
        {
            Display = "Trim";
        }
    }
}
