// Renderer.cs - Nick Monaco

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameProject.Code.Core.Components {
    
    /// <summary>
    /// Governs all renderable components.
    /// No other type of component should be drawing anything if it isn't a subclass of Renderer.
    /// </summary>
    public class Renderer : Component, IGameDrawable {
        public Renderer(GameObject attached) : base(attached) {
            Material = new Material();
        }

        public Material Material { get; set; }

        public bool IsInCamera(Camera camera) {
            return false;
        }
    }
}
