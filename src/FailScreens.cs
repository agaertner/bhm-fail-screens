using Blish_HUD;
using Blish_HUD.Modules;
using Blish_HUD.Modules.Managers;
using Blish_HUD.Settings;
using Microsoft.Xna.Framework;
using Nekres.FailScreens.Core.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
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

        internal SettingEntry<DefeatedService.FailScreens>     FailScreen;
        internal SettingEntry<bool>                            Random;
        internal Dictionary<DefeatedService.FailScreens, SettingEntry<bool>> ToggleScreens;
        internal SettingEntry<float>                           Volume;
        internal SettingEntry<bool>                            Muted;
        internal SettingEntry<bool>                            UseArcDps;

        internal float SoundVolume = 1f;

        protected override void DefineSettings(SettingCollection settings) {
            var visualsCol = settings.AddSubCollection("visuals", true, () => "Defeated Screen");
            FailScreen = visualsCol.DefineSetting("fail_screen", DefeatedService.FailScreens.DarkSouls,
                                                  () => "Appearance",
                                                  () => "Visual to display upon defeat.");
            Random     = visualsCol.DefineSetting("random",      true,
                                                  () => "Randomize",
                                                  () => "Ignores selection if set.");
            var screensCol = visualsCol.AddSubCollection("screens", true, () => "Randomizer Toggles");
            ToggleScreens = new Dictionary<DefeatedService.FailScreens, SettingEntry<bool>>();
            foreach (var screen in Enum.GetValues(typeof(DefeatedService.FailScreens)).Cast<DefeatedService.FailScreens>()) {
                ToggleScreens.Add(screen, screensCol.DefineSetting("enable_" + screen.ToString().ToLower(), true,
                                                                   () => "Enable " + screen.ToString().SplitCamelCase(),
                                                                   () => "Enable or disable the \"" + screen.ToString().SplitCamelCase() + "\" screen for randomization."));
            }
            var soundCol = settings.AddSubCollection("sound", true, () => "Sound Options"); 
            Volume = soundCol.DefineSetting("volume", 0.05f,   
                                            () => "Volume",
                                            () => "Adjusts the audio volume.");
            Muted = soundCol.DefineSetting("mute",   false,
                                           () => "Mute",
                                           () => "Mutes the audio.");

            var generalCol = settings.AddSubCollection("general", true, () => "General");
            UseArcDps = generalCol.DefineSetting("use_arcdps", false, () => "Use ArcDps for Detection", () => "Use ArcDps Bridge for Defeated state detection if available.\nBy default (ie. disabled), the module checks if the game accesses a dummy sound file referenced in /Documents/Guild Wars 2/music/Defeated.m3u.");

            Volume.SetRange(0, 0.1f);
            Volume.SetValidation(ValidateVolume);
            if (!ValidateVolume(Volume.Value).Valid) {
                Volume.Value = 0.05f;
            }
        }

        private SettingValidationResult ValidateVolume(float vol) {
            return new SettingValidationResult(vol is >= 0f and <= 0.1f);
        }

        protected override void Initialize() {
            State    = new StateService();
            Defeated = new DefeatedService();
        }

        protected override async Task LoadAsync() {
            await State.SetupLockFiles(StateService.State.Defeated);
        }

        protected override void Update(GameTime gameTime) {
            State?.Update();
            SoundVolume = Muted.Value ? 0f : Math.Min(GameService.GameIntegration.Audio.Volume, Volume.Value);
        }

        protected override void OnModuleLoaded(EventArgs e) {
            // Base handler must be called
            base.OnModuleLoaded(e);
        }

        /// <inheritdoc />
        protected override void Unload() {
            Defeated?.Dispose();
            State?.Dispose();
            // All static members must be manually unset
            Instance = null;
        }
    }
}
