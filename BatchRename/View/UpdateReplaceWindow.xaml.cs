using BatchRename.DataTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BatchRename.View
{
    /// <summary>
    /// Interaction logic for UpdateReplaceWindow.xaml
    /// </summary>
    public partial class UpdateReplaceWindow : Window, INotifyPropertyChanged
    {
        private BindingList<string> wordBinding = new BindingList<string>();
        public List<string> Needles;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Replacer { set; get; }
        public UpdateReplaceWindow(RuleUI rule)
        {
            InitializeComponent();
            Needles = new List<string>(((ReplaceRuleUI)rule).Needles);
            Replacer = ((ReplaceRuleUI)rule).Replacer;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            wordList.ItemsSource = wordBinding;
            DataContext = this;
            foreach(var needle in Needles)
            {
                wordBinding.Add($"\"{needle}\"");
            }
        }
        private void Add_Button_Click(object sender, RoutedEventArgs e)
        {
            string word = WordTxt.Text;
            if (String.IsNullOrEmpty(word))
                return;
            if (String.IsNullOrWhiteSpace(word))
                word = " ";
            WordTxt.Text = "";
            if (Needles.IndexOf(word) != -1)
                return;
            wordBinding.Add($"\"{word}\"");
            Needles.Add(word);
        }
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            int index = wordList.SelectedIndex;
            if (index == -1)
                return;
            wordBinding.RemoveAt(index);
            Needles.RemoveAt(index);
        }

        private void Confirm_Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
