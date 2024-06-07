using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows;

namespace Instance_Timer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow AppWindow; // lägg till denna för att kunna prata mellan classerna 
        public static Stopwatch watch = new Stopwatch();//.StartNew();
        private static BackgroundWorker backgroundWorker;
        public static StringBuilder timer_sb = new StringBuilder();
        public static StringBuilder outputtext_sb = new StringBuilder();
        public static string Currentfolder;
        public static string Outputfilename = "InstanceTimer.txt";

        public static bool AlarmTriggerMet;
        public static string AlarmTimer = "04:00:00";

        public static bool arewerunning = false;



        public MainWindow()
        {
            InitializeComponent();
            AppWindow = this; // clona fönstret hit

            MainWindow.AppWindow.startbutton.Content = "Start";

            //hämta upp versionsnumret från assemblyet
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);

            //get current path, used for the inifile and the savefile
            Currentfolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            //Sätt windows title
            AppWindow.Title = AppWindow.Title + " v" + fvi.FileVersion;

        }



        public static void AlarmTrigger(string StopWatchTime)
        {
            if  (StopWatchTime == AlarmTimer)
                {
                    AlarmTriggerMet = true;
                    MessageBox.Show("Endtime reached");
                }
            
        }



        public static void Backgroundworker_doWork(object sender, DoWorkEventArgs e)
        {
                do
                {
                    AppWindow.timerlabel.Dispatcher.Invoke((Action)(() =>
                    {
                        try
                        {


                            timer_sb.Clear();
                            timer_sb.AppendFormat("{0:hh\\:mm\\:ss}", watch.Elapsed);

                            if (AlarmTriggerMet == false) { AlarmTrigger(timer_sb.ToString()); }

                            //Console.WriteLine(timer_sb.ToString());

                            AppWindow.timerlabel.Content = timer_sb.ToString();

                            ExportCounter(timer_sb.ToString(), MainWindow.AppWindow.instance_ownname.Text);
                        }
                        catch (Exception e2)
                        {
                            MessageBox.Show(e2.ToString());
                        }
                    }));


                    Thread.Sleep(1000);
                } while (true);

        }



        public static void ExportCounter(string elapsed, string ownname)
        {
            outputtext_sb.Clear();
            outputtext_sb.AppendFormat("Instance Timer{0}", Environment.NewLine);
            outputtext_sb.AppendFormat("{0}{1}", elapsed, Environment.NewLine);

            if (ownname.Length != 0) 
            { 
                outputtext_sb.AppendFormat("{0}{1}", ownname, Environment.NewLine);
            }


            if (arewerunning == true) { File.WriteAllText(Currentfolder + "\\" + Outputfilename, outputtext_sb.ToString(), Encoding.UTF8); }
        }



        public static void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Console.WriteLine("progress changed");
        }



        public static void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Console.WriteLine("run worker completed");
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {

            //detta händer när man klickar på start

            if (MainWindow.AppWindow.startbutton.Content.ToString() == "Start")
            {
                AppWindow.startbutton.Dispatcher.Invoke((Action)(() =>
                {
                    try
                    {
                        AppWindow.startbutton.Content = "Stop";
                        ControlStopWatch(true);
                        AppWindow.instance_ownname.IsEnabled = false;
                        AlarmTriggerMet = false;
                        arewerunning = true;

    }
                    catch (Exception e2)
                    {
                        MessageBox.Show(e2.ToString());
                    }
                }));

            }
            else             //detta händer när man klickar på stop
            {
                AppWindow.startbutton.Dispatcher.Invoke((Action)(() =>
                {
                    try
                    {
                        AppWindow.startbutton.Content = "Start";
                        ControlStopWatch(false);
                        AppWindow.instance_ownname.IsEnabled = true;
                        arewerunning = false;
                        if (File.Exists(Currentfolder + "\\" + Outputfilename)) { File.Delete(Currentfolder + "\\" + Outputfilename); }
                    }
                    catch (Exception e2)
                    {
                        MessageBox.Show(e2.ToString());
                    }
                }));

            }


        }



        public static void ControlStopWatch(bool statustimer)
        {

            if (statustimer == true) 
            {
                watch.Start();
                backgroundWorker = new BackgroundWorker
                {
                    WorkerReportsProgress = true,
                    WorkerSupportsCancellation = true
                };

                backgroundWorker.DoWork += Backgroundworker_doWork;
                backgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;
                backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
                backgroundWorker.RunWorkerAsync("Press enter in the next 5 seconds");
                //test
            }

            if (statustimer == false) 
            { 
                watch.Stop();watch.Reset(); 
            }

        }
    }





    
}
