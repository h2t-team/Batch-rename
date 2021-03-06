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

namespace BatchRename.View
{
    /// <summary>
    /// Interaction logic for CounterWindow.xaml
    /// </summary>
    public partial class CounterWindow : Window, INotifyPropertyChanged
    {
        public int Start { get; set; }
        public int Step { get; set; }
        public int Digit { get; set; }
        public CounterWindow()
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Confirm_Button_Click(object sender, RoutedEventArgs e)
        {
            Start = int.Parse(StartTxt.Text);
            Step = int.Parse(StepTxt.Text);
            Digit = int.Parse(DigitTxt.Text);
            DialogResult = true;
        }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = this;
        }
    }
}
