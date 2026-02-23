using System.Diagnostics;
using System.IO;

namespace TradeNexus.Web.Services
{
    public class PythonRiskService
    {
        public string ExecuteRiskEngine(string jsonInput)
        {
            var scriptPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "PythonEngine",
                "risk_engine.py"
            );

            ProcessStartInfo start = new ProcessStartInfo
            {
                FileName = "python",
                Arguments = $"\"{scriptPath}\" \"{jsonInput}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using (Process process = Process.Start(start))
            {
                string result = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                if (!string.IsNullOrEmpty(error))
                    return error;

                return result;
            }
        }
    }
}