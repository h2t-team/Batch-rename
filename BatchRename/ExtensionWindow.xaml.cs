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

namespace BatchRename
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
