using System;
using System.Collections.Generic;

namespace GitNoob.Git.Command
{
    public abstract class Command
    {
        protected GitWorkingDirectory _gitworkingdirectory;
        private Dictionary<string, ExecutorGit> _executors;
        private Dictionary<string, Command> _commands;

        public Command(GitWorkingDirectory gitworkingdirectory)
        {
            _gitworkingdirectory = gitworkingdirectory;
            _executors = new Dictionary<string, ExecutorGit>();
            _commands = new Dictionary<string, Command>();
        }

        public ExecutorGit RunGit(string executorName, string parms, string workingDirectory = null)
        {
            var executor = new ExecutorGit(
                _gitworkingdirectory.GitExecutable,
                (String.IsNullOrEmpty(workingDirectory) ? _gitworkingdirectory.WorkingPath : workingDirectory),
                parms);

            _executors.Add(executorName, executor);

            return executor;
        }

        public Command RunCommand(string executorName, Command command)
        {
            if (command == null) return null;

            _commands.Add(executorName, command);

            return command;
        }

        public void WaitFor(string executorName)
        {
            {
                ExecutorGit executor = null;
                _executors.TryGetValue(executorName, out executor);
                if (executor != null)
                {
                    executor.WaitFor();
                }
            }

            {
                Command command = null;
                _commands.TryGetValue(executorName, out command);
                if (command != null)
                {
                    command.WaitFor();
                }
            }
        }

        public void WaitFor()
        {
            foreach (var executor in _executors)
            {
                executor.Value.WaitFor();
            }

            foreach (var command in _commands)
            {
                command.Value.WaitFor();
            }

            RunGitDone();
        }

        protected ExecutorGit GetGitExecutor(string executorName)
        {
            return _executors[executorName];
        }

        protected Command GetCommand(string executorName)
        {
            return _commands[executorName];
        }

        protected abstract void RunGitDone();

    }
}
