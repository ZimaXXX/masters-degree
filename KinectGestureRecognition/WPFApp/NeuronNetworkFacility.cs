using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using HAKGERSoft.Synapse;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using System.Timers;
using System.Windows.Threading;
using System.Collections;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Runtime.Remoting.Messaging;
using Microsoft.Research.Kinect.Nui;
using WPFApp.Kinect;

namespace WPFApp
{
    public enum RECOGNITION_MODE
    {
        SINGLE,
        BACKGROUND
    }
    public class NeuronNetworkFacility
    {
        public static double MIN_VALUE = 0.0001;
        public static double MAX_VALUE = 0.9999;

        private delegate BPResponse AsyncLearnMethodCaller(BPRequest bpr); 
        int HiddenNeurons { get; set; }
        double Momentum { get; set; }
        int Epochs { get; set; }
        double LearnRate { get; set; }
        public int RecordInterval { get; set; }
        private Canvas MainCanvas { get; set; }
        private List<double[]> capturedGesture = null;
        DispatcherTimer timer = null;
        public MultilayerPerceptron currentNetwork = null;
        ArrayList currentGesturesList = null;
        App app = null;
        MainWindow mw = null;
        RECOGNITION_MODE currentMode;
        public double[] CurrentInput { get; set; }
        public double[] CurrentOutput { get; set; }

        public NeuronNetworkFacility()
        {
            App app = ((App)Application.Current);
            mw = (MainWindow)app.MainWindow;
            capturedGesture = new List<double[]>();
            HiddenNeurons = Properties.Settings.Default.HiddenNeurons;
            Epochs = Properties.Settings.Default.Epochs;
            Momentum = Properties.Settings.Default.Momentum;
            LearnRate = Properties.Settings.Default.LearnRate;
            RecordInterval = 1000;
        }

        public NeuronNetworkFacility(int recordInterval)
        {
            App app = ((App)Application.Current);
            mw = (MainWindow)app.MainWindow;
            capturedGesture = new List<double[]>();
            HiddenNeurons = Properties.Settings.Default.HiddenNeurons;
            Epochs = Properties.Settings.Default.Epochs;
            Momentum = Properties.Settings.Default.Momentum;
            LearnRate = Properties.Settings.Default.LearnRate;
            RecordInterval = recordInterval;
        }

        public static T[] RandomPermutation<T>(T[] array)
        {
            T[] retArray = new T[array.Length];
            array.CopyTo(retArray, 0);

            Random random = new Random();
            for (int i = 0; i < array.Length; i += 1)
            {
                int swapIndex = random.Next(i, array.Length);
                if (swapIndex != i)
                {
                    T temp = retArray[i];
                    retArray[i] = retArray[swapIndex];
                    retArray[swapIndex] = temp;
                }
            }

            return retArray;
        }

        public static void AttachValuesToGestures(ArrayList gesturesList)
        {

        }

        //public static void AttachValuesToGestures(ArrayList gesturesList)
        //{
        //    int count = gesturesList.Count;
        //    //int i = 0;
        //    double value = 1.0 / (count - 1);
        //    //string[] keysArray = new string[count];
        //    //gesturesList..CopyTo(keysArray, 0);
        //    for (int i = 0; i < count; ++i)
        //    {
        //        GestureMetadata gesture = (GestureMetadata)gesturesList[i];
        //        if (i == 0)
        //            gesture.Value = 0.0001;
        //        else if (i == count - 1)
        //            gesture.Value = 0.9999;
        //        else
        //            gesture.Value = value * i;
        //        System.Console.WriteLine("Key: " + gesture.GestureName + " Value: " + gesture.Value.ToString());
        //        gesturesList[i] = gesture;
        //    }           
        //}

        public MultilayerPerceptron LearnNetwork(List<TrainingData> trainingList)
        {
            mw.frameworkConstants.AppendToFile("Learning network\n");
            GestureMetadata gm = (GestureMetadata)mw.frameworkConstants.GesturesList[0];
            int[] layers = new int[] { gm.NumberOfInputs, HiddenNeurons, gm.NumberOfOutputs };
            MultilayerPerceptron ann = new MLPGenerator().Create(layers, 1, new Sigmoid(2));
            System.Console.WriteLine("HiddenNeurons: " + HiddenNeurons + " Epochs: " + Epochs + " Momentum: " + Momentum + " LearnRate: " + LearnRate);
            ann.Reset(-1, 1);
            ann.Momentum = Momentum;
            ann.LearnFactor = LearnRate;
            //var res = ann.BP(new BPRequest(trainingList.ToArray(), Epochs));
            TrainingData[] tdArray = trainingList.ToArray();
            mw.frameworkConstants.AppendToFile("Number of examples: " + tdArray.Length + "\n");
            tdArray = RandomPermutation<TrainingData>(tdArray);
            AsyncLearnMethodCaller caller = new AsyncLearnMethodCaller(ann.BP);
            IAsyncResult result = caller.BeginInvoke(new BPRequest(tdArray, Epochs), new AsyncCallback(CallbackMethod), null);
            //var res = caller.EndInvoke(result);
            
            currentNetwork = ann;
            return ann;
        }

        public void CallbackMethod(IAsyncResult ar)
        {
            
            // Retrieve the delegate.
            AsyncResult result = (AsyncResult)ar;
            AsyncLearnMethodCaller caller = (AsyncLearnMethodCaller)result.AsyncDelegate;
            var res = caller.EndInvoke(ar);
            mw.frameworkConstants.CurrentInformation = mw.frameworkConstants.Info_NetworkLearned;
            InfoBoard ib = ((InfoBoard)mw.ibMap[FrameworkConstants.IB_INFORMATION]);
            mw.frameworkConstants.EndMeasuring();
            
            ib.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new System.Windows.Threading.DispatcherOperationCallback(delegate
           {
               ib.ShowAndHideInfoBoard();

               return null;
           }), null);
        } 

        public void CaptureMousePositions()
        {

            Point p = Mouse.GetPosition(MainCanvas);
            p.X = p.X / MainCanvas.ActualWidth;
            p.Y = p.Y / MainCanvas.ActualHeight;
            Console.WriteLine("X: " + p.X + " Y: " + p.Y);
            double[] inputs = new double[] { p.X, p.Y };
            capturedGesture.Add(inputs);
        }

        public void CaptureKinectPositions()
        {
            JointsCollection positions = mw.kinectFacility.LastRecordedJoints;
            ArrayList positionsAl = new ArrayList();
            // double HandLeftX = 0;
            // double HandRightX = 0;
            foreach (Joint joint in positions)
            {
                //Console.WriteLine("Joint Z: " + joint.Position.Z);
                switch (joint.ID)
                {
                    case JointID.HandLeft:
                        //Console.WriteLine("HandLeft");
                        //HandLeftX = mw.kinectFacility.getDisplayPosition(joint).X;
                        positionsAl.Add(mw.kinectFacility.getDisplayPosition(joint));
                        break;
                    case JointID.ElbowLeft:
                        //Console.WriteLine("ElbowLeft");
                        positionsAl.Add(mw.kinectFacility.getDisplayPosition(joint));
                        break;
                    case JointID.ElbowRight:
                        //Console.WriteLine("ElbowRight");
                        positionsAl.Add(mw.kinectFacility.getDisplayPosition(joint));
                        break;
                    case JointID.HandRight:
                        //Console.WriteLine("HandRight");
                        //HandRightX = mw.kinectFacility.getDisplayPosition(joint).X;
                        //Console.WriteLine("Hand distance: " + Math.Abs(HandLeftX - HandRightX));
                        positionsAl.Add(mw.kinectFacility.getDisplayPosition(joint));
                        break;
                }
            }
            double[] inputs = new double[positionsAl.Count * 2];
            int i = 0;
            foreach (Point p in positionsAl)
            {
                inputs[i] = p.X / mw.kinectImage.ActualWidth;
                inputs[i + 1] = p.Y / mw.kinectImage.ActualHeight;
                i = i + 2;
            }
            //Console.WriteLine("Inputs: " + inputs);
            capturedGesture.Add(inputs);
        }

        public void CaptureKinectRelativePositions2D()
        {
            JointsCollection positions = mw.kinectFacility.LastRecordedJoints;
            ArrayList positionsAl = new ArrayList();
            Point spinePosition = new Point();
            int temporaryArmsLengthConstant = 500;
            Point jointPosition = new Point();
            Point relativeJointPosition = new Point();
            foreach (Joint joint in positions)
            {
                switch (joint.ID)
                {
                    case JointID.Spine:
                        //Console.WriteLine("Spine");
                        spinePosition = mw.kinectFacility.getDisplayPosition(joint);
                        break;
                    case JointID.HandLeft:
                        //Console.WriteLine("HandLeft");
                        jointPosition = mw.kinectFacility.getDisplayPosition(joint);
                        relativeJointPosition = new Point();
                        relativeJointPosition.X = temporaryArmsLengthConstant/2 + jointPosition.X - spinePosition.X;
                        relativeJointPosition.Y = temporaryArmsLengthConstant / 2 + jointPosition.Y - spinePosition.Y;
                        positionsAl.Add(relativeJointPosition);
                        break;
                    case JointID.ElbowLeft:
                        //Console.WriteLine("ElbowLeft");
                        jointPosition = mw.kinectFacility.getDisplayPosition(joint);
                        relativeJointPosition = new Point();
                        relativeJointPosition.X = temporaryArmsLengthConstant / 2 + jointPosition.X - spinePosition.X;
                        relativeJointPosition.Y = temporaryArmsLengthConstant / 2 + jointPosition.Y - spinePosition.Y;
                        positionsAl.Add(relativeJointPosition);
                        break;
                    case JointID.ElbowRight:
                        //Console.WriteLine("ElbowRight");
                        jointPosition = mw.kinectFacility.getDisplayPosition(joint);
                        relativeJointPosition = new Point();
                        relativeJointPosition.X = temporaryArmsLengthConstant / 2 + jointPosition.X - spinePosition.X;
                        relativeJointPosition.Y = temporaryArmsLengthConstant / 2 + jointPosition.Y - spinePosition.Y;
                        positionsAl.Add(relativeJointPosition);
                        break;
                    case JointID.HandRight:
                        //Console.WriteLine("HandRight");
                        jointPosition = mw.kinectFacility.getDisplayPosition(joint);
                        relativeJointPosition = new Point();
                        relativeJointPosition.X = temporaryArmsLengthConstant / 2 + jointPosition.X - spinePosition.X;
                        relativeJointPosition.Y = temporaryArmsLengthConstant / 2 + jointPosition.Y - spinePosition.Y;
                        positionsAl.Add(relativeJointPosition);
                        break;
                }
            }
            double[] inputs = new double[positionsAl.Count * 2];
            int i = 0;
            for (int j = 0; j < positionsAl.Count; ++j)
            {
                Point p = (Point)positionsAl[j];
                if (p.X <= 0)
                    p.X = 0.0001;
                else if (p.X >= temporaryArmsLengthConstant)
                    p.X = temporaryArmsLengthConstant - 0.0001;
                if (p.Y <= 0)
                    p.Y = 0.0001;
                else if (p.Y >= temporaryArmsLengthConstant)
                    p.Y = temporaryArmsLengthConstant - 0.0001;
                inputs[i] = p.X / temporaryArmsLengthConstant;
                inputs[i + 1] = p.Y / temporaryArmsLengthConstant;
                i = i + 2;
            }
            //for security (under testing)
            foreach (double d in inputs)
            {
                if(d <= 0 || d >= 1)
                    MessageBox.Show("MEGA ERROR: input not in (0,1) range!");
            }
            //Console.WriteLine("Inputs: " + inputs);
            capturedGesture.Add(inputs);
        }

        public void CaptureKinectRelativePositions3D()
        {
            JointsCollection positions = mw.kinectFacility.LastRecordedJoints;
            ArrayList positionsAl = new ArrayList();
            Point3D spinePosition = null;
            int temporaryArmsLengthConstant = 500;
            int temporaryArmsLengthInMeters = 2;
            Point3D jointPosition = null;
            Point3D relativeJointPosition = null;
            foreach (Joint joint in positions)
            {
                switch (joint.ID)
                {
                    case JointID.Spine:
                        //Console.WriteLine("Spine");
                        spinePosition = new Point3D(mw.kinectFacility.getDisplayPosition(joint), (double)joint.Position.Z);
                        break;
                    case JointID.HandLeft:
                        //Console.WriteLine("HandLeft");
                        jointPosition = new Point3D(mw.kinectFacility.getDisplayPosition(joint), (double)joint.Position.Z);
                        relativeJointPosition = new Point3D();
                        relativeJointPosition.X = temporaryArmsLengthConstant / 2 + jointPosition.X - spinePosition.X;
                        relativeJointPosition.Y = temporaryArmsLengthConstant / 2 + jointPosition.Y - spinePosition.Y;
                        relativeJointPosition.Z = temporaryArmsLengthInMeters / 2 + jointPosition.Z - spinePosition.Z;
                        positionsAl.Add(relativeJointPosition);
                        break;
                    case JointID.ElbowLeft:
                        //Console.WriteLine("ElbowLeft");
                        jointPosition = new Point3D(mw.kinectFacility.getDisplayPosition(joint), (double)joint.Position.Z);
                        relativeJointPosition = new Point3D();
                        relativeJointPosition.X = temporaryArmsLengthConstant / 2 + jointPosition.X - spinePosition.X;
                        relativeJointPosition.Y = temporaryArmsLengthConstant / 2 + jointPosition.Y - spinePosition.Y;
                        relativeJointPosition.Z = temporaryArmsLengthInMeters / 2 + jointPosition.Z - spinePosition.Z;
                        positionsAl.Add(relativeJointPosition);
                        break;
                    case JointID.ElbowRight:
                        //Console.WriteLine("ElbowRight");
                        jointPosition = new Point3D(mw.kinectFacility.getDisplayPosition(joint), (double)joint.Position.Z);
                        relativeJointPosition = new Point3D();
                        relativeJointPosition.X = temporaryArmsLengthConstant / 2 + jointPosition.X - spinePosition.X;
                        relativeJointPosition.Y = temporaryArmsLengthConstant / 2 + jointPosition.Y - spinePosition.Y;
                        relativeJointPosition.Z = temporaryArmsLengthInMeters / 2 + jointPosition.Z - spinePosition.Z;
                        positionsAl.Add(relativeJointPosition);
                        break;
                    case JointID.HandRight:
                        //Console.WriteLine("HandRight");
                        jointPosition = new Point3D(mw.kinectFacility.getDisplayPosition(joint), (double)joint.Position.Z);
                        relativeJointPosition = new Point3D();
                        relativeJointPosition.X = temporaryArmsLengthConstant / 2 + jointPosition.X - spinePosition.X;
                        relativeJointPosition.Y = temporaryArmsLengthConstant / 2 + jointPosition.Y - spinePosition.Y;
                        relativeJointPosition.Z = temporaryArmsLengthInMeters / 2 + jointPosition.Z - spinePosition.Z;
                        positionsAl.Add(relativeJointPosition);
                        break;
                }
            }
            double[] inputs = new double[positionsAl.Count * 3];
            int i = 0;
            for (int j = 0; j < positionsAl.Count; ++j)
            {
                Point3D p = (Point3D)positionsAl[j];
                if (p.X <= 0)
                    p.X = 0.0001;
                else if (p.X >= temporaryArmsLengthConstant)
                    p.X = temporaryArmsLengthConstant - 0.0001;
                if (p.Y <= 0)
                    p.Y = 0.0001;
                else if (p.Y >= temporaryArmsLengthConstant)
                    p.Y = temporaryArmsLengthConstant - 0.0001;
                if (p.Z <= 0)
                    p.Z = 0.0001;
                else if (p.Z >= temporaryArmsLengthInMeters)
                    p.Z = temporaryArmsLengthInMeters - 0.0001;
                inputs[i] = p.X / temporaryArmsLengthConstant;
                inputs[i + 1] = p.Y / temporaryArmsLengthConstant;
                inputs[i + 2] = p.Z / temporaryArmsLengthInMeters;
                i = i + 3;
            }
            //for security (under testing)
            foreach (double d in inputs)
            {
                if (d <= 0 || d >= 1)
                    MessageBox.Show("MEGA ERROR: input not in (0,1) range!");
            }
            //Console.WriteLine("Inputs: " + inputs);
            capturedGesture.Add(inputs);
        }

        public void ProcessRecognition(Canvas canvas, ArrayList gesturesList, RECOGNITION_MODE mode)
        {
            mw.frameworkConstants.AppendToFile("Performing recognition\n");
            currentMode = mode;
            currentGesturesList = gesturesList;
            //currentNetwork = LearnNetwork(currentGesturesList);
            //MLPXMLSerializer serializer = new MLPXMLSerializer();
            //XDocument doc = serializer.Serialize(currentNetwork);
            //doc.Save("ocr.xml");
            MainCanvas = canvas;
            timer = new DispatcherTimer();
                        
            timer.Interval = TimeSpan.FromMilliseconds(RecordInterval);
            timer.Tick += (sender, args) => timer_Tick(this, mode);
            timer.Start();
            //CaptureMousePositions();     
            //CaptureKinectPositions();
        }

        // Specify what you want to happen when the Elapsed event is raised.
        private void timer_Tick(object source, RECOGNITION_MODE mode)
        {
            //Console.WriteLine("TICK " + source.ToString());
            if ((mw.frameworkConstants.BackgroundRecognition && mode.Equals(RECOGNITION_MODE.BACKGROUND) && mw.kinectFacility.isFrameReady || mode.Equals(RECOGNITION_MODE.SINGLE)))
            {
                //CaptureMousePositions();
                int dimensions = ((GestureMetadata)currentGesturesList[0]).NumberOfDimensions;
                if(dimensions == 2)
                    CaptureKinectRelativePositions2D();
                else if(dimensions == 3)
                    CaptureKinectRelativePositions3D();
                int numberOfFrames = 3;
                double jitter = 0.05;
                
                if (capturedGesture.Count >= numberOfFrames + 1)
                {
                    bool isStatic = VerifyStaticPosition(jitter, numberOfFrames);
                    if (isStatic)
                    {
                        timer.Stop();
                        for (int i = 0; i < numberOfFrames - 1; ++i)
                            capturedGesture.RemoveAt(capturedGesture.Count - 1);

                        //ArrayList altd = new ArrayList();
                        //altd.Add(new LearnElement(capturedGesture[capturedGesture.Count - 1], new double[]{0,0,0,1}));
                        //string xml = XmlUtils.SerializeArrayList(altd, typeof(LearnElement));
                        ////NeuronNetworkFacility.AttachValuesToGestures(GesturesList);
                        ////String xml = XmlUtils.SerializeArrayList(GesturesList, typeof(GestureMetadata));
                        //Console.WriteLine(xml);


                        string gesture = RecognizeGesture();

                        mw.frameworkConstants.GesturesStatus_Gesture = gesture;
                        mw.frameworkConstants.NotifyPropertyChanged("GesturesStatus_Gesture");
                        if (mode.Equals(RECOGNITION_MODE.BACKGROUND))
                        {
                            //ProcessGestureEvent(gesture);
                            DispatcherEventArgs dea = new DispatcherEventArgs(gesture, Commands.PROCESS_GESTURE, Properties.Settings.Default.AnimationTime);
                            mw.eventDispatcher.ExecuteEvent(this, dea);
                        }
                        else if (mode.Equals(RECOGNITION_MODE.SINGLE))
                        {
                            DispatcherEventArgs dea = new DispatcherEventArgs(gesture, Commands.UNHIDE_GESTURES, Properties.Settings.Default.AnimationTime);
                            mw.eventDispatcher.ExecuteEvent(this, dea);
                        }
                    }
                    if (currentMode.Equals(RECOGNITION_MODE.BACKGROUND))
                    {
                        isStatic = false;
                        timer.Start();
                    }
                }
            }
        }

        

        private string RecognizeGesture()
        {
            //mw.frameworkConstants.StartMeasuring();
            //Point lastPoint = capturedGesture.Last<Point>();
            double[] lastInput = capturedGesture[capturedGesture.Count - 1];
            
            //double[] inputArray = new double[] { lastPoint.X, lastPoint.Y };
            CurrentInput = lastInput;
            //DateTime a = DateTime.Now;
            double[] output = currentNetwork.Pulse(lastInput);
            //TimeSpan ts = DateTime.Now - a;
            //MessageBox.Show(ts.Milliseconds.ToString());
            LinkedList<string> al = new LinkedList<string>();
            al.AddLast("Recognition values");
            for(int i = 0; i < output.Length; ++i)
            {
                String s = "Name: " + ((GestureMetadata)currentGesturesList[i]).GestureName + " Value: " + output[i];
                Console.WriteLine(s);
                al.AddLast(s);
            }
            System.IO.File.AppendAllLines(mw.frameworkConstants.pathToFile, al.ToArray<string>());
            //TODO: Dorobić xmla
            //System.Console.WriteLine("Value: " + out);

            //TODO: obliczać jakoś tolerancję
            string gesture = DecideRecognitionValue(output, 0.15);
            Console.WriteLine("Gesture: " + gesture);
            mw.frameworkConstants.EndMeasuring();
            return gesture;
        }
        private string DecideRecognitionValue(double[] output, double tolerance)
        {
            int indexOfHighestValue = -1;
            double highestValue = 0;
            for (int i = 0; i < output.Length; ++i)
            {
                if (output[i] > highestValue)
                {
                    highestValue = output[i];
                    indexOfHighestValue = i;
                }
            }
            if(highestValue >= MAX_VALUE - tolerance)
            {
                //UWAGA! Lista musi być posortowana po outputposition! Może by użyc ICompare?
                int outputPosition = ((GestureMetadata)currentGesturesList[indexOfHighestValue]).OutputPosition;
                CurrentOutput = GenerateOutput(outputPosition, output.Length);
                String result = ((GestureMetadata)currentGesturesList[indexOfHighestValue]).GestureName;
                mw.frameworkConstants.AppendToFile("Recognized gesture: " + result + "\n");
                return result;
            }
            else
                return mw.frameworkConstants.Info_CannotDecide;           
        }

        public double[] GenerateOutput(int outputPosition, int outputLength)
        {
            double[] newOutput = new double[outputLength];
            for (int i = 0; i < outputLength; ++i)
            {
                if (i == outputPosition)
                    newOutput[i] = MAX_VALUE;
                else
                    newOutput[i] = MIN_VALUE;
            }
            return newOutput;
        }

        //private string DecideRecognitionValue(double output, double tolerance)
        //{
        //    for(int i = 0; i < currentGesturesList.Count; ++i)
        //    {
        //        if (Math.Abs(output - ((GestureMetadata)currentGesturesList[i]).Value) < tolerance || Math.Abs(output + ((GestureMetadata)currentGesturesList[i]).Value) < tolerance) 
        //        {
        //            CurrentOutput = new double[] {((GestureMetadata)currentGesturesList[i]).Value };
        //            return ((GestureMetadata)currentGesturesList[i]).GestureName;
        //        }
        //    }            
        //    //TODO: Translate this
        //    return "Cannot decide!";
        //}

        private bool VerifyStaticPosition(double jitter, int numberOfFrames)
        {
            int count = capturedGesture.Count() - 1;
            bool isStatic = true;
            //Point[] pointMap = capturedGesture.ToArray<Point>();
            for (int i = 0; i < numberOfFrames; ++i)
            {
                double[] differences = new double[capturedGesture[0].Length];
                for(int j = 0; j < capturedGesture[0].Length; ++j)
                {
                    differences[j] = Math.Abs(capturedGesture[count - i][j] - capturedGesture[count - i - 1][j]);
                    if (differences[j] <= jitter)
                        continue;
                    else
                    {
                        isStatic = false;
                        break;
                    }
                }                
            }
            if (isStatic)
            {
                Console.WriteLine("EndRecord");
                return true;
            }
            else
            {
                return false;
            }
        }

        //private bool VerifyStaticPosition(double jitter, int numberOfFrames)
        //{
        //    double jitterX = jitter;
        //    double jitterY = jitter;
        //    int count = capturedGesture.Count() - 1;
        //    bool isStatic = true;
        //    Point[] pointMap = capturedGesture.ToArray<Point>();
        //    for (int i = 0; i < numberOfFrames; ++i)
        //    {
        //        double currentDifferenceX = Math.Abs(pointMap[count - i].X - pointMap[count - i - 1].X);
        //        double currentDifferenceY = Math.Abs(pointMap[count - i].Y - pointMap[count - i - 1].Y);
        //        if ((currentDifferenceX <= jitterX) && (currentDifferenceY <= jitterY))
        //        {
        //            continue;
        //        }
        //        else
        //        {
        //            isStatic = false;
        //            break;
        //        }
        //    }
        //    if (isStatic)
        //    {
        //        Console.WriteLine("EndRecord");
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        public void Test()
        {
            // Display the number of command line arguments:
            System.Console.WriteLine("start");
            int cycles = 2000;
            int hiddenNeurons = 10;
            double learnRate = 0.9;
            double momentum = 0.5;
            int[] layers = new int[] { 6, hiddenNeurons, 1 };
            MultilayerPerceptron ann = new MLPGenerator().Create(layers, 1, new Sigmoid(2));
            ann.Reset(-1, 1);
            ann.Momentum = momentum;
            ann.LearnFactor = learnRate;
            List<TrainingData> trainingList = new List<TrainingData>();

            double[] input = { 0.25, 0.25, 0.5, 0.75, 0.75, 0.5 };
            double[] output = { 0.1 };
            trainingList.Add(new TrainingData(input, output));

            double[] input1 = { 0.50, 0.50, 0.5, 0.50, 0.50, 0.5 };
            double[] output1 = { 0.2 };
            trainingList.Add(new TrainingData(input1, output1));

            double[] input4 = { 0.53, 0.54, 0.4, 0.52, 0.53, 0.6 };
            double[] output4 = { 0.2 };
            trainingList.Add(new TrainingData(input4, output4));

            double[] input5 = { 0.50, 0.40, 0.5, 0.50, 0.58, 0.6 };
            double[] output5 = { 0.2 };
            trainingList.Add(new TrainingData(input5, output5));


            double[] input2 = { 0.50, 0.50, 0.5, 0.75, 0.75, 0.5 };
            double[] output2 = { 0.3 };
            trainingList.Add(new TrainingData(input2, output2));


            var res = ann.BP(new BPRequest(trainingList.ToArray(), cycles));

            double[] input3 = { 0.47, 0.49, 0.5, 0.53, 0.54, 0.5 };

            //double[] values = ann.Pulse(input3);
            double value = ann.Pulse(input3)[0];

            System.Console.WriteLine("Value: " + value);
        }
    }
}
