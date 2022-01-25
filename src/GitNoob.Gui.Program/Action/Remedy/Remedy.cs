using System;
using System.Collections.Generic;
using System.Threading;

namespace GitNoob.Gui.Program.Action.Remedy
{
    public class Remedy : StepsExecutor.IExecutableByStepsExecutor, IVisualizerMessage
    {
        public StepsExecutor.StepsExecutor StepsExecutor { get; set; }
        public Remedy FailureRemedy { get; set; }

        public IVisualizerMessageType VisualizerMessageType { get; protected set; }
        public MessageWithLinks VisualizerMessageText { get; protected set; }
        public Dictionary<string, System.Action<MessageInput>> VisualizerMessageButtons { get; protected set; }

        protected Remedy(Step.Step Step, ref MessageWithLinks Message)
        {
            StepsExecutor = Step.StepsExecutor;
            FailureRemedy = null;

            VisualizerMessageType = IVisualizerMessageType.options;
            VisualizerMessageText = new MessageWithLinks(Message);
            VisualizerMessageText.Append(Environment.NewLine);
            VisualizerMessageText.Append(Environment.NewLine);

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
