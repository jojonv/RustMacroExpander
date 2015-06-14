using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Helpers.RustMacroExpander
{
    internal class Cmd
    {
        public enum MessageType
        {
            Normal,
            Error
        }

        public struct CmdMessage
        {
            public string Message { get; private set; }

            public MessageType Type { get; private set; }

            public CmdMessage(string m, MessageType t)
            {
                Message = m;
                Type = t;
            }
        }

        readonly string args;
        readonly string currWorkingDir = Directory.GetCurrentDirectory();


        public Cmd(string args)
        {
            if (string.IsNullOrWhiteSpace(args)) throw new ArgumentNullException("args");
            this.args = args; 
        }

        public IObservable<CmdMessage> Run()
        {
            var cmdInfo = new ProcessStartInfo("cmd", args)
            {
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                WorkingDirectory = currWorkingDir,
                CreateNoWindow = true,
            };

            using (var proc = new Process()
            {
                StartInfo = cmdInfo,
                EnableRaisingEvents = true
            })
            {
                var normalStream = Observable
                    .FromEventPattern<DataReceivedEventHandler, DataReceivedEventArgs>(
                          h => proc.OutputDataReceived += h
                        , h => proc.OutputDataReceived -= h)
                    .Where(e => e.EventArgs.Data != null);
                var errorStream = Observable.FromEventPattern<DataReceivedEventHandler, DataReceivedEventArgs>(
                          h => proc.ErrorDataReceived += h
                        , h => proc.ErrorDataReceived -=h)
                    .Where(e => e.EventArgs.Data != null);

                var mergedStream = Observable.Merge(
                      normalStream.Select(e => new CmdMessage(e.EventArgs.Data, MessageType.Normal))
                    , errorStream.Select(e => new CmdMessage(e.EventArgs.Data, MessageType.Error)));

                proc.Start();
                proc.BeginErrorReadLine();
                proc.BeginOutputReadLine();
                return mergedStream;
            }
        }
    }
}
