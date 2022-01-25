using System;

namespace GitNoob.Gui.Program.Action.Step
{
    public abstract class Step : StepsExecutor.IExecutableByStepsExecutor
    {
        public StepsExecutor.StepsExecutor StepsExecutor { get; set; }
        public Remedy.Remedy FailureRemedy { get; set; }

        private string _busyMessage;
        public string BusyMessage
        {
            get { return _busyMessage; }
            set
            {
                _busyMessage = value;

                if (_executing)
                {
                    StepsExecutor.BusyMessage(_busyMessage);
                }
            }
        }

        public string MainBranch { get { return this.StepsExecutor.Config.Git.MainBranch; } }



        protected Step()
        {
            this.StepsExecutor = null;
            _busyMessage = String.Empty;

            this.FailureRemedy = null;
        }

        private bool _executing = false;
        public bool execute()
        {
            if (_executing) throw new Exception("already executing");

            this.FailureRemedy = null;

            StepsExecutor.BusyMessage(_busyMessage);
            _executing = true;
            try
            {
                try
                {
                    return run();
                }
                catch (Exception ex)
                {
                    this.FailureRemedy = new Remedy.MessageException(this, new MessageWithLinks(), ex);
                    return false;
                }
            }
            finally
            {
                _executing = false;
                StepsExecutor.BusyMessage(null);
            }
        }

        protected abstract bool run();
    }
}
