using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Extended;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Nekres.FailScreens.Core.Services {
    internal class StateService : IDisposable {

        public event EventHandler<ValueEventArgs<State>> StateChanged;

        public State CurrentState { get; private set; }

        public enum State {
            StandBy,
            Defeated
        }

        private string   _lockFile          = "silence.wav";
        private DateTime _lastLockFileCheck = DateTime.UtcNow.AddSeconds(10);

        public async Task SetupLockFiles(State state) {
            var relLockFilePath = $"{state}\\{_lockFile}";
            await FailScreensModule.Instance.ContentsManager.Extract($"audio/{_lockFile}", Path.Combine(DirectoryUtil.MusicPath, relLockFilePath), false);
            try {
                FailScreensModule.Logger.Info($"Creating {state} playlist.");

                var path = Path.Combine(DirectoryUtil.MusicPath, $"{state}.m3u");
                relLockFilePath = $"{relLockFilePath}\r\n";
                if (File.Exists(path)) {
                    var lines = File.ReadAllText(path, Encoding.UTF8);
                    if (lines.Equals(relLockFilePath)) {
                        FailScreensModule.Logger.Info($"{state} playlist already exists. Skipping.");
                        return;
                    }
                    FailScreensModule.Logger.Info($"Storing existing {state} playlist as backup.");
                    File.Copy(path, Path.Combine(DirectoryUtil.MusicPath, $"{state}.backup.m3u"), true);
                }
                using var file = File.Create(path);
                file.Position = 0;
                var content = Encoding.UTF8.GetBytes(relLockFilePath);
                await file.WriteAsync(content, 0, content.Length);
                FailScreensModule.Logger.Info($"{state} playlist created.");
                ScreenNotification.ShowNotification($"{state} playlist created. Game restart required.", ScreenNotification.NotificationType.Warning);
            } catch (Exception e) {
                FailScreensModule.Logger.Info(e, e.Message);
            }
        }

        public void RevertLockFiles(State state) {
            try {
                var backupPath = Path.Combine(DirectoryUtil.MusicPath, $"{state}.backup.m3u");
                var path       = Path.Combine(DirectoryUtil.MusicPath, $"{state}.m3u");

                if (File.Exists(backupPath)) {
                    File.Copy(backupPath, path, true);
                    File.Delete(backupPath);
                } else if (File.Exists(path)) {
                    File.Delete(path);
                }
                ScreenNotification.ShowNotification($"{state} playlist reverted. Game restart required.", ScreenNotification.NotificationType.Warning);
            } catch (Exception e) {
                FailScreensModule.Logger.Info(e, e.Message);
            }
        }

        public void Update() {
            if (DateTime.UtcNow.Subtract(_lastLockFileCheck).TotalMilliseconds > 100) {
                _lastLockFileCheck = DateTime.UtcNow;
                CheckLockFile(State.Defeated);
            }
        }

        private void CheckLockFile(State state) {
            var absLockFilePath = Path.Combine(DirectoryUtil.MusicPath, $"{state}\\{_lockFile}");
            if (FileUtil.IsFileLocked(absLockFilePath)) {
                ChangeState(state);
            } else if (CurrentState == state) {
                ChangeState(State.StandBy);
            }
        }

        private void ChangeState(State state) {
            if (CurrentState != state) {
                CurrentState = state;
                StateChanged?.Invoke(this, new ValueEventArgs<State>(CurrentState));
            }
        }

        public void Dispose() {
            //RevertLockFiles(State.Defeated); //TODO
        }
    }
}
