using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameProject.Code.Scripts.Components.UI {
    public class UI_LayoutItem : Component {
        protected static Action EmptyAction = () => { };

        public UI_LayoutItem(GameObject attached) : base(attached) {
            //OrderIndex = GameManager.UILayoutMembers.Count;
            //GameManager.UILayoutMembers.Add(this);

            //GameManager.OnSelectIndexChange += SelectCheck;
        }

        public void ForceAddToUIList() {
            OrderIndex = GameManager.UILayoutMembers.Count;
            GameManager.UILayoutMembers.Add(this);

            GameManager.OnSelectIndexChange += SelectCheck;
        }

        public void ForceRemoveFromUIList() {
            GameManager.UILayoutMembers.Remove(this);

            GameManager.OnSelectIndexChange -= SelectCheck;

            Selected = false;
            OnDeselected();
            ExtraDeselectAction();
        }


        public int OrderIndex = 0;

        public bool Selected { get; private set; }
        public Action ExtraSelectAction = EmptyAction;
        public Action ExtraDeselectAction = EmptyAction;


        private void SelectCheck(int newIndex) {
            if(newIndex == OrderIndex && !Selected) {
                Selected = true;
                OnSelected();
                Input.OnAnyUpDown += MoveToPrevious;
                Input.OnAnyDownDown += MoveToNext;
                ExtraSelectAction();
            }else if (Selected) {
                Selected = false;
                OnDeselected();
                Input.OnAnyUpDown -= MoveToPrevious;
                Input.OnAnyDownDown -= MoveToNext;
                ExtraDeselectAction();
            }
        }

        public virtual void OnSelected() { }
        public virtual void OnDeselected() { }

        public Action OnActivate = EmptyAction;
        public void DoActivate() {
            if (Enabled) {
                OnActivate();
            }
        }

        private void MoveToPrevious() {
            if (Enabled && Selected) {
                int i = GameManager.CurrentUIIndex - 1;
                GameManager.CurrentUIIndex = i < 0 ? GameManager.UILayoutMembers.Count-1 : i;
                Resources.Sound_Menu_Move.Play(GameManager.RealSoundVolume);
            }
        }

        private void MoveToNext() {
            if (Enabled && Selected) {
                int i = GameManager.CurrentUIIndex + 1;
                GameManager.CurrentUIIndex = i >= GameManager.UILayoutMembers.Count ? 0 : i;
                Resources.Sound_Menu_Move.Play(GameManager.RealSoundVolume);
            }
        }



        public override void OnDestroy() {
            if (Selected) {
                Input.OnAnyUpDown -= MoveToPrevious;
                Input.OnAnyDownDown -= MoveToNext;
                ExtraDeselectAction();
            }

            base.OnDestroy();

            ExtraSelectAction = EmptyAction;
            ExtraDeselectAction = EmptyAction;
            OnActivate = EmptyAction;

            GameManager.OnSelectIndexChange -= SelectCheck;

            GameManager.UILayoutMembers.Remove(this);
        }

    }
}