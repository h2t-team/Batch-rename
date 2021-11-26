using System.Windows;

namespace BatchRename.View
{
    /// <summary>
    /// Interaction logic for ChangeExtRule.xaml
    /// </summary>
    public partial class ExtensionWindow : Window
    {
        public string Ext { set; get; }
        public ExtensionWindow()
        {
            InitializeComponent();
        }

        private void Confirm_Button_Click(object sender, RoutedEventArgs e)
        {
            Ext = ExtTxt.Text;
            DialogResult = true;
        }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
