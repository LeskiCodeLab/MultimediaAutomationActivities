using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Activities;
using System.ComponentModel;
using System.Reflection;
using System.IO;
using System.Diagnostics;

namespace MultimediaAutomationActivities
{
    public class MultimediaProcess : CodeActivity
    {

        [Category("Input")]
        [RequiredArgument]
        public InArgument<String> InputFile { get; set; }

        [Category("Input")]
        [RequiredArgument]
        public InArgument<String> OutputFolder { get; set; }

        [Category("Input")]
        public InArgument<String> OutputContainer { get; set; }

        [Category("Input")]
        [RequiredArgument]
        public InArgument<String> Command { get; set; }

        [DefaultValue(false)]
        [Category("Input")]
        public InArgument<bool> DebuggingMode { get; set; } = false;


        /// <summary>
        /// StreamToBytes - Converts a Stream to a byte array. Eg: Get a Stream from a file,url, or open file handle.
        /// </summary>
        /// <param name="input">input is the stream we are to return as a byte array</param>
        /// <returns>byte[] The Array of bytes that represents the contents of the stream</returns>
        static byte[] StreamToBytes(Stream input)
        {

            int capacity = input.CanSeek ? (int)input.Length : 0; //Bitwise operator - If can seek, Capacity becomes Length, else becomes 0.
            using (MemoryStream output = new MemoryStream(capacity)) //Using the MemoryStream output, with the given capacity.
            {
                int readLength;
                byte[] buffer = new byte[capacity/*4096*/];  //An array of bytes
                do
                {
                    readLength = input.Read(buffer, 0, buffer.Length);   //Read the memory data, into the buffer
                    output.Write(buffer, 0, readLength); //Write the buffer to the output MemoryStream incrementally.
                }
                while (readLength != 0); //Do all this while the readLength is not 0
                return output.ToArray();  //When finished, return the finished MemoryStream object as an array.
            }

        }


        protected override void Execute(CodeActivityContext context)
        {
            var inputFile = InputFile.Get(context);
            var outputFolder = OutputFolder.Get(context);
            var command = Command.Get(context);
            var outputContainer = OutputContainer.Get(context);
            var debuggingMode = DebuggingMode.Get(context);

            String ffmpegPath = "ffmpeg.exe";
            string tempPath = Path.GetTempPath();
            using (Stream input = Assembly.GetExecutingAssembly().GetManifestResourceStream("MultimediaAutomationActivities.Resources.ffmpeg.exe"))
            {

                byte[] byteData = StreamToBytes(input);

                ffmpegPath = Path.Combine(tempPath, "ffmpeg.exe");
                if (!System.IO.File.Exists(ffmpegPath))
                {
                    System.IO.File.WriteAllBytes(ffmpegPath, byteData);
                }
            }

            var startInfo = new ProcessStartInfo(ffmpegPath);
            startInfo.WindowStyle = ProcessWindowStyle.Normal;
            startInfo.WorkingDirectory = tempPath;

            string inputContainer = inputFile.Substring(inputFile.LastIndexOf('.'));
            if (outputContainer == "")
            {
                outputContainer = inputContainer;
            }

            string fileNameWithoutExtensions = inputFile.Replace(inputContainer, "");
            var fileName = fileNameWithoutExtensions.Substring(fileNameWithoutExtensions.LastIndexOf(@"\"));
            fileName = fileName.Replace(@"\", "");


            var uniqueId = (DateTime.Now.Ticks - new DateTime(2016, 1, 1).Ticks).ToString("x");
            startInfo.Arguments = "-i " + '"' + inputFile + '"' + " " + command + " " + '"' + outputFolder + @"\" + fileName + "." + outputContainer + '"';

   


            if (debuggingMode)
            {
                Process p = new Process();
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = "CMD.EXE";
                psi.Arguments = "/K " + ffmpegPath + " " + startInfo.Arguments;
                p.StartInfo = psi;
                p.Start();
                p.WaitForExit();
            }
            else
            {
                var processn = Process.Start(startInfo);
                processn.EnableRaisingEvents = true;

                processn.WaitForExit();
            }
        }


        public void Execute()
        {
            var inputFile = @"C:\Scratchdisk\input.mp4";
            var outputFolder = @"C:\Scratchdisk";


            //var command = "";  //PNG To JPG
            //var command = "";  //PNG To JPG
            //var command = "-acodec libmp3lame";  //WAV To MP3
            //var command = "-vcodec libx264";  // H264
            var command = "-vf noise=c0s=1:allf=t -vcodec prores_ks -profile:v 0"; //PRORES
            var outputContainer = "mov";

            String ffmpegPath = "ffmpeg.exe";
            string tempPath = Path.GetTempPath();
            using (Stream input = Assembly.GetExecutingAssembly().GetManifestResourceStream("MultimediaAutomationActivities.Resources.ffmpeg.exe"))
            {

                byte[] byteData = StreamToBytes(input);

                ffmpegPath = Path.Combine(tempPath, "ffmpeg.exe");
                if (!System.IO.File.Exists(ffmpegPath))
                {
                    System.IO.File.WriteAllBytes(ffmpegPath, byteData);
                }
            }

            var startInfo = new ProcessStartInfo(ffmpegPath);
            startInfo.WindowStyle = ProcessWindowStyle.Normal;
            startInfo.WorkingDirectory = tempPath;

            string inputContainer = inputFile.Substring(inputFile.LastIndexOf('.'));
            if (outputContainer == "")
            {
                outputContainer = inputContainer;
            }

            string fileNameWithoutExtensions = inputFile.Replace(inputContainer, "");
            var fileName = fileNameWithoutExtensions.Substring(fileNameWithoutExtensions.LastIndexOf(@"\"));
            fileName = fileName.Replace(@"\", "");


            var uniqueId = (DateTime.Now.Ticks - new DateTime(2016, 1, 1).Ticks).ToString("x");
            startInfo.Arguments = "-i " + '"' + inputFile + '"' + " " + command + " " + '"' + outputFolder + @"\" + fileName + "." + outputContainer + '"';

            var processn = Process.Start(startInfo);
            processn.EnableRaisingEvents = true;

            processn.WaitForExit();
        }
    }
}
