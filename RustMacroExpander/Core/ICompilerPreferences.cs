using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RustMacroExpander.Core
{
    public interface ICompilerPreferences
    {
        string TempFileName { get; }
        string Compiler { get; }
        string CompilerBuildFlags { get; }
        string CompilerVersionCommand { get; }
    }
}
