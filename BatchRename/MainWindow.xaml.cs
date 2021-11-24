using Microsoft.Win32;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Collections.Generic;
using RenameLib;
using System.Linq;

namespace BatchRename
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BindingList<File> files = new BindingList<File>();
        BindingList<RuleUI> presets = new BindingList<RuleUI>();
        List<IRenameRule> rules = new List<IRenameRule>();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            fileList.ItemsSource = files;
            presetList.ItemsSource = presets;
        }
        private void Add_Button_Click(object sender, RoutedEventArgs e)
        {
            if (file.IsChecked == true)
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Multiselect = true;
                if (dialog.ShowDialog() == true)
                {
                    foreach (string file in dialog.FileNames)
                        files.Add(new File() { 
                            Name = Path.GetFileName(file), 
                            Path = Path.GetDirectoryName(file), 
                            Preview = Path.GetFileName(file) , 
                            Status=""});
                }
            }
        }

        private void Batch_Button_Click(object sender, RoutedEventArgs e)
        {
            
        }
        private void Delete_Preset_Click(object sender, RoutedEventArgs e)
        {
            int index = presetList.SelectedIndex;
            if (index == -1)
                return;
            rules.RemoveAt(index);
            presets.RemoveAt(index);
        }

        private void Add_Preset_Click(object sender, RoutedEventArgs e)
        {
            string option = presetComboBox.Text;
            switch (option)
            {
                case "Add":
                    AddRules addDialog = new AddRules();
                    if(addDialog.ShowDialog() == true)
                    {
                        if (addDialog.RuleName.Equals("Add Prefix"))
                        {
                            rules.Add(new AddPrefixRule(addDialog.Word));
                            presets.Add(new AddPrefixRuleUI(addDialog.Word));
                        }
                        else
                        {
                            rules.Add(new AddSuffixRule(addDialog.Word));
                            presets.Add(new AddSuffixRuleUI(addDialog.Word));
                        }
                    }
                    break;
                case "New Case":
                    NewCaseRule caseDialog = new NewCaseRule();
                    if (caseDialog.ShowDialog() == true)
                    {
                        presetComboBox.Items.Remove(presetComboBox.SelectedItem);
                        if (caseDialog.RuleName.Equals("All Upper Case"))
                        {
                            rules.Add(new AllUpperRule());
                            presets.Add(new AllUpperRuleUI());
                        }
                        else if(caseDialog.RuleName.Equals("All Lower Case"))
                        {
                            rules.Add(new AllLowerRule());
                            presets.Add(new AllLowerRuleUI());
                        }
                        else
                        {
                            rules.Add(new PascalCaseRule());
                            presets.Add(new PascalCaseRuleUI());
                        }
                    }
                    break;
                case "Trim":
                    presetComboBox.Items.Remove(presetComboBox.SelectedItem);
                    rules.Add(new TrimRule());
                    presets.Add(new TrimRuleUI());
                    break;
                case "Change Extension":
                    ChangeExtRule extDialog = new ChangeExtRule();
                    if (extDialog.ShowDialog() == true)
                    {
                        presetComboBox.Items.Remove(presetComboBox.SelectedItem);
                        rules.Add(new ChangeExtensionRule(extDialog.Ext));
                        presets.Add(new ChangeExtRuleUI(extDialog.Ext));
                    }
                    break;
                case "Replace":
                    ReplaceWindow replaceDialog = new ReplaceWindow();
                    if (replaceDialog.ShowDialog() == true)
                    {
                        presets.Add(new ReplaceRuleUI(replaceDialog.wordBinding.ToList<string>(), replaceDialog.Replacer));
                        rules.Add(new ReplaceRule(replaceDialog.wordBinding.ToList<string>(), replaceDialog.Replacer));
                    }
                    break;
            }
        }

        private void Update_Preset_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
