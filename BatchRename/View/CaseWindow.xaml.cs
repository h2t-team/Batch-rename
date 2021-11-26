using System.Windows;

namespace BatchRename.View
{
    /// <summary>
    /// Interaction logic for NewCaseRule.xaml
    /// </summary>
    public partial class CaseWindow : Window
    {
        public string RuleName { get; set; }
        public CaseWindow()
        {
            InitializeComponent();
        }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void Confirm_Button_Click(object sender, RoutedEventArgs e)
        {
            RuleName = RuleBox.Text;
            DialogResult = true;
        }
    }
}
