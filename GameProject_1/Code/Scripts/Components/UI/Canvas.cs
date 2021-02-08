using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameProject.Code.Scripts.Components {
    public class Canvas : Component {
        public Canvas(GameObject attached) : base(attached) {
            GameManager.MainCanvas = this;
        }




        //public override void Start() {
        //    base.Start();
        //    transform.Parent = Camera.main.transform;
        //    transform.LocalPosition = Vector3.Zero;
        //}

        //public override void Update() {
            
        //    //Debug.Log($"Canvas position: {transform.Parent.Position}");
        //}
    }
}