using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RustMacroExpander.Helpers
{
    public class Name : Attribute
    {
        readonly string name;
        public Name(string n)
        {
            name = n;
        }
    }
}
