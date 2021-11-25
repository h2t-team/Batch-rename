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

namespace BatchRename
{
    /// <summary>
    /// Interaction logic for ReplaceWindow.xaml
    /// </summary>
    public partial class ReplaceWindow : Window
    {
        private BindingList<string> wordBinding = new BindingList<string>();
        public List<string> needles = new List<string>();
        public string Replacer { set; get; }
        public ReplaceWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            wordList.ItemsSource = wordBinding;
        }
        private void Add_Button_Click(object sender, RoutedEventArgs e)
        {
            string word = WordTxt.Text;
            if (String.IsNullOrEmpty(word))
                return;
            if (String.IsNullOrWhiteSpace(word))
                word = " ";
            if (needles.IndexOf(word) != -1)
            {
                WordTxt.Text = "";
                return;
            }
            wordBinding.Add($"\"{word}\"");
            needles.Add(word);
            WordTxt.Text = "";
        }
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            int index = wordList.SelectedIndex;
            if (index == -1)
                return;
            wordBinding.RemoveAt(index);
        }

        private void Confirm_Button_Click(object sender, RoutedEventArgs e)
        {
            Replacer = ReplacerTxt.Text;
            DialogResult = true;
        }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

    }
}
