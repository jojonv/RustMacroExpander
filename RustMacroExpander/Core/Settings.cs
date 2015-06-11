using RustMacroExpander.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RustMacroExpander.ViewModels
{
    [DataContract]
    public class Settings :  ICompilerPreferences, INotifyPropertyChanged
    {

        [DataMember]
        string compilerBuildFlags;
        [DataMember]
        string compiler;

        public Settings()
        {
            compilerBuildFlags = "-Z unstable-options --pretty=expanded";
            compiler = "rustc";
        }

        /// <summary>
        /// Flags used by the compiler
        /// </summary>
        public string CompilerBuildFlags
        {
            get { return compilerBuildFlags; }
            set
            {
                if (value == null)
                    return;
                compilerBuildFlags = value;
                RaisePropertyChanged();
            }
        } 
        /// <summary>
        /// File used by the compiler
        /// </summary>
        [DataMember]
        public string TempFileName { get; set; } = "tmp";

        /// <summary>
        /// Command used to run compiler
        /// </summary>
        public string Compiler
        {
            get { return compiler; }
            set
            {
                if (value == null)
                    return;
                compiler = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// Command used to retrieve rust version
        /// </summary>
        [DataMember]
        public string CompilerVersionCommand { get; set; } = "rustc -V";

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged([CallerMemberName] string name = "")
        {
            var e = PropertyChanged;
            if (e == null)
                return;
            PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
