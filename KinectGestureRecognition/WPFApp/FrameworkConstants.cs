using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.ComponentModel;
using System.Windows.Data;
using System.Collections;
using System.Reflection;
using WPFApp.Properties;
using System.Configuration;

namespace WPFApp
{
    //public struct Gesture
    //{
    //    public string name;
    //    public double value;
    //    public static double NO_VALUE = -1;
    //    public Gesture(string name, double value) 
    //    {
    //        this.name = name;
    //        this.value = value;
    //    }
    //    public Gesture(string name)
    //    {
    //        this.name = name;
    //        this.value = NO_VALUE;
    //    }
    //}
    public enum Languages
    {
        PL,
        EN,
        NO_LANGUAGE = -1
    }
    public enum Commands
    {
        EMPTY_COMMAND,
        LOAD_MENU_COMMAND,
        LOAD_MAIN_MENU,
        CHANGE_VALUE,
        APPLY_CHANGES,
        RECORD_GESTURE,
        UNHIDE_GESTURES,
        MORE,
        LEARN,
        IS_CORRECT,
        SAVE_TO_DB,
        RESET_TRAINING_DATA,
        RELEARN_WITH_CURRENT_TRAINING_DATA,
        PROCESS_GESTURE,
        SHOW_TEST_MESSAGE,
        SELECT_GESTURE,
        GESTURE_SELECTED,
        EXPORT_NETWORK
    }
    public enum Pages
    {
        MAIN_PAGE,
        CONFIGURATION_PAGE,
        GESTURES_PAGE,
        //STATISTICS_PAGE,
        LEARNING_PAGE,
        NO_PAGE
    }
    
    public class FrameworkConstants : INotifyPropertyChanged
    {
        public void StartMeasuring()
        {
            StartTime = DateTime.Now;
            System.IO.File.AppendAllText(pathToFile, "Measuring started\n");
        }

        public void EndMeasuring()
        {
            //StartTime = new DateTime();
            //if(StartTime.M
            DifferenceTime = DateTime.Now - StartTime;
            System.IO.File.AppendAllText(pathToFile, "Measuring time: " + TimeLapsed + "\n\n");
            StartTime = new DateTime();
        }

        public void AppendToFile(String text)
        {
            System.IO.File.AppendAllText(pathToFile, text);
        }

        public string pathToFile = @Properties.Settings.Default.LogPath;
        private DateTime startTime;
        private DateTime StartTime 
        {
            get
            {
                return startTime;
            }
            set
            {
                startTime = value;
                
            }
        }
        private TimeSpan differenceTime;
        public TimeSpan DifferenceTime 
        {
            get
            {
                return differenceTime;
            }
            set
            {
                differenceTime = value;
                TimeLapsed = differenceTime.Milliseconds.ToString();
                NotifyPropertyChanged("TimeLapsed");
            }
        }

        public LinkedList<int> HiddenNeuronsNumber = null;
        public LinkedList<int> EpochsNumber = null;
        public LinkedList<double> Momentums = null;
        public LinkedList<double> LearnRates = null;

        
        private ArrayList gesturesList { get; set; }
        public ArrayList GesturesList
        {
            get { return gesturesList; }
            set
            {
                gesturesList = value;
                GesturesEventMap = new Hashtable();
                foreach (GestureMetadata gm in gesturesList)
                {
                    GesturesEventMap.Add(gm.GestureName, Commands.EMPTY_COMMAND);
                }
            }
        }
        public Hashtable GesturesEventMap { get; set; }
        public LinkedList<string> GesturesBases = null;

        public static readonly string CMT_GESTURES = "cmtGestures";
        public static readonly string CMT_LEARNING = "cmtLearning";
        public static readonly string CMT_CONFIGURATION = "cmtConfiguration";
        public static readonly string CMT_EXIT = "cmtExit";
        public static readonly string CMT_TEST = "cmtTest";
        
        //Configuration
        public static readonly string CMT_CONFIGURATION_LANGUAGE = "cmtConfigurationLanguage";
        public static readonly string CMT_CONFIGURATION_TEST = "cmtConfigurationTest";
        public static readonly string CMT_CONFIGURATION_APPLY_CHANGES = "cmtConfigurationApplyChanges";
        public static readonly string CMT_CONFIGURATION_MAIN_MENU = "cmtConfigurationMainMenu";
        public static readonly string CMT_CONFIGURATION_EXPORT_NETWORK = "cmtConfigurationExportNetwork";
        //Learning
        public static readonly string CMT_LEARNING_HIDDEN_NEURONS = "cmtLearningHiddenNeurons";
        public static readonly string CMT_LEARNING_MOMENTUM = "cmtLearningMomentum";
        public static readonly string CMT_LEARNING_EPOCHS = "cmtLearningEpochs";
        public static readonly string CMT_LEARNING_LEARN_RATE = "cmtLearningLearnRate";
        public static readonly string CMT_LEARNING_APPLY_CHANGES = "cmtLearningApplyChanges";
        public static readonly string CMT_LEARNING_MAIN_MENU = "cmtLearningMainMenu";
        public static readonly string CMT_LEARNING_MORE = "cmtLearningMore";
        public static readonly string CMT_LEARNING_LEARN = "cmtLearningLearn";
        public static readonly string CMT_LEARNING_LEARNING_BASE = "cmtLearningLearningBase";

        //Gestures
        public static readonly string CMT_GESTURES_RECORDING = "cmtGesturesRecording";
        public static readonly string CMT_GESTURES_MAIN_MENU = "cmtGesturesMainMenu";
        public static readonly string CMT_GESTURES_IS_CORRECT = "cmtGesturesIsCorrect";
        public static readonly string CMT_GESTURES_SAVE_TO_DB = "cmtGesturesSave";
        public static readonly string CMT_GESTURES_RESET = "cmtGesturesReset";
        //public static readonly string CMT_GESTURES_RELEARN = "cmtGesturesRelearn";
        public static readonly string CMT_GESTURES_LEARN_GESTURE = "cmtGesturesLearnGesture";
        public static readonly string CMT_GESTURES_LEARN_GESTURE_BACK = "cmtGesturesLearnGestureBack";
        public static readonly string CMT_GESTURES_LEARN_GESTURE_CONFIRM = "cmtGesturesLearnGestureConfirm";
        public static readonly string CMT_GESTURES_LEARN_GESTURE_MORE = "cmtGesturesLearnGestureMore";

        public static readonly string IB_GESTURES = "ibGestures";
        public static readonly string IB_INFORMATION = "ibInformation";

        public static readonly string GESTURE_PREFIX = "gesturePrefix";

        public Pages CurrentPage
        {
            get {return this.currentPage;}

            set
            {
                currentPage = value;
                switch (value)
                {
                    case (Pages.MAIN_PAGE):
                        {
                            CurrentPageCaption = MainPageCaption;
                            break;
                        }
                    case (Pages.GESTURES_PAGE):
                        {
                            CurrentPageCaption = GesturesPageCaption;
                            break;
                        }
                    //case (Pages.STATISTICS_PAGE):
                    //    {
                    //        CurrentPageCaption = StatisticsPageCaption;
                    //        break;
                    //    }
                    case (Pages.LEARNING_PAGE):
                        {
                            CurrentPageCaption = LearningPageCaption;
                            break;
                        }
                    case (Pages.CONFIGURATION_PAGE):
                        {
                            CurrentPageCaption = ConfigurationPageCaption;
                            break;
                        }
                    case (Pages.NO_PAGE):
                        {
                            CurrentPageCaption = NoPageCaption;
                            break;
                        }
                }
                NotifyPropertyChanged("CurrentPageCaption");
            }
        }
        private Pages currentPage;
        public event PropertyChangedEventHandler PropertyChanged;

        public bool BackgroundRecognition { get; set; }

        //Time lapsed (for tests)
        public string TimeLapsed { get; set; }

        //Main menu CMTS
        public string Gestures { get; set; }
        public string Configuration { get; set; }
        public string Learning { get; set; }
        public string Exit { get; set; }
        public string Test { get; set; }

        public string NoName { get; set; }
        public string NoData { get; set; }

        //GesturesStatus
        public string GesturesStatus_Gesture { get; set; }
        public string GesturesStatus_NoGesture { get; set; }
        string currentInformation = "";
        public string CurrentInformation { 
            get 
            {
                return currentInformation;
            }
            set
            {
                currentInformation = value;
                NotifyPropertyChanged("CurrentInformation");
            }
        }

        //Main menu Page Title
        public string MainPageCaption { get; set; }
        public string ConfigurationPageCaption { get; set; }
        public string LearningPageCaption { get; set; }
        //public string StatisticsPageCaption { get; set; }
        public string GesturesPageCaption { get; set; }
        public string NoPageCaption { get; set; }
        public string CurrentPageCaption { get; set; }

        //Languages
        public Languages CurrentLanguage 
        {
            get
            {
                return Properties.Settings.Default.Language;
            }
            set
            {
                Properties.Settings.Default.Language = value;
                //InitializeCaptions(CurrentLanguage);
            } 
        }

        //Learning
        public int HiddenNeurons
        {
            get
            {
                return Properties.Settings.Default.HiddenNeurons;
            }
            set
            {
                Properties.Settings.Default.HiddenNeurons = value;
            }
        }
        public double Momentum
        {
            get
            {
                return Properties.Settings.Default.Momentum;
            }
            set
            {
                Properties.Settings.Default.Momentum = value;
            }
        }
        public double LearnRate
        {
            get
            {
                return Properties.Settings.Default.LearnRate;
            }
            set
            {
                Properties.Settings.Default.LearnRate = value;
            }
        }
        public int Epochs
        {
            get
            {
                return Properties.Settings.Default.Epochs;
            }
            set
            {
                Properties.Settings.Default.Epochs = value;
            }
        }


        private static Languages defaultLanguage = Languages.PL;

        public string MainMenu { get; set; }

        //Configuration menu CMTS
        public string Configuration_Language { get; set; }
        public string ApplyChanges { get; set; }
        public string Configuration_Export_Network { get; set; }

        //Learning menu CMTS
        public string Learning_Hidden_Neurons { get; set; }
        public string Learning_Momentum { get; set; }
        public string Learning_Learn_Rate { get; set; }
        public string Learning_Epochs { get; set; }
        public string Learning_More { get; set; }
        public string Learning_Learn { get; set; }
        public string Learning_Learning_Base { get; set; }

        //Gestures menu CMTS
        public string Gestures_Recording { get; set; }
        public string Gestures_Save_To_DB { get; set; }
        public string Gestures_Is_Correct { get; set; }
        public string Gestures_Reset_Training_Data { get; set; }
        //public string Gestures_Relearn { get; set; }
        public string Gestures_Learn_Gesture { get; set; }

        //Helpful words
        //public string Gestures_Recording { get; set; }
        //public string Gestures_Recording { get; set; }

        //Informations
        public string Info_Recognized_Gesture { get; set; }
        public string Info_CannotDecide { get; set; }
        public string Info_Information { get; set; }
        public string Info_GestureSaved { get; set; }
        public string Info_GestureAlreadySaved { get; set; }
        public string Info_DatabaseUpdated { get; set; }
        public string Info_NetworkLearned { get; set; }
        public string Info_ChangesApplied { get; set; }
        public string Info_NoGestureSelected { get; set; }
        public string Info_NetworkExportedSuccesfullyTo { get; set; }

        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this,
                    new PropertyChangedEventArgs(propertyName));
            }
        }

        public FrameworkConstants()
        {
            BackgroundRecognition = false;
            InitializeCaptions(CurrentLanguage);
            InitializeCollections();
            CurrentPage = Pages.MAIN_PAGE;
        }

        private void InitializeCollections()
        {
            HiddenNeuronsNumber = new LinkedList<int>();
            EpochsNumber = new LinkedList<int>();
            Momentums = new LinkedList<double>();
            LearnRates = new LinkedList<double>();
            GesturesList = new ArrayList();
            GesturesBases = new LinkedList<String>();

            HiddenNeuronsNumber.AddLast(1);
            HiddenNeuronsNumber.AddLast(2);
            HiddenNeuronsNumber.AddLast(5);
            HiddenNeuronsNumber.AddLast(10);
            HiddenNeuronsNumber.AddLast(20);
            HiddenNeuronsNumber.AddLast(50);

            EpochsNumber.AddLast(100);
            EpochsNumber.AddLast(1000);
            EpochsNumber.AddLast(2000);
            EpochsNumber.AddLast(5000);
            EpochsNumber.AddLast(10000);
            EpochsNumber.AddLast(20000);
            EpochsNumber.AddLast(50000);

            Momentums.AddLast(0.1);
            Momentums.AddLast(0.2);
            Momentums.AddLast(0.3);
            Momentums.AddLast(0.4);
            Momentums.AddLast(0.5);
            Momentums.AddLast(0.6);
            Momentums.AddLast(0.7);
            Momentums.AddLast(0.8);
            Momentums.AddLast(0.9);

            LearnRates.AddLast(0.1);
            LearnRates.AddLast(0.2);
            LearnRates.AddLast(0.3);
            LearnRates.AddLast(0.4);
            LearnRates.AddLast(0.5);
            LearnRates.AddLast(0.6);
            LearnRates.AddLast(0.7);
            LearnRates.AddLast(0.8);
            LearnRates.AddLast(0.9);

            //GesturesList.Add(new GestureMetadata("TOP_LEFT", 2, 4, 0));
            //GesturesList.Add(new GestureMetadata("TOP_RIGHT", 2, 4, 1));
            //GesturesList.Add(new GestureMetadata("BOTTOM_LEFT", 2, 4, 2));
            //GesturesList.Add(new GestureMetadata("BOTTOM_RIGHT", 2, 4, 3));

            //GesturesList.Add(new GestureMetadata("GESTURE 1", 8, 4, 0));
            //GesturesList.Add(new GestureMetadata("GESTURE 2", 2, 4, 1));
            //GesturesList.Add(new GestureMetadata("GESTURE 3", 2, 4, 2));
            //GesturesList.Add(new GestureMetadata("GESTURE 4", 2, 4, 3));

            //NeuronNetworkFacility.AttachValuesToGestures(GesturesList);
            //String xml = XmlUtils.SerializeArrayList(GesturesList, typeof(GestureMetadata));
            //Console.WriteLine(xml);

        }
        public object[] GetCorrespondingArray(string name)
        {
            if (name.Equals(CMT_LEARNING_HIDDEN_NEURONS))
            {
                int[] array = HiddenNeuronsNumber.ToArray<int>();
                object[] objectArray = new object[array.Length];
                array.CopyTo(objectArray, 0);
                return objectArray;
            }
            else if (name.Equals(CMT_LEARNING_EPOCHS))
            {
                int[] array = EpochsNumber.ToArray<int>();
                object[] objectArray = new object[array.Length];
                array.CopyTo(objectArray, 0);
                return objectArray;
            }
            else if (name.Equals(CMT_LEARNING_MOMENTUM))
            {
                double[] array = Momentums.ToArray<double>();
                object[] objectArray = new object[array.Length];
                array.CopyTo(objectArray, 0);
                return objectArray;
            }
            else if (name.Equals(CMT_LEARNING_LEARN_RATE))
            {
                double[] array = LearnRates.ToArray<double>();
                object[] objectArray = new object[array.Length];
                array.CopyTo(objectArray, 0);
                return objectArray;
            }
            else if (name.Equals(CMT_LEARNING_LEARNING_BASE))
            {
                string[] array = GesturesBases.ToArray<String>();
                object[] objectArray = new object[array.Length];
                array.CopyTo(objectArray, 0);
                return objectArray;
            }
            else
                return null;
        }

        public FrameworkConstants(Languages language)
        {
            if (language.Equals(Languages.NO_LANGUAGE))
            {
                CurrentLanguage = defaultLanguage;
            }
            else
            {
                CurrentLanguage = language;
            }
            InitializeCaptions(CurrentLanguage);
            InitializeCollections();
            CurrentPage = Pages.MAIN_PAGE;
        }

        public void NotifyCaptionsChanged()
        {
            CurrentPage = CurrentPage;
            NotifyPropertyChanged("Gestures");
            NotifyPropertyChanged("Configuration");
            NotifyPropertyChanged("Learning");
            NotifyPropertyChanged("Exit");
            NotifyPropertyChanged("Test");
            NotifyPropertyChanged("NoName");
            NotifyPropertyChanged("NoData");
            NotifyPropertyChanged("MainPageCaption");
            NotifyPropertyChanged("ConfigurationPageCaption");
            NotifyPropertyChanged("LearningPageCaption");
            NotifyPropertyChanged("GesturesPageCaption");
            NotifyPropertyChanged("Info_Recognized_Gesture");
            //NotifyPropertyChanged("StatisticsPageCaption");
            NotifyPropertyChanged("Info_CannotDecide");
            NotifyPropertyChanged("Configuration_Language");
            NotifyPropertyChanged("ApplyChanges");
            NotifyPropertyChanged("Learning_Hidden_Neurons");
            NotifyPropertyChanged("Learning_Momentum");
            NotifyPropertyChanged("Learning_Learn_Rate");
            NotifyPropertyChanged("Learning_Epochs");
            NotifyPropertyChanged("MainMenu");
            NotifyPropertyChanged("CurrentPageCaption");
            NotifyPropertyChanged("Gestures_Recording");
            NotifyPropertyChanged("CurrentLanguage");
            NotifyPropertyChanged("GesturesStatus_Gesture");
            NotifyPropertyChanged("Learning_More");
            NotifyPropertyChanged("Learning_Learn");
            NotifyPropertyChanged("Learning_Learning_Base");
            NotifyPropertyChanged("Gestures_Save_To_DB");
            NotifyPropertyChanged("Gestures_Is_Correct");
            NotifyPropertyChanged("Gestures_Reset_Training_Data");
            //NotifyPropertyChanged("Gestures_Relearn");
            NotifyPropertyChanged("CurrentInformation");
            NotifyPropertyChanged("Info_Information");
            NotifyPropertyChanged("Gestures_Learn_Gesture");
            NotifyPropertyChanged("Info_NoGestureSelected");
            NotifyPropertyChanged("Configuration_Export_Network");
            NotifyPropertyChanged("Info_NetworkExportedSuccesfullyTo");
        }

        public void NotifyPropertiesChanged()
        {
            //foreach (SettingsProperty currentProperty in Properties.Settings.Default.Properties)
            //{
            //    NotifyPropertyChanged(currentProperty.ToString());
            //} 
            NotifyPropertyChanged("CurrentLanguage");
        }
       
        public void InitializeCaptions(Languages language)
        {
            
            switch (language)
            {
                case Languages.PL:
                {
                    Gestures = "Gesty";
                    Configuration = "Konfiguracja";
                    Learning = "Nauka";
                    Exit = "Wyjdź";

                    Test = "Główne menu";
                    NoName = "Brak nazwy";
                    NoData = "Brak danych";

                    MainPageCaption = "Ekran główny";
                    ConfigurationPageCaption = "Konfiguracja";
                    LearningPageCaption = "Nauka";
                    GesturesPageCaption = "Gesty";
                    //StatisticsPageCaption = "Statystyki";

                    Configuration_Language = "Język";
                    ApplyChanges = "Zastosuj";
                    Configuration_Export_Network = "Eksportuj sieć";

                    Learning_Epochs = "Epoki";
                    Learning_Hidden_Neurons = "Ukryte neurony";
                    Learning_Learn_Rate = "Współczynnik uczenia";
                    Learning_Momentum = "Bezwładność";
                    Learning_More = "Więcej";
                    Learning_Learn = "Naucz sieć";
                    Learning_Learning_Base = "Baza gestów";

                    Gestures_Recording = "Nagraj gest";
                    Gestures_Is_Correct = "Poprawnie rozpoznany";
                    Gestures_Save_To_DB = "Zapisz do bazy danych";
                    Gestures_Reset_Training_Data = "Odrzuć zmiany";
                    //Gestures_Relearn = "Naucz ponownie";
                    Gestures_Learn_Gesture = "Naucz gestu";

                    MainMenu = "Powrót";

                    GesturesStatus_NoGesture = "Brak gestu";
                    GesturesStatus_Gesture = GesturesStatus_NoGesture;
                    CurrentInformation = "Brak informacji";

                    Info_CannotDecide = "Gest nierozpoznany";
                    Info_Recognized_Gesture = "Rozpoznany gest";
                    Info_Information = "Informacja:";
                    Info_DatabaseUpdated = "Sieć zapisano pomyślnie";
                    Info_GestureAlreadySaved = "Zatwierdzono już ten gest!";
                    Info_GestureSaved = "Gest zatwierdzony";
                    Info_NetworkLearned = "Sieć nauczona";
                    Info_ChangesApplied = "Zastosowano zmiany";
                    Info_NoGestureSelected = "Nie wybrano gestu!";
                    Info_NetworkExportedSuccesfullyTo = "Poprawnie eksportowano sieć do";
                    break;
                }
                case Languages.EN:
                {
                    Gestures = "Gestures";
                    Configuration = "Configuration";
                    Learning = "Learning";
                    Exit = "Exit program";

                    Test = "Main menu";
                    NoName = "No name";
                    NoData = "No data";

                    MainPageCaption = "Main screen";
                    ConfigurationPageCaption = "Configuration";
                    LearningPageCaption = "Learning";
                    GesturesPageCaption = "Gestures";
                    //StatisticsPageCaption = "Statistics";

                    Configuration_Language = "Language";
                    ApplyChanges = "Apply";
                    Configuration_Export_Network = "Export network";

                    Learning_Epochs = "Epochs";
                    Learning_Hidden_Neurons = "Hidden neurons";
                    Learning_Learn_Rate = "Learn rate";
                    Learning_Momentum = "Momentum";
                    Learning_More = "More";
                    Learning_Learn = "Learn network";
                    Learning_Learning_Base = "Gesture base";

                    Gestures_Recording = "Record a gesture";
                    Gestures_Is_Correct = "Correctly recognized?";
                    Gestures_Save_To_DB = "Save to DB";
                    Gestures_Reset_Training_Data = "Reset changes";
                    //Gestures_Relearn = "Learn again";
                    Gestures_Learn_Gesture = "Learn gesture";

                    MainMenu = "Back";

                    GesturesStatus_NoGesture = "No gesture";
                    GesturesStatus_Gesture = GesturesStatus_NoGesture;
                    CurrentInformation = "No information";

                    Info_CannotDecide = "Not recognized";
                    Info_Recognized_Gesture = "Recognized gesture";
                    Info_Information = "Information:";
                    Info_DatabaseUpdated = "Network saved succesfully";
                    Info_GestureAlreadySaved = "This gesture was already confirmed!";
                    Info_GestureSaved = "Gesture confirmed";
                    Info_NetworkLearned = "Network learned";
                    Info_ChangesApplied = "Applied changes";
                    Info_NoGestureSelected = "No gesture selected!";
                    Info_NetworkExportedSuccesfullyTo = "Network exported succesfully to";
                    break;
                }                    
            }
            NotifyCaptionsChanged();
        }
    }
}
