using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GesturesEditor
{
    public class GestureMetadata
    {
        public string GestureName { get; set; }
        public int OutputPosition { get; set; }
        public int NumberOfInputs { get; set; }
        public int NumberOfOutputs { get; set; }
        public int NumberOfDimensions { get; set; } 
        public static double NO_VALUE = -1;

        public GestureMetadata(string gestureName, int numberOfInputs, int numberOfOutputs, int outputPosition, int numberOfDimensions)
        {
            this.GestureName = gestureName;
            this.NumberOfInputs = numberOfInputs;
            this.NumberOfOutputs = numberOfOutputs;
            this.OutputPosition = outputPosition;
            this.NumberOfDimensions = numberOfDimensions;
        }

        public GestureMetadata()
        {

        }
    }
}
