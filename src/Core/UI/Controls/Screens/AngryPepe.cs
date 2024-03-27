using Blish_HUD;
using Blish_HUD.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.ComponentModel;

namespace Nekres.FailScreens.Core.UI.Controls.Screens {
    internal class AngryPepe : Control {

        private Texture2D   _pepeTex;
        private SoundEffect _soundEffect;
        private float       _ragePercent;

        public AngryPepe() {
            _soundEffect = FailScreensModule.Instance.ContentsManager.GetSound("screens/angrypepe/reeee.wav");
            _pepeTex = FailScreensModule.Instance.ContentsManager.GetTexture("screens/angrypepe/angry-pepe.png");
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
            _pepeTex?.Dispose();
            base.DisposeControl();
        }

        protected override CaptureType CapturesInput() {
            return CaptureType.Filter;
        }

        private void PlayAnimation() {
            _soundEffect?.Play(FailScreensModule.Instance.SoundVolume, 0, 0); // duration: 6s

            // Animate rage percent
            GameService.Animation.Tweener
                                   .Tween(this, new { _ragePercent = 1f }, 3.5f, 0.02f)
                       .OnComplete(Dispose);
        }

        protected override void Paint(SpriteBatch spriteBatch, Rectangle bounds) {
            if (Parent == null) {
                return;
            }

            var size = PointExtensions.ResizeKeepAspect(new Point(_pepeTex.Width, _pepeTex.Height), 
                                                        (int)Math.Round(0.8f * bounds.Width), 
                                                        (int)Math.Round(0.8f * bounds.Height));
            var width = size.X;
            var height = size.Y;

            var centerX = (bounds.X + bounds.Width  - width)  / 2 + (int)Math.Round(_ragePercent * RandomUtil.GetRandom(-100, 100));
            var centerY = (bounds.Y + bounds.Height - height) / 2 + (int)Math.Round(_ragePercent * RandomUtil.GetRandom(-100, 100));

            var pepeBounds = new Rectangle(centerX, centerY, width, height);

            // Draw pepe
            spriteBatch.DrawOnCtrl(this, _pepeTex, pepeBounds);

            // Draw foreground (rage red fade)
            var rect = new Rectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height + 1);
            spriteBatch.DrawOnCtrl(this, ContentService.Textures.Pixel, rect, Color.Red * 0.5f * _ragePercent);
        }
    }
}
