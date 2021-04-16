using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameProject.Code.Core {
    public class Material {
        public static int ShaderCount = 12; // This has to be hardcoded or read in from a file or something.


        public Material() { }

        public Material(IGameDrawable attachedDrawable) {
            AttachedDrawable = attachedDrawable;
        }



        public IGameDrawable AttachedDrawable;

        private BatchID _batchID = BatchID.BehindEntities;
        public BatchID BatchID {
            get => _batchID;
            set {
                _batchID = value;
                if (value == BatchID.NonAuto) {
                    NonAutoIndex = SolveIndex(AttachedDrawable.DrawLayer, AttachedDrawable.OrderInLayer);
                }
            }
        }


        public Effect Shader = Resources.Effect_Base;
        public float NonAutoIndex = 0;
        public Texture Texture = Resources.Sprite_Pixel;
        public Color Color = Color.White;


        public static float SolveIndex(int drawLayer, int orderInLayer) {
            return (drawLayer * 10000 + orderInLayer) / 500000f;
        }
    }


    public enum BatchID {
        NonAuto = -1,

        Background = 0,
        Room = 1,
        BehindEntities = 2,
        Entities = 3,
        BehindPlayer = 4,
        Player = 5,
        AbovePlayer = 6,
        OverlayFX = 7,
        UnderHUD = 8,
        HUD = 9,
        AboveAll = 10,
        TotalOverlay = 11


        //background
        //room
        //behind entities
        //entities
        //above entities
        //player
        //above player
        //overlayeffects
        //under hud
        //hud
        //above all
        //total overlay
    }
}
