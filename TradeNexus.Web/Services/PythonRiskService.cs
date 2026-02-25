using System.Diagnostics;
using System.IO;

namespace TradeNexus.Web.Services
{
    public class PythonRiskService
    {
        private static string _pythonCommand = null;

        public string ExecuteRiskEngine(string jsonInput)
        {
            if (_pythonCommand == null)
            {
                _pythonCommand = DiscoverPythonCommand();
            }

            var scriptPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "PythonEngine",
                "risk_engine.py"
            );

            ProcessStartInfo start = new ProcessStartInfo
            {
                FileName = _pythonCommand,
                Arguments = $"\"{scriptPath}\"",
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using (Process process = Process.Start(start))
            {
                if (process == null) return "Error: Could not start Python process.";

                // Write JSON to stdin to avoid shell escaping issues
                using (StreamWriter sw = process.StandardInput)
                {
                    if (sw.BaseStream.CanWrite)
                    {
                        sw.Write(jsonInput);
                    }
                }

                string result = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                if (!string.IsNullOrEmpty(error) && !error.Contains("DeprecationWarning"))
                    return "Python Error: " + error;

                return result;
            }
        }

        private string DiscoverPythonCommand()
        {
            if (CanRunCommand("python")) return "python";
            if (CanRunCommand("py")) return "py";
            return "python"; // Default
        }

        private bool CanRunCommand(string cmd)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = cmd,
                    Arguments = "--version",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };
                using (Process p = Process.Start(psi))
                {
                    p.WaitForExit(1000);
                    return p.ExitCode == 0;
                }
            }
            catch { return false; }
        }
    }
}