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
using System.Windows.Media.Animation;

namespace WPFApp
{
    /// <summary>
    /// Interaction logic for InfoBoard.xaml
    /// </summary>
    public partial class InfoBoard : UserControl
    {
        MainWindow mw = null;
        public double PositionLeft { get; set; }
        public double PositionTop { get; set; }

        public InfoBoard()
        {
            InitializeComponent();
        }

        private void setInfoBoardPositionOnCanvas()
        {
            Canvas.SetLeft(this, PositionLeft - ActualWidth / 2);
            Canvas.SetTop(this, PositionTop - ActualHeight / 2);
        }

        private void infoBoard_Loaded(object sender, RoutedEventArgs e)
        {
            setInfoBoardPositionOnCanvas();
            this.SizeChanged += new SizeChangedEventHandler(InfoBoard_SizeChanged);
        }

        public void AddTextBlock(TextBlock tb)
        {
            tb.HorizontalAlignment = HorizontalAlignment.Center;
            StackPanel sp = (StackPanel)FindName("MainStackPanel");
            sp.Children.Add(tb);
        }

        public void ShowInfoBoard()
        {
            Canvas.SetLeft(this, PositionLeft - ActualWidth / 2);
            Canvas.SetTop(this, PositionTop - ActualHeight / 2);  
            Storyboard sbdShowInfoBoard = (Storyboard)FindResource("ShowInfoBoard");
            sbdShowInfoBoard.Completed += new EventHandler(sbdShowInfoBoard_Completed);
            sbdShowInfoBoard.Begin(this);
        }

        public void HideInfoBoard()
        {
            ((Storyboard)FindResource("HideInfoBoard")).Begin(this);
        }

        public void ShowAndHideInfoBoard()
        {
            Storyboard sbdShowAndHideInfoBoard = (Storyboard)FindResource("ShowAndHideInfoBoard");
            sbdShowAndHideInfoBoard.Completed += new EventHandler(sbdShowAndHideInfoBoard_Completed);
            sbdShowAndHideInfoBoard.Begin(this);
        }

        public void sbdShowInfoBoard_Completed(object sender, EventArgs e)
        {
            
        }
        public void sbdShowAndHideInfoBoard_Completed(object sender, EventArgs e)
        {

        }

        private void InfoBoard_SizeChanged (object sender, System.EventArgs e)
        {
            setInfoBoardPositionOnCanvas();
        }
    }
}
