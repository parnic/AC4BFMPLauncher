using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace AC4BFMPLauncher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly List<string> IntroMovieFilenames = new List<string>()
        {
            "abstergo_logo.bik",
            "ac4bf_logo.bik",
            "TWIMTBP.bik",
            "ubi_logo.bik",
        };

        public MainWindow()
        {
            InitializeComponent();

            // load settings
            txtLocateUplay.Text = Properties.Settings.Default.ubiLauncherPath;
            txtLocateAc4.Text = Properties.Settings.Default.ac4MpPath;

            // configure video replacement and 'done' button visibility/content
            TryEnableDoneButton();
            SetupToggleVideosButton();
        }

        private void ShowFileBrowser(string forFile, TextBox txtUpdate)
        {
            if (string.IsNullOrEmpty(forFile))
            {
                return;
            }

            var dlg = new Microsoft.Win32.OpenFileDialog();
            // we're only searching for a specific file
            dlg.Filter = forFile + "|" + forFile;
            // pre-fill the directory and file in case the user has already filled in text here
            if (!string.IsNullOrEmpty(txtUpdate.Text))
            {
                dlg.FileName = txtUpdate.Text;
                dlg.InitialDirectory = Path.GetDirectoryName(txtUpdate.Text);
            }
            dlg.ShowDialog(this);
            if (txtUpdate != null && !string.IsNullOrEmpty(dlg.FileName) && File.Exists(dlg.FileName))
            {
                txtUpdate.Text = dlg.FileName;
            }
        }

        private void TryEnableDoneButton()
        {
            btnDone.IsEnabled = File.Exists(txtLocateUplay.Text) && File.Exists(txtLocateAc4.Text);
        }

        private void btnLocateUplay_Clicked(object sender, RoutedEventArgs e)
        {
            ShowFileBrowser("UbisoftGameLauncher.exe", txtLocateUplay);

            // save the path the user just chose
            Properties.Settings.Default.ubiLauncherPath = txtLocateUplay.Text;
            Properties.Settings.Default.Save();

            TryEnableDoneButton();
        }

        private void btnLocateAc4_Clicked(object sender, RoutedEventArgs e)
        {
            ShowFileBrowser("AC4BFMP.exe", txtLocateAc4);

            // save the path the user just chose
            Properties.Settings.Default.ac4MpPath = txtLocateAc4.Text;
            Properties.Settings.Default.Save();

            TryEnableDoneButton();
            SetupToggleVideosButton();
        }

        private void SetupToggleVideosButton()
        {
            btnToggleVideos.IsEnabled = File.Exists(txtLocateAc4.Text);
            if (btnToggleVideos.IsEnabled)
            {
                // only checking for one of the four videos because it stands to reason that if one has been messed with, they all have.
                if (File.Exists(Path.Combine(Path.GetDirectoryName(txtLocateAc4.Text), "multi", "videos", "abstergo_logo.bik")))
                {
                    btnToggleVideos.Content = "Remove intro videos";
                }
                else
                {
                    btnToggleVideos.Content = "Replace intro videos";
                }
            }
        }

        private void ConditionalTryRemoveFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                try
                {
                    File.Move(filePath, filePath + ".orig");
                }
                catch { }
            }
        }

        private void ConditionalTryAddFile(string filePath)
        {
            if (File.Exists(filePath + ".orig"))
            {
                try
                {
                    File.Move(filePath + ".orig", filePath);
                }
                catch { }
            }
        }

        private void btnToggleVideos_Clicked(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtLocateAc4.Text) || !File.Exists(txtLocateAc4.Text)
                || IntroMovieFilenames.Count <= 0)
            {
                return;
            }

            try
            {
                var rootDir = Path.Combine(Path.GetDirectoryName(txtLocateAc4.Text), "multi", "videos");
                if (Directory.Exists(rootDir))
                {
                    if (File.Exists(Path.Combine(rootDir, IntroMovieFilenames[0])))
                    {
                        foreach (var movie in IntroMovieFilenames)
                        {
                            ConditionalTryRemoveFile(Path.Combine(rootDir, movie));
                            if (File.Exists(Path.Combine(rootDir, movie)))
                            {
                                MessageBox.Show("Couldn't remove intro videos. You may need to run this application as an administrator.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                break;
                            }
                        }
                    }
                    else
                    {
                        foreach (var movie in IntroMovieFilenames)
                        {
                            ConditionalTryAddFile(Path.Combine(rootDir, movie));
                            if (!File.Exists(Path.Combine(rootDir, movie)))
                            {
                                MessageBox.Show("Couldn't replace intro videos. You may have deleted them or you can try running this application as an administrator.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                break;
                            }
                        }
                    }
                }
            }
            catch
            {
                // Path.Combine can get real unhappy real quick
                MessageBox.Show("Something went wrong. Please verify your AC4 install location is correct and try again. Send an email to ac4launcher@parnic.com if you've done this and are still having trouble.",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            SetupToggleVideosButton();
        }

        private void btnDone_Clicked(object sender, RoutedEventArgs e)
        {
            if (!File.Exists(txtLocateUplay.Text) || !File.Exists(txtLocateAc4.Text))
            {
                MessageBox.Show("You must set your Uplay and AC4 install locations properly first.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                var appidFile = Path.Combine(Path.GetDirectoryName(txtLocateAc4.Text), "steam_appid.txt");
                if (!File.Exists(appidFile))
                {
                    try
                    {
                        File.WriteAllText(appidFile, "242050");
                    }
                    catch
                    {
                        MessageBox.Show(String.Format("Couldn't create steam_appid.txt file. You can either run this application as an administrator or"
                                + " manually create {0} - it should contain only \"242050\" (without quotes) and nothing else.", appidFile),
                            "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                var dlg = new Done(txtLocateUplay.Text, txtLocateAc4.Text);
                dlg.ShowDialog();
            }
        }
    }
}
