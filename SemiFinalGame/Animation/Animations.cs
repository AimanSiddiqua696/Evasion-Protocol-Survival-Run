using SemiFinalGame.Entities;
using SemiFinalGame.Interfaces;
using System;
using System.Drawing;

namespace SemiFinalGame.Animation
{
    public class AnimationSystem : IAnimation
    {
        private Dictionary<string, List<Image>> animations;
        private string current;
        private int frame;
        private float timer;
        private float frameDuration = 0.15f;
        private bool shouldReset = false;

        public AnimationSystem(Dictionary<string, List<Image>> animations, string startState)
        {
            this.animations = animations;
            current = startState;
            frame = 0;
            timer = 0;
            frameDuration = 0.08f;
        }
        public void SetSpeed(float duration)
        {
            frameDuration = duration;
        }

        public void Update(GameTime time)
        {
            timer += time.DeltaTime;

            if (timer >= frameDuration)
            {
                // Reset animation if requested
                if (shouldReset)
                {
                    frame = 0;
                    shouldReset = false;
                }
                else
                {
                    frame = (frame + 1) % animations[current].Count;
                }
                timer = 0;
            }
        }

        public Image GetCurrentFrame()
        {
            if (animations.ContainsKey(current) && animations[current].Count > 0)
            {
                frame = Math.Min(frame, animations[current].Count - 1); // safety
                return animations[current][frame];
            }

            return animations[current].FirstOrDefault();

        }

        public void Play(string name)
        {

            if (current == name) return;
            {
                current = name;
                frame = 0;
                timer = 0;
                shouldReset = false;
            }
        }
    }
}


