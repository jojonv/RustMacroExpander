using Caliburn.Micro;
using Helpers.RustMacroExpander;
using RustMacroExpander.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RustMacroExpander.ViewModels
{
    /// <summary>
    /// This class exists to allow serialization and maybe some other stuff down the line
    /// </summary>
    [DataContract]
    internal class RustBridge : INotifyPropertyChanged
    {
        #region Private members

        Rust rust;
        [DataMember]
        string contentStr;
        XDocument buildResult;
        ICompilerPreferences settings;

        IObservable<Cmd.CmdMessage> versionObservable;
        IObservable<XDocument> buildObservable;
        #endregion Private members

        #region Private methods

        [OnSerializing]
        void OnSerialization(StreamingContext c) => contentStr = buildResult?.ToString();

        /// <summary>
        /// Additional dependencies need to be set with `SetDependencies`
        /// That's the reason constructor is not used
        /// </summary>
        /// <param name="c"></param>
        [OnDeserialized]
        void OnDeserialization(StreamingContext c) 
        {
            if (contentStr != null)
                Content = XDocument.Parse(contentStr);
        }

        void RaisePropertyChanged([CallerMemberName] string name = "")
        {
            var e = PropertyChanged;
            if (e == null)
                return;
            PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        #endregion Private methods

        public event PropertyChangedEventHandler PropertyChanged;

        public XDocument Content
        {
            get { return buildResult; }
            set
            {
                buildResult = value;
                RaisePropertyChanged();
            }
        }

        [DataMember]
        public string Input { get; set; }

        public string Version { get; private set; }

        //public Settings Settings { get; set; }
        public void SetDependencies(ICompilerPreferences s)
        {
            if (s == null)
                throw new ArgumentNullException($"{nameof(Settings)} is null");
            if (string.IsNullOrWhiteSpace(s.Compiler))
                throw new SettingsException($"{nameof(s.Compiler)} is not valid");
            if (string.IsNullOrWhiteSpace(s.CompilerBuildFlags))
                throw new SettingsException($"{nameof(s.CompilerBuildFlags)} is not valid");

            settings = s;
            rust = new Rust(s);
        }

        public void UpdateVersion()
        {
            var cmd = new Cmd($"/c {settings.CompilerVersionCommand}");
            versionObservable = cmd.Run();
            versionObservable.Subscribe(m =>
            {
                Version += m.Message;
                RaisePropertyChanged(nameof(Version));
            });
        }


        /// <summary>
        /// Builds `content` 
        /// </summary>
        /// <param name="content"></param>
        public void Build(string content)
        {
            // Remove previous content
            //
            Content = null;

            rust.WriteToFile(content);
            buildObservable = rust.BuildFile();
            buildObservable.Subscribe(xdoc => Content = xdoc);
        }
    }
}
