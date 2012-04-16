using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Collections;

namespace WPFApp
{
    public enum Directions
    {
        TOP,
        DOWN,
        LEFT,
        RIGHT
    }
    public enum CombinedDirections
    {
        SIDEWAYS,
        STRAIGHT,
        RANDOM,
        NO_DIRECTION
    }

    public class GraphicalEffects
    {
        MainWindow mw = null;
        public GraphicalEffects()
        {
            App app = ((App)Application.Current);
            mw = (MainWindow)app.MainWindow;
        }

        public static Random rand = new Random(System.DateTime.Now.Millisecond);
        public T RandomEnum<T>()
        {
            T[] values = (T[])Enum.GetValues(typeof(T));
            return values[rand.Next(0, values.Length)];
        }

        public void HideCmtOnCanvas(GraphicalEffectsMetadata gem, double canvasHidingPosition, double hidingTime)
        {
            if (mw.cmtsVisibleMap.Contains(gem.Cmt.Name))
                mw.cmtsVisibleMap.Remove(gem.Cmt.Name);
            Canvas canvas = (Canvas)mw.FindName(Properties.Settings.Default.MainCanvasName);
            DependencyProperty directionProperty = null;
            double position = 0;
            switch (gem.Direction)
            {
                case Directions.LEFT:
                    {
                        directionProperty = Canvas.LeftProperty;
                        position = Canvas.GetLeft(gem.Cmt);
                        break;
                    }
                case Directions.RIGHT:
                    {
                        directionProperty = Canvas.LeftProperty;
                        position = Canvas.GetLeft(gem.Cmt);
                        break;
                    }
                case Directions.TOP:
                    {
                        directionProperty = Canvas.TopProperty;
                        position = Canvas.GetTop(gem.Cmt);
                        break;
                    }
                case Directions.DOWN:
                    {
                        directionProperty = Canvas.TopProperty;
                        position = Canvas.GetTop(gem.Cmt);
                        break;
                    }
            }
            Console.WriteLine("Caption: " + gem.Cmt.Caption + " direction: " + gem.Direction + " position:  " + position + " cHP: " + canvasHidingPosition);
            DoubleAnimation shapeAnimation = new DoubleAnimation();
            shapeAnimation.From = position;
            shapeAnimation.To = canvasHidingPosition;
            shapeAnimation.Duration = new Duration(TimeSpan.FromSeconds(hidingTime));
            gem.Cmt.BeginAnimation(directionProperty, shapeAnimation);
        }

        public void HideElementOnCanvas(FrameworkElement element, Directions direction, double canvasHidingPositionLeft, double canvasHidingPositionTop, double hidingTime)
        {
            Canvas canvas = (Canvas)mw.FindName(Properties.Settings.Default.MainCanvasName);
            CircularMinuteTimer cmt = (CircularMinuteTimer)element;
            double left = 0;
            double top = 0;
            switch (direction)
            {
                case Directions.LEFT:
                case Directions.RIGHT:
                    left = Canvas.GetLeft(cmt);
                    break;
                case Directions.DOWN:
                case Directions.TOP:
                    top = Canvas.GetTop(cmt);
                    break;
            }
            if (canvasHidingPositionLeft != double.MaxValue)
            {
                DoubleAnimation shapeAnimation = new DoubleAnimation();
                shapeAnimation.From = left;
                shapeAnimation.To = canvasHidingPositionLeft;
                shapeAnimation.Duration = new Duration(TimeSpan.FromSeconds(hidingTime));
                cmt.BeginAnimation(Canvas.LeftProperty, shapeAnimation);
            }
            if (canvasHidingPositionTop != double.MaxValue)
            {
                DoubleAnimation shapeAnimation = new DoubleAnimation();
                shapeAnimation.From = top;
                shapeAnimation.To = canvasHidingPositionTop;
                shapeAnimation.Duration = new Duration(TimeSpan.FromSeconds(hidingTime));
                cmt.BeginAnimation(Canvas.TopProperty, shapeAnimation);
            }
        }
        public GraphicalEffectsMetadata GenerateGraphicalEffectsMetadata(CircularMinuteTimer cmt, Directions direction)
        {
            GraphicalEffectsMetadata gem = new GraphicalEffectsMetadata(cmt, direction);
            return gem;
        }

        public GraphicalEffectsMetadata GenerateGraphicalEffectsMetadata(CircularMinuteTimer cmt)
        {
            GraphicalEffectsMetadata gem = new GraphicalEffectsMetadata(cmt);
            return gem;
        }

        public void HideCmtsOnCanvas(LinkedList<GraphicalEffectsMetadata> gems, double maxHidingTime, Directions direction)
        {
            foreach (GraphicalEffectsMetadata gem in gems)
            {
                gem.Direction = direction;
            }
            HideCmtsOnCanvas(gems, maxHidingTime);
        }

        public void HideCmtsOnCanvas(LinkedList<GraphicalEffectsMetadata> gems, double maxHidingTime, CombinedDirections combinedDirections)
        {
            Canvas mainCanvas = (Canvas)mw.FindName(Properties.Settings.Default.MainCanvasName);
            switch (combinedDirections)
            {
                case (CombinedDirections.SIDEWAYS):
                    {
                        double canvasVerticalMiddle = mainCanvas.ActualWidth / 2;
                        foreach (GraphicalEffectsMetadata gem in gems)
                        {
                            if (Canvas.GetLeft(gem.Cmt) + gem.Cmt.ActualWidth / 2 < canvasVerticalMiddle)
                            {
                                gem.Direction = Directions.LEFT;
                            }
                            else
                            {
                                gem.Direction = Directions.RIGHT;
                            }
                        }
                        HideCmtsOnCanvas(gems, maxHidingTime);
                        break;
                    }
                case (CombinedDirections.RANDOM):
                    {
                        foreach (GraphicalEffectsMetadata gem in gems)
                        {
                            gem.GenerateRandomDirection();
                        }
                        HideCmtsOnCanvas(gems, maxHidingTime);
                        break;
                    }
                case (CombinedDirections.STRAIGHT):
                    {
                        HideCmtsOnCanvas(gems, maxHidingTime, Properties.Settings.Default.AnimationDirection);
                        break;
                    }
                case (CombinedDirections.NO_DIRECTION):
                    {
                        //I don't know what i Can do with this:P
                        throw new NotImplementedException("I don't know what i Can do with this:P");
                        //break;
                    }
            }
        }

        public void HideCmtsOnCanvas(LinkedList<GraphicalEffectsMetadata> gems, double maxHidingTime)
        {
            double maxDistance = 0;
            double maxElementWidth = 0;
            double maxElementHeight = 0;
            Canvas canvas = (Canvas)mw.FindName(Properties.Settings.Default.MainCanvasName);
            double canvasHidingPositionLeft = double.MaxValue;
            double canvasHidingPositionTop = double.MaxValue;
            double canvasHidingPositionRight = double.MaxValue;
            double canvasHidingPositionDown = double.MaxValue;

            foreach (GraphicalEffectsMetadata gem in gems)
            {

                //if (randomDirections)
                //    gem.GenerateRandomDirection();
                double elementWidth = 0;
                elementWidth = gem.Cmt.ActualWidth;
                if (elementWidth > maxElementWidth)
                    maxElementWidth = elementWidth;
                double elementHeight = 0;
                elementHeight = gem.Cmt.ActualWidth;
                if (elementHeight > maxElementHeight)
                    maxElementHeight = elementHeight;
            }
            canvasHidingPositionRight = canvas.ActualWidth + 5;
            canvasHidingPositionDown = canvas.ActualHeight + 5;
            canvasHidingPositionLeft = -maxElementWidth - 5;
            canvasHidingPositionTop = -maxElementHeight - 5;
            foreach (GraphicalEffectsMetadata gem in gems)
            {
                switch (gem.Direction)
                {
                    case Directions.RIGHT:
                        {
                            double distance = Math.Abs(canvasHidingPositionRight - Canvas.GetLeft(gem.Cmt));
                            if (distance > maxDistance)
                                maxDistance = distance;
                            break;
                        }
                    case Directions.LEFT:
                        {
                            double distance = Math.Abs(canvasHidingPositionLeft - Canvas.GetLeft(gem.Cmt));
                            if (distance > maxDistance)
                                maxDistance = distance;
                            break;
                        }
                    case Directions.DOWN:
                        {
                            double distance = Math.Abs(canvasHidingPositionDown - Canvas.GetTop(gem.Cmt));
                            if (distance > maxDistance)
                                maxDistance = distance;
                            break;
                        }
                    case Directions.TOP:
                        {
                            double distance = Math.Abs(canvasHidingPositionTop - Canvas.GetTop(gem.Cmt));
                            if (distance > maxDistance)
                                maxDistance = distance;
                            break;
                        }
                }
            }
            double speed = 0;
            speed = maxDistance / maxHidingTime;
            if (speed != 0)
            {
                foreach (GraphicalEffectsMetadata gem in gems)
                {
                    double distance = 0;
                    double calculatedTime = 0;
                    switch (gem.Direction)
                    {
                        case Directions.LEFT:
                            distance = Math.Abs(canvasHidingPositionLeft - Canvas.GetLeft(gem.Cmt));
                            calculatedTime = distance / speed;
                            HideCmtOnCanvas(gem, canvasHidingPositionLeft, calculatedTime);
                            break;
                        case Directions.RIGHT:
                            distance = Math.Abs(canvasHidingPositionRight - Canvas.GetLeft(gem.Cmt));
                            calculatedTime = distance / speed;
                            HideCmtOnCanvas(gem, canvasHidingPositionRight, calculatedTime);
                            break;
                        case Directions.DOWN:
                            distance = Math.Abs(canvasHidingPositionDown - Canvas.GetTop(gem.Cmt));
                            calculatedTime = distance / speed;
                            HideCmtOnCanvas(gem, canvasHidingPositionDown, calculatedTime);
                            break;
                        case Directions.TOP:
                            distance = Math.Abs(canvasHidingPositionTop - Canvas.GetTop(gem.Cmt));
                            calculatedTime = distance / speed;
                            HideCmtOnCanvas(gem, canvasHidingPositionTop, calculatedTime);
                            break;
                    }
                }
            }
        }

        public void HideElementsOnCanvas(LinkedList<FrameworkElement> elements, Directions direction, double maxHidingTime)
        {
            double maxDistance = 0;
            double maxElementWidth = 0;
            double maxElementHeight = 0;
            Canvas canvas = (Canvas)mw.FindName(Properties.Settings.Default.MainCanvasName);
            double canvasHidingPositionLeft = double.MaxValue;
            double canvasHidingPositionTop = double.MaxValue;
            LinkedList<CircularMinuteTimer> cmts = new LinkedList<CircularMinuteTimer>();
            //LinkedList<Hashtable> cmts2 = new LinkedList<Hashtable>();
            foreach (FrameworkElement fe in elements)
            {
                cmts.AddLast(fe as CircularMinuteTimer);
                double elementWidth = 0;
                elementWidth = fe.ActualWidth;
                if (elementWidth > maxElementWidth)
                    maxElementWidth = elementWidth;
                double elementHeight = 0;
                elementHeight = fe.ActualWidth;
                if (elementHeight > maxElementHeight)
                    maxElementHeight = elementHeight;
            }
            switch (direction)
            {
                case Directions.RIGHT:
                    {
                        canvasHidingPositionLeft = canvas.ActualWidth;
                        foreach (CircularMinuteTimer cmt in cmts)
                        {
                            double distance = Math.Abs(canvasHidingPositionLeft - Canvas.GetLeft(cmt));
                            if (distance > maxDistance)
                                maxDistance = distance;
                        }
                        break;
                    }
                case Directions.LEFT:
                    {
                        canvasHidingPositionLeft = -maxElementWidth;
                        foreach (CircularMinuteTimer cmt in cmts)
                        {
                            double distance = Math.Abs(canvasHidingPositionLeft - Canvas.GetLeft(cmt));
                            if (distance > maxDistance)
                                maxDistance = distance;
                        }
                        break;
                    }
                case Directions.DOWN:
                    {
                        canvasHidingPositionTop = canvas.ActualHeight;
                        foreach (CircularMinuteTimer cmt in cmts)
                        {
                            double distance = Math.Abs(canvasHidingPositionTop - Canvas.GetTop(cmt));
                            if (distance > maxDistance)
                                maxDistance = distance;
                        }
                        break;
                    }
                case Directions.TOP:
                    {
                        canvasHidingPositionLeft = -maxElementHeight;
                        foreach (CircularMinuteTimer cmt in cmts)
                        {
                            double distance = Math.Abs(canvasHidingPositionTop - Canvas.GetTop(cmt));
                            if (distance > maxDistance)
                                maxDistance = distance;
                        }
                        break;
                    }
            }

            double speed = 0;
            speed = maxDistance / maxHidingTime;
            foreach (CircularMinuteTimer cmt in cmts)
            {
                double distance = 0;
                switch (direction)
                {
                    case Directions.LEFT:
                    case Directions.RIGHT:
                        distance = Math.Abs(canvasHidingPositionLeft - Canvas.GetLeft(cmt));
                        break;
                    case Directions.DOWN:
                    case Directions.TOP:
                        distance = Math.Abs(canvasHidingPositionTop - Canvas.GetTop(cmt));
                        break;
                }
                double calculatedTime = 0;
                calculatedTime = distance / speed;
                HideElementOnCanvas(cmt, direction, canvasHidingPositionLeft, canvasHidingPositionTop, calculatedTime);
            }
        }

        public void UnhideElementOnCanvas(FrameworkElement element, double unhidingLeftTime, double unhidingTopTime, double unhidingLeftMaxTime, double unhidingTopMaxTime)
        {
            Console.WriteLine("Caption: " + (element as CircularMinuteTimer).Caption + " unhidingLeftTime: " + unhidingLeftTime + " unhidingTopTime: " + unhidingTopTime + " unhidingLeftMaxTime: " + unhidingLeftMaxTime + " unhidingTopMaxTime: " + unhidingTopMaxTime);
            Canvas canvas = (Canvas)mw.FindName(Properties.Settings.Default.MainCanvasName);
            CircularMinuteTimer cmt = (CircularMinuteTimer)element;

            double actualLeft = Canvas.GetLeft(cmt);
            double actualTop = Canvas.GetTop(cmt);
            double desiredLeft = cmt.DesiredPositionLeft;
            double desiredTop = cmt.DesiredPositionTop;
            //Console.WriteLine("Caption: " + cmt.Caption + " actualLeft: " + actualLeft.ToString() + " actualTop: " + actualTop.ToString() + " desiredLeft: " + desiredLeft.ToString() + " desiredTop: " + desiredTop.ToString());
            if (unhidingLeftTime > 0)
            {
                DoubleAnimation leftAnimation = new DoubleAnimation();
                leftAnimation.From = actualLeft;
                leftAnimation.To = desiredLeft;
                leftAnimation.BeginTime = TimeSpan.FromSeconds(unhidingLeftMaxTime - unhidingLeftTime);
                leftAnimation.Duration = new Duration(TimeSpan.FromSeconds(unhidingLeftTime));
                cmt.BeginAnimation(Canvas.LeftProperty, leftAnimation);
            }
            if (unhidingTopTime > 0)
            {
                DoubleAnimation topAnimation = new DoubleAnimation();
                topAnimation.From = actualTop;
                topAnimation.To = desiredTop;
                topAnimation.BeginTime = TimeSpan.FromSeconds(unhidingTopMaxTime - unhidingTopTime);
                topAnimation.Duration = new Duration(TimeSpan.FromSeconds(unhidingTopTime));
                cmt.BeginAnimation(Canvas.TopProperty, topAnimation);
            }
        }

        public void UnhideCmtsOnCanvas(LinkedList<GraphicalEffectsMetadata> gems, double maxUnhidingTime)
        {
            double maxDistance = 0;
            Canvas canvas = (Canvas)mw.FindName(Properties.Settings.Default.MainCanvasName);
            foreach (GraphicalEffectsMetadata gem in gems)
            {
                double distanceLeft = 0;
                double distanceTop = 0;
                distanceLeft = Math.Abs(gem.Cmt.DesiredPositionLeft - Canvas.GetLeft(gem.Cmt));
                distanceTop = Math.Abs(gem.Cmt.DesiredPositionTop - Canvas.GetTop(gem.Cmt));
                if (distanceLeft > maxDistance)
                    maxDistance = distanceLeft;
                if (distanceTop > maxDistance)
                    maxDistance = distanceTop;
            }

            double speed = maxDistance / maxUnhidingTime;
            foreach (GraphicalEffectsMetadata gem in gems)
            {
                double distanceLeft = 0;
                double distanceTop = 0;
                distanceLeft = Math.Abs(gem.Cmt.DesiredPositionLeft - Canvas.GetLeft(gem.Cmt));
                distanceTop = Math.Abs(gem.Cmt.DesiredPositionTop - Canvas.GetTop(gem.Cmt));

                double calculatedTimeLeft = 0;
                double calculatedTimeTop = 0;
                calculatedTimeLeft = distanceLeft / speed;
                calculatedTimeTop = distanceTop / speed;
                //Console.WriteLine("???Caption: " + cmt.Caption + " distanceLeft: " + distanceLeft + " maxDistanceLeft: " + maxDistanceLeft + " distanceTop: " + distanceTop + " maxDistanceTop: " + maxDistanceTop);
                //Console.WriteLine("!!!Caption: " + cmt.Caption + " calculatedTimeLeft: " + calculatedTimeLeft + " calculatedTimeTop: " + calculatedTimeTop);
                //MessageBox.Show(calculatedTimeLeft.ToString()+ " " + calculatedTimeTop.ToString());
                UnhideCmtOnCanvas(gem, calculatedTimeLeft, calculatedTimeTop, maxUnhidingTime, maxUnhidingTime);
            }
        }

        public void UnhideCmtOnCanvas(GraphicalEffectsMetadata gem, double unhidingLeftTime, double unhidingTopTime, double unhidingLeftMaxTime, double unhidingTopMaxTime)
        {
            if (!mw.cmtsVisibleMap.Contains(gem.Cmt.Name))
                mw.cmtsVisibleMap.Add(gem.Cmt.Name, gem.Cmt);

            Canvas canvas = (Canvas)mw.FindName(Properties.Settings.Default.MainCanvasName);
            CircularMinuteTimer cmt = gem.Cmt;

            double actualLeft = Canvas.GetLeft(cmt);
            double actualTop = Canvas.GetTop(cmt);
            double desiredLeft = cmt.DesiredPositionLeft;
            double desiredTop = cmt.DesiredPositionTop;
            //Console.WriteLine("Caption: " + cmt.Caption + " actualLeft: " + actualLeft.ToString() + " actualTop: " + actualTop.ToString() + " desiredLeft: " + desiredLeft.ToString() + " desiredTop: " + desiredTop.ToString());
            if (unhidingLeftTime > 0)
            {
                DoubleAnimation leftAnimation = new DoubleAnimation();
                leftAnimation.From = actualLeft;
                leftAnimation.To = desiredLeft;
                leftAnimation.BeginTime = TimeSpan.FromSeconds(unhidingLeftMaxTime - unhidingLeftTime);
                leftAnimation.Duration = new Duration(TimeSpan.FromSeconds(unhidingLeftTime));
                cmt.BeginAnimation(Canvas.LeftProperty, leftAnimation);
            }
            if (unhidingTopTime > 0)
            {
                DoubleAnimation topAnimation = new DoubleAnimation();
                topAnimation.From = actualTop;
                topAnimation.To = desiredTop;
                topAnimation.BeginTime = TimeSpan.FromSeconds(unhidingTopMaxTime - unhidingTopTime);
                topAnimation.Duration = new Duration(TimeSpan.FromSeconds(unhidingTopTime));
                cmt.BeginAnimation(Canvas.TopProperty, topAnimation);
            }
        }

        public void UnhideElementsOnCanvas(LinkedList<FrameworkElement> elements, double maxUnhidingTime)
        {
            double maxDistance = 0;
            LinkedList<CircularMinuteTimer> cmts = new LinkedList<CircularMinuteTimer>();
            Canvas canvas = (Canvas)mw.FindName(Properties.Settings.Default.MainCanvasName);
            foreach (FrameworkElement fe in elements)
            {
                cmts.AddLast(fe as CircularMinuteTimer);
            }
            foreach (CircularMinuteTimer cmt in cmts)
            {
                double distanceLeft = 0;
                double distanceTop = 0;
                distanceLeft = Math.Abs(cmt.DesiredPositionLeft - Canvas.GetLeft(cmt));
                distanceTop = Math.Abs(cmt.DesiredPositionTop - Canvas.GetTop(cmt));
                if (distanceLeft > maxDistance)
                    maxDistance = distanceLeft;
                if (distanceTop > maxDistance)
                    maxDistance = distanceTop;
            }

            double speed = maxDistance / maxUnhidingTime;
            foreach (CircularMinuteTimer cmt in cmts)
            {
                double distanceLeft = 0;
                double distanceTop = 0;
                distanceLeft = Math.Abs(cmt.DesiredPositionLeft - Canvas.GetLeft(cmt));
                distanceTop = Math.Abs(cmt.DesiredPositionTop - Canvas.GetTop(cmt));

                double calculatedTimeLeft = 0;
                double calculatedTimeTop = 0;
                calculatedTimeLeft = distanceLeft / speed;
                calculatedTimeTop = distanceTop / speed;
                //Console.WriteLine("???Caption: " + cmt.Caption + " distanceLeft: " + distanceLeft + " maxDistanceLeft: " + maxDistanceLeft + " distanceTop: " + distanceTop + " maxDistanceTop: " + maxDistanceTop);
                //Console.WriteLine("!!!Caption: " + cmt.Caption + " calculatedTimeLeft: " + calculatedTimeLeft + " calculatedTimeTop: " + calculatedTimeTop);
                //MessageBox.Show(calculatedTimeLeft.ToString()+ " " + calculatedTimeTop.ToString());
                UnhideElementOnCanvas(cmt, calculatedTimeLeft, calculatedTimeTop, maxUnhidingTime, maxUnhidingTime);
            }
        }
    }
}
