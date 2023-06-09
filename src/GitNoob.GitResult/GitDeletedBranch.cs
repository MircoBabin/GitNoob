using GitNoob.Utils;
using System;

namespace GitNoob.GitResult
{
    public class GitDeletedBranch
    {
        public string BranchName { get; set; }
        public string MainBranchName { get; set; }
        public DateTime? DeletionTime { get; set; }
        public string Message { get; set; }

        public GitTag Tag { get; set; }

        public GitDeletedBranch(GitTag Tag)
        {
            this.Tag = Tag;

            SplitDeletedMessage(Tag.Message);
        }

        private void SplitDeletedMessage(string deletedmessage)
        {
            BranchName = String.Empty;
            MainBranchName = String.Empty;
            DeletionTime = null;
            Message = String.Empty;

            var parts = deletedmessage.Split('[');
            int partno = 0;
            foreach (var part in parts)
            {
                var value = part.Trim();
                if (value.EndsWith("]") && value.Length > 1)
                {
                    value = value.Substring(0, value.Length - 1).Trim();

                    partno++;
                    switch (partno)
                    {
                        case 1:
                            try
                            {
                                BranchName = GitUtils.DecodeUtf8Base64(value);
                            }
                            catch { }
                            break;

                        case 2:
                            try
                            {
                                MainBranchName = GitUtils.DecodeUtf8Base64(value);
                            }
                            catch { }
                            break;

                        case 3:
                            try
                            {
                                DeletionTime = DateTime.Parse(value);
                            }
                            catch { }
                            break;

                        case 4:
                            try
                            {
                                Message = GitUtils.DecodeUtf8Base64(value);
                            }
                            catch { }
                            break;
                    }
                }
            }
        }

    }
}
