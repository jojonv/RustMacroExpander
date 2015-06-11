using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RustMacroExpander.ViewModels
{
    //public class SettingsViewModel : PropertyChangedBase, IFlyoutViewModel
    public class SettingsViewModel : ViewModelBase, IFlyoutViewModel
    {
        bool isOpen;


        public SettingsViewModel(Settings s) : base(s) { }


        public bool IsOpen { get { return isOpen; } set { isOpen = value; NotifyOfPropertyChange(nameof(IsOpen)); } }

        public void ChangeState(bool s)
        {
            // If is opened already close, else open
            //
            IsOpen = s ? false : true;
        }
    }
}
