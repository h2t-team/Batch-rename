using System.Windows;

namespace BatchRename.View
{
    /// <summary>
    /// Interaction logic for AddRules.xaml
    /// </summary>
    public partial class AddWindow : Window
    {
        public string RuleName { get; set; }
        public string Word { set; get; }
        public AddWindow()
        {
            InitializeComponent();
        }

        private void Confirm_Button_Click(object sender, RoutedEventArgs e)
        {
            RuleName = RuleBox.Text;
            Word = WordTxt.Text;
            DialogResult = true;
        }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
