using Blish_HUD;
using Blish_HUD.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.ComponentModel;
using Blish_HUD.Extended;
using Nekres.FailScreens.Properties;

namespace Nekres.FailScreens.Core.UI.Controls.Screens {
    internal class Sekiro : Control {

        private Color        _kanjiFlashColor;
        private Color        _textColor;
        private Texture2D    _kanjiTex;
        private Texture2D    _kanjiShadowTex;
        private SoundEffect  _soundEffect;
        private float        _textOpacityPercent;
        private float        _kanjiFlashScalePercent = 1f;
        private float        _kanjiFlashOpacityPercent;
        private float        _bgOpacityPercent;
        private string       _text;
        private BitmapFontEx _font;
        public Sekiro() {
            _textColor       = new Color(177, 12, 16);
            _kanjiFlashColor = new Color(250,120, 120);
            _text            = BoldLetters(new[] { Resources.Death, "Yeet", "Lmaooooo" }[RandomUtil.GetRandom(0, 2)]);
            _soundEffect     = FailScreensModule.Instance.ContentsManager.GetSound("screens/sekiro/sekiro_death.wav");
            _kanjiTex        = FailScreensModule.Instance.ContentsManager.GetTexture("screens/sekiro/sekiro_death.png");
            _kanjiShadowTex  = FailScreensModule.Instance.ContentsManager.GetTexture("screens/sekiro/sekiro_death_out.png");
            _font            = FailScreensModule.Instance.ContentsManager.GetBitmapFont("fonts/Athelas-Regular.ttf", 250);
            PlayAnimation();

            PropertyChanged += OnPropertyChanged;
        }

        private string BoldLetters(string text) {
            return string.Join(" ", text.ToUpper().ToCharArray());
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName.Equals("Parent") && Parent != null) {
                Size = Parent.Size;
            }
        }

        protected override void DisposeControl() {
            _soundEffect?.Dispose();
            _kanjiTex?.Dispose();
            _kanjiShadowTex?.Dispose();
            _font?.Dispose();
            base.DisposeControl();
        }

        protected override CaptureType CapturesInput() {
            return CaptureType.Filter;
        }

        private void PlayAnimation() {
            _soundEffect?.Play(FailScreensModule.Instance.SoundVolume, 0, 0);

            // Animate text fade
            GameService.Animation.Tweener.Timer(4).OnComplete(() => {
                    GameService.Animation.Tweener.Tween(this, new { _textOpacityPercent = 0f }, 2);
            });

            // Animate flash scale
            GameService.Animation.Tweener.Tween(this, new { _kanjiFlashScalePercent = 1.05f }, 0.08f).Repeat(1).Reflect();

            // Animate flash opacity
            GameService.Animation.Tweener.Tween(this, new { _kanjiFlashOpacityPercent = 1f, _textOpacityPercent = 1f }, 0.08f).OnComplete(() => {
                            GameService.Animation.Tweener.Tween(this, new {_kanjiFlashOpacityPercent = 0f}, 2);
            });

            // Animate background fade
            GameService.Animation.Tweener.Tween(this, new { _bgOpacityPercent = 1f }, 1f)
                                     .RepeatDelay(5).Repeat(1).Reflect().OnComplete(Dispose);
        }
        
        protected override void Paint(SpriteBatch spriteBatch, Rectangle bounds) {
            if (Parent == null) {
                return;
            }
            var textSize   = _font.MeasureString(_text);
            var textWidth  = (int)Math.Round(textSize.Width);
            var textHeight = (int)Math.Round(textSize.Height);

            var width   = _kanjiTex.Width;
            var height  = _kanjiTex.Height;
            var centerX = (bounds.X + bounds.Width  - width)               / 2;
            var centerY = (bounds.Y + bounds.Height - height - textHeight) / 2;

            var kanjiBounds = new Rectangle(centerX, centerY, width, height);

            // Draw background
            var rect = new Rectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height + 1);
            spriteBatch.DrawOnCtrl(this, ContentService.Textures.Pixel, rect, Color.Black * 0.7f * _bgOpacityPercent);

            // Draw kanji texture
            spriteBatch.DrawOnCtrl(this, _kanjiTex, kanjiBounds, _textColor * _textOpacityPercent);
            // Draw kanji shadow texture
            spriteBatch.DrawOnCtrl(this, _kanjiShadowTex, kanjiBounds, _textColor * _textOpacityPercent);

            // Calculate flash
            width       = (int)Math.Round(kanjiBounds.Width  * _kanjiFlashScalePercent);
            height      = (int)Math.Round(kanjiBounds.Height * _kanjiFlashScalePercent);
            centerX     = (bounds.X + bounds.Width  - width)               / 2;
            centerY     = (bounds.Y + bounds.Height - height - textHeight) / 2;
            kanjiBounds = new Rectangle(centerX, centerY, width, height);

            // Draw kanji texture as flash
            spriteBatch.DrawOnCtrl(this, _kanjiTex, kanjiBounds, _kanjiFlashColor * _kanjiFlashOpacityPercent);
            // Draw kanji shadow texture as flash
            spriteBatch.DrawOnCtrl(this, _kanjiShadowTex, kanjiBounds, _kanjiFlashColor * _kanjiFlashOpacityPercent);

            // Draw text
            var textBounds = new Rectangle((bounds.X + bounds.Width - textWidth) / 2, kanjiBounds.Bottom, textWidth, textHeight);
            spriteBatch.DrawStringOnCtrl(this, _text, _font, textBounds, _textColor * _textOpacityPercent);
        }
    }
}
