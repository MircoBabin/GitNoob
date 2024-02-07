using GitNoob.Gui.Visualizer;
using System;
using System.Collections.Generic;

namespace GitNoob.Gui.Program.Remedy
{
    public class MessageRemoteNotReachable : Remedy
    {
        public MessageRemoteNotReachable(Step.Step Step, VisualizerMessageWithLinks Message, string remoteUrl) :
            base(Step, ref Message)
        {
            VisualizerMessageText.Append("Remote \"");
            VisualizerMessageText.AppendLink(remoteUrl, () => {
                StepsExecutor.CopyToClipboard(remoteUrl);
            });
            VisualizerMessageText.Append("\" is not reachable.");
            VisualizerMessageText.Append(Environment.NewLine);
            VisualizerMessageText.Append(Environment.NewLine);
            VisualizerMessageText.Append("Possible causes:");
            VisualizerMessageText.Append(Environment.NewLine);
            VisualizerMessageText.Append("- There is no connection with the internet.");
            VisualizerMessageText.Append(Environment.NewLine);
            VisualizerMessageText.Append("- The credentials (username/password) for the remote are invalid.");
            VisualizerMessageText.Append(Environment.NewLine);
            VisualizerMessageText.Append("- The credentials are not automatically filled in. Are not retrievable via the git credential manager.");
            VisualizerMessageText.Append(Environment.NewLine);
            VisualizerMessageText.Append("- The remote is down.");
            VisualizerMessageText.Append(Environment.NewLine);

            VisualizerMessageButtons =
                new List<VisualizerMessageButton>()
                {
                    new VisualizerMessageButton("Cancel", (input) => {
                        Cancel();
                    }),
                    new VisualizerMessageButton("Execute the check command in a command prompt. And manually investigate the problem.", (input) => {
                        var batfile = new Utils.BatFile(this.StepsExecutor.Config.visualizerShowException, "git-remote",
                            Utils.BatFile.RunAsType.runAsInvoker, Utils.BatFile.WindowType.showWindow, "Git remote",
                            this.StepsExecutor.Config.Project,
                            this.StepsExecutor.Config.ProjectWorkingDirectory,
                            this.StepsExecutor.Config.PhpIni);
                        batfile.Append(
                            "@echo off" + Environment.NewLine +
                            "    echo %cd%" + Environment.NewLine +
                            "    echo git ls-remote \"" + remoteUrl + "\"" + Environment.NewLine +
                            "    echo." + Environment.NewLine +
                            "    echo." + Environment.NewLine +
                            "    git ls-remote \"" + remoteUrl + "\"" + Environment.NewLine +
                            "    echo." + Environment.NewLine +
                            "    echo." + Environment.NewLine +
                            "    pause" + Environment.NewLine);
                        batfile.Start();
                    }),
                };
        }
    }
}
