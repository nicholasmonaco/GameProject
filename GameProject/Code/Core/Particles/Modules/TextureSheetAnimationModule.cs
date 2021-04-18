using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using GameProject.Code.Core.Components;

namespace GameProject.Code.Core.Particles {
    public class TextureSheetAnimationModule : IParticleModule {

        public bool Enabled { get; set; }
        public ParticleSystem AttachedSystem { get; set; }

        public Point Tiles { get; set; } = new Point(1, 1);
        public Point TileSize { get; private set; }
        public List<(float, Point)> Gradient;


        public void Initialize() {
            Enabled = false;

            TileSize = AttachedSystem.Sprite.Bounds.Size / Tiles;

            Gradient = new List<(float, Point)>() {
                (1, Point.Zero)
            };
        }

        public void SetFramesEvenly_X() {
            TileSize = AttachedSystem.Sprite.Bounds.Size / Tiles;

            float distance = 1f / Tiles.X;
            float curDist = 0;

            Gradient = new List<(float, Point)>(Tiles.X);
            for(int i = 0; i < Tiles.X; i++) {
                Gradient.Add((curDist, new Point(TileSize.X * i, 0)));
                curDist += distance;
            }
        }

        public Rectangle? GetCurrentSpriteRect(float fraction) {
            if (Gradient.Count == 1) return null;

            for(int i = 0; i < Gradient.Count-1; i++) {
                (float, Point) data = Gradient[i];
                if(fraction >= data.Item1 && fraction < Gradient[i+1].Item1) {
                    return new Rectangle(data.Item2, TileSize);
                }
            }

            (float, Point) dataFinal = Gradient[Gradient.Count - 1];
            if (fraction >= dataFinal.Item1) {
                return new Rectangle(dataFinal.Item2, TileSize);
            }

            return null;
        } 

    }
}
