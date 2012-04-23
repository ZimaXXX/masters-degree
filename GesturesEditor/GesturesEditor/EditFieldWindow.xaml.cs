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
using System.ComponentModel;

namespace GesturesEditor
{
    /// <summary>
    /// Interaction logic for EditFieldWindow.xaml
    /// </summary>
    public partial class EditFieldWindow : Window, INotifyPropertyChanged
    {
        private String fieldText = null;
        public String FieldText
        {
            get
            {
                return fieldText;
            }
            set
            {
                fieldText = value;
                NotifyPropertyChanged("FieldText");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public EditFieldWindow()
        {
            base.DataContext = this;
            InitializeComponent();
        }

        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this,
                    new PropertyChangedEventArgs(propertyName));
            }
        }

        private void bSave_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            this.Close();
        }

        private void bCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
