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
using System.Windows.Data;
using System.Globalization;
using System.Windows.Threading;
using System.Collections.Specialized;

namespace BatchRename
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        List<string> newNames = new();
        BindingList<FileUI> files = new BindingList<FileUI>(); // file rename list
        BindingList<FileUI> folders = new BindingList<FileUI>(); // folder rename list
        BindingList<RuleUI> presets = new BindingList<RuleUI>();
        List<IRenameRule> rules = new List<IRenameRule>();
        List<string> actions = new List<string>
        {
            "Replace",
            "Add",
            "Add Counter",
            "Change Extension",
            "New Case",
            "Trim",
        };
        BindingList<string> actionsUI = new BindingList<string>();
        string stateFile = "LastTimeState.bin";  //last time state filename
        string autoSaveFile = "AutoSaveFile.bin"; // autosave filename
        private string currentPreset;
        public string CurrentPreset { 
            get 
            {
                return currentPreset;
            } 
            set 
            {
                currentPreset = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CurrentPreset"));
            } 
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindow()
        {
            InitializeComponent();

            // add listview event 
            ((INotifyCollectionChanged)fileList.Items).CollectionChanged += AutoSave_Conditions;
            ((INotifyCollectionChanged)presetList.Items).CollectionChanged += AutoSave_Conditions;
            DataContext = this;
        }

        private void LoadRuleFromUI()
        {
            rules.Clear();
            newNames.Clear();
            //Load rule dynamically
            foreach (var preset in presets)
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
            foreach (var item in arr)
            {
                newNames.Add(item.Name);
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            fileList.ItemsSource = files;
            presetList.ItemsSource = presets;
            actionsUI = new BindingList<string>(actions.ToList());
            presetComboBox.ItemsSource = actionsUI;

            // load the last time state 
            if (File.Exists(stateFile))
            {
                string[] filelines = File.ReadAllLines(stateFile);
                loadState(filelines);
            }

            // load the working condition
            if (File.Exists(autoSaveFile))
            {
                string[] filelines = File.ReadAllLines(autoSaveFile);
                loadWorkingCondition(filelines);
            }
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
        private void loadState(string[] filelines)
        {
            string[] tokens;
            string line, type;

            // load the size of the screen
            type = filelines[0].Substring(0, filelines[0].IndexOf(':'));
            if (type == "Size")
            {
                line = filelines[0].Substring(filelines[0].IndexOf(':') + 2);
                tokens = line.Split(new string[] { " " }, StringSplitOptions.None);
                Main.Width = double.Parse(tokens[0]);
                Main.Height = double.Parse(tokens[1]);
            }

            // load the location of the screen
            type = filelines[1].Substring(0, filelines[1].IndexOf(':'));
            if (type == "Location")
            {
                line = filelines[1].Substring(filelines[1].IndexOf(':') + 2);
                tokens = line.Split(new string[] { " " }, StringSplitOptions.None);
                Main.Top = double.Parse(tokens[0]);
                Main.Left = double.Parse(tokens[1]);
            }

            // load the last chosen preset
            type = filelines[2].Substring(0, filelines[2].IndexOf(':'));
            if (type == "Preset")
            {
                line = filelines[2].Substring(filelines[2].IndexOf(':') + 2);
                CurrentPreset = line;
            }
        }
        private void loadWorkingCondition(string[] filelines)
        {
            int preIndex = 0;
            int index;

            //load the file list
            index = Array.IndexOf(filelines, "~", preIndex);
            string[] fileArr = filelines.ToList().GetRange(preIndex, index - preIndex).ToArray();
            foreach (var line in fileArr)
            {
                string[] tokens = line.Split(new string[] { "|" }, StringSplitOptions.None); ;
                files.Add(new FileUI()
                {
                    Name = tokens[0],
                    Path = tokens[1],
                    Preview = "",
                    Status = ""
                });
            }

            // load the folder list
            preIndex = index + 1;
            index = Array.IndexOf(filelines, "~", preIndex);
            string[] folderArr = filelines.ToList().GetRange(preIndex, index - preIndex).ToArray();
            foreach (var line in folderArr)
            {
                string[] tokens = line.Split(new string[] { "|" }, StringSplitOptions.None);
                folders.Add(new FileUI()
                {
                    Name = tokens[0],
                    Path = tokens[1],
                    Preview = "",
                    Status = ""
                });
            }

            // load the rules
            preIndex = index + 1;
            index = Array.IndexOf(filelines, "~", preIndex);
            string[] presetArr = filelines.ToList().GetRange(preIndex, index - preIndex).ToArray();
            loadPreset(presetArr);
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
        private void Open_Preset_Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true)
            {
                ShowAllActions();
                presets.Clear();
                string filename = dialog.FileName;
                string[] filelines = File.ReadAllLines(filename);
                loadPreset(filelines);
                Debug.WriteLine(dialog.FileName);
                FileInfo fi = new FileInfo(dialog.FileName);
                CurrentPreset = fi.Name;
            }
        }
        //When open another preset => all actions are shown again 
        private void ShowAllActions()
        {
            actionsUI = new BindingList<string>(actions.ToList());
            presetComboBox.ItemsSource = actionsUI;
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

                FileInfo fi = new FileInfo(saveFileDialog.FileName);
                CurrentPreset = fi.Name;
            }

        }
        private void Clear_Preset_Button_Click(object sender, RoutedEventArgs e)
        {
            foreach (RuleUI item in presets)
            {
                Return_Action(item);
            }
            presets.Clear();
        }
        private void Clear_Files_Button_Click(object sender, RoutedEventArgs e)
        {
            var arr = files;
            if (file.IsChecked == true)
                arr = files;
            else if (folder.IsChecked == true)
                arr = folders;
            arr.Clear();
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
            foreach (var rule in rules)
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
            bool isCopy = false;
            string copyPath = "";
            if (file.IsChecked == true)
            {
                do
                {
                    var dialog = new ConfirmWindow("Do you want to create a copy of file instead of overwrite it ?");
                    if (dialog.ShowDialog() == true)
                    {
                        VistaFolderBrowserDialog folderDialog = new VistaFolderBrowserDialog();
                        if (folderDialog.ShowDialog() == true)
                        {
                            copyPath = folderDialog.SelectedPath;
                            isCopy = true;
                            break;
                        }
                    }
                    else
                        break;
                } while (true);
            }
            foreach (var rule in rules)
                newNames = rule.Rename(newNames);
            for (int i = 0; i < arr.Count; i++)
            {
                try
                {
                    if (file.IsChecked == true)
                    {
                        if (isCopy)
                            File.Copy($"{arr[i].Path}\\{arr[i].Name}", $"{copyPath}\\{newNames[i]}");
                        else
                            File.Move($"{arr[i].Path}\\{arr[i].Name}", $"{arr[i].Path}\\{newNames[i]}");
                    }
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
        private void Return_Action(RuleUI rule)
        {
            switch (rule.TYPE)
            {
                case "AddCounter":
                    //presetComboBox.Items.Add(new ComboBoxItem() { Content = "Add Counter" });
                    actionsUI.Add("Add Counter");
                    break;
                case "Trim":
                    //presetComboBox.Items.Add(new ComboBoxItem() { Content = "Trim" });
                    actionsUI.Add("Trim");
                    break;
                case "AllLower":
                case "AllUpper":
                case "PascalCase":
                    //presetComboBox.Items.Add(new ComboBoxItem() { Content = "New Case" });
                    actionsUI.Add("New Case");
                    break;
                case "ChangeExtension":
                    //presetComboBox.Items.Add(new ComboBoxItem() { Content = "Change Extension" });
                    actionsUI.Add("Change Extension");
                    break;
            }
        }
        private void Delete_Preset_Click(object sender, RoutedEventArgs e)
        {
            int index = presetList.SelectedIndex;
            if (index == -1)
                return;
            RuleUI selected = presets.ElementAt(index);
            Return_Action(selected);
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
                    if (counterDialog.ShowDialog() == true)
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
                        ((ReplaceRuleUI)selected).Replacer = replaceDialog.Replacer;
                    }
                    break;
            }
            selected.Update();
        }

        private void Add_Preset_Click(object sender, RoutedEventArgs e)
        {
            string option = presetComboBox.Text;
            switch (option)
            {
                case "Add":
                    AddWindow addDialog = new AddWindow();
                    if (addDialog.ShowDialog() == true)
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
                    if (counterDialog.ShowDialog() == true)
                    {
                        //presetComboBox.Items.Remove(presetComboBox.SelectedItem);
                        actionsUI.Remove("Add Counter");
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
                        //presetComboBox.Items.Remove(presetComboBox.SelectedItem);
                        actionsUI.Remove("New Case");
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
                    //presetComboBox.Items.Remove(presetComboBox.SelectedItem);
                    actionsUI.Remove("Trim");
                    presets.Add(new TrimRuleUI());
                    break;
                case "Change Extension":
                    ExtensionWindow extDialog = new ExtensionWindow();
                    if (extDialog.ShowDialog() == true)
                    {
                        //presetComboBox.Items.Remove(presetComboBox.SelectedItem);
                        actionsUI.Remove("Change Extension");
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

        // Responsive listview
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

        // Drag and drop files to the list
        private void HandleFileDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] fileArr = (string[])e.Data.GetData(DataFormats.FileDrop);
                addFileListView(fileArr);
            }
        }

        // dynamic height listview
        private void handleCardSize(object sender, SizeChangedEventArgs e)
        {
            if (fileCard.ActualHeight - fileOptions.ActualHeight > 0)
            {
                fileList.Height = fileCard.ActualHeight - fileOptions.ActualHeight;
            }
            else
            {
                fileList.Height = 0;
            }
        }

        private void Delete_File_Click(object sender, RoutedEventArgs e)
        {
            int index = fileList.SelectedIndex;
            if (index == -1)
                return;
            if (file.IsChecked == true)
            {
                files.RemoveAt(index);
            }
            else if (folder.IsChecked == true)
            {
                folders.RemoveAt(index);
            }
        }
       
        void loadPreset(string[] filelines)
        {
            for (int i = 0; i < filelines.Length; i++)
            {
                string line = filelines[i];
                int firstColonIndex = line.IndexOf(":");
                string type = "";
                if (firstColonIndex > 0)
                {
                    type = line.Substring(0, firstColonIndex);
                }
                else
                {
                    type = line;
                }
                string[] tokens;
                string[] parts;
                switch (type)
                {
                    case "Add Counter":
                        tokens = line.Split(new string[] { "Add Counter: " }, StringSplitOptions.None);
                        parts = tokens[1].Split(new string[] { " " }, StringSplitOptions.None);
                        int start = int.Parse(parts[0].Substring(1, parts[0].Length - 1));
                        int step = int.Parse(parts[1].Substring(1, parts[1].Length - 1));
                        int digit = int.Parse(parts[2].Substring(1, parts[2].Length - 1));
                        presets.Add(new AddCounterRuleUI(start, step, digit));
                        actionsUI.Remove("Add Counter");
                        break;
                    case "Add Prefix":
                        tokens = line.Split(new string[] { "Add Prefix: " }, StringSplitOptions.None);
                        string prefix = tokens[1];
                        presets.Add(new AddPrefixRuleUI(prefix));
                        break;
                    case "Add Suffix":
                        tokens = line.Split(new string[] { "Add Suffix: " }, StringSplitOptions.None);
                        string suffix = tokens[1];
                        presets.Add(new AddSuffixRuleUI(suffix));
                        break;
                    case "Change Extension":
                        tokens = line.Split(new string[] { "Change Extension: " }, StringSplitOptions.None);
                        string ext = tokens[1];
                        presets.Add(new ChangeExtRuleUI(ext));
                        actionsUI.Remove("Change Extension");
                        break;
                    case "Replace":
                        tokens = line.Split(new string[] { "Replace: " }, StringSplitOptions.None);
                        parts = tokens[1].Split(new string[] { " => " }, StringSplitOptions.None);
                        string[] words = parts[0].Substring(1, parts[0].Length - 2).Split(new string[] { ", " }, StringSplitOptions.None);
                        List<string> needles = new List<string>();
                        foreach (string word in words)
                        {
                            needles.Add(word.Substring(1, word.Length - 2));
                        }
                        string replacement = parts[1].Substring(1, parts[1].Length - 2);
                        presets.Add(new ReplaceRuleUI(needles, replacement));
                        break;
                    case "All Upper Case":
                        presets.Add(new AllUpperRuleUI());
                        actionsUI.Remove("New Case");
                        break;
                    case "All Lower Case":
                        presets.Add(new AllLowerRuleUI());
                        actionsUI.Remove("New Case");
                        break;
                    case "Pascal Case":
                        presets.Add(new PascalCaseRuleUI());
                        actionsUI.Remove("New Case");
                        break;
                    case "Trim":
                        presets.Add(new TrimRuleUI());
                        actionsUI.Remove("Trim");
                        break;
                }
            }
        }
        private void AutoSave_Conditions(object sender, NotifyCollectionChangedEventArgs e)
        {
            // event triggered after 1 second
            DispatcherTimer timer = new() { Interval = TimeSpan.FromSeconds(1) };
            timer.Start();
            timer.Tick += (sender, args) =>
            {
                timer.Stop();
                AutoSaveFile();
            };
        }
        private void AutoSaveFile()
        {
            string textout = "";

            //auto the save the current file list
            foreach (FileUI file in files)
            {
                textout += file.Name + "|" + file.Path;
                textout += Environment.NewLine;
            }
            textout += "~" + Environment.NewLine;

            //auto the save the current folder list
            foreach (FileUI folder in folders)
            {
                textout += folder.Name + "|" + folder.Path;
                textout += Environment.NewLine;
            }
            textout += "~" + Environment.NewLine;

            // auto save the current set of renaming rules, together with the parameters
            foreach (RuleUI rule in presets)
            {
                textout = textout + rule.Display + Environment.NewLine;
            }
            textout += "~" + Environment.NewLine;
            File.WriteAllText(autoSaveFile, textout);
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            // get current size
            double width = Main.Width;
            double height = Main.Height;

            // get current location
            double top = Main.Top;
            double left = Main.Left;

            //write the sizes and locations into the file
            string textout = "";
            textout += $"Size: {width} {height}" + Environment.NewLine;
            textout += $"Location: {top} {left}" + Environment.NewLine;
            textout += $"Preset: {CurrentPreset}" + Environment.NewLine;
            File.WriteAllText(stateFile, textout);
        }
    }
}
