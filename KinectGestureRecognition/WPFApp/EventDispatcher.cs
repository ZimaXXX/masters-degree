using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections;
using HAKGERSoft.Synapse;
using System.Xml.Linq;
using System.IO;

namespace WPFApp
{
    public enum ScreenCounter
    {
        First,
        Second
    }
    public class EventDispatcher
    {
        bool isGestureAlreadySaved = false;
        int currentGestureScreen = 1;
        private ScreenCounter currentLearningScreen = ScreenCounter.First;
        LinkedList<GraphicalEffectsMetadata> gemsGesturesShown = new LinkedList<GraphicalEffectsMetadata>();
        private string selectedGesture = "";
        GraphicalEffects graphicalEffects;
        public EventDispatcher()
        {
            graphicalEffects = new GraphicalEffects();
        }
        public void ExecuteEvent(object sender, DispatcherEventArgs args)
        {
            App app = ((App)Application.Current);
            MainWindow mw = (MainWindow)app.MainWindow;
            Canvas canvas = (Canvas)mw.FindName(Properties.Settings.Default.MainCanvasName);
            CircularMinuteTimer cmt = sender as CircularMinuteTimer;
            string caption = args.caption;
            Commands command = args.command;
            double animationTime = args.animationTime;

            //MessageBox.Show("Caption: " + args.caption + ", Command: " + args.command);

            switch (command)
            {
                case Commands.LOAD_MAIN_MENU:
                {
                    mw.frameworkConstants.AppendToFile("###############\n");
                    mw.frameworkConstants.AppendToFile("Application started\n");
                    mw.frameworkConstants.AppendToFile("###############\n");
                    LinkedList<GraphicalEffectsMetadata> gems = new LinkedList<GraphicalEffectsMetadata>();
                    gems.AddLast(mw.cmtsHashMain[FrameworkConstants.CMT_GESTURES] as GraphicalEffectsMetadata);
                    gems.AddLast(mw.cmtsHashMain[FrameworkConstants.CMT_LEARNING] as GraphicalEffectsMetadata);
                    gems.AddLast(mw.cmtsHashMain[FrameworkConstants.CMT_CONFIGURATION] as GraphicalEffectsMetadata);
                    gems.AddLast(mw.cmtsHashMain[FrameworkConstants.CMT_EXIT] as GraphicalEffectsMetadata);
                    mw.frameworkConstants.CurrentPage = Pages.MAIN_PAGE;
                    graphicalEffects.UnhideCmtsOnCanvas(gems, animationTime);

                    //TEMPORARY
                    mw.frameworkConstants.HiddenNeurons = (int)((CircularMinuteTimer)(mw.cmtsMap[FrameworkConstants.CMT_LEARNING_HIDDEN_NEURONS])).Value;
                    mw.frameworkConstants.Epochs = (int)((CircularMinuteTimer)(mw.cmtsMap[FrameworkConstants.CMT_LEARNING_EPOCHS])).Value;
                    mw.frameworkConstants.Momentum = (double)((CircularMinuteTimer)(mw.cmtsMap[FrameworkConstants.CMT_LEARNING_MOMENTUM])).Value;
                    mw.frameworkConstants.LearnRate = (double)((CircularMinuteTimer)(mw.cmtsMap[FrameworkConstants.CMT_LEARNING_LEARN_RATE])).Value;

                    String gesture_base_name = (String)((CircularMinuteTimer)(mw.cmtsMap[FrameworkConstants.CMT_LEARNING_LEARNING_BASE])).Value;
                    String sql = "select learn_xml, gestures_map_xml from neural_gestures where gesture_base_name = '" + gesture_base_name + "'";
                    ArrayList al = mw.dbUtils.PerformMySQLCommand(sql);
                    String learn_xml = (String)((ArrayList)al[0])[0];
                    String gestures_map_xml = (String)((ArrayList)al[0])[1];
                    ArrayList learnElements = SerializationUtils.DeSerializeArrayList(learn_xml, typeof(LearnElement));
                    ArrayList gestures_map_al = SerializationUtils.DeSerializeArrayList(gestures_map_xml, typeof(GestureMetadata));
                    mw.frameworkConstants.GesturesList = gestures_map_al;
                    int screens = mw.CreateGesturesCMTs(mw.frameworkConstants.GesturesList);

                    mw.frameworkConstants.StartMeasuring();
                    List<TrainingData> training = new List<TrainingData>();
                    foreach (LearnElement le in learnElements)
                    {
                        training.Add(le.ToTrainingData());
                    }
                    mw.currentTrainingData = training;
                    mw.nnf.LearnNetwork(training);

                    
                    //~TEMPORARY

                    break;
                }
                case Commands.LOAD_MENU_COMMAND:
                {
                    LinkedList<GraphicalEffectsMetadata> gemsMain = new LinkedList<GraphicalEffectsMetadata>();
                    gemsMain.AddLast(mw.cmtsHashMain[FrameworkConstants.CMT_GESTURES] as GraphicalEffectsMetadata);
                    gemsMain.AddLast(mw.cmtsHashMain[FrameworkConstants.CMT_LEARNING] as GraphicalEffectsMetadata);
                    gemsMain.AddLast(mw.cmtsHashMain[FrameworkConstants.CMT_CONFIGURATION] as GraphicalEffectsMetadata);
                    gemsMain.AddLast(mw.cmtsHashMain[FrameworkConstants.CMT_EXIT] as GraphicalEffectsMetadata);

                    LinkedList<GraphicalEffectsMetadata> gemsConfiguration = new LinkedList<GraphicalEffectsMetadata>();
                    gemsConfiguration.AddLast(mw.cmtsHashConfiguration[FrameworkConstants.CMT_CONFIGURATION_LANGUAGE] as GraphicalEffectsMetadata);
                    gemsConfiguration.AddLast(mw.cmtsHashConfiguration[FrameworkConstants.CMT_CONFIGURATION_APPLY_CHANGES] as GraphicalEffectsMetadata);
                    gemsConfiguration.AddLast(mw.cmtsHashConfiguration[FrameworkConstants.CMT_CONFIGURATION_MAIN_MENU] as GraphicalEffectsMetadata);
                    gemsConfiguration.AddLast(mw.cmtsHashConfiguration[FrameworkConstants.CMT_CONFIGURATION_EXPORT_NETWORK] as GraphicalEffectsMetadata);

                    LinkedList<GraphicalEffectsMetadata> gemsGesturesEveryScreen = new LinkedList<GraphicalEffectsMetadata>();
                    LinkedList<GraphicalEffectsMetadata> gemsGesturesSecondScreen = new LinkedList<GraphicalEffectsMetadata>();
                    gemsGesturesEveryScreen.AddLast(mw.cmtsHashGestures[FrameworkConstants.CMT_GESTURES_MAIN_MENU] as GraphicalEffectsMetadata);
                    gemsGesturesEveryScreen.AddLast(mw.cmtsHashGestures[FrameworkConstants.CMT_GESTURES_RECORDING] as GraphicalEffectsMetadata);
                    gemsGesturesSecondScreen.AddLast(mw.cmtsHashGestures[FrameworkConstants.CMT_GESTURES_IS_CORRECT] as GraphicalEffectsMetadata);
                    gemsGesturesSecondScreen.AddLast(mw.cmtsHashGestures[FrameworkConstants.CMT_GESTURES_SAVE_TO_DB] as GraphicalEffectsMetadata);
                    gemsGesturesSecondScreen.AddLast(mw.cmtsHashGestures[FrameworkConstants.CMT_GESTURES_LEARN_GESTURE] as GraphicalEffectsMetadata);

                    //LinkedList<GraphicalEffectsMetadata> gemsStatistics = new LinkedList<GraphicalEffectsMetadata>();
                    //gemsStatistics.AddLast(mw.cmtsHashConfiguration[FrameworkConstants.CMT_CONFIGURATION_MAIN_MENU] as GraphicalEffectsMetadata);

                    LinkedList<GraphicalEffectsMetadata> gemsLearningFirstScreen = new LinkedList<GraphicalEffectsMetadata>();
                    LinkedList<GraphicalEffectsMetadata> gemsLearningSecondScreen = new LinkedList<GraphicalEffectsMetadata>();
                    LinkedList<GraphicalEffectsMetadata> gemsLearningEveryScreen = new LinkedList<GraphicalEffectsMetadata>();
                    gemsLearningEveryScreen.AddLast(mw.cmtsHashLearning[FrameworkConstants.CMT_LEARNING_MAIN_MENU] as GraphicalEffectsMetadata);
                    gemsLearningFirstScreen.AddLast(mw.cmtsHashLearning[FrameworkConstants.CMT_LEARNING_LEARNING_BASE] as GraphicalEffectsMetadata);
                    gemsLearningFirstScreen.AddLast(mw.cmtsHashLearning[FrameworkConstants.CMT_LEARNING_LEARN] as GraphicalEffectsMetadata);
                    gemsLearningEveryScreen.AddLast(mw.cmtsHashLearning[FrameworkConstants.CMT_LEARNING_MORE] as GraphicalEffectsMetadata);
                    gemsLearningSecondScreen.AddLast(mw.cmtsHashLearning[FrameworkConstants.CMT_LEARNING_HIDDEN_NEURONS] as GraphicalEffectsMetadata);
                    //gemsLearningSecondScreen.AddLast(mw.cmtsHashLearning[FrameworkConstants.CMT_LEARNING_APPLY_CHANGES] as GraphicalEffectsMetadata);
                    gemsLearningSecondScreen.AddLast(mw.cmtsHashLearning[FrameworkConstants.CMT_LEARNING_EPOCHS] as GraphicalEffectsMetadata);
                    gemsLearningSecondScreen.AddLast(mw.cmtsHashLearning[FrameworkConstants.CMT_LEARNING_MOMENTUM] as GraphicalEffectsMetadata);
                    gemsLearningSecondScreen.AddLast(mw.cmtsHashLearning[FrameworkConstants.CMT_LEARNING_LEARN_RATE] as GraphicalEffectsMetadata);


                    
                    if (cmt.Name.Equals(FrameworkConstants.CMT_GESTURES))
                    {
                        mw.frameworkConstants.BackgroundRecognition = false;
                        mw.frameworkConstants.CurrentPage = Pages.GESTURES_PAGE;
                        ((InfoBoard)mw.ibMap[FrameworkConstants.IB_GESTURES]).ShowInfoBoard();
                        graphicalEffects.HideCmtsOnCanvas(gemsMain, animationTime, Properties.Settings.Default.CombinedAnimationDirection);
                        graphicalEffects.UnhideCmtsOnCanvas(gemsGesturesEveryScreen, animationTime);
                    }
                    else if (cmt.Name.Equals(FrameworkConstants.CMT_LEARNING))
                    {
                        mw.frameworkConstants.CurrentPage = Pages.LEARNING_PAGE;
                        graphicalEffects.HideCmtsOnCanvas(gemsMain, animationTime, Properties.Settings.Default.CombinedAnimationDirection);
                        graphicalEffects.UnhideCmtsOnCanvas(gemsLearningEveryScreen, animationTime);
                        graphicalEffects.UnhideCmtsOnCanvas(gemsLearningFirstScreen, animationTime);
                        //currentLearningScreen = gemsLearningFirstScreen;
                    }
                    else if (cmt.Name.Equals(FrameworkConstants.CMT_EXIT))
                    {
                        //mw.frameworkConstants.CurrentPage = Pages.STATISTICS_PAGE;
                        //graphicalEffects.HideCmtsOnCanvas(gemsMain, animationTime, Properties.Settings.Default.CombinedAnimationDirection);
                        //graphicalEffects.UnhideCmtsOnCanvas(gemsStatistics, animationTime);
                        Application.Current.Shutdown();
                    }
                    else if (cmt.Name.Equals(FrameworkConstants.CMT_CONFIGURATION))
                    {
                        mw.frameworkConstants.CurrentPage = Pages.CONFIGURATION_PAGE;
                        graphicalEffects.HideCmtsOnCanvas(gemsMain, animationTime, Properties.Settings.Default.CombinedAnimationDirection);
                        graphicalEffects.UnhideCmtsOnCanvas(gemsConfiguration, animationTime);
                    }
                    else if (cmt.Name.Equals(FrameworkConstants.CMT_CONFIGURATION_MAIN_MENU))
                    {
                        mw.frameworkConstants.CurrentPage = Pages.MAIN_PAGE;
                        graphicalEffects.HideCmtsOnCanvas(gemsConfiguration, animationTime, Properties.Settings.Default.CombinedAnimationDirection);
                        graphicalEffects.UnhideCmtsOnCanvas(gemsMain, animationTime);
                        String s = ((CircularMinuteTimer)mw.cmtsMap[FrameworkConstants.CMT_CONFIGURATION_LANGUAGE]).Value.ToString();
                        mw.frameworkConstants.NotifyPropertiesChanged();
                        String s1 = ((CircularMinuteTimer)mw.cmtsMap[FrameworkConstants.CMT_CONFIGURATION_LANGUAGE]).Value.ToString();
                    }
                    else if (cmt.Name.Equals(FrameworkConstants.CMT_LEARNING_MAIN_MENU))
                    {
                        mw.frameworkConstants.CurrentPage = Pages.MAIN_PAGE;
                        graphicalEffects.HideCmtsOnCanvas(gemsLearningFirstScreen, animationTime, Properties.Settings.Default.CombinedAnimationDirection);
                        graphicalEffects.HideCmtsOnCanvas(gemsLearningSecondScreen, animationTime, Properties.Settings.Default.CombinedAnimationDirection);
                        graphicalEffects.HideCmtsOnCanvas(gemsLearningEveryScreen, animationTime, Properties.Settings.Default.CombinedAnimationDirection);
                        graphicalEffects.UnhideCmtsOnCanvas(gemsMain, animationTime);
                    }
                    else if (cmt.Name.Equals(FrameworkConstants.CMT_GESTURES_MAIN_MENU))
                    {
                        mw.frameworkConstants.CurrentPage = Pages.MAIN_PAGE;
                        ((InfoBoard)mw.ibMap[FrameworkConstants.IB_GESTURES]).HideInfoBoard();
                        graphicalEffects.HideCmtsOnCanvas(gemsGesturesEveryScreen, animationTime, Properties.Settings.Default.CombinedAnimationDirection);
                        graphicalEffects.HideCmtsOnCanvas(gemsGesturesSecondScreen, animationTime, Properties.Settings.Default.CombinedAnimationDirection);
                        graphicalEffects.UnhideCmtsOnCanvas(gemsMain, animationTime);
                        mw.frameworkConstants.BackgroundRecognition = true;
                    }         
                    break;
                }
                case Commands.CHANGE_VALUE:
                {
                    cmt.NextValue();
                    break;
                }
                case Commands.APPLY_CHANGES:
                {
                    if (cmt.Name.Equals(FrameworkConstants.CMT_CONFIGURATION_APPLY_CHANGES))
                    {
                        mw.frameworkConstants.CurrentLanguage = (Languages)((CircularMinuteTimer)(mw.cmtsMap[FrameworkConstants.CMT_CONFIGURATION_LANGUAGE])).Value;
                        mw.frameworkConstants.InitializeCaptions(mw.frameworkConstants.CurrentLanguage);
                        mw.frameworkConstants.CurrentInformation = mw.frameworkConstants.Info_ChangesApplied;
                        ((InfoBoard)mw.ibMap[FrameworkConstants.IB_INFORMATION]).ShowAndHideInfoBoard();
                    }
                    else if (cmt.Name.Equals(FrameworkConstants.CMT_GESTURES_LEARN_GESTURE_CONFIRM))
                    {
                        if (selectedGesture == "")
                        {
                            mw.frameworkConstants.CurrentInformation = mw.frameworkConstants.Info_NoGestureSelected;
                            ((InfoBoard)mw.ibMap[FrameworkConstants.IB_INFORMATION]).ShowAndHideInfoBoard();
                        }
                        else if (!isGestureAlreadySaved && selectedGesture != "")
                        {
                            //dostosowac ponizsza linijke!
                            foreach (GestureMetadata gm in mw.frameworkConstants.GesturesList)
                            {
                                if (gm.GestureName.Equals(selectedGesture))
                                {
                                    double[] output = mw.nnf.GenerateOutput(gm.OutputPosition, gm.NumberOfOutputs);
                                    mw.currentTrainingData.Add(new TrainingData(mw.nnf.CurrentInput, output));
                                    isGestureAlreadySaved = true;
                                    mw.frameworkConstants.CurrentInformation = mw.frameworkConstants.Info_GestureSaved;
                                    ((InfoBoard)mw.ibMap[FrameworkConstants.IB_INFORMATION]).ShowAndHideInfoBoard();
                                    break;
                                }
                            }
                            if (!isGestureAlreadySaved)
                            {
                                //TODO: Reaction on error when there is no selectedGesture in gesturesList (theoretically cannot) 
                                mw.frameworkConstants.CurrentInformation = "Error";
                                ((InfoBoard)mw.ibMap[FrameworkConstants.IB_INFORMATION]).ShowAndHideInfoBoard();
                            }

                        }
                        else
                        {
                            mw.frameworkConstants.CurrentInformation = mw.frameworkConstants.Info_GestureAlreadySaved;
                            ((InfoBoard)mw.ibMap[FrameworkConstants.IB_INFORMATION]).ShowAndHideInfoBoard();
                        }
                    }
                    
                    break;
                }
                case Commands.MORE:
                {
                    if (cmt.Name.Equals(FrameworkConstants.CMT_LEARNING_MORE))
                    {
                        LinkedList<GraphicalEffectsMetadata> gemsLearningFirstScreen = new LinkedList<GraphicalEffectsMetadata>();
                        LinkedList<GraphicalEffectsMetadata> gemsLearningSecondScreen = new LinkedList<GraphicalEffectsMetadata>();
                        //LinkedList<GraphicalEffectsMetadata> gemsLearningEveryScreen = new LinkedList<GraphicalEffectsMetadata>();
                        //gemsLearningEveryScreen.AddLast(mw.cmtsHashLearning[FrameworkConstants.CMT_LEARNING_MAIN_MENU] as GraphicalEffectsMetadata);
                        gemsLearningFirstScreen.AddLast(mw.cmtsHashLearning[FrameworkConstants.CMT_LEARNING_LEARNING_BASE] as GraphicalEffectsMetadata);
                        gemsLearningFirstScreen.AddLast(mw.cmtsHashLearning[FrameworkConstants.CMT_LEARNING_LEARN] as GraphicalEffectsMetadata);
                        //gemsLearningEveryScreen.AddLast(mw.cmtsHashLearning[FrameworkConstants.CMT_LEARNING_MORE] as GraphicalEffectsMetadata);
                        gemsLearningSecondScreen.AddLast(mw.cmtsHashLearning[FrameworkConstants.CMT_LEARNING_HIDDEN_NEURONS] as GraphicalEffectsMetadata);
                        //gemsLearningSecondScreen.AddLast(mw.cmtsHashLearning[FrameworkConstants.CMT_LEARNING_APPLY_CHANGES] as GraphicalEffectsMetadata);
                        gemsLearningSecondScreen.AddLast(mw.cmtsHashLearning[FrameworkConstants.CMT_LEARNING_EPOCHS] as GraphicalEffectsMetadata);
                        gemsLearningSecondScreen.AddLast(mw.cmtsHashLearning[FrameworkConstants.CMT_LEARNING_MOMENTUM] as GraphicalEffectsMetadata);
                        gemsLearningSecondScreen.AddLast(mw.cmtsHashLearning[FrameworkConstants.CMT_LEARNING_LEARN_RATE] as GraphicalEffectsMetadata);
                        if (currentLearningScreen == ScreenCounter.First)
                        {
                            graphicalEffects.HideCmtsOnCanvas(gemsLearningFirstScreen, animationTime, Properties.Settings.Default.CombinedAnimationDirection);
                            graphicalEffects.UnhideCmtsOnCanvas(gemsLearningSecondScreen, animationTime);
                            currentLearningScreen = ScreenCounter.Second;
                        }
                        else if (currentLearningScreen == ScreenCounter.Second)
                        {
                            graphicalEffects.HideCmtsOnCanvas(gemsLearningSecondScreen, animationTime, Properties.Settings.Default.CombinedAnimationDirection);
                            graphicalEffects.UnhideCmtsOnCanvas(gemsLearningFirstScreen, animationTime);
                            currentLearningScreen = ScreenCounter.First;
                        }
                    }
                    else if (cmt.Name.Equals(FrameworkConstants.CMT_GESTURES_LEARN_GESTURE_MORE))
                    {
                        int numberOfGesturesOnScreen = 3;
                        LinkedList<GraphicalEffectsMetadata> gemsGesturesToShow = new LinkedList<GraphicalEffectsMetadata>();
                        LinkedList<GraphicalEffectsMetadata> gemsGesturesToHide = new LinkedList<GraphicalEffectsMetadata>();

                        gemsGesturesToHide = gemsGesturesShown;
                        for (int i = numberOfGesturesOnScreen * currentGestureScreen; i < mw.frameworkConstants.GesturesList.Count && i < numberOfGesturesOnScreen * (currentGestureScreen+1); ++i)
                        {
                            Console.WriteLine("i: " + i);
                            gemsGesturesToShow.AddLast(mw.cmtsHashLearningGestures[FrameworkConstants.GESTURE_PREFIX + i] as GraphicalEffectsMetadata);
                        }

                        graphicalEffects.HideCmtsOnCanvas(gemsGesturesToHide, animationTime, Properties.Settings.Default.CombinedAnimationDirection);
                        graphicalEffects.UnhideCmtsOnCanvas(gemsGesturesToShow, animationTime);
                        gemsGesturesShown = gemsGesturesToShow;
                        if (mw.frameworkConstants.GesturesList.Count - (currentGestureScreen+1) * numberOfGesturesOnScreen <= 0)
                        {
                            currentGestureScreen = 0;
                        }
                        else
                        {
                            ++currentGestureScreen;
                        }
                    }
                    break;
                }
                case Commands.LEARN:
                {
                    if (cmt.Name.Equals(FrameworkConstants.CMT_LEARNING_LEARN))
                    {
                        mw.frameworkConstants.StartMeasuring();

                        mw.frameworkConstants.HiddenNeurons = (int)((CircularMinuteTimer)(mw.cmtsMap[FrameworkConstants.CMT_LEARNING_HIDDEN_NEURONS])).Value;
                        mw.frameworkConstants.Epochs = (int)((CircularMinuteTimer)(mw.cmtsMap[FrameworkConstants.CMT_LEARNING_EPOCHS])).Value;
                        mw.frameworkConstants.Momentum = (double)((CircularMinuteTimer)(mw.cmtsMap[FrameworkConstants.CMT_LEARNING_MOMENTUM])).Value;
                        mw.frameworkConstants.LearnRate = (double)((CircularMinuteTimer)(mw.cmtsMap[FrameworkConstants.CMT_LEARNING_LEARN_RATE])).Value;
                        //TODO: Learn network here
                        String gesture_base_name = (String)((CircularMinuteTimer)(mw.cmtsMap[FrameworkConstants.CMT_LEARNING_LEARNING_BASE])).Value;
                        String sql = "select learn_xml, gestures_map_xml from neural_gestures where gesture_base_name = '" + gesture_base_name + "'";
                        ArrayList al = mw.dbUtils.PerformMySQLCommand(sql);
                        String learn_xml = (String)((ArrayList)al[0])[0];
                        String gestures_map_xml = (String)((ArrayList)al[0])[1];
                        ArrayList learnElements = SerializationUtils.DeSerializeArrayList(learn_xml, typeof(LearnElement));
                        ArrayList gestures_map_al = SerializationUtils.DeSerializeArrayList(gestures_map_xml, typeof(GestureMetadata));
                        mw.frameworkConstants.GesturesList = gestures_map_al;
                        if (mw.cmtsHashLearningGestures.Count > 0)
                            mw.DestroyGesturesCMTs();
                        int screens = mw.CreateGesturesCMTs(mw.frameworkConstants.GesturesList);
                        List<TrainingData> training = new List<TrainingData>();
                        foreach (LearnElement le in learnElements)
                        {
                            training.Add(le.ToTrainingData());
                        }
                        mw.currentTrainingData = training;
                        mw.nnf.LearnNetwork(training);
                    }
                    break;
                }
                case Commands.RECORD_GESTURE:
                {
                    if (cmt.Name.Equals(FrameworkConstants.CMT_GESTURES_RECORDING))
                    {
                        mw.frameworkConstants.StartMeasuring();
                        mw.nnf.CurrentInput = null;
                        mw.nnf.CurrentOutput = null;
                        LinkedList<GraphicalEffectsMetadata> gemsGesturesEveryScreen = new LinkedList<GraphicalEffectsMetadata>();
                        LinkedList<GraphicalEffectsMetadata> gemsGesturesSecondScreen = new LinkedList<GraphicalEffectsMetadata>();
                        gemsGesturesEveryScreen.AddLast(mw.cmtsHashGestures[FrameworkConstants.CMT_GESTURES_MAIN_MENU] as GraphicalEffectsMetadata);
                        gemsGesturesEveryScreen.AddLast(mw.cmtsHashGestures[FrameworkConstants.CMT_GESTURES_RECORDING] as GraphicalEffectsMetadata);
                        gemsGesturesSecondScreen.AddLast(mw.cmtsHashGestures[FrameworkConstants.CMT_GESTURES_IS_CORRECT] as GraphicalEffectsMetadata);
                        gemsGesturesSecondScreen.AddLast(mw.cmtsHashGestures[FrameworkConstants.CMT_GESTURES_SAVE_TO_DB] as GraphicalEffectsMetadata);
                        gemsGesturesSecondScreen.AddLast(mw.cmtsHashGestures[FrameworkConstants.CMT_GESTURES_LEARN_GESTURE] as GraphicalEffectsMetadata);
                        graphicalEffects.HideCmtsOnCanvas(gemsGesturesEveryScreen, animationTime, Properties.Settings.Default.CombinedAnimationDirection);
                        graphicalEffects.HideCmtsOnCanvas(gemsGesturesSecondScreen, animationTime, Properties.Settings.Default.CombinedAnimationDirection);
                       
                        mw.nnf.ProcessRecognition(canvas, mw.frameworkConstants.GesturesList, RECOGNITION_MODE.SINGLE);
                        isGestureAlreadySaved = false;
                    }
                    break;                    
                }
                case Commands.UNHIDE_GESTURES:
                {
                    if (cmt == null)//From NeuronNetworkFacility
                    {
                        LinkedList<GraphicalEffectsMetadata> gemsGesturesEveryScreen = new LinkedList<GraphicalEffectsMetadata>();
                        LinkedList<GraphicalEffectsMetadata> gemsGesturesSecondScreen = new LinkedList<GraphicalEffectsMetadata>();
                        gemsGesturesEveryScreen.AddLast(mw.cmtsHashGestures[FrameworkConstants.CMT_GESTURES_MAIN_MENU] as GraphicalEffectsMetadata);
                        gemsGesturesEveryScreen.AddLast(mw.cmtsHashGestures[FrameworkConstants.CMT_GESTURES_RECORDING] as GraphicalEffectsMetadata);
                        gemsGesturesSecondScreen.AddLast(mw.cmtsHashGestures[FrameworkConstants.CMT_GESTURES_IS_CORRECT] as GraphicalEffectsMetadata);
                        gemsGesturesSecondScreen.AddLast(mw.cmtsHashGestures[FrameworkConstants.CMT_GESTURES_SAVE_TO_DB] as GraphicalEffectsMetadata);
                        gemsGesturesSecondScreen.AddLast(mw.cmtsHashGestures[FrameworkConstants.CMT_GESTURES_LEARN_GESTURE] as GraphicalEffectsMetadata);
                        graphicalEffects.UnhideCmtsOnCanvas(gemsGesturesEveryScreen, animationTime);
                        graphicalEffects.UnhideCmtsOnCanvas(gemsGesturesSecondScreen, animationTime);
                    }
                    else if (cmt.Name.Equals(FrameworkConstants.CMT_GESTURES_LEARN_GESTURE_BACK))
                    {
                        LinkedList<GraphicalEffectsMetadata> gemsLearningGestures = new LinkedList<GraphicalEffectsMetadata>();
                        LinkedList<GraphicalEffectsMetadata> gemsGesturesToShow = new LinkedList<GraphicalEffectsMetadata>();
                        gemsGesturesToShow.AddLast(mw.cmtsHashGestures[FrameworkConstants.CMT_GESTURES_MAIN_MENU] as GraphicalEffectsMetadata);
                        gemsGesturesToShow.AddLast(mw.cmtsHashGestures[FrameworkConstants.CMT_GESTURES_RECORDING] as GraphicalEffectsMetadata);
                        gemsGesturesToShow.AddLast(mw.cmtsHashGestures[FrameworkConstants.CMT_GESTURES_IS_CORRECT] as GraphicalEffectsMetadata);
                        gemsGesturesToShow.AddLast(mw.cmtsHashGestures[FrameworkConstants.CMT_GESTURES_SAVE_TO_DB] as GraphicalEffectsMetadata);
                        gemsGesturesToShow.AddLast(mw.cmtsHashGestures[FrameworkConstants.CMT_GESTURES_LEARN_GESTURE] as GraphicalEffectsMetadata);

                        for (int i = 0; i < mw.frameworkConstants.GesturesList.Count; ++i)
                        {
                            gemsLearningGestures.AddLast(mw.cmtsHashLearningGestures[FrameworkConstants.GESTURE_PREFIX + i] as GraphicalEffectsMetadata);
                        }
                        gemsLearningGestures.AddLast(mw.cmtsHashGestures[FrameworkConstants.CMT_GESTURES_LEARN_GESTURE_BACK] as GraphicalEffectsMetadata);
                        gemsLearningGestures.AddLast(mw.cmtsHashGestures[FrameworkConstants.CMT_GESTURES_LEARN_GESTURE_CONFIRM] as GraphicalEffectsMetadata);
                        gemsLearningGestures.AddLast(mw.cmtsHashGestures[FrameworkConstants.CMT_GESTURES_LEARN_GESTURE_MORE] as GraphicalEffectsMetadata);
                        selectedGesture = "";
                        currentGestureScreen = 1;
                        ((InfoBoard)mw.ibMap[FrameworkConstants.IB_GESTURES]).ShowInfoBoard();
                        graphicalEffects.HideCmtsOnCanvas(gemsLearningGestures, animationTime, Properties.Settings.Default.CombinedAnimationDirection);
                        graphicalEffects.UnhideCmtsOnCanvas(gemsGesturesToShow, animationTime);
                        
                    }
                    break;
                }
                case Commands.IS_CORRECT:
                {
                    if (cmt.Name.Equals(FrameworkConstants.CMT_GESTURES_IS_CORRECT))
                    {
                        if (!isGestureAlreadySaved)
                        {
                            mw.currentTrainingData.Add(new TrainingData(mw.nnf.CurrentInput, mw.nnf.CurrentOutput));
                            isGestureAlreadySaved = true;
                            mw.frameworkConstants.CurrentInformation = mw.frameworkConstants.Info_GestureSaved;
                            ((InfoBoard)mw.ibMap[FrameworkConstants.IB_INFORMATION]).ShowAndHideInfoBoard();
                        }
                        else
                        {
                            mw.frameworkConstants.CurrentInformation = mw.frameworkConstants.Info_GestureAlreadySaved;
                            ((InfoBoard)mw.ibMap[FrameworkConstants.IB_INFORMATION]).ShowAndHideInfoBoard();
                        }
                    }
                    break;
                }
                case Commands.SAVE_TO_DB:
                {
                    if (cmt.Name.Equals(FrameworkConstants.CMT_GESTURES_SAVE_TO_DB))
                    {
                        mw.frameworkConstants.AppendToFile("Saving to database\n");
                        mw.frameworkConstants.StartMeasuring();
                        ArrayList altd = new ArrayList();
                        foreach(TrainingData td in mw.currentTrainingData)
                        {
                            altd.Add(new LearnElement(td));
                        }
                        string xml = SerializationUtils.SerializeArrayList(altd, typeof(LearnElement));
                        string gesture_base_name = (String)((CircularMinuteTimer)(mw.cmtsMap[FrameworkConstants.CMT_LEARNING_LEARNING_BASE])).Value;
                        string sql = "UPDATE neural_gestures SET learn_xml='" + xml + "' WHERE gesture_base_name='" + gesture_base_name + "';";
                        mw.dbUtils.PerformMySQLCommand(sql);
                        mw.frameworkConstants.CurrentInformation = mw.frameworkConstants.Info_DatabaseUpdated;
                        mw.frameworkConstants.EndMeasuring();
                        ((InfoBoard)mw.ibMap[FrameworkConstants.IB_INFORMATION]).ShowAndHideInfoBoard();
                        //string sql = "insert into neural_gestures  '" + gestures_base_name + "'
                    }
                    break;
                }
                case Commands.RESET_TRAINING_DATA:
                {                    
                    break;
                }
                case Commands.PROCESS_GESTURE:
                {
                    foreach (string gesture in mw.frameworkConstants.GesturesEventMap.Keys)
                    {
                        if (caption.Equals(gesture))
                        {
                            Commands command1 = (Commands)mw.frameworkConstants.GesturesEventMap[gesture];
                            DispatcherEventArgs dea = new DispatcherEventArgs(gesture, command1, Properties.Settings.Default.AnimationTime);
                            this.ExecuteEvent(this, dea);
                        }
                    }
                    break;
                }
                case Commands.SELECT_GESTURE:
                {
                    int numberOfGesturesOnScreen = 3;
                    LinkedList<GraphicalEffectsMetadata> gemsLearningGestures = new LinkedList<GraphicalEffectsMetadata>();
                    LinkedList<GraphicalEffectsMetadata> gemsGesturesToHide = new LinkedList<GraphicalEffectsMetadata>();
                    gemsGesturesToHide.AddLast(mw.cmtsHashGestures[FrameworkConstants.CMT_GESTURES_MAIN_MENU] as GraphicalEffectsMetadata);
                    gemsGesturesToHide.AddLast(mw.cmtsHashGestures[FrameworkConstants.CMT_GESTURES_RECORDING] as GraphicalEffectsMetadata);
                    gemsGesturesToHide.AddLast(mw.cmtsHashGestures[FrameworkConstants.CMT_GESTURES_IS_CORRECT] as GraphicalEffectsMetadata);
                    gemsGesturesToHide.AddLast(mw.cmtsHashGestures[FrameworkConstants.CMT_GESTURES_SAVE_TO_DB] as GraphicalEffectsMetadata);
                    gemsGesturesToHide.AddLast(mw.cmtsHashGestures[FrameworkConstants.CMT_GESTURES_LEARN_GESTURE] as GraphicalEffectsMetadata);
                    
                    for (int i = 0; i < mw.frameworkConstants.GesturesList.Count && i < numberOfGesturesOnScreen; ++i )
                    {
                        gemsLearningGestures.AddLast(mw.cmtsHashLearningGestures[FrameworkConstants.GESTURE_PREFIX + i] as GraphicalEffectsMetadata);
                        gemsGesturesShown.AddLast(mw.cmtsHashLearningGestures[FrameworkConstants.GESTURE_PREFIX + i] as GraphicalEffectsMetadata);
                    }
                    
                    gemsLearningGestures.AddLast(mw.cmtsHashGestures[FrameworkConstants.CMT_GESTURES_LEARN_GESTURE_BACK] as GraphicalEffectsMetadata);
                    gemsLearningGestures.AddLast(mw.cmtsHashGestures[FrameworkConstants.CMT_GESTURES_LEARN_GESTURE_CONFIRM] as GraphicalEffectsMetadata);
                    if(mw.frameworkConstants.GesturesList.Count > numberOfGesturesOnScreen)
                        gemsLearningGestures.AddLast(mw.cmtsHashGestures[FrameworkConstants.CMT_GESTURES_LEARN_GESTURE_MORE] as GraphicalEffectsMetadata);

                    ((InfoBoard)mw.ibMap[FrameworkConstants.IB_GESTURES]).HideInfoBoard();
                    graphicalEffects.HideCmtsOnCanvas(gemsGesturesToHide, animationTime, Properties.Settings.Default.CombinedAnimationDirection);
                    graphicalEffects.UnhideCmtsOnCanvas(gemsLearningGestures, animationTime);                    
                    break;
                }
                case Commands.GESTURE_SELECTED:
                {
                    mw.frameworkConstants.CurrentInformation = caption;
                    ((InfoBoard)mw.ibMap[FrameworkConstants.IB_INFORMATION]).ShowAndHideInfoBoard();
                    selectedGesture = caption;
                    break;
                }
                case Commands.EXPORT_NETWORK:
                {
                    String gesture_base_name = (String)((CircularMinuteTimer)(mw.cmtsMap[FrameworkConstants.CMT_LEARNING_LEARNING_BASE])).Value;
                    MLPSerializer serializer = new MLPSerializer();
                    XDocument doc = serializer.Serialize(mw.nnf.currentNetwork);
                    String path = "";
                    if(Properties.Settings.Default.ExportPath.Equals(""))
                        path = gesture_base_name + ".xml";
                    else
                        path = Properties.Settings.Default.ExportPath + gesture_base_name + ".xml";
                    doc.Save(path);
                    mw.frameworkConstants.CurrentInformation = mw.frameworkConstants.Info_NetworkExportedSuccesfullyTo + " : " + Directory.GetCurrentDirectory() + path;
                    ((InfoBoard)mw.ibMap[FrameworkConstants.IB_INFORMATION]).ShowAndHideInfoBoard();
                    break;
                }
                case Commands.SHOW_TEST_MESSAGE:
                {
                    MessageBox.Show("TEST: " + caption);
                    break;
                }
                case Commands.EMPTY_COMMAND:
                {
                    Console.WriteLine("EMPTY_COMMAND invoked by gesture: " + caption);
                    break;
                }
            }            
        }
    }
}