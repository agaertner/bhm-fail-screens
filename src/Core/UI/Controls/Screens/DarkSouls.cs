using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Extended;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.ComponentModel;

namespace Nekres.FailScreens.Core.UI.Controls.Screens {
    internal class DarkSouls : Control {

        private Color       _textColor;
        private Texture2D   _textTex; // font size: 125px, image size: 960x540
        private SoundEffect _soundEffect;
        private float       _textOpacityPercent;
        private float       _textScalePercent = 1f;
        private float       _bgOpacityPercent;

        public DarkSouls() {
            _textColor   = new Color(149, 31, 32);
            _soundEffect = FailScreensModule.Instance.ContentsManager.GetSound("screens/darksouls/darksouls_death.wav");
            _textTex     = FailScreensModule.Instance.ContentsManager.GetTexture($"screens/darksouls/{GameService.Overlay.UserLocale.Value.SupportedOrDefault().Code()}-darksouls.png");
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
            base.DisposeControl();
        }

        protected override CaptureType CapturesInput() {
            return CaptureType.Filter;
        }

        private void PlayAnimation() {
            _soundEffect?.Play(FailScreensModule.Instance.SoundVolume, 0, 0);

            // Animate text fade
            GameService.Animation.Tweener
                                   .Tween(this, new { _textOpacityPercent = 1f}, 2)
                                   .RepeatDelay(3).Repeat(1).Reflect();

            // Animate text scale
            GameService.Animation.Tweener.Tween(this, new { _textScalePercent = 1.7f }, 7);

            // Animate background fade
            GameService.Animation.Tweener.Tween(this, new { _bgOpacityPercent = 1f }, 1f)
                                     .RepeatDelay(6).Repeat(1).Reflect().OnComplete(Dispose);
        }
        
        protected override void Paint(SpriteBatch spriteBatch, Rectangle bounds) {
            if (Parent == null) {
                return;
            }

            var width  = (int)Math.Round(_textScalePercent * _textTex.Width);
            var height = (int)Math.Round(_textScalePercent * _textTex.Height);

            var centerX = (bounds.X + bounds.Width  - width)  / 2;
            var centerY = (bounds.Y + bounds.Height - height) / 2;

            var textBounds = new Rectangle(centerX,  centerY, width, height);

            // Draw background
            var rect = new Rectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height + 1);
            spriteBatch.DrawOnCtrl(this, ContentService.Textures.Pixel, rect, Color.Black * 0.7f * _bgOpacityPercent);

            // Draw text texture
            spriteBatch.DrawOnCtrl(this, _textTex, textBounds, _textColor * _textOpacityPercent);
        }
    }
}
