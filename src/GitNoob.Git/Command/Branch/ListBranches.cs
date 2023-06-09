using System;
using System.Collections.Generic;

namespace GitNoob.Git.Command.Branch
{
    public class ListBranches : Command
    {
        public List<Result.GitBranch> result { get; private set; }

        private bool listRemoteBranches;

        public ListBranches(GitWorkingDirectory gitworkingdirectory, bool listRemoteBranches, string oneBranchName = null) : base(gitworkingdirectory)
        {
            result = null;

            string command = "for-each-ref --color=never \"--format=%(refname)%1f%(refname:short)%1f%(upstream)%1f%(upstream:short)\" ";
            if (String.IsNullOrWhiteSpace(oneBranchName))
            {
                command += "\"refs/heads\"";
            }
            else
            {
                if (oneBranchName.StartsWith("refs/"))
                {
                    command += "\"" + oneBranchName + "\"";
                }
                else
                {
                    command += "\"refs/heads/" + oneBranchName + "\"";
                }
            }
            RunGit("list", command);

            this.listRemoteBranches = listRemoteBranches;
            if (this.listRemoteBranches)
            {
                RunGit("remote", "for-each-ref --color=never \"--format=%(refname)%1f%(refname:short)%1f%(upstream)%1f%(upstream:short)\" \"refs/remotes\"");
            }
        }

        protected override void RunGitDone()
        {
            var executor = GetGitExecutor("list");

            result = new List<Result.GitBranch>();
            foreach (var line in executor.Output.Trim().Split('\n'))
            {
                var parts = line.Trim().Split('\u001f');
                if (parts.Length >= 4)
                {
                    string fullname = parts[0].Trim();
                    string shortname = parts[1].Trim();
                    string remote = parts[2].Trim();
                    string remoteshort = parts[3].Trim();

                    if (String.IsNullOrWhiteSpace(remote))
                    {
                        result.Add(new Result.GitBranch(fullname, shortname, Result.GitBranch.BranchType.Local));
                    }
                    else
                    {
                        result.Add(new Result.GitBranch(fullname, shortname, Result.GitBranch.BranchType.LocalTrackingRemoteBranch, remote, remoteshort));
                    }
                }
            }
            result.Sort(delegate (Result.GitBranch x, Result.GitBranch y)
            {
                return x.ShortName.ToLowerInvariant().CompareTo(y.ShortName.ToLowerInvariant());
            });


            if (this.listRemoteBranches)
            {
                var remotes = new List<Result.GitBranch>();
                executor = GetGitExecutor("remote");
                foreach (var line in executor.Output.Trim().Split('\n'))
                {
                    var parts = line.Trim().Split('\u001f');
                    if (parts.Length >= 4)
                    {
                        string fullname = parts[0].Trim();
                        string shortname = parts[1].Trim();
                        string remote = parts[2].Trim();
                        string remoteshort = parts[3].Trim();

                        if (!fullname.EndsWith("/HEAD"))
                        {
                            bool found = false;
                            foreach (var branch in result)
                            {
                                if (branch.RemoteBranchFullName == fullname)
                                {
                                    found = true;
                                    break;
                                }
                            }
                            if (!found)
                            {
                                remotes.Add(new Result.GitBranch(fullname, shortname, Result.GitBranch.BranchType.UntrackedRemoteBranch));
                            }
                        }
                    }
                }
                remotes.Sort(delegate (Result.GitBranch x, Result.GitBranch y)
                {
                    return x.ShortName.ToLowerInvariant().CompareTo(y.ShortName.ToLowerInvariant());
                });

                foreach(var branch in remotes)
                {
                    result.Add(branch);
                }
            }
        }
    }
}
