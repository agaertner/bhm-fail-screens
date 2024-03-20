using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Extended;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Nekres.FailScreens.Properties;
using System.Collections.Generic;
using System.ComponentModel;

namespace Nekres.FailScreens.Core.UI.Controls.Screens {
    internal class WinXp : Control {
        private Color _textColor;
        private Color _titleColor;

        private Texture2D   _errorBoxTex;
        private SoundEffect _soundEffect;

        private List<Point> _errorBoxRands;
        private Glide.Tween _timer;

        private Color  _blueScreenColor;
        private float  _blueScreenOpacity;
        private string _blueScreenSmile;
        private bool   _hideBoxes;

        private BitmapFontEx _smileFont;
        private BitmapFontEx _descFont;
        private BitmapFontEx _infoFont;
        private BitmapFontEx _boxFont;

        public WinXp() {
            _errorBoxRands   = new List<Point>();
            _titleColor      = Color.White;
            _textColor       = Color.Black;
            _blueScreenColor = new Color(0, 121, 217);
            _blueScreenSmile = ":(";
            _soundEffect     = FailScreensModule.Instance.ContentsManager.GetSound("screens/winxp/winxp-error.wav");
            _errorBoxTex     = FailScreensModule.Instance.ContentsManager.GetTexture("screens/winxp/winxp-error.png");

            _smileFont = FailScreensModule.Instance.ContentsManager.GetBitmapFont("fonts/CONSOLA.TTF", 250);
            _descFont  = FailScreensModule.Instance.ContentsManager.GetBitmapFont("fonts/CONSOLA.TTF", 40);
            _infoFont  = FailScreensModule.Instance.ContentsManager.GetBitmapFont("fonts/CONSOLA.TTF", 20);
            _boxFont   = FailScreensModule.Instance.ContentsManager.GetBitmapFont("fonts/CONSOLA.TTF", 16);

            PlayAnimation();

            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName.Equals("Parent") && Parent != null) {
                Size = Parent.Size;
            }
        }

        protected override void DisposeControl() {
            _timer?.Cancel();
            _smileFont?.Dispose();
            _descFont?.Dispose();
            _infoFont?.Dispose();
            _boxFont?.Dispose();
            _soundEffect?.Dispose();
            _errorBoxTex?.Dispose();
            base.DisposeControl();
        }

        protected override CaptureType CapturesInput() {
            return CaptureType.Filter;
        }

        private void PlayAnimation() {
            _timer = GameService.Animation.Tweener.Timer(0.2f).Repeat(10).OnRepeat(CreateBox).OnComplete(() => {
                _timer = GameService.Animation.Tweener.Timer(0.15f).OnComplete(() => { // fix: OnComplete is called before final repeat has completed.
                    _soundEffect?.Play(FailScreensModule.Instance.SoundVolume, 0, 0);

                    _timer = GameService.Animation.Tweener.Timer(0.5f).OnComplete(() => {
                        _blueScreenOpacity = 1f;
                        _hideBoxes         = true;
                        _timer             = GameService.Animation.Tweener.Tween(this, new { _blueScreenOpacity = 0f }, 2f, 1.25f);
                    });
                });
            });
        }

        private void CreateBox() {
            _soundEffect?.Play(FailScreensModule.Instance.SoundVolume, 0, 0); // duration: 3s

            GameService.Animation.Tweener.Timer(0.5f).OnComplete(() => {
                _errorBoxRands.Add(new Point(RandomUtil.GetRandom(-500, 500), RandomUtil.GetRandom(-500, 500)));
            });
        }

        protected override void Paint(SpriteBatch spriteBatch, Rectangle bounds) {
            if (Parent == null) {
                return;
            }

            if (!_hideBoxes) {
                foreach (var box in _errorBoxRands) {
                    DrawErrorBox(spriteBatch, bounds, box);
                }
            }

            spriteBatch.DrawOnCtrl(this, ContentService.Textures.Pixel, bounds, _blueScreenColor * _blueScreenOpacity);

            var size        = _smileFont.MeasureString(_blueScreenSmile);
            var centerX = (int)(bounds.X + bounds.Width - size.Width) / 3 - 70;
            var centerY     = 240;
            var smileBounds = new Rectangle(centerX, centerY, (int) size.Width, (int) size.Height);
            spriteBatch.DrawStringOnCtrl(this, _blueScreenSmile, _smileFont, smileBounds, Color.White * _blueScreenOpacity);

            var desc = string.Format(Resources._0__ran_into_a_problem_and_needs_to_revive__We_re_just_collecting_some_tears__and_then_we_ll_grief_with_you_, GameService.Gw2Mumble.PlayerCharacter.Name);
            var descBounds = new Rectangle(smileBounds.X + 10, smileBounds.Y + smileBounds.Height / 2, bounds.Width - smileBounds.X * 2 - 150, 500);
            spriteBatch.DrawStringOnCtrl(this, desc, _descFont, descBounds, Color.White * _blueScreenOpacity, true);
            var infoBounds = new Rectangle(descBounds.X, descBounds.Y + 150, bounds.Width - descBounds.X * 2 - 150, 500);
            spriteBatch.DrawStringOnCtrl(this, Resources.For_more_information_about_this_issue_and_possible_fixes__bother_your_party_leader_, _infoFont, infoBounds, Color.White * _blueScreenOpacity, true);
        }

        private void DrawErrorBox(SpriteBatch spriteBatch, Rectangle bounds, Point rand) {
            var width  = _errorBoxTex.Width;
            var height = _errorBoxTex.Height;

            var centerX = (bounds.X + bounds.Width  - width)  / 2 + rand.X;
            var centerY = (bounds.Y + bounds.Height - height) / 2 + rand.Y;

            var textBounds = new Rectangle(centerX, centerY, width, height);

            // Draw text texture
            spriteBatch.DrawOnCtrl(this, _errorBoxTex, textBounds, Color.White);

            spriteBatch.DrawStringOnCtrl(this, Resources.Error, _boxFont, new Rectangle(centerX + 8,  centerY + 6,  100, 20), _titleColor);
            spriteBatch.DrawStringOnCtrl(this, Resources.Fail,  _boxFont, new Rectangle(centerX + 50, centerY + 45, 100, 20), _textColor);
        }
    }
}
