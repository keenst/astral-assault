using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace AstralAssault.Source.Graphics
{
    internal static class AnimationCreator
    {
        public static Animation CreateAnimFromSpriteSheet(
            int spriteWidth,
            int spriteHeight,
            int startFrameIdx,
            int rowLenghtInFrames,
            int numFrames,
            int[] time,
            bool hasRotation,
            bool shouldLoop,
            bool goBackwards)
        {
            Animation anim = new Animation();

            anim.HasRotation = hasRotation;
            anim.IsLooping = shouldLoop;

            var frames = new List<Frame>();

            if (!hasRotation)
            {
                int currentFrameIdx = startFrameIdx;

                for (int i = 0; i < numFrames; i++)
                {
                    int x = (currentFrameIdx % rowLenghtInFrames) * spriteWidth;
                    int y = (currentFrameIdx / rowLenghtInFrames) * spriteHeight;

                    Frame frame = new Frame(new Rectangle(x, y, spriteWidth, spriteHeight), time[i]);

                    frames.Add(frame);

                    if (goBackwards)
                    {
                        currentFrameIdx--;
                    }
                    else
                    {
                        currentFrameIdx++;
                    }
                }
            }
            else
            {
                for (int i = 0; i < numFrames; i++)
                {
                    int x = ((i + startFrameIdx) % rowLenghtInFrames) * spriteWidth;
                    int y = ((i + startFrameIdx) / rowLenghtInFrames) * spriteHeight;

                    Frame frame = new Frame
                    (
                        new Rectangle(x, y, spriteWidth, spriteHeight),
                        new Rectangle(x, y + spriteHeight, spriteWidth, spriteHeight),
                        new Rectangle(x, y + spriteHeight * 2, spriteWidth, spriteHeight),
                        new Rectangle(x, y + spriteHeight * 3, spriteWidth, spriteHeight),
                        (time.Length > 1)
                            ? time[i]
                            : time[0]
                    );

                    frames.Add(frame);
                }
            }

            anim.Frames = frames.ToArray();

            return anim;
        }
    }
}
