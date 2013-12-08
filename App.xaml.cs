using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;

namespace AC4BFMPLauncher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if ( e.Args.Length == 2
                && File.Exists(e.Args[0]) && File.Exists(e.Args[1]))
            {
                var p = new ProcessStartInfo(e.Args[0],
                    String.Format("-upc_uplay_id 437 -upc_game_version 0 -upc_exe_path {0} -upc_working_directory {1} -uplay_steam_mode",
                        Convert.ToBase64String(Encoding.UTF8.GetBytes(e.Args[1])), Convert.ToBase64String(Encoding.UTF8.GetBytes(Path.GetDirectoryName(e.Args[1])))));
                p.UseShellExecute = false;
                Process.Start(p);
            }
            else
            {
                new MainWindow().ShowDialog();
            }
            this.Shutdown();
        }
    }
}
