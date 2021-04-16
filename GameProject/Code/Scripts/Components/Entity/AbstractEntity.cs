using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using GameProject.Code.Scripts.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameProject.Code.Scripts.Components.Entity {
    public abstract class AbstractEntity : Component {
        private static Action EmptyAction = () => { };
        
        public AbstractEntity(GameObject attached, EntityID id) : base(attached) {
            ID = id;
        }

        public EntityID ID { get; protected set; } = EntityID.None;

        protected bool _dead = false;
        public Action OnDeathFlag = EmptyAction;
        public Action ExtraOnDestroy = EmptyAction;


        public override void OnDestroy() {
            ExtraOnDestroy();
            base.OnDestroy();
        }



        public static GameObject GetEntityFromID(EntityID id) {
            //todo
            return new GameObject();
        }

    }
}