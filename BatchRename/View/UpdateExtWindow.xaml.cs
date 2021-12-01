using BatchRename.DataTypes;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for UpdateExtWindow.xaml
    /// </summary>
    public partial class UpdateExtWindow : Window
    {
        public string Ext { get; set; }
        public UpdateExtWindow(RuleUI rule)
        {
            InitializeComponent();
            Ext = ((ChangeExtRuleUI)rule).Ext;
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
