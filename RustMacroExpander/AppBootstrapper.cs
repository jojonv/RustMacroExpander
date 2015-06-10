using Caliburn.Micro;
using RustMacroExpander.Core;
using RustMacroExpander.Helpers;
using RustMacroExpander.Presentation.Triggers;
using RustMacroExpander.ViewModels;
using RustMacroExpander.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RustMacroExpander
{
    public class AppBootstrapper : BootstrapperBase
    {
        readonly SimpleContainer container = new SimpleContainer();

        public AppBootstrapper() { Initialize(); }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<MainWindowViewModel>();
        }

        protected override void Configure()
        {
            var settings = StateManager.Load<Settings>() ?? new Settings();
            StateManager.Save(settings);
            container.Instance<Settings>(settings);
            container.Singleton<IWindowManager, WindowManager>();
            container.Singleton<IEventAggregator, EventAggregator>();
            container.PerRequest<MainWindowViewModel>();

            var trigger = Parser.CreateTrigger;

            Parser.CreateTrigger = (target, triggerText) => 
            {
                if (triggerText == null)
                {
                    ElementConvention defaults = ConventionManager.GetElementConvention(target.GetType());
                    return defaults.CreateTrigger();
                }

                var triggerDetail = triggerText
                    .Replace("[", string.Empty)
                    .Replace("]", string.Empty);

                var splits = triggerDetail.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
                if (splits[0] == "Key")
                {
                    var key = (Key)Enum.Parse(typeof(Key), splits[1], true);
                    return new KeyTrigger { Key = key };
                }

                return trigger(target, triggerText);
            };
        }

        // TODO read more about this method
        //
        protected override object GetInstance(Type service, string key)
        {
            var instance = container.GetInstance(service, key);
            if (instance != null)
                return instance;
            throw new InvalidOperationException("Could not locate instance");
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            container.BuildUp(instance);
        }
    }

}
