using System;
using System.Collections.Generic;
using System.Text;
using GameProject.Code.Core.Components;
using Microsoft.Xna.Framework;

namespace GameProject.Code.Core.Particles {
    public class SizeOverLifetimeModule : IParticleModule {
        public bool Enabled { get; set; }
        public ParticleSystem AttachedSystem { get; set; }

        public List<(float, Vector3)> Gradient;


        public void Initialize() {
            Enabled = false;

            Gradient = new List<(float, Vector3)>() { (1, new Vector3(1, 1, 1)) };
        }


        public void UpdateParticle(Particle p) {
            if (!Enabled) return;

            if (Gradient.Count == 1) {
                p.Scale3D = Gradient[0].Item2;
            } else {
                int currentMinIndex = 0;
                for (int i = 0; i < Gradient.Count - 1; i++) {
                    if (p.LifetimeRatio >= Gradient[i].Item1 && p.LifetimeRatio < Gradient[i + 1].Item1) {
                        currentMinIndex = i;
                        break;
                    }
                }

                float min = Gradient[currentMinIndex].Item1;
                float max = Gradient[currentMinIndex + 1].Item1;
                float ratio = (max - p.LifetimeRatio) / (max - min);

                p.Scale3D = Vector3.Lerp(Gradient[currentMinIndex + 1].Item2, Gradient[currentMinIndex].Item2, ratio);
            }
        }
    }
}
