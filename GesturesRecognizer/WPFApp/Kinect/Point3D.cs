using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace WPFApp.Kinect
{
    class Point3D
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public Point3D(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }
        public Point3D(Point xyPoint, double z)
        {
            this.X = xyPoint.X;
            this.Y = xyPoint.Y;
            this.Z = z;
        }
        public Point3D()
        {
        }
    }
}
