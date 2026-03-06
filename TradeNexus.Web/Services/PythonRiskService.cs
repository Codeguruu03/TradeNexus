using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Text;

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

            var attempts = BuildPythonAttempts();
            var errors = new StringBuilder();

            foreach (var attempt in attempts)
            {
                var outcome = RunPython(attempt.fileName, attempt.argsPrefix, scriptPath, jsonInput);
                if (outcome.success)
                {
                    return outcome.output;
                }

                if (!string.IsNullOrWhiteSpace(outcome.error))
                {
                    errors.Append($"[{attempt.fileName} {attempt.argsPrefix}] ");
                    errors.AppendLine(outcome.error.Trim());
                }
            }

            return "Python Error: Could not execute risk engine. " + errors.ToString().Trim();
        }

        private (bool success, string output, string error) RunPython(string fileName, string argsPrefix, string scriptPath, string jsonInput)
        {
            var args = string.IsNullOrWhiteSpace(argsPrefix)
                ? $"\"{scriptPath}\""
                : $"{argsPrefix} \"{scriptPath}\"";

            ProcessStartInfo start = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = args,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using (Process process = Process.Start(start))
            {
                if (process == null) return (false, null, "Could not start Python process.");

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
                process.WaitForExit();

                if (process.ExitCode != 0)
                    return (false, null, string.IsNullOrWhiteSpace(error) ? $"Python process exited with code {process.ExitCode}." : error);

                if (!string.IsNullOrEmpty(error) && !error.Contains("DeprecationWarning"))
                    return (false, null, error);

                return (true, result, null);
            }
        }

        private IEnumerable<(string fileName, string argsPrefix)> BuildPythonAttempts()
        {
            var cwd = Directory.GetCurrentDirectory();
            var venvPython = Path.Combine(cwd, ".venv", "Scripts", "python.exe");

            if (File.Exists(venvPython))
                yield return (venvPython, string.Empty);

            // Windows launcher (preferred over python app alias)
            if (CanRunCommand("py", "--version"))
            {
                yield return ("py", "-3");
                yield return ("py", string.Empty);
            }

            if (CanRunCommand("python", "--version"))
                yield return ("python", string.Empty);

            // Last fallback, still attempt python even if version check fails
            yield return ("python", string.Empty);
        }

        private bool CanRunCommand(string cmd, string arguments)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = cmd,
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
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