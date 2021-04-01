using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using GameProject.Code.Core.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameProject.Code.Core.Components {
    public class ParticleSystem : Component, IGameDrawable {
        public ParticleSystem(GameObject attached) : base(attached) {
            Material = new Material();
        }

        public Material Material { get; set; }
        private Particle[] particles;


        #region Rendering Information

        [AnimatableValue]
        public Texture2D Sprite {
            get => Material.Texture as Texture2D;
            set { if(Material != null) Material.Texture = value; }
        }

        public int DrawLayer {
            get { return _drawLayer; }
            set {
                _drawLayer = value;
                _realDrawOrder = (_drawLayer * 10000 + _orderInLayer) / 500000f;
            }
        }

        public int OrderInLayer {
            get { return _orderInLayer; }
            set {
                _orderInLayer = value;
                _realDrawOrder = (_drawLayer * 10000 + _orderInLayer) / 500000f;
            }
        }

        private int _drawLayer = 0;
        private int _orderInLayer = 0;
        private float _realDrawOrder = 0;

        #endregion





        public override void Draw(SpriteBatch sb) {
            foreach(Particle p in particles) {
                if(p.ParticleType == ParticleType.Sprite) {
                    //sprite draw
                    sb.Draw(Sprite,
                            transform.Position.ToVector2() + p.Position.ToVector2(),
                            null,
                            p.Color,
                            transform.Rotation_Rads + p.Rotation,
                            new Vector2(Sprite.Width / 2f, Sprite.Height / 2f),
                            transform.Scale.ToVector2().FlipY() * p.StartSize,
                            SpriteEffects.None,
                            _realDrawOrder);

                } else if(p.ParticleType == ParticleType.Model) {
                    //model draw
                }
            }
        }
    }
}