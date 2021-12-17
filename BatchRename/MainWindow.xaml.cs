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
using Ookii.Dialogs.Wpf;
using System.Diagnostics;

namespace BatchRename
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<string> newNames = new();
        BindingList<FileUI> files = new BindingList<FileUI>(); // file rename list
        BindingList<FileUI> folders = new BindingList<FileUI>(); // folder rename list
        BindingList<RuleUI> presets = new BindingList<RuleUI>();
        List<IRenameRule> rules = new List<IRenameRule>();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoadRuleFromUI()
        {
            rules.Clear();
            newNames.Clear();
            //Load rule dynamically
            foreach(var preset in presets)
            {
                string exePath = Assembly.GetExecutingAssembly().Location;
                string folder = Path.GetDirectoryName(exePath);
                FileInfo info = new DirectoryInfo(folder).GetFiles($"DLL/{preset.TYPE}Rule.dll")[0];
                Assembly assembly = Assembly.LoadFile(info.FullName);
                var type = assembly.GetTypes()[0];
                if (type.IsClass && typeof(IRenameRule).IsAssignableFrom(type))
                {
                    switch (preset.TYPE)
                    {
                        case "AddCounter":
                            rules.Add(Activator.CreateInstance(type, new object[] {
                                ((AddCounterRuleUI)preset).Start,
                                ((AddCounterRuleUI)preset).Step,
                                ((AddCounterRuleUI)preset).Digit
                            }) as IRenameRule);
                            break;
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
            //Load file name to list
            var arr = files;
            if (file.IsChecked == true)
                arr = files;
            else if (folder.IsChecked == true)
                arr = folders;
            foreach(var item in arr)
            {
                newNames.Add(item.Name);
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            fileList.ItemsSource = files;
            presetList.ItemsSource = presets;
        }
        private void File_Active(object sender, RoutedEventArgs e)
        {
            if (fileList != null)
                fileList.ItemsSource = files;
        }
        private void Folder_Active(object sender, RoutedEventArgs e)
        {
            if (fileList != null)
                fileList.ItemsSource = folders;
        }
        private void addFileListView(string[] fileArr)
        {
            var arr = files;
            if (file.IsChecked == true)
                arr = files;
            else if (folder.IsChecked == true)
                arr = folders;
            bool check;
            foreach (string item in fileArr)
            {
                Debug.WriteLine(File.Exists(item));
                check = true;
                if ((file.IsChecked == true && File.Exists(item) == false) || (folder.IsChecked == true && Directory.Exists(item) == false))
                    check = false;
                else
                {
                    //Handling duplication
                    foreach (FileUI existedItem in arr)
                    {
                        if (Path.GetFileName(item) == existedItem.Name && Path.GetDirectoryName(item) == existedItem.Path)
                        {
                            check = false;
                            break;
                        }
                    }
                }
                if (!check) continue;
                arr.Add(new FileUI()
                {
                    Name = Path.GetFileName(item),
                    Path = Path.GetDirectoryName(item),
                    Preview = "",
                    Status = ""
                });
            }
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
            else if (folder.IsChecked == true)
            {
                VistaFolderBrowserDialog dialog = new VistaFolderBrowserDialog();
                dialog.Multiselect = true;
                if (dialog.ShowDialog() == true)
                {
                    addFileListView(dialog.SelectedPaths);
                }
            }
        }
        private void Preview_Button_Click(object sender, RoutedEventArgs e)
        {
            LoadRuleFromUI();
            var arr = files;
            if (file.IsChecked == true)
                arr = files;
            else if (folder.IsChecked == true)
                arr = folders;
            foreach(var rule in rules)
                newNames = rule.Rename(newNames);
            for (int i = 0; i < arr.Count; i++) 
                arr[i].Preview = newNames[i];
            arr.ResetBindings();
        }
        private void Batch_Button_Click(object sender, RoutedEventArgs e)
        {
            LoadRuleFromUI();
            var arr = files;
            if (file.IsChecked == true)
                arr = files;
            else if (folder.IsChecked == true)
                arr = folders;
            if (rules.Count == 0 || arr.Count == 0) 
                return;
            foreach (var rule in rules)
                newNames = rule.Rename(newNames);
            for (int i = 0; i < arr.Count; i++)
            {              
                try
                {
                    if(file.IsChecked == true)
                        File.Move($"{arr[i].Path}\\{arr[i].Name}", $"{arr[i].Path}\\{newNames[i]}");
                    else if (folder.IsChecked == true)
                        Directory.Move($"{arr[i].Path}\\{arr[i].Name}", $"{arr[i].Path}\\{newNames[i]}");
                    arr[i].Status = "Success";
                    arr[i].Name = newNames[i];
                }
                catch (Exception)
                {
                    arr[i].Status = "Failed";
                }
            }
            arr.ResetBindings();
        }
        private void Delete_Preset_Click(object sender, RoutedEventArgs e)
        {
            int index = presetList.SelectedIndex;
            if (index == -1)
                return;
            RuleUI selected = presets.ElementAt(index);
            switch (selected.TYPE)
            {
                case "AddCounter":
                    presetComboBox.Items.Add(new ComboBoxItem() { Content = "Add Counter" });
                    break;
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
                case "AddCounter":
                    UpdateCounterWindow counterDialog = new UpdateCounterWindow(selected);
                    if(counterDialog.ShowDialog()== true)
                    {
                        ((AddCounterRuleUI)selected).Start = counterDialog.Start;
                        ((AddCounterRuleUI)selected).Step = counterDialog.Step;
                        ((AddCounterRuleUI)selected).Digit = counterDialog.Digit;
                    }
                    break;
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
                    }
                    break;
                case "ChangeExtension":
                    UpdateExtWindow extDialog = new UpdateExtWindow(selected);
                    if (extDialog.ShowDialog() == true)
                    {
                        ((ChangeExtRuleUI)selected).Ext = extDialog.Ext;
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
                    }
                    break;
            }
            selected.Update();
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
                case "Add Counter":
                    CounterWindow counterDialog = new CounterWindow();
                    if(counterDialog.ShowDialog() == true)
                    {
                        presetComboBox.Items.Remove(presetComboBox.SelectedItem);
                        presets.Add(new AddCounterRuleUI(
                            counterDialog.Start,
                            counterDialog.Step,
                            counterDialog.Digit));
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
        //Drag and drop files to the list
        private void HandleFileDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] fileArr = (string[])e.Data.GetData(DataFormats.FileDrop);
                addFileListView(fileArr);
            }
        }

        private void handleCardSize(object sender, SizeChangedEventArgs e)
        {
            fileList.Height = fileCard.ActualHeight - fileOptions.ActualHeight;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            double width = Main.Width;
            double height = Main.Height;
        }
    }
}
