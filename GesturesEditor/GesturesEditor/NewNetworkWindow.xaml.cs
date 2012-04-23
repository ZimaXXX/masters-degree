using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GesturesEditor
{
    /// <summary>
    /// Interaction logic for NewNetwork.xaml
    /// </summary>
    public partial class NewNetworkWindow : Window
    {
        public String NewNetworkName { get; set; }
        public int NumberOfPoints { get; set; }
        public int NumberOfOutputs { get; set; }
        public int NumberOfDimensions { get; set; }

        public NewNetworkWindow()
        {
            NewNetworkName = "test2";
            NumberOfPoints = 4;
            NumberOfDimensions = 3;
            NumberOfOutputs = 4;
            base.DataContext = this;
            InitializeComponent();
        }

        private void bSave_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            this.Close();
        }

        private void bCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }
    }
}
