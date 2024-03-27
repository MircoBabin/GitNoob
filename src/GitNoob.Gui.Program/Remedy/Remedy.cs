using GitNoob.Gui.Visualizer;
using System;
using System.Collections.Generic;
using System.Threading;

namespace GitNoob.Gui.Program.Remedy
{
    public class Remedy : StepsExecutor.IExecutableByStepsExecutor, IVisualizerMessage
    {
        public StepsExecutor.StepsExecutor StepsExecutor { get; set; }
        public Remedy FailureRemedy { get; set; }

        public IVisualizerMessageType VisualizerMessageType { get; protected set; }
        public VisualizerMessageWithLinks VisualizerMessageText { get; protected set; }
        public VisualizerMessageWithLinks VisualizerMessageInput2 { get; protected set; }
        public List<VisualizerMessageButton> VisualizerMessageButtons { get; protected set; }

        protected Remedy(Step.Step Step, ref VisualizerMessageWithLinks Message)
        {
            StepsExecutor = Step.StepsExecutor;
            FailureRemedy = null;

            VisualizerMessageType = IVisualizerMessageType.options;
            VisualizerMessageText = new VisualizerMessageWithLinks(Message);
            VisualizerMessageText.Append(Environment.NewLine);
            VisualizerMessageText.Append(Environment.NewLine);

            VisualizerMessageInput2 = new VisualizerMessageWithLinks();

            VisualizerMessageButtons = null;
            Message = null; //prevent changing the wrong message
        }

        public string MainBranch
        {
            get
            {
                return StepsExecutor.Config.Git.MainBranch;
            }
        }

        private AutoResetEvent _executeDone = null;
        public bool execute()
        {
            if (VisualizerMessageButtons != null)
            {
                _executeDone = new AutoResetEvent(false);
                StepsExecutor.Message(this);
                _executeDone.WaitOne();
                _executeDone = null;
                return true;
            }

            return false;
        }

        public void Cancel()
        {
            StepsExecutor.Cancel();
            if (_executeDone != null) _executeDone.Set();
        }

        public void Done()
        {
            if (_executeDone != null) _executeDone.Set();
        }
    }
}
