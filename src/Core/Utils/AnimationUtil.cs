using Blish_HUD;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Nekres.FailScreens.Core {
    internal static class AnimationUtil {
        public static int Column(int currentFrame, int framesPerRow) {
            return currentFrame % framesPerRow;
        }

        public static int Row(int currentFrame, int framesPerRow) {
            return currentFrame / framesPerRow;
        }

        public static int Frame(int totalFrames, int updatesPerFrame) {
            double elapsedTime = GameService.Overlay.CurrentGameTime.TotalGameTime.TotalSeconds;
            return (int)(elapsedTime * updatesPerFrame) % totalFrames;
        }

        public static Point FramePos(int row, int col, int frameWidth, int frameHeight) {
            return new Point(col * frameWidth, row * frameHeight);
        }

        public static Point Animate(int totalFrames, int framesPerRow, int frameWidth, int frameHeight, int updatesPerFrame) {
            var currentFrame = Frame(totalFrames, updatesPerFrame);
            var col          = Column(currentFrame, framesPerRow);
            var row          = Row(currentFrame, framesPerRow);
            return FramePos(row, col, frameWidth, frameHeight);
        }
    }
}
