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
using System.Windows.Media.Animation;
using System.ComponentModel;
using System.Reflection;

namespace WPFApp
{
    /// <summary>
    /// Interaction logic for CircularMinuteTimer.xaml
    /// </summary>
    public partial class CircularMinuteTimer : UserControl
    {
        public enum Modes
        {
            SIMPLE,
            EXTENDED
        }
        public CircularMinuteTimer()
        {
            InitializeComponent();
        }
        public static readonly DependencyProperty CaptionProperty;
        public static readonly DependencyProperty CommandProperty;
        public static readonly DependencyProperty DesiredPositionLeftProperty;
        public static readonly DependencyProperty DesiredPositionTopProperty;
        public static readonly DependencyProperty ModeProperty;
        public static readonly DependencyProperty ValueProperty;
        public event EventHandler<DispatcherEventArgs> CallParentWindow;

        MainWindow mw = null;

        public bool IsSelected { get; set; }

        public static Window FindParentWindow(DependencyObject child)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(child);

            //CHeck if this is the end of the tree
            if (parent == null) return null;

            Window parentWindow = parent as Window;
            if (parentWindow != null)
            {
                return parentWindow;
            }
            else
            {
                //use recursion until it reaches a Window
                return FindParentWindow(parent);
            }
        }

        static CircularMinuteTimer()
        {
            PropertyMetadata captionMetadata =
                new PropertyMetadata("NO NAME"); // default value
            CaptionProperty = DependencyProperty.Register("Caption",
            typeof(string), typeof(CircularMinuteTimer), captionMetadata);

            PropertyMetadata commandMetadata =
                new PropertyMetadata(Commands.EMPTY_COMMAND); // default value
            CommandProperty = DependencyProperty.Register("Command",
            typeof(Commands), typeof(CircularMinuteTimer), commandMetadata);

            PropertyMetadata desiredPositionLeftMetadata =
                new PropertyMetadata((double)0); // default value
            DesiredPositionLeftProperty = DependencyProperty.Register("DesiredPositionLeft",
            typeof(double), typeof(CircularMinuteTimer), desiredPositionLeftMetadata);

            PropertyMetadata desiredPositionTopMetadata =
                new PropertyMetadata((double)0); // default value
            DesiredPositionTopProperty = DependencyProperty.Register("DesiredPositionTop",
            typeof(double), typeof(CircularMinuteTimer), desiredPositionTopMetadata);

            PropertyMetadata modeMetadata =
                new PropertyMetadata(Modes.SIMPLE); // default value
            ModeProperty = DependencyProperty.Register("Mode",
            typeof(Modes), typeof(CircularMinuteTimer), modeMetadata);

            PropertyMetadata valueMetadata =
                new PropertyMetadata(null); // default value
            ValueProperty = DependencyProperty.Register("Value",
            typeof(object), typeof(CircularMinuteTimer), valueMetadata);
        }

        public object Value
        {
            get { return (object)GetValue(ValueProperty); }
            set
            {
                SetValue(ValueProperty, value);
            }
        }

        public Modes Mode
        {
            get { return (Modes)GetValue(ModeProperty); }
            set { SetValue(ModeProperty, value); }
        }

        public string Caption
        {
            get { return (string)GetValue(CaptionProperty); }
            set { SetValue(CaptionProperty, value); }
        }

        public Commands Command
        {
            get { return (Commands)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public double DesiredPositionLeft
        {
            get { return (double)GetValue(DesiredPositionLeftProperty); }
            set { SetValue(DesiredPositionLeftProperty, value); }
        }

        public double DesiredPositionTop
        {
            get { return (double)GetValue(DesiredPositionTopProperty); }
            set { SetValue(DesiredPositionTopProperty, value); }
        }

        public Type ValueType { get; set; }


        private void Progress_Completed(object sender, EventArgs e)
        {
            //Storyboard deactivateEllipse = (Storyboard)FindResource("DeactivateEllipse");
            //deactivateEllipse.Begin(this);
            Window mainWindow = FindParentWindow(this);

            OnCallParentWindow();
        }


        //MethodInfo castMethod = ValueType.GetType().GetMethod("Cast").MakeGenericMethod(t);
        //object castedObject = castMethod.Invoke(null, new object[] { obj });

        private int indexOfNextValidValue(Array array, int i)
        {
            int length = array.Length;
            int index = -1;
            if (i >= length - 1)
            {
                if ((int)array.GetValue(0) < 0)
                {
                    index = indexOfNextValidValue(array, 0);
                }
                else
                {
                    index = 0;
                }
            }
            else
            {
                if ((int)array.GetValue(i + 1) < 0)
                {
                    index = indexOfNextValidValue(array, i + 1);
                }
                else
                {
                    index = i + 1;
                }
            }
            return index;
        }

        private int indexOfNextValue(Array array, int i)
        {
            int length = array.Length;
            int index = -1;
            if (i >= length - 1)
            {
                index = 0;
            }
            else
            {
                index = i + 1;
            }
            return index;
        }

        public void NextValue()
        {

            //MessageBox.Show(ValueType.ToString());
            bool isEnum = false;
            Array array = null;
            try
            {
                array = Enum.GetValues(ValueType);
                isEnum = true;
            }
            catch (System.ArgumentException)
            {
                array = mw.frameworkConstants.GetCorrespondingArray(this.Name);
                isEnum = false;
            }
            //Languages[] values = (ValueType[])(object[])array;
            var converter = TypeDescriptor.GetConverter(ValueType);
            if (converter.GetType() == typeof(System.ComponentModel.StringConverter))
            {
                var currentValue = converter.ConvertFrom(Value);
                for (int i = 0; i < array.Length; ++i)
                {
                    var convertedValue = converter.ConvertFrom(array.GetValue(i));
                    //Console.WriteLine(((ValueType)array.GetValue(i)).ToString());
                    if ((convertedValue).Equals(currentValue))
                    {
                        int index = -1;
                        if (isEnum)
                            index = indexOfNextValidValue(array, i);
                        else
                            index = indexOfNextValue(array, i);
                        Value = array.GetValue(index);
                    }
                }


            }
            else
            {
                ValueType currentValue = Value as ValueType;
                for (int i = 0; i < array.Length; ++i)
                {
                    Console.WriteLine(((ValueType)array.GetValue(i)).ToString());
                    if (((ValueType)array.GetValue(i)).Equals(currentValue))
                    {
                        int index = -1;
                        if (isEnum)
                            index = indexOfNextValidValue(array, i);
                        else
                            index = indexOfNextValue(array, i);
                        Value = array.GetValue(index);
                    }
                }
            }
            //Console.WriteLine("Value: " + Value.ToString() + " TypeOf: " + Value.GetType().ToString());            
        }

        public bool IsInBounds(Point point)
        {
            double relX = Canvas.GetLeft(this);
            double relY = Canvas.GetTop(this);
            double maxX = relX + this.ActualWidth;
            double maxY = relY + this.ActualHeight;
            //Console.WriteLine(relX + " " + relY + " " + maxX + " " + maxY);
            if (point.X >= relX && point.X <= maxX && point.Y >= relY && point.Y <= maxY)
            {
                return true;
            }
            return false;
        }

        protected virtual void OnCallParentWindow()
        {
            DispatcherEventArgs args = new DispatcherEventArgs(this.Caption, this.Command);
            if (CallParentWindow != null)
            {
                CallParentWindow(this, args);
            }
        }

        private void circularMinuteTimer_Loaded(object sender, RoutedEventArgs e)
        {
            App app = ((App)Application.Current);
            mw = (MainWindow)app.MainWindow;
            switch (Mode)
            {
                case Modes.SIMPLE:
                    {
                        break;
                    }
                case Modes.EXTENDED:
                    {
                        TextBlock tb = new TextBlock();
                        Binding bind = new Binding("Value");
                        bind.Source = this;
                        tb.SetBinding(TextBlock.TextProperty, bind);
                        //if (Value == null)
                        //    tb.Text = mw.frameworkConstants.NoData;
                        //else
                        //    tb.Text = Value.ToString();
                        tb.Foreground = new SolidColorBrush(Colors.LightBlue);
                        tb.VerticalAlignment = VerticalAlignment.Center;
                        tb.HorizontalAlignment = HorizontalAlignment.Center;
                        tb.FontWeight = FontWeights.Bold;
                        StackPanel sp = (StackPanel)FindName("ViewBoxStackPanel");
                        sp.Children.Add(tb);
                        break;
                    }
            }
        }
    }
}