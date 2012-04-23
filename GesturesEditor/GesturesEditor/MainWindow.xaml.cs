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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Collections.ObjectModel;
using MySql.Data.MySqlClient;

namespace GesturesEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        DbUtils dbUtils = null;
        //public ArrayList GestureBaseNames { get; set;}
        public ObservableCollection<GestureMetadataContainer> GestureMetadataContainers { get; set; }
        //public ObservableCollection<ArrayList> GesturesMapXMLs { get; set; }
        //public ObservableCollection<String> GestureBaseNames { get; set; }
        public ArrayList RemovedGMC { get; set; }
        public String Test { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindow()
        {
            base.DataContext = this;
            RemovedGMC = new ArrayList();
            Test = "nic";
            GestureMetadataContainers = new ObservableCollection<GestureMetadataContainer>();
            //GestureBaseNames = new ObservableCollection<String>();
            //GestureBaseNames.Add("nic2");
            //GestureBaseNames.Add("nic3");
            //GesturesMapXMLs = new ObservableCollection<ArrayList>();
            InitializeComponent();
            dbUtils = new DbUtils("localhost", "gestures", "kinect", "kinect");
        }

        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this,
                    new PropertyChangedEventArgs(propertyName));
            }
        }


        private void bLoadNetworks_Click(object sender, RoutedEventArgs e)
        {
            LoadNetworks();
        }

        private void LoadNetworks()
        {
            String sql = "select idneural_gestures, gesture_base_name, gestures_map_xml from neural_gestures";
            ArrayList al = dbUtils.PerformMySQLCommand(sql);
            //GestureBaseNames.Clear();
            //GesturesMapXMLs.Clear();
            GestureMetadataContainers.Clear();
            foreach (ArrayList row in al)
            {
                int id = Int32.Parse((String)((ArrayList)row)[0]);
                String name = (String)((ArrayList)row)[1];
                String gestures_map_xml = (String)((ArrayList)row)[2];
                GestureMetadataContainers.Add(new GestureMetadataContainer(id, name, SerializationUtils.DeSerializeArrayList(gestures_map_xml, typeof(GestureMetadata))));
            }
            //Test = "asd";
            //NotifyPropertyChanged("Test");
            //NotifyPropertyChanged("GestureBaseNames");
            //NotifyCollectionChanged(GestureBaseNames);

            Console.WriteLine("Networks loaded!");
        }

        private void bNewNetwork_Click(object sender, RoutedEventArgs e)
        {
            // Create dialog and show it modally, centered on the owner
            NewNetworkWindow dlg = new NewNetworkWindow();
            dlg.Owner = this;
            if (dlg.ShowDialog() == true)
            {
                GestureMetadataContainer gmc = CreateNewNetwork(dlg.NewNetworkName, dlg.NumberOfPoints * dlg.NumberOfDimensions, dlg.NumberOfOutputs, dlg.NumberOfDimensions);
                GestureMetadataContainers.Add(gmc);
            }
        }

        private GestureMetadataContainer CreateNewNetwork(String name, int numberOfInputs, int numberOfOutputs, int numberOfDimensions)
        {
            ArrayList gesturesMetadata = new ArrayList();
            for (int i = 0; i < numberOfOutputs; ++i)
            {
                gesturesMetadata.Add(new GestureMetadata("No name" + i,numberOfInputs, numberOfOutputs, i, numberOfDimensions));         
            }
            GestureMetadataContainer gmc = new GestureMetadataContainer(-1, name, gesturesMetadata);
            return gmc;
        }

        private void listBoxGesturesBase_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListBoxItem item = sender as ListBoxItem;
            GestureMetadataContainer gmc = (GestureMetadataContainer)item.Content;
            EditFieldWindow efw = new EditFieldWindow();
            efw.FieldText = gmc.Name;
            efw.Owner = this;
            if (efw.ShowDialog() == true)
            {
                gmc.Name = efw.FieldText;
                listBoxGesturesBase.Items.Refresh();
            }
        }

        private void listBoxGesturesNames_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListBoxItem item = sender as ListBoxItem;
            GestureMetadata gm = (GestureMetadata)item.Content;
            EditFieldWindow efw = new EditFieldWindow();
            efw.FieldText = gm.GestureName;
            efw.Owner = this;
            if (efw.ShowDialog() == true)
            {
                gm.GestureName = efw.FieldText;
                listBoxGesturesNames.Items.Refresh();
            }
        }

        private void bRemoveNetwork_Click(object sender, RoutedEventArgs e)
        {
            GestureMetadataContainer gmc = listBoxGesturesBase.SelectedItem as GestureMetadataContainer;
            String text = "Do you really want to delete " + gmc.Name + " network?";
            String caption = "Warning!";
            MessageBoxResult mbr = MessageBox.Show(text, caption, MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if(mbr.Equals(MessageBoxResult.Yes))
            {
                RemovedGMC.Add(gmc);
                GestureMetadataContainers.Remove(gmc);
            }
        }

        private void bSaveToDB_Click(object sender, RoutedEventArgs e)
        {
            String text = "Do you really want to save current networks to DB?";
            String caption = "Warning!";
            MessageBoxResult mbr = MessageBox.Show(text, caption, MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (mbr.Equals(MessageBoxResult.Yes))
            {
                SaveNetworksToDB();
                LoadNetworks();
            }
        }

        private void SaveNetworksToDB()
        {
            String sql = "";
            foreach (GestureMetadataContainer gmc in RemovedGMC)
            {
                sql = "DELETE FROM neural_gestures WHERE idneural_gestures = '" + gmc.Id + "'";
                dbUtils.PerformMySQLCommand(sql);
            }
            RemovedGMC.Clear();            
            String empty_learn_xml = "<?xml version=\"1.0\"?><ArrayOfAnyType xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"></ArrayOfAnyType>";
            sql = "";
            foreach (GestureMetadataContainer gmc in GestureMetadataContainers)
            {
                String idneural_gestures = gmc.Id.ToString();
                String gesture_base_name = gmc.Name;
                String gestures_map_xml = SerializationUtils.SerializeArrayList(gmc.GesturesMetadata, typeof(GestureMetadata));
                if (idneural_gestures.Equals("-1"))
                {
                    sql = "INSERT INTO neural_gestures (gesture_base_name, gestures_map_xml, learn_xml) VALUES ('" + gesture_base_name + "', '" + gestures_map_xml + "',  '" + empty_learn_xml + "')";
                }
                else
                {
                    sql = "UPDATE neural_gestures SET gesture_base_name = '" + gesture_base_name + "', gestures_map_xml = '" + gestures_map_xml + "' WHERE idneural_gestures='" + idneural_gestures + "'";
                }
                try
                {
                    dbUtils.PerformMySQLCommand(sql);
                }
                catch (MySqlException)
                {
                    MessageBox.Show("error");
                }
            }
        }
    }
}
