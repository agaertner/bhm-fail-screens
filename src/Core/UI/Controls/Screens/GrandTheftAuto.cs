using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Extended;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System.ComponentModel;

namespace Nekres.FailScreens.Core.UI.Controls.Screens {
    internal class GrantTheftAuto : Control {

        private Color       _textColor;
        private Texture2D   _textTex; // font size: 125px, image size: 960x540
        private Texture2D   _photoTex;
        private Texture2D   _flashTex;
        private SoundEffect _soundEffect;

        private float _textOpacityPercent;
        private float _flashOpacityPercent;
        private float _photoOpacityPercent;
        private float _bgOpacityPercent;

        public GrantTheftAuto() {
            _textColor   = new Color(149, 31, 32);
            _soundEffect = FailScreensModule.Instance.ContentsManager.GetSound("screens/gta/gta_wasted.wav");
            _textTex     = FailScreensModule.Instance.ContentsManager.GetTexture($"screens/gta/{GameService.Overlay.UserLocale.Value.SupportedOrDefault().Code()}-wasted.png");
            _photoTex    = FailScreensModule.Instance.ContentsManager.GetTexture("screens/gta/photo-vignette.png");
            _flashTex    = FailScreensModule.Instance.ContentsManager.GetTexture("screens/gta/flash-vignette.png");
            PlayAnimation();

            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName.Equals("Parent") && Parent != null) {
                Size = Parent.Size;
            }
        }

        protected override void DisposeControl() {
            _soundEffect?.Dispose();
            _textTex?.Dispose();
            _photoTex?.Dispose();
            _flashTex?.Dispose();
            base.DisposeControl();
        }

        protected override CaptureType CapturesInput() {
            return CaptureType.Filter;
        }

        private void PlayAnimation() {
            _soundEffect?.Play(FailScreensModule.Instance.SoundVolume, 0, 0);

            // Animate photo opacity
            GameService.Animation.Tweener.Tween(this, new { _photoOpacityPercent = 0.6f }, 0.15f)
                       .RepeatDelay(0.05f)
                       .Repeat(1)
                       .Reflect();

            GameService.Animation.Tweener.Timer(2.25f).OnComplete(() => {
                // Animate flash opacity
                GameService.Animation.Tweener.Tween(this, new { _flashOpacityPercent = 0.6f }, 0.15f)
                           .RepeatDelay(0.05f)
                           .Repeat(1)
                           .Reflect();

                // Animate text fade
                GameService.Animation.Tweener.Timer(0.15f).OnComplete(() => {
                    GameService.Animation.Tweener
                               .Tween(this, new { _textOpacityPercent = 1f }, 0.008f)
                               .RepeatDelay(5).Repeat(1).Reflect();
                });
            });

            // Animate background fade
            GameService.Animation.Tweener.Tween(this, new { _bgOpacityPercent = 1f }, 1f)
                       .RepeatDelay(6).Repeat(1).Reflect().OnComplete(Dispose);
        }
        
        protected override void Paint(SpriteBatch spriteBatch, Rectangle bounds) {
            if (Parent == null) {
                return;
            }

            var width  = _textTex.Width;
            var height = _textTex.Height;

            var centerX = (bounds.X + bounds.Width  - width)  / 2;
            var centerY = (bounds.Y + bounds.Height - height) / 2;

            var textBounds = new Rectangle(centerX,  centerY, width, height);

            // Draw background
            var rect = new Rectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height + 1);
            spriteBatch.DrawOnCtrl(this, ContentService.Textures.Pixel, rect, Color.Black * 0.7f * _bgOpacityPercent);

            // Draw photo vignette
            spriteBatch.DrawOnCtrl(this, _photoTex, rect, _textColor * _photoOpacityPercent);

            // Draw flash vignette
            spriteBatch.DrawOnCtrl(this, _flashTex, rect, _textColor * _flashOpacityPercent);

            // Draw text texture
            spriteBatch.DrawOnCtrl(this, _textTex, textBounds, _textColor * _textOpacityPercent);
        }
    }
}
