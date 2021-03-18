using Microsoft.Xna.Framework;
using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using GameProject.Code.Core.UI;
using GameProject.Code.Scripts.Components;

namespace GameProject.Code.Prefabs {
    public class Prefab_Canvas : GameObject {
    
        public Prefab_Canvas() : base() {
            Name = "Canvas";

            AddComponent<Canvas>();

            transform.Parent = Camera.main.transform;
            transform.LocalPosition = Vector3.Zero;
            transform.Scale = Vector3.One;
        }

    }
}
