using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cassie.git.module
{
    public class Command
    {
        private string name;
        private List<string> args;
        private Dictionary<string,string> envs;

        public Command(params string[] args)
        {
            this.name = "git";
            this.args = args.ToList();
            this.envs = new Dictionary<string, string>();
        }

        public string String()
        {
            if(this.args.Count == 0) return this.name;
            return this.name + " " + string.Join(" ",this.args);
        }   

        public void AddArgs(params string[] args)
        {
            if(args != null && args.Length >0)
                this.args.AddRange(args);
        }  

        public void AddEnvs(string key,string val)
        {
            if(string.IsNullOrEmpty(key) || string.IsNullOrEmpty(val)) return;  
            this.envs.Add(key,val);
        } 

        public void AddEnvs(Dictionary<string,string> dics)
        {
            if(dics != null && dics.Count > 0)
            {
                foreach (var item in dics)
                {
                    this.envs.Add(item.Key,item.Value);
                }
            }
        }

        public List<string> GetArgs(){
            return this.args;
        }

        public Dictionary<string,string> GetEnvs(){
            return this.envs;
        }

        private ProcessStartInfo GenerateStartInfo(string dir)
        {
            var startInfo = new ProcessStartInfo();
            startInfo.FileName = this.name;
            if (this.args.Count > 0)
                startInfo.Arguments = string.Join(" ", this.args);
            if (this.envs.Count > 0)
            {
                foreach (var item in this.envs)
                {
                    startInfo.EnvironmentVariables[item.Key] = item.Value;
                }
            }
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.CreateNoWindow = true;
            if(!string.IsNullOrEmpty(dir)) startInfo.WorkingDirectory = dir;
            return startInfo;
        }

        /// <summary>
        /// Run a process asynchronously
        /// </summary>
        /// <param name="dir">command excuting working directory</param>
        /// <param name="stdOut">To capture STDOUT, pass callback stdOut</param>
        /// <param name="stdErr">To capture STDERR, pass callback stdErr</param>
        /// <param name="timeoutMs">The timeout in milliseconds (null for no timeout)</param>
        /// <returns>Result object</returns>
        public async Task<Result> RunAsync(string dir = null,
                                           Action<string> stdOut = null,
                                           Action<string> stdErr = null,
                                           char? splitChar = null,
                                           Int64? timeoutMs = null)
        {
            
            Result result = new Result();
            var startInfo = GenerateStartInfo(dir);
            using (var process = new Process() { StartInfo = startInfo, EnableRaisingEvents = true })
            {
                // List of tasks to wait for a whole process exit
                List<Task> processTasks = new List<Task>();

                // === EXITED Event handling ===
                var processExitEvent = new TaskCompletionSource<object>();
                process.Exited += (sender, args) =>
                {
                    processExitEvent.TrySetResult(true);
                };
                processTasks.Add(processExitEvent.Task);

                // === STDOUT handling ===
                var stdOutBuilder = new StringBuilder();
                if (process.StartInfo.RedirectStandardOutput)
                {
                    var stdOutCloseEvent = new TaskCompletionSource<bool>();

                    process.OutputDataReceived += (s, e) =>
                    {
                        if (e.Data == null)
                        {
                            stdOutCloseEvent.TrySetResult(true);
                        }
                        else
                        {
                            var str = e.Data;
                            if(stdOut != null) stdOut(str);
                            stdOutBuilder.Append(str);
                            if (splitChar != null) stdOutBuilder.Append(splitChar);
                        }
                    };

                    processTasks.Add(stdOutCloseEvent.Task);
                }
                else
                {
                    // STDOUT is not redirected, so we won't look for it
                }

                // === STDERR handling ===
                var stdErrBuilder = new StringBuilder();
                if (process.StartInfo.RedirectStandardError)
                {
                    var stdErrCloseEvent = new TaskCompletionSource<bool>();

                    process.ErrorDataReceived += (s, e) =>
                    {
                        if (e.Data == null)
                        {
                            stdErrCloseEvent.TrySetResult(true);
                        }
                        else
                        {
                            var str = e.Data;
                            if(stdErr != null) stdErr(str);
                            stdErrBuilder.Append(str);
                        }
                    };

                    processTasks.Add(stdErrCloseEvent.Task);
                }
                else
                {
                    // STDERR is not redirected, so we won't look for it
                }

                // === START OF PROCESS ===
                if (!process.Start())
                {
                    result.ExitCode = process.ExitCode;
                    return result;
                }

                // Reads the output stream first as needed and then waits because deadlocks are possible
                if (process.StartInfo.RedirectStandardOutput)
                {
                    process.BeginOutputReadLine();
                }
                else
                {
                    // No STDOUT
                }

                if (process.StartInfo.RedirectStandardError)
                {
                    process.BeginErrorReadLine();
                }
                else
                {
                    // No STDERR
                }

                // === ASYNC WAIT OF PROCESS ===

                // Process completion = exit AND stdout (if defined) AND stderr (if defined)
                Task processCompletionTask = Task.WhenAll(processTasks);

                // Task to wait for exit OR timeout (if defined)
                Task<Task> awaitingTask = timeoutMs.HasValue
                    ? Task.WhenAny(Task.Delay(Convert.ToInt32(timeoutMs.Value)), processCompletionTask)
                    : Task.WhenAny(processCompletionTask);

                // Let's now wait for something to end...
                if ((await awaitingTask.ConfigureAwait(false)) == processCompletionTask)
                {   
                    // -> Process exited cleanly
                    result.ExitCode = process.ExitCode;
                }
                else
                {
                    // -> Timeout, let's kill the process
                    try
                    {
                        process.Kill();
                    }
                    catch
                    {
                        // ignored
                    }
                }

                // Read stdout/stderr
                result.StdOut = stdOutBuilder.ToString();
                result.StdErr = stdErrBuilder.ToString();
            }

            return result;
        }
    }

    /// <summary>
    /// Run process result
    /// </summary>
    public class Result
    {
        /// <summary>
        /// Exit code
        /// <para>If NULL, process exited due to timeout</para>
        /// </summary>
        public int? ExitCode { get; set; } = null;

        /// <summary>
        /// Standard error stream
        /// </summary>
        public string StdErr { get; set; } = "";

        /// <summary>
        /// Standard output stream
        /// </summary>
        public string StdOut { get; set; } = "";
    }
}