
using System.Diagnostics;
namespace PortSniffer
{
    public class OutPutModel
    {
        private string? _outputFlag = null;
        private const string OUTPUT_FILE_PATH = "output.txt";

        public OutPutModel() { }

        public void OutPut(string output)
        {
            Console.WriteLine(output);
            OutPutByFlag(output + "\n");
        }

        public void OutPut(string output, ConsoleColor consoleColor)
        {
            Console.ForegroundColor = consoleColor;
            Console.WriteLine(output);
            Console.ResetColor();
            OutPutByFlag(output + "\n");
        }

        public void OpenResultsFile()
        {
            if (File.Exists(OUTPUT_FILE_PATH))
            {
                Process process = new Process();
                process.StartInfo.FileName = OUTPUT_FILE_PATH;
                process.StartInfo.UseShellExecute = true; // UseShellExecute = true opens the file with the default associated application
                process.Start();
            }
        }

        public void DeleteResultsFile()
        {
            if (File.Exists(OUTPUT_FILE_PATH))
            {
                File.Delete(OUTPUT_FILE_PATH);
            }
        }
        public void SetFlag(string flag)
        {
            _outputFlag = flag;
        }

        private void OutPutByFlag(string output)
        {
            if (string.IsNullOrEmpty(_outputFlag))
                return;
            if (!File.Exists(OUTPUT_FILE_PATH))
            {
                File.WriteAllText(OUTPUT_FILE_PATH, "");
            }
            switch (_outputFlag)
            {
                case "-wf":
                    File.AppendAllText(OUTPUT_FILE_PATH, output);
                    break;
            }
        }
    }
}
