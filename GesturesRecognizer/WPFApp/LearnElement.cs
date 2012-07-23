using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HAKGERSoft.Synapse;

namespace WPFApp
{
    public class LearnElement
    {
        public double[] Input {get; set;}
        public double[] Output { get; set; }

        public LearnElement()
        {
        }

        public LearnElement(int inputLength, int outputLength)
        {            
            Input = new double[inputLength];
            Output = new double[outputLength];
        }

        public LearnElement(double[] input, double[] output)
        {
            Input = input;
            Output = output;
        }

        public LearnElement(TrainingData td)
        {
            Input = td.Input;
            Output = td.Output;
        }

        public TrainingData ToTrainingData()
        {
            return new TrainingData(Input, Output);
        }
    }


}
