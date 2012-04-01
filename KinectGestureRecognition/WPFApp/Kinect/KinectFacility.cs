using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.Kinect.Nui;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Diagnostics;
using System.Windows;
using WPFApp.Kinect;
using System.Windows.Shapes;

namespace WPFApp.Kinect
{
    public class KinectFacility
    {
        //Dictionary<JointID, LinkedList<KinectGUIShape>> registeredKGS = new Dictionary<JointID, LinkedList<KinectGUIShape>>();
        BitmapSource bmp = null;
        public static Color currentColor = Colors.Red;
        public static Color incorrectDistanceColor = Colors.Red;
        public static Color correctDistanceColor = Colors.Blue;
        Stopwatch stopwatch = new Stopwatch();
        int i = 1;
        bool isTimerInUse = false;
        public bool isFrameReady = false;
        JointID timerJointID = JointID.Count;
        MainWindow mw = null;
        LinkedList<double> positions = new LinkedList<double>();
        public JointsCollection LastRecordedJoints { get; set; }

        Runtime nui = new Runtime();
       
        public void Initialize(object sender, RoutedEventArgs e)
        {
            mw = (MainWindow)sender;
            nui.Initialize(RuntimeOptions.UseColor | RuntimeOptions.UseDepthAndPlayerIndex | RuntimeOptions.UseSkeletalTracking);
            nui.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(nui_SkeletonFrameReady);
            //nui.VideoFrameReady += new EventHandler<ImageFrameReadyEventArgs>(nui_VideoFrameReady);
            //nui.DepthFrameReady += new EventHandler<ImageFrameReadyEventArgs>(nui_DepthFrameReady);
            nui.VideoStream.Open(ImageStreamType.Video, 2, ImageResolution.Resolution640x480, ImageType.Color);

            nui.DepthStream.Open(ImageStreamType.Depth, 2, ImageResolution.Resolution320x240, ImageType.DepthAndPlayerIndex);
            mw.kinectImage.Source = nui.CreateLivePlayerRenderer(RuntimeExtensions.RAW_PLAYER);

            //image1.Source = nui.CreateLivePlayerRenderer(RuntimeExtensions.IMAGE_PLAYER);

        }

        public Point getDisplayPosition(Joint joint)
        {
            float depthX, depthY;
            nui.SkeletonEngine.SkeletonToDepthImage(joint.Position, out depthX, out depthY);
            depthX = Math.Max(0, Math.Min(depthX * 320, 320));  //convert to 320, 240 space
            depthY = Math.Max(0, Math.Min(depthY * 240, 240));  //convert to 320, 240 space
            int colorX, colorY;
            ImageViewArea iv = new ImageViewArea();
            // only ImageResolution.Resolution640x480 is supported at this point
            nui.NuiCamera.GetColorPixelCoordinatesFromDepthPixel
            (ImageResolution.Resolution640x480, iv, (int)depthX, (int)depthY,
            (short)0, out colorX, out colorY);

            // map back to skeleton.Width & skeleton.Height
            
            return new Point((int)(mw.kinectImage.Width * colorX / 640.0),
            (int)(mw.kinectImage.Height * colorY / 480));
        }

        private Boolean isInBounds(Rectangle rect, Point point)
        {
            int relX = (int)rect.Margin.Left;
            int relY = (int)rect.Margin.Top;
            int maxX = relX+(int)rect.Width;
            int maxY = relY+(int)rect.Height;
            //Console.WriteLine(relX + " " + relY + " " + maxX + " " + maxY);
            if (point.X >= relX && point.X <= maxX && point.Y >= relY && point.Y <= maxY)
            {
                return true;
            }
            return false;
        }
        //enum ShapeProperties
        //{
        //    marginLeft,
        //    marginRight,
        //    marginTop,
        //    marginBottom,
        //    shapeWidth,
        //    shapeHeight
        //}

        //private void registerKinectGUIShape(JointID name, KinectGUIShape shape)
        //{
        //    if (registeredKGS.ContainsKey(name))
        //    {
        //        registeredKGS[name].AddLast(shape);
        //    }
        //    else
        //    {
        //        LinkedList<KinectGUIShape> list = new LinkedList<KinectGUIShape>();
        //        list.AddLast(shape);
        //        registeredKGS.Add(name, list);
        //    }
        //}

        //private bool unregisterKinectGUIShape(JointID name)
        //{
        //    if (registeredKGS.ContainsKey(name))
        //    {
        //        registeredKGS.Remove(name);
        //        return true;
        //    }
        //    return false;
        //}

        //private void addShapeToGUI(Shape shape, String content, int left, int top, JointID joint)
        //{
        //    Canvas.SetTop(shape, top);
        //    Canvas.SetLeft(shape, left);
        //    MainCanvas.Children.Add(shape);
        //    Label label = KinectGUIShape.LabelFactory(content, shape);
        //    KinectGUIShape kgs = new KinectGUIShape(shape, label, joint);
        //    registerKinectGUIShape(joint, kgs);
        //}



        void nui_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            if(!isFrameReady)
                isFrameReady = true;
            foreach (SkeletonData data in e.SkeletonFrame.Skeletons)
            {
                
                //Console.WriteLine(data.UserIndex);
                //if(data.UserIndex == 0)
                {
                    //Tracked that defines whether a skeleton is 'tracked' or not. 
                    //The untracked skeletons only give their position. 
                    if (SkeletonTrackingState.Tracked != data.TrackingState) continue;

                    //Each joint has a Position property that is defined by a Vector4: (x, y, z, w). 
                    //The first three attributes define the position in camera space. 
                    //The last attribute (w)
                    //gives the quality level (between 0 and 1) of the 
                    Point rhp = new Point();
                    Point lhp = new Point();
                    foreach (Joint joint in data.Joints)
                    {
                        if (joint.Position.W < 0.6f) return;// Quality check 
                        LastRecordedJoints = data.Joints;
                        switch (joint.ID)
                        {
                            case JointID.Spine:
                                //Point point = getDisplayPosition(joint);
                                Double userDistance = joint.Position.Z;
                                if (userDistance > 1.8 && userDistance < 2.5)
                                {
                                    currentColor = correctDistanceColor;
                                }
                                else
                                {
                                    currentColor = incorrectDistanceColor;
                                }
                                //Console.WriteLine("Z: " + d);
                                break;
                            case JointID.HandRight:
                                rhp = getDisplayPosition(joint);
                                //TODO:Poqiniwnwm tutaj dać cmtsVisibleMap, ale sa bledy "jezeli animacja przejdzie to cmt jest usuwany z cmtsVisibleMap i nie moze sie zakonczyc:/"
                                foreach (CircularMinuteTimer cmt in mw.cmtsMap.Values)
                                {
                                    if (timerJointID.Equals(JointID.Count) || timerJointID.Equals(JointID.HandRight))
                                    {
                                        if (cmt.IsInBounds(rhp) && !cmt.IsSelected)
                                        {
                                            RutedEvent.GoStartTimerEvent(cmt);
                                            timerJointID = JointID.HandRight;
                                            cmt.IsSelected = true;
                                        }
                                        else if (!cmt.IsInBounds(rhp) && cmt.IsSelected)
                                        {
                                            RutedEvent.GoStopTimerEvent(cmt);
                                            timerJointID = JointID.Count;
                                            cmt.IsSelected = false;
                                        }
                                        else
                                        {
                                            //Console.WriteLine(cmt.IsInBounds(rhp).ToString() + " " + cmt.IsSelected.ToString());
                                        }
                                    }
                                }
                                break;
                            case JointID.HandLeft:
                                lhp = getDisplayPosition(joint);
                                foreach (CircularMinuteTimer cmt in mw.cmtsVisibleMap.Values)
                                {
                                    if (timerJointID.Equals(JointID.Count) || timerJointID.Equals(JointID.HandLeft))
                                    {
                                        if (cmt.IsInBounds(lhp) && !cmt.IsSelected)
                                        {
                                            RutedEvent.GoStartTimerEvent(cmt);
                                            timerJointID = JointID.HandLeft;
                                            cmt.IsSelected = true;
                                        }
                                        else if (!cmt.IsInBounds(lhp) && cmt.IsSelected)
                                        {
                                            RutedEvent.GoStopTimerEvent(cmt);
                                            timerJointID = JointID.Count;
                                            cmt.IsSelected = false;
                                        }
                                    }
                                }
                                break;
                            //                if (registeredKGS.ContainsKey(JointID.HandRight))
                            //                {
                            //                    foreach (KinectGUIShape kgs in registeredKGS[JointID.HandRight])
                            //                    {
                            //                        if (kgs.isInBounds(rhp))
                            //                        {
                            //                            KinectTimer.setCurrentContainer(kgs);
                            //                            if (!kgs.hasEntered)
                            //                            {
                            //                                kgs.hasEntered = true;
                            //                                kgs.hasLeft = false;
                            //                                if (!isTimerInUse)
                            //                                {
                            //                                    isTimerInUse = true;
                            //                                    kgs.hasTimer = true;
                            //                                    kgs.focusShape();
                            //                                    RutedEvent.GoStartTimerEvent(KinectTimer);
                            //                                }
                            //                            }
                            //                            if (kgs.hasTimer)
                            //                            {
                            //                                KinectTimer.Visibility = Visibility.Visible;
                            //                                Canvas.SetLeft(KinectTimer, rhp.X);
                            //                                Canvas.SetTop(KinectTimer, rhp.Y);
                            //                            }

                            //                        }
                            //                        else
                            //                        {

                            //                            if (!kgs.hasLeft)
                            //                            {
                            //                                kgs.hasLeft = true;
                            //                                kgs.hasEntered = false;
                            //                                if (kgs.hasTimer)
                            //                                {
                            //                                    kgs.unfocusShape();
                            //                                    KinectTimer.Visibility = Visibility.Hidden;
                            //                                    RutedEvent.GoStopTimerEvent(KinectTimer);
                            //                                    kgs.hasTimer = false;
                            //                                    isTimerInUse = false;
                            //                                }
                            //                            }
                            //                        }
                            //                    }
                            //                }
                            //                break;
                            //            case JointID.HandLeft:
                            //                lhp = getDisplayPosition(joint);
                            //                if (registeredKGS.ContainsKey(JointID.HandLeft))
                            //                {
                            //                    foreach (KinectGUIShape kgs in registeredKGS[JointID.HandLeft])
                            //                    {
                            //                        if (kgs.isInBounds(lhp))
                            //                        {
                            //                            KinectTimer.setCurrentContainer(kgs);
                            //                            if (!kgs.hasEntered)
                            //                            {
                            //                                kgs.hasEntered = true;
                            //                                kgs.hasLeft = false;
                            //                                if (!isTimerInUse)
                            //                                {
                            //                                    isTimerInUse = true;
                            //                                    kgs.hasTimer = true;
                            //                                    kgs.focusShape();
                            //                                    RutedEvent.GoStartTimerEvent(KinectTimer);
                            //                                }
                            //                            }
                            //                            if (kgs.hasTimer)
                            //                            {
                            //                                KinectTimer.Visibility = Visibility.Visible;
                            //                                Canvas.SetLeft(KinectTimer, lhp.X);
                            //                                Canvas.SetTop(KinectTimer, lhp.Y);
                            //                            }
                            //                        }
                            //                        else
                            //                        {

                            //                            if (!kgs.hasLeft)
                            //                            {
                            //                                kgs.hasLeft = true;
                            //                                kgs.hasEntered = false;
                            //                                if (kgs.hasTimer)
                            //                                {
                            //                                    kgs.unfocusShape();
                            //                                    KinectTimer.Visibility = Visibility.Hidden;
                            //                                    RutedEvent.GoStopTimerEvent(KinectTimer);
                            //                                    kgs.hasTimer = false;
                            //                                    isTimerInUse = false;
                            //                                }
                            //                            }
                            //                        }
                            //                    }
                            //                }
                            //                break;
                            //        }
                            //    }

                            //}
                            //if (!isTimerInUse)
                            //    KinectTimer.setCurrentContainer(null);
                            //if (KinectTimer.getCurrentContainer() != null)
                            //    Console.WriteLine(KinectTimer.getCurrentContainer().name);
                            //else
                            //    Console.WriteLine("null");
                        }
                    }
                }
            }
        }

        void nui_VideoFrameReady(object sender, ImageFrameReadyEventArgs e)
        {
            PlanarImage image = e.ImageFrame.Image;
            //image1.Source = BitmapSource.Create(image.Width, image.Height, 96, 96, PixelFormats.Bgr32, null, image.Bits, image.Width * image.BytesPerPixel);
            //image1.Source = e.ImageFrame.ToBitmapSource();
            //image1.Source = bmp = BitmapSource.Create(image.Width, image.Height, 96, 96, PixelFormats.Bgr32, null, image.Bits, image.Width * image.BytesPerPixel);
        }

        void nui_DepthFrameReady(object sender, ImageFrameReadyEventArgs e)
        {
            //image2.Source = e.ImageFrame.ToBitmapSource();
            byte[] coloredBytes = GenerateColoredBytes(e.ImageFrame);
            PlanarImage image = e.ImageFrame.Image;
            //image1.Source = BitmapSource.Create(image.Width, image.Height, 96, 96, PixelFormats.Bgra32, null,
            //    coloredBytes, image.Width * PixelFormats.Bgra32.BitsPerPixel / 8);


            //BitmapSource bmp = BitmapSource.Create(image.Width, image.Height, 96, 96, PixelFormats.Bgr32, null,
            //    coloredBytes, image.Width * PixelFormats.Bgr32.BitsPerPixel / 8);
            //bmp.
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            nui.Uninitialize();
        }

        private int GetDistanceWithPlayerIndex(byte firstFrame, byte secondFrame)
        {
            //offset by 3 in first byte to get value after player index
            int distance = (int)(firstFrame >> 3 | secondFrame << 5);
            return distance;
        }

        private static int GetPlayerIndex(byte firstFrame)
        {
            //returns 0 = no player, 1 = 1st player, 2 = 2nd player...
            return (int)firstFrame & 7;
        }

        private byte[] GenerateColoredBytes(ImageFrame imageFrame)
        {
            int height = imageFrame.Image.Height;
            int width = imageFrame.Image.Width;

            //Depth data for each pixel
            Byte[] depthData = imageFrame.Image.Bits;

            //colorFrame contains color information for all pixels in image
            //Height x Width x 4 (Red, Green, Blue, empty byte)
            Byte[] colorFrame = new byte[imageFrame.Image.Height * imageFrame.Image.Width * 4];

            //Bgr32  - Blue, Green, Red, empty byte
            //Bgra32 - Blue, Green, Red, transparency
            //You must set transparency for Bgra as .NET defaults a byte to 0 = fully transparent

            //hardcoded locations to Blue, Green, Red (BGR) index positions      
            const int BlueIndex = 0;
            const int GreenIndex = 1;
            const int RedIndex = 2;
            const int AlphaIndex = 3;
            int minDistance = -1;
            int maxDistance = -1;

            var depthIndex = 0;
            for (var y = 0; y < height; y++)
            {
                var heightOffset = y * width;

                for (var x = 0; x < width; x++)
                {
                    var index = (x + heightOffset) * 4;

                    //var distance = GetDistance(depthData[depthIndex], depthData[depthIndex + 1]);
                    var distance = GetDistanceWithPlayerIndex(depthData[depthIndex], depthData[depthIndex + 1]);
                    if (GetPlayerIndex(depthData[depthIndex]) == 1)
                    {
                        if (minDistance == -1)
                            minDistance = distance;
                        else if (minDistance > distance)
                            minDistance = distance;
                        if (maxDistance == -1)
                            maxDistance = distance;
                        else if (maxDistance < distance)
                            maxDistance = distance;
                    }
                    /*if (distance <= 900)
                    {
                        //we are very close
                        colorFrame[index + BlueIndex] = 255;
                        colorFrame[index + GreenIndex] = 0;
                        colorFrame[index + RedIndex] = 0;
                    }
                    else if (distance > 900 && distance < 2000)
                    {
                        //we are a bit further away
                        colorFrame[index + BlueIndex] = 0;
                        colorFrame[index + GreenIndex] = 255;
                        colorFrame[index + RedIndex] = 0;
                    }
                    else if (distance > 2000)
                    {
                        //we are the farthest
                        colorFrame[index + BlueIndex] = 0;
                        colorFrame[index + GreenIndex] = 0;
                        colorFrame[index + RedIndex] = 255;
                    }*/

                    //colorFrame[index + BlueIndex] = (byte)(distance % 255);
                    //colorFrame[index + GreenIndex] = 255;// (byte)(distance + 1 % 255);
                    //colorFrame[index + RedIndex] = 255;// (byte)(distance + 2 % 255);

                    ////Color a player
                    //if (GetPlayerIndex(depthData[depthIndex]) > 0)
                    //{
                    //    //we are the farthest
                    //    int dist = maxDistance - minDistance;
                    //    colorFrame[index + BlueIndex] = (byte)(dist % 255);
                    //    colorFrame[index + GreenIndex] = (byte)(dist % 255);
                    //    colorFrame[index + RedIndex] = (byte)(dist % 255);
                    //}

                    //jump two bytes at a time
                    depthIndex += 2;
                }
            }
                depthIndex = 0;
            for (var y = 0; y < height; y++)
            {
                var heightOffset = y * width;

                for (var x = 0; x < width; x++)
                {
                    var index = (x + heightOffset) * 4;
                    var distance = GetDistanceWithPlayerIndex(depthData[depthIndex], depthData[depthIndex + 1]);
                    if (GetPlayerIndex(depthData[depthIndex]) == 1)
                    {
                        int relDistance = distance - minDistance;
                        int relDistanceMax = maxDistance - minDistance;
                        //byte relColor = (byte)((relDistanceMax - relDistance) % 255);
                        byte relColor = (byte)((int)(relDistance * 255 / relDistanceMax / 5));
                        //Console.WriteLine("relMax: " + relDistanceMax + " rel: " + relDistance + " arelColor: " + relColor);
                        //we are the farthest
                        //int dist = maxDistance - minDistance;
                        colorFrame[index + BlueIndex] = (byte)(255 - relColor);
                        colorFrame[index + GreenIndex] = (byte)(255 - relColor);
                        colorFrame[index + RedIndex] = 0;
                        colorFrame[index + AlphaIndex] = 255;
                    }
                        depthIndex += 2;
                }
            }

            return colorFrame;
        }
        bool image = false;
        //private void button2_Click(object sender, RoutedEventArgs e)
        //{
            
        //    //Storyboard storyboard = new Storyboard();
        //    //storyboard.Children.Add(myDoubleAnimation);
            
        //    foreach (KinectGUIShape kgs in registeredKGS[JointID.HandLeft])
        //    {
        //        DoubleAnimation shapeAnimation = new DoubleAnimation();
        //        DoubleAnimation labelAnimation = new DoubleAnimation();
        //        double leftShape = Canvas.GetLeft(kgs.shape);
        //        double leftLabel = Canvas.GetLeft(kgs.label);
        //        double leftDifference = leftLabel - leftShape;
        //        shapeAnimation.From = leftShape;
        //        shapeAnimation.To = MainCanvas.Width + 10;
        //        shapeAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.5));
        //        labelAnimation.From = leftLabel;
        //        labelAnimation.To = MainCanvas.Width + leftDifference + 10;
        //        labelAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.5));
        //        kgs.shape.BeginAnimation(Canvas.LeftProperty, shapeAnimation);
        //        kgs.label.BeginAnimation(Canvas.LeftProperty, labelAnimation);
        //    }

        //    foreach (KinectGUIShape kgs in registeredKGS[JointID.HandRight])
        //    {
        //        DoubleAnimation shapeAnimation = new DoubleAnimation();
        //        DoubleAnimation labelAnimation = new DoubleAnimation();
        //        double leftShape = Canvas.GetLeft(kgs.shape);
        //        double leftLabel = Canvas.GetLeft(kgs.label);
        //        double leftDifference = leftLabel - leftShape;
        //        shapeAnimation.From = leftShape;
        //        shapeAnimation.To = MainCanvas.Width + 10;
        //        shapeAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.5));
        //        labelAnimation.From = leftLabel;
        //        labelAnimation.To = MainCanvas.Width + leftDifference + 10;
        //        labelAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.5));
        //        kgs.shape.BeginAnimation(Canvas.LeftProperty, shapeAnimation);
        //        kgs.label.BeginAnimation(Canvas.LeftProperty, labelAnimation);
        //    }

        //}
        private void Window_Initialized(object sender, EventArgs e)
        {

            //int elHeight = 200;
            //int elWidth = 200;
            //Ellipse ellipse = new Ellipse();
            //ellipse = (Ellipse)KinectGUIShape.ShapeFactory(ellipse, elHeight, elWidth, "Gesty");
            //addShapeToGUI(ellipse, "Gesty", (int)image1.Margin.Left, 0, JointID.HandLeft);

            //ellipse = new Ellipse();
            //ellipse = (Ellipse)KinectGUIShape.ShapeFactory(ellipse, elHeight, elWidth, "Konfiguracja");
            //addShapeToGUI(ellipse, "Konfiguracja", (int)image1.Margin.Left, 300, JointID.HandLeft);

            //ellipse = new Ellipse();
            //ellipse = (Ellipse)KinectGUIShape.ShapeFactory(ellipse, elHeight, elWidth, "Nauka");
            //addShapeToGUI(ellipse, "Nauka", (int)image1.Margin.Left + (int)image1.Width - elWidth, 0, JointID.HandRight);

            //ellipse = new Ellipse();
            //ellipse = (Ellipse)KinectGUIShape.ShapeFactory(ellipse, elHeight, elWidth, "Statystyki");
            //addShapeToGUI(ellipse, "Statystyki", (int)image1.Margin.Left + (int)image1.Width - elWidth, 300, JointID.HandRight);
        }
    }
}
