using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Reflection;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace WPFApp
{
    public static class RutedEvent
    {
        public static RoutedEvent StartTimerEvent =
            EventManager.RegisterRoutedEvent("startTimer",
                                RoutingStrategy.Bubble,
                                typeof(RoutedEventHandler),
                                typeof(RutedEvent));

        public static RoutedEvent StopTimerEvent =
            EventManager.RegisterRoutedEvent("stopTimer",
                                RoutingStrategy.Bubble,
                                typeof(RoutedEventHandler),
                                typeof(RutedEvent));

        internal static RoutedEventArgs GoStartTimerEvent(DependencyObject dependencyObject){
            Console.WriteLine("go");
            if (dependencyObject == null)
                throw null;
            if (dependencyObject is UIElement)
            {
                var ui = ((UIElement)dependencyObject);
                //Label b = null;
                //GetElement(ui, ref b, "labelTimer");
                RoutedEventArgs routedEventArgs = new RoutedEventArgs(StartTimerEvent);
                //b.RaiseEvent(routedEventArgs);
                ui.RaiseEvent(routedEventArgs);
                return routedEventArgs;
            }
            return null;
        }

        internal static RoutedEventArgs GoStopTimerEvent(DependencyObject dependencyObject)
        {
            if (dependencyObject == null)
                throw null;
            if (dependencyObject is UIElement)
            {
                var ui = ((UIElement)dependencyObject);
                //Label b = null;
                //GetElement(ui, ref b, "labelTimer");
                RoutedEventArgs routedEventArgs = new RoutedEventArgs(StopTimerEvent);
                //b.RaiseEvent(routedEventArgs);
                ui.RaiseEvent(routedEventArgs);
                return routedEventArgs;
            }
            return null;
        }


        private static void GetElement(UIElement root, ref Label cache, string name)
        {
            int count = VisualTreeHelper.GetChildrenCount(root);
            if (count > 0)
                for (int i = 0; i < count; ++i)
                {
                    var child = (FrameworkElement)VisualTreeHelper.GetChild(root, i);
                    if (child != null && child.Name.Equals(name))
                    {
                        cache = (Label)child;
                        return;
                    }
                    GetElement(child, ref cache, name);
                }
        }
    }
}
