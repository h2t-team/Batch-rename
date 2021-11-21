using Microsoft.Win32;
using System.ComponentModel;
using System.IO;
using System.Windows;

namespace BatchRename
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        BindingList<File> files = new BindingList<File>();
        private void TwitterButton_OnClick(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            fileList.ItemsSource = files;
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
                        files.Add(new File() { Name = Path.GetFileName(file), Path = Path.GetDirectoryName(file) });
                }
            }
        }
    }
}
