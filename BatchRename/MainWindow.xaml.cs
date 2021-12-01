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
using System;
using System.Reflection;

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
        private void LoadRuleFromUI()
        {
            rules.Clear();
            foreach(var preset in presets)
            {
                string exePath = Assembly.GetExecutingAssembly().Location;
                string folder = Path.GetDirectoryName(exePath);
                FileInfo info = new DirectoryInfo(folder).GetFiles($"{preset.TYPE}Rule.dll")[0];
                Assembly assembly = Assembly.LoadFile(info.FullName);
                var type = assembly.GetTypes()[0];
                if (type.IsClass && typeof(IRenameRule).IsAssignableFrom(type))
                {
                    switch (preset.TYPE)
                    {                        
                        case "AddPrefix":
                            rules.Add(Activator.CreateInstance(type, new object[] { 
                                ((AddPrefixRuleUI)preset).Prefix 
                            }) as IRenameRule);
                            break;
                        case "AddSuffix":
                            rules.Add(Activator.CreateInstance(type, new object[] {
                                ((AddSuffixRuleUI)preset).Suffix
                            }) as IRenameRule);
                            break;
                        case "ChangeExtension":
                            rules.Add(Activator.CreateInstance(type, new object[] {
                                ((ChangeExtRuleUI)preset).Ext
                            }) as IRenameRule);
                            break;
                        case "Replace":
                            rules.Add(Activator.CreateInstance(type, new object[] {
                                ((ReplaceRuleUI)preset).Needles, 
                                ((ReplaceRuleUI)preset).Replacer
                            }) as IRenameRule);
                            break;
                        case "AllUpper":
                        case "AllLower":
                        case "PascalCase":
                        case "Trim":
                            rules.Add(Activator.CreateInstance(type) as IRenameRule);
                            break;
                    }   
                }
            }
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
            LoadRuleFromUI();
            foreach(var file in files)
            {
                file.Preview = file.Name;
                foreach (var rule in rules)
                {
                    file.Preview = rule.Rename(file.Preview);
                }
            }
        }
        private void Batch_Button_Click(object sender, RoutedEventArgs e)
        {
            LoadRuleFromUI();
            if (rules.Count == 0 || files.Count == 0) 
                return;
            foreach(var file in files)
            {
                string newName = file.Name;
                foreach(var rule in rules)
                {
                    newName = rule.Rename(newName);
                }
                try
                {
                    File.Move($"{file.Path}\\{file.Name}", $"{file.Path}\\{newName}");
                    file.Status = "Success";
                    file.Name = newName;
                }
                catch (Exception)
                {
                    file.Status = "Failed";
                }
            }
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
            rules.RemoveAt(index);
            presets.RemoveAt(index);
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
                                break;
                            case "AddSuffix":
                                ((AddSuffixRuleUI)selected).Suffix = addDialog.Word;
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
                                break;
                            case "AllLower":
                                presets.Insert(index, new AllLowerRuleUI());
                                break;   
                            case "PascalCase":
                                presets.Insert(index, new PascalCaseRuleUI());
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
                                presets.Add(new AddPrefixRuleUI(addDialog.Word));
                                break;
                            case "Add Suffix":
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
                                presets.Add(new AllUpperRuleUI());
                                break;
                            case "All Lower Case":
                                presets.Add(new AllLowerRuleUI());
                                break;
                            case "Pascal Case":
                                presets.Add(new PascalCaseRuleUI());
                                break;
                        }
                    }
                    break;
                case "Trim":
                    presetComboBox.Items.Remove(presetComboBox.SelectedItem);
                    presets.Add(new TrimRuleUI());
                    break;
                case "Change Extension":
                    ExtensionWindow extDialog = new ExtensionWindow();
                    if (extDialog.ShowDialog() == true)
                    {
                        presetComboBox.Items.Remove(presetComboBox.SelectedItem);
                        presets.Add(new ChangeExtRuleUI(extDialog.Ext));
                    }
                    break;
                case "Replace":
                    ReplaceWindow replaceDialog = new ReplaceWindow();
                    if (replaceDialog.ShowDialog() == true)
                    {
                        presets.Add(new ReplaceRuleUI(replaceDialog.Needles, replaceDialog.Replacer));
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

        private void Save_Preset_Button_Click(object sender, RoutedEventArgs e)
        {
            // Displays a SaveFileDialog so the user can save the current preset.
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text Documents|*.txt";
            saveFileDialog.Title = "Save the current preset";
            saveFileDialog.ShowDialog();
            // If the file name is not an empty string open it for saving.
            if (saveFileDialog.FileName != "")
            {
                //write all rules in textout
                string textout = "";
                foreach (var rule in presets)
                {
                    textout = textout + rule.Display + Environment.NewLine;
                }
                File.WriteAllText(saveFileDialog.FileName, textout);
            }
        }
    }
}
