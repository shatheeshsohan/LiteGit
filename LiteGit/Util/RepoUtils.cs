using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using LibGit2Sharp;
using LibGit2Sharp.Handlers;
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

        public static List<string> GetAllRemoteBranches(string repoDir,string remoteRepo, string filter)
        {
            var repo = new Repository(repoDir);
            var refs = repo.Network.ListReferences(remoteRepo).ToList();
            List<string> allRemoteBranches = new List<string>();
            if (repo != null && refs != null && !string.IsNullOrEmpty(filter))
            {
                allRemoteBranches = refs.Select(remoteRef => remoteRef.CanonicalName.Replace("refs/heads/", "")).Where(selectedRef => selectedRef != "HEAD" && selectedRef.Contains(filter)).ToList();
            }
            return allRemoteBranches;
        }

        public static void Pull(Repository repository, string branchName)
        {
            try
            {
                branchName = "origin/" + branchName;
                var trackingBranch = repository.Branches[branchName];
   
                if (trackingBranch.IsRemote) // even though I dont want to set tracking branch like this
                {
                    var branch = repository.Head;
                    repository.Branches.Update(branch, b => b.TrackedBranch = trackingBranch.CanonicalName);
                }
                PullOptions pullOptions = new PullOptions()
                {
                    MergeOptions = new MergeOptions()
                    {
                        FastForwardStrategy = FastForwardStrategy.Default
                    }
                };

                MergeResult mergeResult = Commands.Pull(
                    repository,
                    new Signature("shatheeshsohan", "usohanac@gmail.com", DateTimeOffset.Now),
                    pullOptions
                );
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static void Fetch(Repository repository, string branch)
        {
            try
            {
                var options = new FetchOptions();
                options.Prune = true;
                options.TagFetchMode = TagFetchMode.Auto;
                options.CredentialsProvider = new CredentialsHandler(
                    (url, usernameFromUrl, types) =>
                        new UsernamePasswordCredentials()
                        {
                            Username = "shatheeshsohan",
                            Password = "fucksex145@123"
                        });

                var remote = repository.Network.Remotes["origin"];
                var msg = "Fetching remote";
                var refSpecs = remote.FetchRefSpecs.Select(x => x.Specification);
                Commands.Fetch(repository, remote.Name, refSpecs, options, msg);
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
                if (trackingBranch == null) 
                {
                    branchName = "origin/" + branchName;
                    trackingBranch = repository.Branches[branchName];
                }
                if (trackingBranch != null && trackingBranch.IsRemote)
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
