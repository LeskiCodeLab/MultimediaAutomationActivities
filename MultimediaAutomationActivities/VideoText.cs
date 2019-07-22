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
    public class VideoText : CodeActivity
    {
        [Category("Input")]
        [RequiredArgument]
        public InArgument<String> InputFile { get; set; }

        [Category("Input")]
        public InArgument<String> OutputFolder { get; set; }

        [DefaultValue("mov")]
        [Category("Input")]
        public InArgument<String> OutputContainer { get; set; } = "mov";

        [Category("Input")]
        public InArgument<String> FontFile { get; set; }

        [Category("Input")]
        public InArgument<String> Text { get; set; }

        [DefaultValue("-vcodec prores_ks -profile:v 0")]
        [Category("Input")]
        public InArgument<String> Command { get; set; } = "-vcodec prores_ks -profile:v 3";

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
            var outputContainer = OutputContainer.Get(context);
            var command = Command.Get(context);
            var fontFile = FontFile.Get(context);
            var debuggingMode = DebuggingMode.Get(context);
            var text = Text.Get(context);


            string tempPath = Path.GetTempPath();
            var fontFileName = fontFile.Substring(fontFile.LastIndexOf(@"\"));
            fontFileName = fontFileName.Replace(@"\", "");

            String ffmpegPath = "ffmpeg.exe";
            String fontFilePath = Path.Combine(tempPath, fontFileName);

            if (!System.IO.File.Exists(fontFilePath))
            {
                System.IO.File.Copy(fontFile, fontFilePath, true);
            }
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
            startInfo.Arguments = "-i " + '"' + inputFile + '"' + " " + "-vf drawtext=enable='between(t,2,8)':\"fontfile = " + fontFileName + '"' + ":text=\"" + text + "\":fontcolor=white:fontsize=124:x=(w-text_w)/2:y=(h-text_h)/2 " + command + " " + '"' + outputFolder + @"\" + fileName + "." + outputContainer + '"';

            var processn = Process.Start(startInfo);
            processn.EnableRaisingEvents = true;

            processn.WaitForExit();

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
        }


        public void Execute()
        {
            var inputFile = @"C:\Scratchdisk\audio_video_combined\combined_2019_7_21__21_45_18.mov";
            var outputFolder = @"C:\Scratchdisk\texted";
            var lutFile = @"C:\Scratchdisk\faded.cube";
            var outputContainer = "mov";
            var command = "-vcodec prores_ks -profile:v 3";

            var fontFile = @"C:\Scratchdisk\font_library\travelling.regular.ttf";
            var debuggingMode = true;
            var text = "Made by a Robot";


            string tempPath = Path.GetTempPath();
            var fontFileName = fontFile.Substring(fontFile.LastIndexOf(@"\"));
            fontFileName = fontFileName.Replace(@"\", "");

            String ffmpegPath = "ffmpeg.exe";
            String fontFilePath = Path.Combine(tempPath, fontFileName);

            if (!System.IO.File.Exists(fontFilePath))
            {
                System.IO.File.Copy(fontFile, fontFilePath, true);
            }
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
            startInfo.Arguments = "-i " + '"' + inputFile + '"' + " " + "-vf drawtext=enable='between(t,2,8)':\"fontfile = " + fontFileName + '"' + ":text=\"" + text + "\":fontcolor=white:fontsize=124:x=(w-text_w)/2:y=(h-text_h)/2 " + command + " " + '"' + outputFolder + @"\" + fileName + "." + outputContainer + '"';

            var processn = Process.Start(startInfo);
            processn.EnableRaisingEvents = true;

            processn.WaitForExit();

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
        }
    }
}
