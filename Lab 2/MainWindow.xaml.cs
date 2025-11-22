using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Lab_2.Models;
using Lab_2.ViewModels;
using Microsoft.Win32;

namespace Lab_2
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            var result = MessageBox.Show(
                "Чи дійсно ви хочете завершити роботу з програмою?",
                "Підтвердження виходу",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
            else
            {
                if (DataContext is MainViewModel viewModel)
                {
                    viewModel.Dispose();
                }
            }

            base.OnClosing(e);
        }
    }
}