using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchRename.DataTypes
{
    public abstract class RuleUI : INotifyPropertyChanged
    { 
        public string TYPE { get; set; }
        public string Display { set; get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public abstract void Update();
    }

    public class AddPrefixRuleUI : RuleUI
    {
        public string Prefix { get; set; }
        public AddPrefixRuleUI(string prefix)
        {
            TYPE = "AddPrefix";
            Prefix = prefix;
            Update();
        }

        public override void Update()
        {
            Display = $"Add Prefix: {Prefix}";
        }
    }
    public class AddSuffixRuleUI : RuleUI
    {
        public string Suffix { get; set; }
        public AddSuffixRuleUI(string suffix)
        {
            TYPE = "AddSuffix";
            Suffix = suffix;
            Update();
        }

        public override void Update()
        {
            Display = $"Add Suffix: {Suffix}";
        }
    }
    public class AllLowerRuleUI : RuleUI
    {
        public AllLowerRuleUI()
        {
            TYPE = "AllLower";
            Update();
        }

        public override void Update()
        {
            Display = "All Lower Case";
        }
    }
    public class AllUpperRuleUI : RuleUI
    {
        public AllUpperRuleUI()
        {
            TYPE = "AllUpper";
            Update();
        }

        public override void Update()
        {
            Display = "All Upper Case";
        }
    }
    public class ChangeExtRuleUI : RuleUI
    {
        public string Ext { set; get; }
        public ChangeExtRuleUI(string ext)
        {
            TYPE = "ChangeExtension";
            Ext = ext;
            Update();
        }

        public override void Update()
        {
            Display = $"Change Extension: {Ext}";
        }
    }
    public class PascalCaseRuleUI : RuleUI
    {
        public PascalCaseRuleUI()
        {
            TYPE = "PascalCase";
            Update();
        }

        public override void Update()
        {
             Display = "Pascal Case";
        }
    }
    public class ReplaceRuleUI : RuleUI
    {
        public List<string> Needles { set; get; } 
        public string Replacer { set; get; }
        public ReplaceRuleUI(List<string> needles, string replacer)
        {
            TYPE = "Replace";
            Needles = new List<string>(needles);
            Replacer = replacer;
            Update();
        }

        public override void Update()
        {
            Display = $"Replace: [\"{Needles[0]}\"";
            for (int i = 1; i < Needles.Count(); i++)
                Display += $", \"{Needles[i]}\"";
            Display += $"] => \"{Replacer}\"";
        }
    }
    public class TrimRuleUI : RuleUI
    {
        public TrimRuleUI()
        {
            TYPE = "Trim";
            Update();
        }

        public override void Update()
        {
            Display = "Trim";
        }
    }
}
