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
using System.ComponentModel;
using System.Collections;
using HAKGERSoft.Synapse;
using System.Threading;
using System.Windows.Threading;
using WPFApp.Kinect;

namespace WPFApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public DbUtils dbUtils = null;
        public FrameworkConstants frameworkConstants = null;
        public EventDispatcher eventDispatcher = null;
        public event PropertyChangedEventHandler PropertyChanged;
        GraphicalEffects graphicalEffects = null;
        public NeuronNetworkFacility nnf = null;
        public LinkedList<GraphicalEffectsMetadata> gems = null;
        public Hashtable cmtsHashMain = null;
        public Hashtable cmtsHashGestures = null;
        public Hashtable cmtsHashLearning = null;
        public Hashtable cmtsHashConfiguration = null;
        //public Hashtable cmtsHashStatistics = null;
        public Hashtable cmtsMap = null;
        public List<TrainingData> currentTrainingData = null;
        public Hashtable ibMap = null;
        //ThreadStart gestureThread = null;
        public Thread thread = null;
        public KinectFacility kinectFacility = null;
        public Hashtable cmtsVisibleMap = null;
        public ArrayList canvasCMTPoints = null;
        public Hashtable cmtsHashLearningGestures = null;

        public MainWindow()
        {
            InitializeComponent();
            kinectFacility = new KinectFacility();
            dbUtils = new DbUtils("localhost", "gestures", "kinect", "kinect");
            //MidpointValueConverter mvc = new MidpointValueConverter();
            //currentTrainingData = new LinkedList<TrainingData>();
            cmtsVisibleMap = new Hashtable();
            cmtsHashMain = new Hashtable();
            cmtsHashConfiguration = new Hashtable();
            cmtsHashGestures = new Hashtable();
            cmtsHashLearning = new Hashtable();
            //cmtsHashStatistics = new Hashtable();
            cmtsHashLearningGestures = new Hashtable();
            graphicalEffects = new GraphicalEffects();
            frameworkConstants = new FrameworkConstants();
            canvasCMTPoints = new ArrayList();
            cmtsMap = new Hashtable();
            ibMap = new Hashtable();
            nnf = new NeuronNetworkFacility();
            base.DataContext = frameworkConstants;      

            //gestureThread = new ThreadStart(WorkThreadFunction);
        }

        public void WorkThreadFunction()
        {
            try
            {
                nnf.ProcessRecognition(mainCanvas, frameworkConstants.GesturesList, RECOGNITION_MODE.BACKGROUND);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public CircularMinuteTimer CreateCircularMinuteTimer(string name, Binding caption, double width, double height, Point[] hideAndShowPoints, Commands command)
        {
            CircularMinuteTimer cmt = new CircularMinuteTimer();
            cmt.Name = name;
            cmt.SetBinding(CircularMinuteTimer.CaptionProperty, caption);
            Canvas.SetLeft(cmt, hideAndShowPoints[0].X);
            Canvas.SetTop(cmt, hideAndShowPoints[0].Y);
            cmt.DesiredPositionLeft = hideAndShowPoints[1].X;
            cmt.DesiredPositionTop = hideAndShowPoints[1].Y;
            cmt.Command = command;
            cmt.Width = width;
            cmt.Height = height;
            cmt.Mode = CircularMinuteTimer.Modes.SIMPLE;
            
            return cmt;
        }

        public CircularMinuteTimer CreateCircularMinuteTimer(string name, String caption, double width, double height, Point[] hideAndShowPoints, Commands command)
        {
            CircularMinuteTimer cmt = new CircularMinuteTimer();
            cmt.Name = name;
            cmt.Caption = caption;
            Canvas.SetLeft(cmt, hideAndShowPoints[0].X);
            Canvas.SetTop(cmt, hideAndShowPoints[0].Y);
            cmt.DesiredPositionLeft = hideAndShowPoints[1].X;
            cmt.DesiredPositionTop = hideAndShowPoints[1].Y;
            cmt.Command = command;
            cmt.Width = width;
            cmt.Height = height;
            cmt.Mode = CircularMinuteTimer.Modes.SIMPLE;

            return cmt;
        }

        public CircularMinuteTimer CreateExtendedCircularMinuteTimer(string name, Binding caption, double width, double height, Point[] hideAndShowPoints, Commands command, Type valueType, object value)
        {
            CircularMinuteTimer cmt = new CircularMinuteTimer();
            cmt.Name = name;
            cmt.SetBinding(CircularMinuteTimer.CaptionProperty, caption);
            Canvas.SetLeft(cmt, hideAndShowPoints[0].X);
            Canvas.SetTop(cmt, hideAndShowPoints[0].Y);
            cmt.DesiredPositionLeft = hideAndShowPoints[1].X;
            cmt.DesiredPositionTop = hideAndShowPoints[1].Y;
            cmt.Command = command;
            cmt.Width = width;
            cmt.Height = height;
            cmt.Mode = CircularMinuteTimer.Modes.EXTENDED;
            cmt.ValueType = valueType;
            
            cmt.Value = value;
            return cmt;
        }

        public InfoBoard CreateInfoBoard(TextBlock[] textBlocks, double positionLeft, double positionTop)
        {
            InfoBoard ib = new InfoBoard();
            foreach(TextBlock tb in textBlocks)
            {
                ib.AddTextBlock(tb);
            }
            ib.PositionLeft = positionLeft;
            ib.PositionTop = positionTop;
            return ib;
        }

        void OnPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(
                    this, new PropertyChangedEventArgs(propName));
        }

        protected void ParentWPF_method(object sender, DispatcherEventArgs args)
        {
            if (sender is CircularMinuteTimer)
            {
                CircularMinuteTimer cmt = sender as CircularMinuteTimer;
                
                eventDispatcher.ExecuteEvent(sender, args);
            }
        }
       
        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            String sql = "select gesture_base_name from neural_gestures;";
            ArrayList responseRows = dbUtils.PerformMySQLCommand(sql);
            string[] response = new string[responseRows.Count];
            int i = 0;
            foreach (ArrayList row in (ArrayList)responseRows)
            {
                response[i] = (string)row[0];
                ++i;
            }
            ArrayList responseRow = (ArrayList)responseRows[0];
            frameworkConstants.GesturesBases = new LinkedList<string>(response);
            frameworkConstants.BackgroundRecognition = true;
            //MessageBox.Show(response[0]);
            //MessageBox.Show(response[1]);
            eventDispatcher = new EventDispatcher();
            CircularMinuteTimer cmt = null;
            GraphicalEffectsMetadata gem = null;
            double width = Properties.Settings.Default.CMTWidth;
            double height = Properties.Settings.Default.CMTHeight;
            double cHPLeft = -width;
            double cHPRight = mainCanvas.ActualWidth;
            double cHPTop = -height;
            double cHPBottom = mainCanvas.ActualHeight;
            double cHPMiddleY = (mainCanvas.ActualHeight / 2) - (width / 2);

            Point HLeftTopPoint = new Point(cHPLeft, 5);
            Point HLeftCentralPoint = new Point(cHPLeft, cHPMiddleY);
            Point HLeftBottomPoint = new Point(cHPLeft, 490);
            Point HRightTopPoint = new Point(cHPRight, 5);
            Point HRightCentralPoint = new Point(cHPRight, cHPMiddleY);
            Point HRightBottomPoint = new Point(cHPRight, 490);

            Point SLeftTopPoint = new Point(5, 5);
            Point SLeftCentralPoint = new Point(5, cHPMiddleY);
            Point SLeftBottomPoint = new Point(5, 490);
            Point SRightTopPoint = new Point(575, 5);
            Point SRightCentralPoint = new Point(575, cHPMiddleY);
            Point SRightBottomPoint = new Point(575, 490);

            Point[] LeftTopPoints = { HLeftTopPoint, SLeftTopPoint };
            Point[] LeftCentralPoints = { HLeftCentralPoint, SLeftCentralPoint };
            Point[] LeftBottomPoints = { HLeftBottomPoint, SLeftBottomPoint };

            Point[] RightTopPoints = { HRightTopPoint, SRightTopPoint };
            Point[] RightCentralPoints = { HRightCentralPoint, SRightCentralPoint };
            Point[] RightBottomPoints = { HRightBottomPoint, SRightBottomPoint };

            canvasCMTPoints.Add(LeftTopPoints);
            canvasCMTPoints.Add(LeftCentralPoints);
            canvasCMTPoints.Add(LeftBottomPoints);
            canvasCMTPoints.Add(RightTopPoints);
            canvasCMTPoints.Add(RightCentralPoints);
            canvasCMTPoints.Add(RightBottomPoints);

            //InfoBoard ib = new InfoBoard();
            ArrayList al = new ArrayList();
            
            TextBlock tbStatus = new TextBlock();
            //tbStatus.Text = "Status";
            //tbStatus.FontSize = 30;
            //al.Add(tbStatus);
            //TextBlock tbRecordingIn = new TextBlock();
            //tbRecordingIn.Text = "Nagrywanie za: ";
            //al.Add(tbRecordingIn);
            TextBlock tbRecognizedGesture = new TextBlock();
            Binding recognizedGestureBinding = new Binding("Info_Recognized_Gesture");
            tbRecognizedGesture.SetBinding(TextBlock.TextProperty, recognizedGestureBinding);
            tbRecognizedGesture.FontSize = 15;
            //ib.AddTextBlock(tbRecognizedGesture);
            al.Add(tbRecognizedGesture);
            TextBlock tbGesture = new TextBlock();
            Binding gestureBinding = new Binding("GesturesStatus_Gesture");
            tbGesture.SetBinding(TextBlock.TextProperty, gestureBinding);
            tbGesture.FontSize = 20;
            //ib.AddTextBlock(tbGesture);
            al.Add(tbGesture);
            double PositionLeft = 5 + width / 2;
            double PositionTop = mainCanvas.ActualHeight/2;
            TextBlock[] tbs = (TextBlock[])al.ToArray(typeof(TextBlock));
            InfoBoard ib = CreateInfoBoard(tbs, PositionLeft, PositionTop);
            this.mainCanvas.Children.Add(ib);
            ibMap.Add(FrameworkConstants.IB_GESTURES, ib);
            al.Clear();

            TextBlock tbInformation = new TextBlock();
            Binding infoBinding = new Binding("Info_Information");
            tbInformation.SetBinding(TextBlock.TextProperty, infoBinding);
            tbInformation.FontSize = 30;
            al.Add(tbInformation);
            TextBlock tbCurrentInfo = new TextBlock();
            infoBinding = new Binding("CurrentInformation");
            tbCurrentInfo.SetBinding(TextBlock.TextProperty, infoBinding);
            //tbCurrentInfo.Text = "Tekst informacyjny";
            tbCurrentInfo.FontSize = 20;
            al.Add(tbCurrentInfo);

            PositionLeft = mainCanvas.ActualWidth/2;
            PositionTop = mainCanvas.ActualHeight/2;
            tbs = (TextBlock[])al.ToArray(typeof(TextBlock));
            ib = CreateInfoBoard(tbs, PositionLeft, PositionTop);
            this.mainCanvas.Children.Add(ib);
            ibMap.Add(FrameworkConstants.IB_INFORMATION, ib);
            //ib.ShowInfoBoard();
            al.Clear();

            //Main menu
            cmt = CreateCircularMinuteTimer(FrameworkConstants.CMT_GESTURES, new Binding("Gestures"), width, height, LeftTopPoints, Commands.LOAD_MENU_COMMAND);
            gem = graphicalEffects.GenerateGraphicalEffectsMetadata(cmt);
            cmtsHashMain.Add(cmt.Name, gem);
            this.mainCanvas.Children.Add(cmt);

            cmt = CreateCircularMinuteTimer(FrameworkConstants.CMT_LEARNING, new Binding("Learning"), width, height, LeftBottomPoints, Commands.LOAD_MENU_COMMAND);
            gem = graphicalEffects.GenerateGraphicalEffectsMetadata(cmt);
            cmtsHashMain.Add(cmt.Name, gem);
            this.mainCanvas.Children.Add(cmt);

            cmt = CreateCircularMinuteTimer(FrameworkConstants.CMT_CONFIGURATION, new Binding("Configuration"), width, height, RightTopPoints, Commands.LOAD_MENU_COMMAND);
            gem = graphicalEffects.GenerateGraphicalEffectsMetadata(cmt);
            cmtsHashMain.Add(cmt.Name, gem);
            this.mainCanvas.Children.Add(cmt);

            cmt = CreateCircularMinuteTimer(FrameworkConstants.CMT_EXIT, new Binding("Exit"), width, height, RightBottomPoints, Commands.LOAD_MENU_COMMAND);
            gem = graphicalEffects.GenerateGraphicalEffectsMetadata(cmt);
            cmtsHashMain.Add(cmt.Name, gem);
            this.mainCanvas.Children.Add(cmt);

            //TODO: Czy to jest gdzieś używane?
            cmt = CreateCircularMinuteTimer(FrameworkConstants.CMT_CONFIGURATION_MAIN_MENU, new Binding("MainMenu"), width, height, LeftBottomPoints, Commands.LOAD_MENU_COMMAND);
            gem = graphicalEffects.GenerateGraphicalEffectsMetadata(cmt);
            cmtsHashConfiguration.Add(cmt.Name, gem);
            this.mainCanvas.Children.Add(cmt);

            //Configuration menu
            cmt = CreateExtendedCircularMinuteTimer(FrameworkConstants.CMT_CONFIGURATION_LANGUAGE, new Binding("Configuration_Language"), width, height, LeftTopPoints, Commands.CHANGE_VALUE, typeof(Languages), frameworkConstants.CurrentLanguage);
            gem = graphicalEffects.GenerateGraphicalEffectsMetadata(cmt);
            cmtsHashConfiguration.Add(cmt.Name, gem);
            this.mainCanvas.Children.Add(cmt);

            cmt = CreateCircularMinuteTimer(FrameworkConstants.CMT_CONFIGURATION_APPLY_CHANGES, new Binding("ApplyChanges"), width, height, RightBottomPoints, Commands.APPLY_CHANGES);
            gem = graphicalEffects.GenerateGraphicalEffectsMetadata(cmt);
            cmtsHashConfiguration.Add(cmt.Name, gem);
            this.mainCanvas.Children.Add(cmt);

            cmt = CreateCircularMinuteTimer(FrameworkConstants.CMT_CONFIGURATION_EXPORT_NETWORK, new Binding("Configuration_Export_Network"), width, height, RightTopPoints, Commands.EXPORT_NETWORK);
            gem = graphicalEffects.GenerateGraphicalEffectsMetadata(cmt);
            cmtsHashConfiguration.Add(cmt.Name, gem);
            this.mainCanvas.Children.Add(cmt);

            //Learning menu
            cmt = CreateExtendedCircularMinuteTimer(FrameworkConstants.CMT_LEARNING_HIDDEN_NEURONS, new Binding("Learning_Hidden_Neurons"), width, height, LeftTopPoints, Commands.CHANGE_VALUE, typeof(int), frameworkConstants.HiddenNeurons);
            gem = graphicalEffects.GenerateGraphicalEffectsMetadata(cmt);
            cmtsHashLearning.Add(cmt.Name, gem);
            this.mainCanvas.Children.Add(cmt);

            //cmt = CreateCircularMinuteTimer(FrameworkConstants.CMT_LEARNING_APPLY_CHANGES, new Binding("ApplyChanges"), width, height, cHPRight, 490, 575, 490, Commands.APPLY_CHANGES);
            //gem = graphicalEffects.GenerateGraphicalEffectsMetadata(cmt);
            //cmtsHashLearning.Add(cmt.Name, gem);
            //this.mainCanvas.Children.Add(cmt);

            cmt = CreateCircularMinuteTimer(FrameworkConstants.CMT_LEARNING_MAIN_MENU, new Binding("MainMenu"), width, height, LeftBottomPoints, Commands.LOAD_MENU_COMMAND);
            gem = graphicalEffects.GenerateGraphicalEffectsMetadata(cmt);
            cmtsHashLearning.Add(cmt.Name, gem);
            this.mainCanvas.Children.Add(cmt);

            cmt = CreateExtendedCircularMinuteTimer(FrameworkConstants.CMT_LEARNING_EPOCHS, new Binding("Learning_Epochs"), width, height, LeftCentralPoints, Commands.CHANGE_VALUE, typeof(int), frameworkConstants.Epochs);
            gem = graphicalEffects.GenerateGraphicalEffectsMetadata(cmt);
            cmtsHashLearning.Add(cmt.Name, gem);
            this.mainCanvas.Children.Add(cmt);

            cmt = CreateExtendedCircularMinuteTimer(FrameworkConstants.CMT_LEARNING_MOMENTUM, new Binding("Learning_Momentum"), width, height, RightTopPoints, Commands.CHANGE_VALUE, typeof(double), frameworkConstants.Momentum);
            gem = graphicalEffects.GenerateGraphicalEffectsMetadata(cmt);
            cmtsHashLearning.Add(cmt.Name, gem);
            this.mainCanvas.Children.Add(cmt);

            cmt = CreateExtendedCircularMinuteTimer(FrameworkConstants.CMT_LEARNING_LEARN_RATE, new Binding("Learning_Learn_Rate"), width, height, RightCentralPoints, Commands.CHANGE_VALUE, typeof(double), frameworkConstants.LearnRate);
            gem = graphicalEffects.GenerateGraphicalEffectsMetadata(cmt);
            cmtsHashLearning.Add(cmt.Name, gem);
            this.mainCanvas.Children.Add(cmt);

            cmt = CreateExtendedCircularMinuteTimer(FrameworkConstants.CMT_LEARNING_LEARNING_BASE, new Binding("Learning_Learning_Base"), width, height, LeftTopPoints, Commands.CHANGE_VALUE, typeof(string), frameworkConstants.GesturesBases.First.Value);
            gem = graphicalEffects.GenerateGraphicalEffectsMetadata(cmt);
            cmtsHashLearning.Add(cmt.Name, gem);
            this.mainCanvas.Children.Add(cmt);

            cmt = CreateCircularMinuteTimer(FrameworkConstants.CMT_LEARNING_LEARN, new Binding("Learning_Learn"), width, height, RightTopPoints, Commands.LEARN);
            gem = graphicalEffects.GenerateGraphicalEffectsMetadata(cmt);
            cmtsHashLearning.Add(cmt.Name, gem);
            this.mainCanvas.Children.Add(cmt);

            cmt = CreateCircularMinuteTimer(FrameworkConstants.CMT_LEARNING_MORE, new Binding("Learning_More"), width, height, RightBottomPoints, Commands.MORE);
            gem = graphicalEffects.GenerateGraphicalEffectsMetadata(cmt);
            cmtsHashLearning.Add(cmt.Name, gem);
            this.mainCanvas.Children.Add(cmt);

            //Gestures menu
            cmt = CreateCircularMinuteTimer(FrameworkConstants.CMT_GESTURES_RECORDING, new Binding("Gestures_Recording"), width, height, LeftTopPoints, Commands.RECORD_GESTURE);
            gem = graphicalEffects.GenerateGraphicalEffectsMetadata(cmt);
            cmtsHashGestures.Add(cmt.Name, gem);
            this.mainCanvas.Children.Add(cmt);

            cmt = CreateCircularMinuteTimer(FrameworkConstants.CMT_GESTURES_MAIN_MENU, new Binding("MainMenu"), width, height, LeftBottomPoints, Commands.LOAD_MENU_COMMAND);
            gem = graphicalEffects.GenerateGraphicalEffectsMetadata(cmt);
            cmtsHashGestures.Add(cmt.Name, gem);
            this.mainCanvas.Children.Add(cmt);

            cmt = CreateCircularMinuteTimer(FrameworkConstants.CMT_GESTURES_IS_CORRECT, new Binding("Gestures_Is_Correct"), width, height, RightTopPoints, Commands.IS_CORRECT);
            gem = graphicalEffects.GenerateGraphicalEffectsMetadata(cmt);
            cmtsHashGestures.Add(cmt.Name, gem);
            this.mainCanvas.Children.Add(cmt);

            cmt = CreateCircularMinuteTimer(FrameworkConstants.CMT_GESTURES_SAVE_TO_DB, new Binding("Gestures_Save_To_DB"), width, height, RightBottomPoints, Commands.SAVE_TO_DB);
            gem = graphicalEffects.GenerateGraphicalEffectsMetadata(cmt);
            cmtsHashGestures.Add(cmt.Name, gem);
            this.mainCanvas.Children.Add(cmt);

            cmt = CreateCircularMinuteTimer(FrameworkConstants.CMT_GESTURES_RESET, new Binding("Gestures_Reset_Training_Data"), width, height, RightTopPoints, Commands.RESET_TRAINING_DATA);
            gem = graphicalEffects.GenerateGraphicalEffectsMetadata(cmt);
            cmtsHashGestures.Add(cmt.Name, gem);
            this.mainCanvas.Children.Add(cmt);

            //cmt = CreateCircularMinuteTimer(FrameworkConstants.CMT_GESTURES_RELEARN, new Binding("Gestures_Relearn"), width, height, cHPRight, 490, 575, 490, Commands.RELEARN_WITH_CURRENT_TRAINING_DATA);
            //gem = graphicalEffects.GenerateGraphicalEffectsMetadata(cmt);
            //cmtsHashGestures.Add(cmt.Name, gem);
            //this.mainCanvas.Children.Add(cmt);

            cmt = CreateCircularMinuteTimer(FrameworkConstants.CMT_GESTURES_LEARN_GESTURE, new Binding("Gestures_Learn_Gesture"), width, height, RightCentralPoints, Commands.SELECT_GESTURE);
            gem = graphicalEffects.GenerateGraphicalEffectsMetadata(cmt);
            cmtsHashGestures.Add(cmt.Name, gem);
            this.mainCanvas.Children.Add(cmt);

            cmt = CreateCircularMinuteTimer(FrameworkConstants.CMT_GESTURES_LEARN_GESTURE_BACK, new Binding("MainMenu"), width, height, LeftBottomPoints, Commands.UNHIDE_GESTURES);
            gem = graphicalEffects.GenerateGraphicalEffectsMetadata(cmt);
            cmtsHashGestures.Add(cmt.Name, gem);
            this.mainCanvas.Children.Add(cmt);

            cmt = CreateCircularMinuteTimer(FrameworkConstants.CMT_GESTURES_LEARN_GESTURE_CONFIRM, new Binding("ApplyChanges"), width, height, RightBottomPoints, Commands.APPLY_CHANGES);
            gem = graphicalEffects.GenerateGraphicalEffectsMetadata(cmt);
            cmtsHashGestures.Add(cmt.Name, gem);
            this.mainCanvas.Children.Add(cmt);

            cmt = CreateCircularMinuteTimer(FrameworkConstants.CMT_GESTURES_LEARN_GESTURE_MORE, new Binding("Learning_More"), width, height, RightCentralPoints, Commands.MORE);
            gem = graphicalEffects.GenerateGraphicalEffectsMetadata(cmt);
            cmtsHashGestures.Add(cmt.Name, gem);
            this.mainCanvas.Children.Add(cmt);

            foreach (CircularMinuteTimer cmt1 in FindVisualChildren<CircularMinuteTimer>(this))
            {
                cmt1.CallParentWindow += new EventHandler<DispatcherEventArgs>(ParentWPF_method);
                cmtsMap.Add(cmt1.Name, cmt1);
            }
            DispatcherEventArgs dea = new DispatcherEventArgs("onload", Commands.LOAD_MAIN_MENU, 1);
            eventDispatcher.ExecuteEvent(this, dea);

            //Thread thread = new Thread(new ThreadStart(WorkThreadFunction));
            thread = new Thread(delegate() { Application.Current.Dispatcher.BeginInvoke(new ThreadStart(() => WorkThreadFunction()), null); });
            //thread.SetApartmentState(ApartmentState.STA);
            //thread.Start();

            kinectFacility.Initialize(this, null);
        }

        public int CreateGesturesCMTs(ArrayList names)
        {
            int numberOfGesturesPerScreen = 3;
            double height = Properties.Settings.Default.CMTHeight;
            double width = Properties.Settings.Default.CMTWidth;
            int screens = 0;
            if (names.Count % numberOfGesturesPerScreen == 0)
                screens = (int)(names.Count / numberOfGesturesPerScreen);
            else
                screens = (int)(names.Count / numberOfGesturesPerScreen) + 1;

            ArrayList gesturesAvailablePosistions = new ArrayList();
            gesturesAvailablePosistions.Add(canvasCMTPoints[0]);
            gesturesAvailablePosistions.Add(canvasCMTPoints[1]);
            gesturesAvailablePosistions.Add(canvasCMTPoints[3]);
            //gesturesAvailablePosistions.Add(canvasCMTPoints[4]);

            for (int i = 0; i < names.Count; ++i)
            {
                CircularMinuteTimer cmt = CreateCircularMinuteTimer(FrameworkConstants.GESTURE_PREFIX + i, ((GestureMetadata)names[i]).GestureName, width, height, (Point[])(gesturesAvailablePosistions[i % gesturesAvailablePosistions.Count]), Commands.GESTURE_SELECTED);
                GraphicalEffectsMetadata gem = graphicalEffects.GenerateGraphicalEffectsMetadata(cmt);
                cmtsHashLearningGestures.Add(cmt.Name, gem);
                cmt.CallParentWindow += new EventHandler<DispatcherEventArgs>(ParentWPF_method);
                cmtsMap.Add(cmt.Name, cmt);
                this.mainCanvas.Children.Add(cmt);
            }
            return screens;
        }

        public void DestroyGesturesCMTs()
        {
            foreach (String name in cmtsHashLearningGestures.Keys)
            {
                CircularMinuteTimer cmt = (CircularMinuteTimer)cmtsMap[name];
                this.mainCanvas.Children.Remove(cmt);
                cmtsMap.Remove(name);
            }
            cmtsHashLearningGestures.Clear();
        }
    }
}
