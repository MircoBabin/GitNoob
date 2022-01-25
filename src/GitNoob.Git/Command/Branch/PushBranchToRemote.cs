using System;

namespace GitNoob.Git.Command.Branch
{
    public class PushBranchToRemote : Command
    {
        public string output { get; private set; }

        public PushBranchToRemote(GitWorkingDirectory gitworkingdirectory, string localBranch) : base(gitworkingdirectory)
        {
            output = null;

            var remote = new GetRemoteBranch(gitworkingdirectory, localBranch);
            remote.WaitFor();
            if (!String.IsNullOrWhiteSpace(remote.remoteName))
            {
                RunGit("push", "push --quiet \"" + remote.remoteName + "\" \"" + localBranch + "\"");
            }
            else
            {
                output = "Branch \"" + localBranch + "\" is not tracking a remote branch.";
            }
        }

        protected override void RunGitDone()
        {
            if (output == null)
            {
                var executor = GetGitExecutor("push");

                output = executor.Output.Trim();
                if (!String.IsNullOrWhiteSpace(output)) output += Environment.NewLine + Environment.NewLine;
                output += executor.Error.Trim();

                /* Warning: BitBucket outputs informational messages like:
                 * 
                 * -------------------------------------------------------------------------
                 * remote: 
                 * remote: Create pull request for <branch>:        
                 * remote:   https://bitbucket.org/.../pull-requests/new?source=<branch>&t=1        
                 * remote:
                 * -------------------------------------------------------------------------
                 * 
                 * 
                 * See also:
                 *     - https://confluence.atlassian.com/bitbucketserverkb/how-do-i-disable-the-remote-create-pull-request-message-when-pushing-changes-779171665.html
                 *     - https://stackoverflow.com/questions/30518603/when-i-git-push-git-now-says-create-pull-request-for-why
                 */

                //result can not be determined. 
            }
        }
    }
}
