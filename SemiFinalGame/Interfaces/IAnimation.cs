using SemiFinalGame.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemiFinalGame.Interfaces
{
    public interface IAnimation
    {
        void Update(GameTime gameTime);
        void Play(string AnimationState);
        Image GetCurrentFrame();
    }
}
