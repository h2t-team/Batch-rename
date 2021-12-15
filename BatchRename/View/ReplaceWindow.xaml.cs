using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;

namespace BatchRename.View
{
    /// <summary>
    /// Interaction logic for ReplaceWindow.xaml
    /// </summary>
    public partial class ReplaceWindow : Window, INotifyPropertyChanged
    {
        private BindingList<string> wordBinding = new BindingList<string>();
        public List<string> Needles = new List<string>();

        public event PropertyChangedEventHandler PropertyChanged;

        public string Replacer { set; get; }
        public ReplaceWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            wordList.ItemsSource = wordBinding;
            DataContext = this;
        }
        private void Add_Button_Click(object sender, RoutedEventArgs e)
        {
            string word = WordTxt.Text;
            if (String.IsNullOrEmpty(word))
                return;
            if (String.IsNullOrWhiteSpace(word))
                word = " ";
            if (Needles.IndexOf(word) != -1)
            {
                WordTxt.Text = "";
                return;
            }
            wordBinding.Add($"\"{word}\"");
            Needles.Add(word);
            WordTxt.Text = "";
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
            Replacer = ReplacerTxt.Text;
            DialogResult = true;
        }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

    }
}
