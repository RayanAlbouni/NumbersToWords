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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Currency.Converter.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ConverterServiceReference.ConverterServiceClient client;
        public MainWindow()
        {
            InitializeComponent();
            txtInput.Focus();
            client = new ConverterServiceReference.ConverterServiceClient();
        }

        private async void txtInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtOutput.Text = await client.ConverterAsync(txtInput.Text, "US");
            if (txtOutput.Text.StartsWith("Error"))
                txtOutput.Foreground = new SolidColorBrush(Colors.Red);
            else if (txtOutput.Text.StartsWith("Result"))
                txtOutput.Foreground = new SolidColorBrush(Colors.LightGray);
            else
                txtOutput.Foreground = new SolidColorBrush(Colors.Black);
        }

        private void btnSourceLink_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://simple.wikipedia.org/wiki/Names_of_numbers_in_English");
        }
    }
}
