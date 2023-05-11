﻿#region
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TheGameOfDoomHmmm.Source.Entity.Components;
#endregion

namespace TheGameOfDoomHmmm.Source.Graphics;

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
        bool goBackwards,
        bool reverseDir,
        int reverseAtIdx
    )
    {
        Animation anim = new Animation
        {
            HasRotation = hasRotation,
            IsLooping = shouldLoop
        };

        List<Frame> frames = new List<Frame>();

        if (!hasRotation)
        {
            int currentFrameIdx = startFrameIdx;

            for (int i = 0; i < numFrames; i++)
            {
                int x = currentFrameIdx % rowLenghtInFrames * spriteWidth;
                int y = currentFrameIdx / rowLenghtInFrames * spriteHeight;

                Frame frame = new Frame
                (
                    new Rectangle(x, y, spriteWidth, spriteHeight), time.Length > 1
                        ? time[i]
                        : time[0]
                );

                frames.Add(frame);

                if (reverseDir && (i == reverseAtIdx)) goBackwards = !goBackwards;

                if (goBackwards) currentFrameIdx--;
                else currentFrameIdx++;
            }
        }
        else
        {
            for (int i = 0; i < numFrames; i++)
            {
                int x = (i + startFrameIdx) % rowLenghtInFrames * spriteWidth;
                int y = (i + startFrameIdx) / rowLenghtInFrames * spriteHeight;

                Frame frame = new Frame
                (
                    new Rectangle(x, y, spriteWidth, spriteHeight),
                    new Rectangle(x, y + spriteHeight, spriteWidth, spriteHeight),
                    new Rectangle(x, y + spriteHeight * 2, spriteWidth, spriteHeight),
                    new Rectangle(x, y + spriteHeight * 3, spriteWidth, spriteHeight),
                    time.Length > 1
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