using Helpers.RustMacroExpander;
using RustMacroExpander.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RustMacroExpander.ViewModels
{
    /// <summary>
    /// Type responsible for compilation only
    /// </summary>
    internal class Rust
    {
        readonly FileStream tempFileStream;
        readonly string currWorkingDir = Directory.GetCurrentDirectory();
        XDocument previousDoc;
        Cmd cmd;

        public Rust(ICompilerPreferences s)
        {
            // Settings werer changed
            // Create new command prompt
            //
            (s as INotifyPropertyChanged).PropertyChanged += (sender, a) =>
            {
                cmd = CreateCmd(s);
            };

            cmd = CreateCmd(s);
            tempFileStream = File.Open(
                s.TempFileName
                , FileMode.OpenOrCreate
                , FileAccess.ReadWrite
                , FileShare.Read);
        }

        Cmd CreateCmd(ICompilerPreferences p)
        {
            return new Cmd($"/c {p.Compiler} {p.TempFileName} {p.CompilerBuildFlags}");
        }

        public void WriteToFile(string text)
        {
            // Erase content of the file
            //
            tempFileStream.SetLength(0);
            // Write a new content to the file
            //
            byte[] data = new UTF8Encoding(true).GetBytes(text);
            tempFileStream.Write(data, 0, data.Length);
            tempFileStream.Flush();
        }


        public IObservable<XDocument> BuildFile()
        {
            // Creates a new XDoc from sentence (message is received in short sentences, not at once from stream by cmd)
            // I would rather create only one XDoc from the full message, unfortunately I'd first need to find
            // a way to get notified when cmd exits, since now it exits first, and just then sends message
            //
            previousDoc = null;

            return Observable.Create((IObserver<XDocument> o) =>
            {
                cmd.Run().Subscribe(cm =>
                {
                    var elem = new XElement(cm.Type == Cmd.MessageType.Error
                                            ? Tags.Error.ToString()
                                            : Tags.Normal.ToString(), cm.Message);

                    // Create new or deep copy previous doc
                    //
                    var doc = previousDoc == null
                              ? new XDocument(new XElement(Tags.Root.ToString()))
                              : new XDocument(previousDoc);
                    doc.Root.Add(elem);
                    previousDoc = doc;
                    o.OnNext(doc);
                });
                return () => { };
            });
        }
    }
}
