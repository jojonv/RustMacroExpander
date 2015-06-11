using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Dynamic;
using System.Runtime.CompilerServices;
using System.Reflection;

namespace RustMacroExpander.ViewModels
{
    abstract public class ViewModelBase : DynamicObject, INotifyPropertyChanged
    {

        object wrappedModel;

        protected void StartWatchingProperties(object o)
        {
            var inp = o as INotifyPropertyChanged;
            if (inp == null)
                return;

            inp.PropertyChanged += (s, a) =>
            {
                NotifyOfPropertyChange(a.PropertyName);
            };
        }

        public ViewModelBase() { }

        public ViewModelBase(object model)
        {
            if (model == null)
                throw new ArgumentNullException("model is null");
            WrappedModel = model;

            StartWatchingProperties(model);
        }

        public object WrappedModel
        {
            get { return wrappedModel; }
            set { wrappedModel = value; StartWatchingProperties(wrappedModel);  }
        }

        PropertyInfo GetProperty(string propName)
        {
            PropertyInfo prop = WrappedModel.GetType().GetProperty(propName);
            return prop;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var prop = GetProperty(binder.Name);
            if (prop == null || !prop.CanRead)
            {
                result = null;
                return false;
            }

            result = prop.GetValue(WrappedModel, null);
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            var prop = GetProperty(binder.Name);
            if (prop == null || !prop.CanWrite)
                return false;

            prop.SetValue(WrappedModel, value, null);
            NotifyOfPropertyChange(binder.Name);
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyOfPropertyChange([CallerMemberName] string name = "")
        {
            var e = PropertyChanged;
            if (e == null)
                return;
            PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
