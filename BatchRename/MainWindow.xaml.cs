using Microsoft.Win32;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Collections.Generic;
using RenameLib;
using System.Linq;
using System.Windows.Controls;

using BatchRename.View;
using BatchRename.DataTypes;

namespace BatchRename
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BindingList<FileUI> files = new BindingList<FileUI>();
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
        private void addFileListView(string[] fileArr)
        {
            foreach (string file in fileArr)
                files.Add(new FileUI()
                {
                    Name = Path.GetFileName(file),
                    Path = Path.GetDirectoryName(file),
                    Preview = Path.GetFileName(file),
                    Status = ""
                });
        }
        private void Add_Button_Click(object sender, RoutedEventArgs e)
        {
            if (file.IsChecked == true)
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Multiselect = true;
                if (dialog.ShowDialog() == true)
                {
                    addFileListView(dialog.FileNames);
                }
            }
        }
        private void Preview_Button_Click(object sender, RoutedEventArgs e)
        {
            for(int i = 0; i < files.Count; i++)
            {
                files[i].Preview = files[i].Name;
                foreach (var rule in rules)
                {
                    files[i].Preview = rule.Rename(files[i].Preview);
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
            RuleUI selected = presets.ElementAt(index);
            switch (selected.TYPE)
            {
                case "Trim":
                    presetComboBox.Items.Add(new ComboBoxItem() { Content="Trim" });
                    break;
                case "AllLower":
                case "AllUpper":
                case "PascalCase":
                    presetComboBox.Items.Add(new ComboBoxItem() { Content = "New Case" });
                    break;
                case "ChangeExtension":
                    presetComboBox.Items.Add(new ComboBoxItem() { Content = "Change Extension" });
                    break;
            }
        }
        private void Update_Preset_Click(object sender, RoutedEventArgs e)
        {
            int index = presetList.SelectedIndex;
            if (index == -1)
                return;
            RuleUI selected = presets.ElementAt(index);
            switch (selected.TYPE)
            {
                case "AddPrefix":
                case "AddSuffix":
                    UpdateAddWindow addDialog = new UpdateAddWindow(selected);
                    if (addDialog.ShowDialog() == true)
                    {
                        switch (selected.TYPE)
                        {
                            case "AddPrefix":
                                ((AddPrefixRuleUI)selected).Prefix = addDialog.Word;                                
                                ((AddPrefixRule)rules[index]).Prefix = addDialog.Word;
                                break;
                            case "AddSuffix":
                                ((AddSuffixRuleUI)selected).Suffix = addDialog.Word;
                                ((AddSuffixRuleUI)rules[index]).Suffix = addDialog.Word;
                                break;
                        }
                        selected.Update();
                    }
                    break;
                case "ChangeExtension":
                    UpdateExtWindow extDialog = new UpdateExtWindow(selected);
                    if (extDialog.ShowDialog() == true)
                    {
                        ((ChangeExtRuleUI)selected).Ext = extDialog.Ext;
                        selected.Update();
                        ((ChangeExtensionRule)rules[index]).Extension = extDialog.Ext;
                    }
                    break;
                case "AllUpper":
                case "AllLower":
                case "PascalCase":
                    UpdateCaseWindow caseDialog = new UpdateCaseWindow(selected);
                    if (caseDialog.ShowDialog() == true)
                    {
                        if (selected.TYPE == caseDialog.RuleName)
                            return;
                        rules.RemoveAt(index);
                        presets.RemoveAt(index);
                        switch (caseDialog.RuleName)
                        {
                            case "AllUpper":
                                presets.Insert(index, new AllUpperRuleUI());
                                rules.Insert(index, new AllUpperRule());
                                break;
                            case "AllLower":
                                presets.Insert(index, new AllLowerRuleUI());
                                rules.Insert(index, new AllLowerRule());
                                break;   
                            case "PascalCase":
                                presets.Insert(index, new PascalCaseRuleUI());
                                rules.Insert(index, new PascalCaseRule());
                                break;
                        }
                    }
                    break;
                case "Replace":
                    UpdateReplaceWindow replaceDialog = new UpdateReplaceWindow(selected);
                    if (replaceDialog.ShowDialog() == true)
                    {
                        ((ReplaceRuleUI)selected).Needles = new List<string>(replaceDialog.Needles);
                        selected.Update();
                        ((ReplaceRule)rules[index]).Needles = new List<string>(replaceDialog.Needles);
                    }
                    break;
            }
        }

        private void Add_Preset_Click(object sender, RoutedEventArgs e)
        {
            string option = presetComboBox.Text;
            switch (option)
            {
                case "Add":
                    AddWindow addDialog = new AddWindow();
                    if(addDialog.ShowDialog() == true)
                    {
                        switch (addDialog.RuleName)
                        {
                            case "Add Prefix":
                                rules.Add(new AddPrefixRule(addDialog.Word));
                                presets.Add(new AddPrefixRuleUI(addDialog.Word));
                                break;
                            case "Add Suffix":
                                rules.Add(new AddSuffixRule(addDialog.Word));
                                presets.Add(new AddSuffixRuleUI(addDialog.Word));
                                break;
                        }
                    }
                    break;
                case "New Case":
                    CaseWindow caseDialog = new CaseWindow();
                    if (caseDialog.ShowDialog() == true)
                    {
                        presetComboBox.Items.Remove(presetComboBox.SelectedItem);
                        switch (caseDialog.RuleName)
                        {
                            case "All Upper Case":
                                rules.Add(new AllUpperRule());
                                presets.Add(new AllUpperRuleUI());
                                break;
                            case "All Lower Case":
                                rules.Add(new AllLowerRule());
                                presets.Add(new AllLowerRuleUI());
                                break;
                            case "Pascal Case":
                                rules.Add(new PascalCaseRule());
                                presets.Add(new PascalCaseRuleUI());
                                break;
                        }
                    }
                    break;
                case "Trim":
                    presetComboBox.Items.Remove(presetComboBox.SelectedItem);
                    rules.Add(new TrimRule());
                    presets.Add(new TrimRuleUI());
                    break;
                case "Change Extension":
                    ExtensionWindow extDialog = new ExtensionWindow();
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
                        presets.Add(new ReplaceRuleUI(replaceDialog.Needles, replaceDialog.Replacer));
                        rules.Add(new ReplaceRule(replaceDialog.Needles, replaceDialog.Replacer));
                    }
                    break;
            }
        }


        private void ListView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ListView listView = sender as ListView;
            GridView gView = listView.View as GridView;

            var workingWidth = listView.ActualWidth - SystemParameters.VerticalScrollBarWidth; 
            var col1 = 0.3;
            var col2 = 0.3;
            var col3 = 0.2;
            var col4 = 0.2;

            gView.Columns[0].Width = workingWidth * col1;
            gView.Columns[1].Width = workingWidth * col2;
            gView.Columns[2].Width = workingWidth * col3;
            gView.Columns[3].Width = workingWidth * col4;
        }

        private void HandleFileDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] fileArr = (string[]) e.Data.GetData(DataFormats.FileDrop);
                addFileListView(fileArr);
            }
        }

        private void handleCardSize(object sender, SizeChangedEventArgs e)
        {
            fileList.Height = fileCard.ActualHeight - fileOptions.ActualHeight;
        }
    }
}
