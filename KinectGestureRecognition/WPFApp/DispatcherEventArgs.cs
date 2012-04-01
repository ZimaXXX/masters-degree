using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WPFApp
{
    public class DispatcherEventArgs : EventArgs
    {

        public string caption;
        public Commands command;
        public double animationTime;

        public DispatcherEventArgs(string caption, Commands command, double animationTime)
        {
            this.caption = caption;
            this.command = command;
            this.animationTime = animationTime;
        }

        public DispatcherEventArgs(string caption, Commands command)
        {
            this.caption = caption;
            this.command = command;
            this.animationTime = Properties.Settings.Default.AnimationTime;
        }

        public DispatcherEventArgs(string caption)
        {
            this.caption = caption;
            this.command = Commands.EMPTY_COMMAND;
            this.animationTime = Properties.Settings.Default.AnimationTime;
        }
    }
}
