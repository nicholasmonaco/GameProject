using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using GameProject.Code.Core;
using GameProject.Code.Scripts.Components.UI;
using GameProject.Code.Core.UI;

namespace GameProject.Code.Prefabs {
    public class Prefab_Panel : GameObject {

        public Prefab_Panel() : base() {
            Name = "Panel";

            //transform.Parent = GameManager.MainCanvas.transform;
            //UpdatePosition();
            //GameManager.MainCanvas.ExtentsUpdate += UpdatePosition;

            transform.UIParentFlag = true;

            AddComponent<Panel>();
        }

        //private void UpdatePosition() {
        //    transform.LocalPosition = new Vector3(-GameManager.MainCanvas.Extents.X,
        //                                          GameManager.MainCanvas.Extents.Y,
        //                                          0);
        //}
    }
}
