using BatchRename.DataTypes;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace BatchRename.View
{
    /// <summary>
    /// Interaction logic for UpdateCaseWindow.xaml
    /// </summary>
    public partial class UpdateCaseWindow : Window
    {
        private readonly BindingList<KeyValuePair<string, string>> rules = new();
        public string RuleName { get; set; }
        public UpdateCaseWindow(RuleUI rule)
        {
            InitializeComponent();
            RuleName = rule.TYPE;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            rules.Add(new KeyValuePair<string, string>("AllLower","All Lower Case"));
            rules.Add(new KeyValuePair<string, string>("AllUpper", "All Upper Case"));
            rules.Add(new KeyValuePair<string, string>("PascalCase", "Pascal Case"));
            RuleBox.ItemsSource = rules;
            RuleBox.DisplayMemberPath = "Value";
            RuleBox.SelectedIndex = rules.IndexOf(rules.ToList().Find(item => item.Key == RuleName));
        }

        private void Confirm_Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void RuleBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            RuleName = rules[RuleBox.SelectedIndex].Key;
        }
    }
}
