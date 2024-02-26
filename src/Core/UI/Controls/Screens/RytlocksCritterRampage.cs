using System;
using System.Collections.Generic;
using Blish_HUD;
using Blish_HUD.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System.ComponentModel;

namespace Nekres.FailScreens.Core.UI.Controls.Screens {
    internal class RytlocksCritterRampage : Control {

        private Texture2D _sadRytlockSheet0;
        //private Texture2D _sadRytlockSheet1;
        //private Texture2D _sadRytlockSheet2;
        //private Texture2D _sadRytlockSheet3;
        //private Texture2D _sadRytlockSheet4;
        //private Texture2D _sadRytlockSheet5;

        private SoundEffect _soundEffect;

        private float _spriteOpacityPercent;
        private float _bgOpacityPercent;

        public RytlocksCritterRampage() {
            _soundEffect      = FailScreensModule.Instance.ContentsManager.GetSound("screens/rytlock/new_music_sadending.wav");

            _sadRytlockSheet0 = FailScreensModule.Instance.ContentsManager.GetTexture("screens/rytlock/sad_rytlock-sheet0.png");
            //_sadRytlockSheet1 = FailScreensModule.Instance.ContentsManager.GetTexture("screens/rytlock/sad_rytlock-sheet1.png");
            //_sadRytlockSheet2 = FailScreensModule.Instance.ContentsManager.GetTexture("screens/rytlock/sad_rytlock-sheet2.png");
            //_sadRytlockSheet3 = FailScreensModule.Instance.ContentsManager.GetTexture("screens/rytlock/sad_rytlock-sheet3.png");
            //_sadRytlockSheet4 = FailScreensModule.Instance.ContentsManager.GetTexture("screens/rytlock/sad_rytlock-sheet4.png");
            //_sadRytlockSheet5 = FailScreensModule.Instance.ContentsManager.GetTexture("screens/rytlock/sad_rytlock-sheet5.png");

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
            _sadRytlockSheet0?.Dispose();
            //_sadRytlockSheet1?.Dispose();
            //_sadRytlockSheet2?.Dispose();
            //_sadRytlockSheet3?.Dispose();
            //_sadRytlockSheet4?.Dispose();
            //_sadRytlockSheet5?.Dispose();
            base.DisposeControl();
        }

        protected override CaptureType CapturesInput() {
            return CaptureType.Filter;
        }

        private void PlayAnimation() {
            _soundEffect?.Play(GameService.GameIntegration.Audio.Volume, 0, 0);

            // Animate fade
            GameService.Animation.Tweener
                       .Tween(this, new { _spriteOpacityPercent = 1f }, 1f)
                       .RepeatDelay(7).Repeat(1).Reflect();

            // Animate background fade
            GameService.Animation.Tweener.Tween(this, new { _bgOpacityPercent = 1f }, 1f)
                       .RepeatDelay(8).Repeat(1).Reflect().OnComplete(Dispose);
        }
        
        protected override void Paint(SpriteBatch spriteBatch, Rectangle bounds) {
            if (Parent == null) {
                return;
            }

            // Draw background
            var rect = new Rectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height + 1);
            spriteBatch.DrawOnCtrl(this, ContentService.Textures.Pixel, rect, Color.Black * 0.7f * _bgOpacityPercent);

            // 190, 153 with speech bubble
            // 175, 150 without

            var frameOrigin = AnimationUtil.Animate(6, 2, 190, 153, 5);

            // Draw animation
            spriteBatch.DrawOnCtrl(this,
                                   _sadRytlockSheet0, 
                                   new Rectangle(bounds.Width / 2 - 190, bounds.Height / 2, 190 * 2, 153 * 2), 
                                   new Rectangle(frameOrigin.X, frameOrigin.Y, 190, 153), Color.White * _spriteOpacityPercent);
        }

        //TODO: Implement advanced sprite animation across multiple sprite sheets.
        private class TextureData : IDisposable {

            private Texture _spriteSheet;

            private int     _totalFrames;
            private int     _framesPerRow;
            private int     _framesPerColumn;
            private Point[] _frameSizes;

            private int _currentFrame;

            public TextureData(Texture2D spriteSheet, int totalFrames, int framesPerRow, int framesPerColumn, params Point[] frameSizes) {
                _spriteSheet     = spriteSheet;
                _totalFrames     = totalFrames;
                _framesPerRow    = framesPerRow;
                _framesPerColumn = framesPerColumn;
                _frameSizes      = frameSizes;
            }

            public int Column(int currentFrame) {
                return currentFrame % _framesPerRow;
            }
            public int Row(int currentFrame) {
                return currentFrame / _framesPerRow;
            }

            public void DrawOnCtrl(Control ctrl, Rectangle destinationRectangle) {

            }

            public void Dispose() {
                _spriteSheet?.Dispose();
            }
        }
    }
}
