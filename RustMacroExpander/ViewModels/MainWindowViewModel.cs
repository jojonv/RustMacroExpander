using Caliburn.Micro;
using RustMacroExpander.Core;
using RustMacroExpander.Helpers;


namespace RustMacroExpander.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        readonly RustBridge model;
        readonly IEventAggregator eventAggregator;

        public MainWindowViewModel(IEventAggregator eventAggregator, Settings settings) 
        {
            // Recover
            //
            var prevModel = StateManager.Load<RustBridge>();
            (model = prevModel ?? new RustBridge()).SetDependencies(settings);
            WrappedModel = model;

            // Queue it to be serialized again when app exits
            //
            StateManager.Save(model);

            // Update rustc? version number
            //
            model.UpdateVersion();

            this.eventAggregator = eventAggregator;
            SettingsViewModel = new SettingsViewModel(settings);
        }


        #region Properties

        public string WindowTitle { get; } = "Rust Macro Expander";

        public IFlyoutViewModel SettingsViewModel { get; private set; }

        #endregion

        public bool CanBuild(string content) => !string.IsNullOrWhiteSpace(content);

        public void Build(string content)
        {
            model.Build(content);
        }

        public void ChangeState(bool f)
        {
            SettingsViewModel.ChangeState(f);
        }

        /// <summary>
        /// Caliburn calls CanBuild at the app start rendering whole window disabled
        /// </summary>
        /// <param name="content"></param>
        public void BuildWorkAround(string content)
        {
            if (CanBuild(content))
                Build(content);
        }
    }
}
