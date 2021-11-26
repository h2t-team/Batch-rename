﻿using System;
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
