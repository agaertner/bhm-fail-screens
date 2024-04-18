using Blish_HUD;
using Blish_HUD.Controls;
using Nekres.FailScreens.Core.UI.Controls.Screens;
using System;
using System.Linq;
using Blish_HUD.Input;

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

        private bool _isSuperAdventureBox;

        private int _dblClickCount;

        public DefeatedService() {
            OnMapChanged(GameService.Gw2Mumble.CurrentMap, new ValueEventArgs<int>(GameService.Gw2Mumble.CurrentMap.Id));

            FailScreensModule.Instance.State.StateChanged += OnStateChanged;
            GameService.Gw2Mumble.CurrentMap.MapChanged   += OnMapChanged;

            GameService.Input.Mouse.LeftMouseButtonPressed += OnLeftMouseButtonReleased;
        }

        private void OnLeftMouseButtonReleased(object sender, MouseEventArgs e) {
            if (_failScreen == null) {
                _dblClickCount = 0;
                return;
            }
            _dblClickCount++;
            if (_dblClickCount > 1) {
                _failScreen?.Dispose();
                _failScreen    = null;
                _dblClickCount = 0;
            }
        }

        private async void OnMapChanged(object sender, ValueEventArgs<int> e) {
            try {
                var map = await GameService.Gw2WebApi.AnonymousConnection.Client.V2.Maps.GetAsync(e.Value);
                _isSuperAdventureBox = map?.RegionId == 29;
            } catch (Exception ex) {
                FailScreensModule.Logger.Info(ex, ex.Message);
                _isSuperAdventureBox = false;
            }
        }

        private void OnStateChanged(object sender, ValueEventArgs<StateService.State> e) {
            if (e.Value != StateService.State.Defeated) {
                if (!_isSuperAdventureBox) {
                    _failScreen?.Dispose();
                    _failScreen = null;
                }
                return;
            }

            _failScreen?.Dispose();
            _failScreen = null;

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
            GameService.Input.Mouse.LeftMouseButtonPressed -= OnLeftMouseButtonReleased;
            GameService.Gw2Mumble.CurrentMap.MapChanged    -= OnMapChanged;
            FailScreensModule.Instance.State.StateChanged  -= OnStateChanged;
            _failScreen?.Dispose();
        }
    }
}
