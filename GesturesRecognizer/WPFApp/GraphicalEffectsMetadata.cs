using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WPFApp
{
    public class GraphicalEffectsMetadata
    {
        public GraphicalEffectsMetadata(CircularMinuteTimer cmt, Directions direction)
        {
            Direction = direction;
            Cmt = cmt;        
        }

        public GraphicalEffectsMetadata(CircularMinuteTimer cmt)
        {
            GenerateRandomDirection();
            Cmt = cmt;
        }

        public void GenerateRandomDirection()
        {
            Direction = RandomEnum<Directions>(GraphicalEffects.rand);
            Console.WriteLine("Direction: " + Direction.ToString());
        }
                
        public T RandomEnum<T>(Random rand)
        {
            T[] values = (T[])Enum.GetValues(typeof(T));
            return values[rand.Next(0, values.Length)];
        }

        public Directions Direction { get; set; }
        public CircularMinuteTimer Cmt { get; set; }
    }
}
