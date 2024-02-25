using Blish_HUD;
using Blish_HUD.Controls;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Blish_HUD.Extended;
using Nekres.FailScreens.Core.UI.Controls;

namespace Nekres.FailScreens.Core.Services {
    internal class StateService : IDisposable {
        public enum State {
            StandBy,
            Mounted,
            Battle,
            Competitive,
            Defeated
        }

        private DarkSoulsDeath darkSoulsDeath;

        private State    _currentState      = State.StandBy;
        private string   _lockFile          = "silence.wav";
        private DateTime _lastLockFileCheck = DateTime.UtcNow.AddSeconds(10);

        public async Task SetupLockFiles(State state) {
            var relLockFilePath = $"{state}\\{_lockFile}";
            await FailScreensModule.Instance.ContentsManager.Extract($"audio/{_lockFile}", Path.Combine(DirectoryUtil.MusicPath, relLockFilePath), false);
            try {
                var path = Path.Combine(DirectoryUtil.MusicPath, $"{state}.m3u");
                relLockFilePath = $"{relLockFilePath}\r\n";
                if (File.Exists(path)) {
                    var lines = File.ReadAllText(path, Encoding.UTF8);
                    if (lines.Equals(relLockFilePath)) {
                        ScreenNotification.ShowNotification($"{state} playlist already exists.");
                        return;
                    }
                    File.Copy(path, Path.Combine(DirectoryUtil.MusicPath, $"{state}.backup.m3u"), true);
                }
                using var file = File.Create(path);
                file.Position = 0;
                var content = Encoding.UTF8.GetBytes(relLockFilePath);
                await file.WriteAsync(content, 0, content.Length);
                ScreenNotification.ShowNotification($"{state} playlist created. Game restart required.", ScreenNotification.NotificationType.Warning);
            } catch (Exception e) {
                FailScreensModule.Logger.Info(e, e.Message);
            }
        }

        public void RevertLockFiles(State state) {
            try {
                var path = Path.Combine(DirectoryUtil.MusicPath, $"{state}.backup.m3u");

                if (File.Exists(path)) {
                    File.Copy(path, Path.Combine(DirectoryUtil.MusicPath, $"{state}.m3u"), true);
                    File.Delete(path);
                    ScreenNotification.ShowNotification($"{state} playlist reverted. Game restart required.", ScreenNotification.NotificationType.Warning);
                }
            } catch (Exception e) {
                FailScreensModule.Logger.Info(e, e.Message);
            }
        }

        public void Update() {
            if (DateTime.UtcNow.Subtract(_lastLockFileCheck).TotalMilliseconds > 200) {
                _lastLockFileCheck = DateTime.UtcNow;
                CheckLockFile(State.Defeated);
            }
        }

        private void CheckLockFile(State state) {
            var absLockFilePath = Path.Combine(DirectoryUtil.MusicPath, $"{state}\\{_lockFile}");
            if (FileUtil.IsFileLocked(absLockFilePath)) {
                ChangeState(state);
            } else if (_currentState == state) {
                ChangeState(State.StandBy);
            }
        }

        private void ChangeState(State state) {
            if (_currentState != state) {
                _currentState = state;

                darkSoulsDeath?.Dispose();
                if (state == State.Defeated) {
                    darkSoulsDeath = new DarkSoulsDeath {
                        Parent = GameService.Graphics.SpriteScreen,
                        Size   = GameService.Graphics.SpriteScreen.Size
                    };
                }
            }
        }

        public void Dispose() {
            RevertLockFiles(State.Defeated);
            darkSoulsDeath?.Dispose();
        }
    }
}
