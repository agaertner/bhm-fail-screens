using Blish_HUD;
using Blish_HUD.Modules;
using Blish_HUD.Modules.Managers;
using Blish_HUD.Settings;
using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Nekres.FailScreens.Core.Services;

namespace Nekres.FailScreens {
    [Export(typeof(Module))]
    public class FailScreensModule : Module {
        internal static readonly Logger Logger = Logger.GetLogger<FailScreensModule>();

        internal static FailScreensModule Instance { get; private set; }

        #region Service Managers
        internal SettingsManager SettingsManager => this.ModuleParameters.SettingsManager;
        internal ContentsManager ContentsManager => this.ModuleParameters.ContentsManager;
        internal DirectoriesManager DirectoriesManager => this.ModuleParameters.DirectoriesManager;
        internal Gw2ApiManager Gw2ApiManager => this.ModuleParameters.Gw2ApiManager;
        #endregion

        [ImportingConstructor]
        public FailScreensModule([Import("ModuleParameters")] ModuleParameters moduleParameters) : base(moduleParameters) => Instance = this;

        internal StateService State;

        protected override void DefineSettings(SettingCollection settings) {
        }

        protected override void Initialize() {
            State = new StateService();
        }
         
        protected override async Task LoadAsync() {
            await State.SetupLockFiles(StateService.State.Defeated);
        }

        protected override void Update(GameTime gameTime) {
            State?.Update();
        }

        protected override void OnModuleLoaded(EventArgs e) {

            // Base handler must be called
            base.OnModuleLoaded(e);
        }

        /// <inheritdoc />
        protected override void Unload() {
            State?.Dispose();

            // All static members must be manually unset
            Instance = null;
        }
    }
}
