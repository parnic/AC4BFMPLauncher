using System;
using System.IO;
using System.Reflection;
using System.Windows;

namespace AC4BFMPLauncher
{
    /// <summary>
    /// Interaction logic for Done.xaml
    /// </summary>
    public partial class Done : Window
    {
        private Done()
        {

        }

        public Done(string uplayLoc, string ac4Loc)
        {
            InitializeComponent();

            var fullPath = Assembly.GetExecutingAssembly().Location;
            lblBrowseTo.Text = String.Format(lblBrowseTo.Text, fullPath);
            txtTarget.Text = String.Format("\"{0}\" \"{1}\" \"{2}\"", fullPath, uplayLoc, ac4Loc);
            txtStartIn.Text = String.Format("\"{0}\"", Path.GetDirectoryName(fullPath));
        }
    }
}
