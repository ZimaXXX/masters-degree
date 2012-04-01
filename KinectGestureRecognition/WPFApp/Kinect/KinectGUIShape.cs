using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;
using Microsoft.Research.Kinect.Nui;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPFApp.Kinect
{
    public class KinectGUIShape : UIElement
    {
        public Shape shape;
        public Label label;
        public String name;
        private static float focused = 0.8f;
        private static float unfocused = 0.5f;
        public JointID reactionJoint;
        public bool hasEntered = false;
        public bool hasLeft = false;
        public bool hasTimer = false;

        public KinectGUIShape(Shape shape, Label label, JointID reactionJoint)
        {
            this.label = label;
            this.shape = shape;
            this.reactionJoint = reactionJoint;
            this.name = shape.Name;
        }

        public void focusShape()
        {
            shape.Opacity = focused;
            label.Opacity = focused;
        }

        public void unfocusShape()
        {
            shape.Opacity = unfocused;
            label.Opacity = unfocused;
        }

        public bool isInBounds(Point point)
        {
            int relX = (int)Canvas.GetLeft(shape);
            int relY = (int)Canvas.GetTop(shape);
            int maxX = relX + (int)shape.Width;
            int maxY = relY + (int)shape.Height;
            //Console.WriteLine(relX + " " + relY + " " + maxX + " " + maxY);
            if (point.X >= relX && point.X <= maxX && point.Y >= relY && point.Y <= maxY)
            {
                return true;
            }
            return false;
        }

        public void moveRight(int length)
        {
            Canvas.SetLeft(shape, Canvas.GetLeft(shape) + length);
            Canvas.SetLeft(label, Canvas.GetLeft(label) + length);
        }

        public void moveDown(int length)
        {
            Canvas.SetTop(shape, Canvas.GetTop(shape) + length);
            Canvas.SetTop(label, Canvas.GetTop(label) + length);
        }

        public static Shape ShapeFactory(Shape shape, int height, int width, String name)
        {
            shape.Width = width;
            shape.Height = height;
            shape.Name = name;
            shape.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#97FFFFFF"));
            shape.Stroke = new SolidColorBrush(Colors.White);
            shape.Opacity = unfocused;
            shape.StrokeThickness = 5;
            shape.OpacityMask = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DB000000"));
            return shape;
        }

        public static Label LabelFactory(String content, Shape shape)
        {
            Label label = new Label();
            label = new Label();
            label.Content = content;
            label.FontSize = 30;
            label.Opacity = unfocused;
            label.Foreground = new SolidColorBrush(Colors.White);
            int shapeLeft = (int)Canvas.GetLeft(shape);
            int shapeTop = (int)Canvas.GetTop(shape);
            System.Drawing.Font font = new System.Drawing.Font(label.FontFamily.Source, (float)label.FontSize);
            System.Drawing.Size textSize = System.Windows.Forms.TextRenderer.MeasureText(content, font);
            int textwidth = (int)textSize.Width;
            Console.WriteLine("textWidth: " + textwidth + ", shape.width: " + shape.Width);
            //Canvas.SetLeft(label, shapeLeft + 10);
            Canvas.SetLeft(label, (int)(shapeLeft + (shape.Width - textwidth) / 2) + content.Length*3);
            Canvas.SetTop(label, shapeTop + (int)(shape.Height/2) - label.FontSize);
            ((Canvas)shape.Parent).Children.Add(label);
            return label;
        }
    }

     
}
