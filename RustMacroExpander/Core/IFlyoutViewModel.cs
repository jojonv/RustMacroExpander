﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RustMacroExpander.Core
{
    public interface IFlyoutViewModel
    {
        bool IsOpen { get; }
        void ChangeState(bool f);
    }
}
