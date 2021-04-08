using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using GameProject.Code.Core.Components;

namespace GameProject.Code.Core.Particles {
    public class ColorOverLifetimeModule : IParticleModule {
        public bool Enabled { get; set; }
        public ParticleSystem AttachedSystem { get; set; }


        public List<(float, Color)> Gradient;


        public void Initialize() {
            Enabled = false;

            Gradient = new List<(float, Color)>() {
                (1, Color.White)
            };
        }


        public void UpdateParticle(Particle p) {
            if (!Enabled) return;

            if(Gradient.Count == 1) {
                p.Color = Gradient[0].Item2;
            } else {

                int currentMinIndex = 0;
                for(int i = 0; i < Gradient.Count-1; i++) {
                    if(p.LifetimeRatio >= Gradient[i].Item1 && p.LifetimeRatio < Gradient[i+1].Item1) {
                        currentMinIndex = i;
                        break;
                    }
                }


                float min = Gradient[currentMinIndex].Item1;
                float max = Gradient[currentMinIndex + 1].Item1;
                float ratio = (max - p.LifetimeRatio) / (max - min);

                p.Color = Color.Lerp(Gradient[currentMinIndex + 1].Item2, Gradient[currentMinIndex].Item2, ratio);
            }
        }
    }
}
