using Smart_Screen_Recorder.Lib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using AForge.Video.FFMPEG;
using AForge;
using AForge.Video;
using System.Globalization;

namespace Smart_Screen_Recorder
{
    public partial class Main : Form
    {
        //Location where images are stored
        string ImagesPath;
        //Location where videos are stored
        string VideosPath;
        //File used to save selected locations by user
        string FilePath;
        DateTime StartTime;

        MovieMaker movieMaker;

        VideoFileWriter writer; 
        public Main()
        {
            InitializeComponent();
            this.MaximumSize = this.Size;
            this.MinimumSize = this.Size;
            
        }

        private void Main_Load(object sender, EventArgs e)
        {
            try
            {
                btnStop.Enabled = false;
                btnPause.Enabled = false;
                string root;
                FilePath = Path.Combine(Environment.CurrentDirectory, "CacheFile.txt");
                //Load the path selected by user from file
                if (File.Exists(FilePath))
                {
                    root = File.ReadAllText(FilePath);
                }
                else
                {
                    root = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                }
                ImagesPath = Path.Combine(root, "Images");
                VideosPath = Path.Combine(root, "Videos");
                txtBxPath.Text = root;
                
            }
            catch
            {
                //Do no thing
            }
            
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            try
            {
                FolderBrowserDialog folderBrowse = new FolderBrowserDialog();
                if (folderBrowse.ShowDialog() == DialogResult.OK)
                {
                    string root = folderBrowse.SelectedPath;
                    File.WriteAllText(FilePath, root);
                    txtBxPath.Text = root;
                }

            }catch(Exception ex)
            {
                //Do no thing
            }
            
        }
        delegate void DoWork();
        private void btnSnapshot_Click(object sender, EventArgs e)
        {
            try
            {
                var objSnapshot = new Snapshot();
                //Set opacity 0 so that application do not show in screenshot
                this.Opacity = 0;
                var image = objSnapshot.Take();
                //Set opacity 1 after taking screen shot
                this.Opacity = 1;
                string name = objSnapshot.GenerateName();
                if (!Directory.Exists(ImagesPath))
                    Directory.CreateDirectory(ImagesPath);
                string filename = Path.Combine(ImagesPath, name);
                objSnapshot.Save(image, filename);
                objSnapshot.PlaySound();
                
            }
            catch
            {
                //Error
            }
        }
        
        private void btnStart_Click(object sender, EventArgs e)
        {
            StartTime = DateTime.Now;
            WatchTimer.Start();
            btnStart.Enabled = false;
            btnStop.Enabled = true;
            btnPause.Enabled = true;
            movieMaker = new MovieMaker();
            if (!Directory.Exists(VideosPath))
                Directory.CreateDirectory(VideosPath);
            var name=movieMaker.GenerateName();
            string filename = Path.Combine(VideosPath, name);
            movieMaker.Start(filename);
        }
        private string TimeString(TimeSpan value)
        {
            return value.ToString(@"hh\:mm\:ss\.ff");
        }
        private void btnStop_Click(object sender, EventArgs e)
        {
            WatchTimer.Stop();
            btnStart.Enabled = true;
            btnStop.Enabled = false;
            btnPause.Enabled = false;
            movieMaker.Stop();
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            if (movieMaker.IsRecording)
            {
                btnPause.Text = "Resume";

                movieMaker.Pause();
                WatchTimer.Stop();
            }
            else
            {
                btnPause.Text = "Pause";
                movieMaker.Resume();
                WatchTimer.Start();
            }
            btnPause.TextAlign = ContentAlignment.MiddleCenter;
            
        }

        private void WatchTimer_Tick(object sender, EventArgs e)
        {
            lblTimer.Text = TimeString(DateTime.Now - StartTime);
        }
    }
}
