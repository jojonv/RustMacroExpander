using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RustMacroExpander.Helpers
{
    public class SettingsException : Exception
    {
        public SettingsException(string message) : base(message)
        {

        }
    }
}
