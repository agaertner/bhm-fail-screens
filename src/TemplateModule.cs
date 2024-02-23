using Blish_HUD;
using Blish_HUD.Modules;
using Blish_HUD.Modules.Managers;
using Blish_HUD.Settings;
using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace Nekres.TemplateModule {
    [Export(typeof(Module))]
    public class TemplateModule : Module {
        internal static readonly Logger Logger = Logger.GetLogger<TemplateModule>();

        internal static TemplateModule Instance { get; private set; }

        #region Service Managers
        internal SettingsManager SettingsManager => this.ModuleParameters.SettingsManager;
        internal ContentsManager ContentsManager => this.ModuleParameters.ContentsManager;
        internal DirectoriesManager DirectoriesManager => this.ModuleParameters.DirectoriesManager;
        internal Gw2ApiManager Gw2ApiManager => this.ModuleParameters.Gw2ApiManager;
        #endregion

        [ImportingConstructor]
        public TemplateModule([Import("ModuleParameters")] ModuleParameters moduleParameters) : base(moduleParameters) => Instance = this;

        protected override void DefineSettings(SettingCollection settings) {
        }

        protected override void Initialize() {
        }

        protected override async Task LoadAsync() {

        }

        protected override void OnModuleLoaded(EventArgs e) {
            // Base handler must be called
            base.OnModuleLoaded(e);
        }

        /// <inheritdoc />
        protected override void Unload() {
            // All static members must be manually unset
            Instance = null;
        }
    }
}
