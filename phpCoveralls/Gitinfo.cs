using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using phpCoveralls.Models.Git;

namespace phpCoveralls
{
    public class GitInfo
    {
        private readonly string gitProcessor = "git";
        private string dir;
        public GitInfo(string dir)
        {
            this.dir = dir;
        }

        public GitHeadModel GetLastCommitInfo()
        {
            //Hash authorName commiterName subject
            var result = ExecuteCommand("log -1 --pretty=format:%H\n%aN\n%cN\n%s");
            if (string.IsNullOrEmpty(result))
                Log.ErrorWriteLine("Last commit not found");
            var arr = result.Split('\n');
            var gitHeadModel = new GitHeadModel();
            gitHeadModel.Id = arr[0];
            gitHeadModel.AuthorName = arr[1];
            gitHeadModel.CommitterName = arr[2];
            gitHeadModel.Message = arr[3];
            return gitHeadModel;
        }

        public string GetCurrentBranchName()
        {
            var result = ExecuteCommand("branch");
            if (string.IsNullOrEmpty(result))
                Log.ErrorWriteLine("Last commit not found");
            var branchName = Regex.Match(result, @"\* (.*?)\n").Groups[1].Value;
            return branchName;
        }

        private string ExecuteCommand(string command)
        {
            try
            {
                var process = Process.Start(gitProcessor, command);
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.WorkingDirectory = dir;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.Start();
                using (var sr = process.StandardOutput)
                {
                    var result = sr.ReadToEnd();
                    return result;
                }
            }
            catch (Exception ex)
            {
                Log.ErrorWriteLine($"[{gitProcessor}] {ex.Message}");
            }
            return string.Empty;
        }
    }
}