using LibGit2Sharp;
using LibGit2Sharp.Handlers;
using LiteGit.Model;
using LiteGit.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

namespace LiteGit
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            AppInit();
            UIInit();
        }

        private void AppInit()
        {
            try
            {
                var lines = File.ReadLines(@"C:\IFS\litegit.properties");
                var data = lines.Select(l => l.Split('='));

                List<BaseComboItem> allReleasePaths = data.Where(arr => arr.Length > 0).Select(arr => new BaseComboItem
                {
                    Release = arr[0].Trim(),
                    Path = arr[1].Trim()
                }).ToList();

                foreach (BaseComboItem item in allReleasePaths)
                {
                    if (item.Release == "git") 
                    {
                        BaseCache.SCache.SetItemsForCache("git", item.Path);
                    }
                    else 
                    {
                        BaseCombo.Items.Add(item.Path);
                    }
                }

                BaseCombo.SelectedIndex = 0;
                FetchedCurrentBranch.Content = "Current branch is not fetched yet.";
                RemoteFilterText.Text = string.Empty;
            }
            catch (FileNotFoundException e)
            {
                System.Windows.Forms.MessageBox.Show("litegit.properties not cound in C/IFS" + e.Message.ToString(), "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(0);
            }

        }

        private void UIInit()
        {
            this.BottomStatusBar.BorderThickness = new Thickness(0.5);
            string currentRepo = (string)BaseCombo.SelectedItem;
            LocalBranchesListBox.Items.Clear();
            RepoUtils.GetAllLocalBranches(currentRepo).ForEach(item => LocalBranchesListBox.Items.Add(item));
            RemoteBranchesListBox.Items.Clear();
            RepoUtils.GetAllRemoteBranches(currentRepo, BaseCache.SCache.GetItemsFromCache("git"), RemoteFilterText.Text).ForEach(item => RemoteBranchesListBox.Items.Add(item));
        }

        private void StatusButton_Click(object sender, RoutedEventArgs e)
        {
            FetchedCurrentBranch.Content = RepoUtils.GetCurrentBranch((string)BaseCombo.SelectedItem);
            CheckOutTextBox.Text = (string)FetchedCurrentBranch.Content;
        }

        private void BaseCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string currentRepo = (string)BaseCombo.SelectedItem;
            FetchedCurrentBranch.Content = RepoUtils.GetCurrentBranch(currentRepo);
            LocalBranchesListBox.Items.Clear();
            RepoUtils.GetAllLocalBranches(currentRepo).ForEach(item => LocalBranchesListBox.Items.Add(item));
            LocalBranchesListBox.SelectedIndex = 0;
            CheckOutTextBox.Clear();
        }

        private void CheckoutButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string currentRepo = (string)BaseCombo.SelectedItem;
                var repo = new Repository(currentRepo);
                RepoUtils.CheckoutBranch(repo, CheckOutTextBox.Text);
                bool isItemAlreadyExists = Utils.IsItemExists(LocalBranchesListBox, CheckOutTextBox.Text);
                if (!isItemAlreadyExists) 
                {
                    LocalBranchesListBox.Items.Add(CheckOutTextBox.Text);
                }
            }
            catch (Exception)
            {
                System.Windows.Forms.MessageBox.Show("Please verify the checkout branch", "Branch Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void LocalBranchesListBox_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            CheckOutTextBox.Text = (string) LocalBranchesListBox.SelectedItem;
        }

        private void RemoteBranchesListBox_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("You are going to pull " + "'" + (string)RemoteBranchesListBox.SelectedItem + "'"+ " remote branch.", "Remote Checkout", MessageBoxButtons.YesNo);
            if (dialogResult == System.Windows.Forms.DialogResult.Yes)
            {
                CheckOutTextBox.Text = (string)RemoteBranchesListBox.SelectedItem;
            }
        }

        private void PullButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string currentRepo = (string)BaseCombo.SelectedItem;
                var repo = new Repository(currentRepo);
                RepoUtils.Pull(repo, CheckOutTextBox.Text);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "Pull Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RemoteFilterButton_Click(object sender, RoutedEventArgs e)
        {
            string currentRepo = (string)BaseCombo.SelectedItem;
            RemoteBranchesListBox.Items.Clear();
            RepoUtils.GetAllRemoteBranches(currentRepo, BaseCache.SCache.GetItemsFromCache("git"), RemoteFilterText.Text).ForEach(item => RemoteBranchesListBox.Items.Add(item));
        }

        private void FetchButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string currentRepo = (string)BaseCombo.SelectedItem;
                var repo = new Repository(currentRepo);
                RepoUtils.Fetch(repo, CheckOutTextBox.Text);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "Fetch Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ChangeRefeshButton_Click(object sender, RoutedEventArgs e)
        {
            string currentRepo = (string)BaseCombo.SelectedItem;
            var repo = new Repository(currentRepo);
            LocalBranchesListBox.Items.Clear();
            foreach (var item in repo.RetrieveStatus())
            {
                LocalChangesListBox.Items.Add(item.FilePath);
            }

        }
    }
}
