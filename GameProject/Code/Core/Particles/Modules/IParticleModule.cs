using System;
using System.Collections.Generic;
using System.Text;
using GameProject.Code.Core.Components;

namespace GameProject.Code.Core.Particles {
    public interface IParticleModule {
        public bool Enabled { get; set; }
        public ParticleSystem AttachedSystem { get; set; }

        public void Initialize();
    }
}
