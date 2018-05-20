using Smart_Screen_Recorder.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Media;
using System.Text;
using System.Windows.Forms;
using AForge.Video.FFMPEG;

namespace Smart_Screen_Recorder.Lib
{
    public class Snapshot
    {
        public Bitmap Take()
        {
            try
            {
                Size imageSize = new Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
                Bitmap bitmap=new Bitmap(imageSize.Width,imageSize.Height);
                Graphics gr = Graphics.FromImage(bitmap as Image);
                
                gr.CopyFromScreen(0, 0, 0, 0,imageSize);
                return bitmap;
            }
            catch
            {
                return null;
            }
        }
        public string GenerateName()
        {
            try
            {
                var time = DateTime.Now;
                string name = "Snapshot "+time.Hour +"-"+time.Minute + "-" + time.Second +".jpg";
                return name;
            }
            catch
            {
                return null;
            }
        }
        public void PlaySound()
        {
            try
            {
                SoundPlayer player = new SoundPlayer(Resources.SnapshotSound);
                player.Play();
            }
            catch
            {
                //Do nothing
            }
        }
        public bool Save(Bitmap image,string filename)
        {
            try
            {
                image.Save(filename);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
    public class MovieMaker:IDisposable
    {
        private int fps = 24;
        private Size Bounds;
        Timer timer;
        VideoFileWriter videoWriter;
        private bool isRecording;
        public bool IsRecording
        {
            get { return isRecording; }
        }
        public MovieMaker()
        {
            isRecording = false;
            timer = new Timer();
            timer.Tick += Timer_Tick;
            videoWriter = new VideoFileWriter();
        }
        public void Dispose()
        {
            isRecording = false;
            videoWriter.Dispose();
            timer.Dispose();
           
        }
        public string GenerateName()
        {
            try
            {
                var time = DateTime.Now;
                string name = "Video " + time.Hour + "-" + time.Minute + "-" + time.Second + ".avi";
                return name;
            }
            catch
            {
                return null;
            }
        }
        public bool Start(string filename)
        {
            try
            {
                isRecording = true;
                Bounds = new Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
                videoWriter.Open(filename, Bounds.Width, Bounds.Height, fps,VideoCodec.MPEG4);
                timer.Interval = 1000/fps;
                timer.Start();
                SystemSounds.Asterisk.Play();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool Pause()
        {
            try
            {
                isRecording = false;
                timer.Stop();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool Resume()
        {
            try
            {
                isRecording = true;
                timer.Start();
                return true;
            }catch
            {
                return false;
            }
        }
        public bool Stop()
        {
            try
            {
                isRecording = false;
                timer.Stop();
                if (videoWriter.IsOpen)
                    videoWriter.Close();
                SystemSounds.Asterisk.Play();
                return true;
            }
            catch
            {
                return false;
            }
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            var bitmap = new Bitmap(Bounds.Width, Bounds.Height);
            var gr = Graphics.FromImage(bitmap as Image);
            gr.CopyFromScreen(0, 0, 0, 0,Bounds);
            videoWriter.WriteVideoFrame(bitmap);
        }
    }
}
