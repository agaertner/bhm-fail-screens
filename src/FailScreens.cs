using Blish_HUD;
using Blish_HUD.Modules;
using Blish_HUD.Modules.Managers;
using Blish_HUD.Settings;
using Microsoft.Xna.Framework;
using Nekres.FailScreens.Core.Services;
using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

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

        internal StateService    State;
        internal DefeatedService Defeated;

        internal SettingEntry<DefeatedService.FailScreens> FailScreen;
        internal SettingEntry<bool>                        Random;
        internal SettingEntry<float>                       Volume;
        internal SettingEntry<bool>                        Muted;

        internal float SoundVolume = 1f;

        protected override void DefineSettings(SettingCollection settings) {
            var visualsCol = settings.AddSubCollection("visuals", true, () => "Defeated Screen");
            FailScreen = visualsCol.DefineSetting("fail_screen", DefeatedService.FailScreens.DarkSouls, () => "Appearance", () => "Visual to display upon defeat.");
            Random     = visualsCol.DefineSetting("random",      true,                                  () => "Randomize", () => "Ignores selection if set.");
            var soundCol = settings.AddSubCollection("sound", true, () => "Sound Options");
            Volume = soundCol.DefineSetting("volume", 50f,   () => "Volume", () => "Adjusts the audio volume.");
            Muted        = soundCol.DefineSetting("mute",   false, () => "Mute",        () => "Mutes the audio.");
        }

        private void OnVolumeSettingChanged(object o, ValueChangedEventArgs<float> e) {
            SoundVolume = Math.Min(GameService.GameIntegration.Audio.Volume, e.NewValue / 1000);
        }

        private void OnMutedSettingChanged(object o, ValueChangedEventArgs<bool> e) {
            SoundVolume = e.NewValue ? 0f : Math.Min(GameService.GameIntegration.Audio.Volume, Volume.Value / 1000);
        }

        protected override void Initialize() {
            State    = new StateService();
            Defeated = new DefeatedService();

            SoundVolume           =  Muted.Value ? 0f : Math.Min(GameService.GameIntegration.Audio.Volume, Volume.Value / 1000);
            Volume.SettingChanged += OnVolumeSettingChanged;
            Muted.SettingChanged  += OnMutedSettingChanged;
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
            Muted.SettingChanged  -= OnMutedSettingChanged;
            Volume.SettingChanged -= OnVolumeSettingChanged;
            Defeated?.Dispose();
            State?.Dispose();
            // All static members must be manually unset
            Instance = null;
        }
    }
}
