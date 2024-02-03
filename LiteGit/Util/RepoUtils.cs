using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibGit2Sharp;
namespace LiteGit.Util
{
    class RepoUtils
    {
        public static string GetCurrentBranch (string repoDir) {
            var repo = new Repository(repoDir);
            if (repo != null)
            {
                string repoName = repo.Head.FriendlyName;
                repo.Dispose();
                return repoName;
            }
            return "";
        }

        public static List<string> GetAllLocalBranches(string repoDir)
        {
            var repo = new Repository(repoDir);
            List<string> allLocalBranches = new List<string>();
            if (repo != null)
            {
                allLocalBranches = repo.Branches.Where(branch => branch.IsRemote == false).Select(branch => branch.FriendlyName).ToList();
                repo.Dispose();
            }
            return allLocalBranches;
        }

        public static void Pull(Repository repository, string branch)
        {
            try
            {
                PullOptions pullOptions = new PullOptions()
                {
                    MergeOptions = new MergeOptions()
                    {
                        FastForwardStrategy = FastForwardStrategy.Default
                    }


                };
                MergeResult mergeResult = Commands.Pull(
                    repository,
                    new Signature("ifs-satrlk", "satheesh.rathnasiri@ifs.com", DateTimeOffset.Now),
                    pullOptions
                );
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static string CheckoutBranch(Repository repository, string branchName)
        {
            try
            {
                var trackingBranch = repository.Branches[branchName];
                if (trackingBranch.IsRemote)
                {
                    branchName = branchName.Replace("origin/", string.Empty);
                    var branch = repository.CreateBranch(branchName, trackingBranch.Tip);
                    repository.Branches.Update(branch, b => b.TrackedBranch = trackingBranch.CanonicalName);
                    Commands.Checkout(repository, branch, new CheckoutOptions { CheckoutModifiers = CheckoutModifiers.Force });
                }
                else
                {
                    Commands.Checkout(repository, trackingBranch, new CheckoutOptions { CheckoutModifiers = CheckoutModifiers.Force });
                }
                return branchName;
            }
            catch (Exception e) 
            {
                throw new Exception(e.Message);
            }
        }
    }
}
