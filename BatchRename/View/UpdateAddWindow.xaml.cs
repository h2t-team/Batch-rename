using BatchRename.DataTypes;
using System.Windows;

namespace BatchRename.View
{
    /// <summary>
    /// Interaction logic for UpdateAddWindow.xaml
    /// </summary>
    public partial class UpdateAddWindow : Window
    {
        public string RuleName { get; set; }
        public string Word { get; set; }
        public UpdateAddWindow(RuleUI rule)
        {
            InitializeComponent();
            switch (rule.TYPE)
            {
                case "AddPrefix":
                    RuleName = "Add Prefix";
                    Word = ((AddPrefixRuleUI)rule).Prefix;
                    break;
                case "AddSuffix":
                    RuleName = "Add Suffix";
                    Word = ((AddSuffixRuleUI)rule).Suffix;
                    break;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = this;
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
