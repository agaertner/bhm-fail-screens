using Blish_HUD;
using Blish_HUD.Controls;
using Nekres.FailScreens.Core.UI.Controls.Screens;
using System;
using System.Linq;

namespace Nekres.FailScreens.Core.Services {
    internal class DefeatedService : IDisposable {

        public enum FailScreens {
            DarkSouls,
            GrandTheftAuto,
            RytlocksCritterRampage,
            Windows,
            AngryPepe
        }

        private Control _failScreen;

        public DefeatedService() {
            FailScreensModule.Instance.State.StateChanged += OnStateChanged;
        }

        private void OnStateChanged(object sender, ValueEventArgs<StateService.State> e) {
            _failScreen?.Dispose();

            if (e.Value != StateService.State.Defeated) {
                return;
            }

            var screen = FailScreensModule.Instance.FailScreen.Value;

            if (FailScreensModule.Instance.Random.Value) {
                var min = Enum.GetValues(typeof(FailScreens)).Cast<int>().Min();
                var max = Enum.GetValues(typeof(FailScreens)).Cast<int>().Max();
                screen = (FailScreens)RandomUtil.GetRandom(min, max);
            }

            var buildScreen = CreateFailScreen(screen);

            if (buildScreen == null) {
                return;
            }

            buildScreen.Parent = GameService.Graphics.SpriteScreen;
            buildScreen.Size   = GameService.Graphics.SpriteScreen.Size;
            _failScreen        = buildScreen;
        }

        private Control CreateFailScreen(FailScreens failScreen) {
            return failScreen switch {
                FailScreens.DarkSouls              => new DarkSouls(),
                FailScreens.GrandTheftAuto         => new GrantTheftAuto(),
                FailScreens.RytlocksCritterRampage => new RytlocksCritterRampage(),
                FailScreens.Windows                => new WinXp(),
                FailScreens.AngryPepe              => new AngryPepe(),
                _                                  => null
            };
        }

        public void Dispose() {
            FailScreensModule.Instance.State.StateChanged -= OnStateChanged;
            _failScreen?.Dispose();
        }
    }
}
