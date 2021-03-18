using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameProject.Code.Core.UI {
    public class ValueSlider : UI_LayoutItem {
        public ValueSlider(GameObject attached, float startValue, float minValue, float maxValue) : base(attached) {
            MinValue = minValue;
            MaxValue = maxValue;
            Value = startValue;

            RefreshBounds();
            SetAdjustActions();

            transform.WorldMatrixUpdateAction += RefreshBounds;
        }

        public ValueSlider(GameObject attached, float startValue) : base(attached) {
            Value = startValue;

            RefreshBounds();
            SetAdjustActions();

            transform.WorldMatrixUpdateAction += RefreshBounds;
        }

        public ValueSlider(GameObject attached) : this(attached, 0.5f) { }



        public float MinValue { get; private set; } = 0;
        public float MaxValue { get; private set; } = 1;

        private float _value;
        public float Value { 
            get => _value;
            private set {
                _value = MathHelper.Clamp(value, MinValue, MaxValue);
                OnValueChanged(_value);
            } 
        }


        private Action<float> OnValueChanged = (value) => { };
        public float ShiftAmount = 12;

        public void SetOnValueChangedAction(Action<float> action) {
            OnValueChanged = action;
            action(_value);
        }

        private void SetAdjustActions() {
            ExtraSelectAction = () => {
                Input.OnAnyLeftDown += OnLeftDown;
                Input.OnAnyRightDown += OnRightDown;
            };

            ExtraDeselectAction = () => {
                Input.OnAnyLeftDown -= OnLeftDown;
                Input.OnAnyRightDown -= OnRightDown;
            };
        }



        public Transform Handle;
        public SpriteRenderer HandleRenderer;
        public SpriteRenderer BackgroundRenderer;

        private float _minX, _maxX;


        public void RefreshBounds() {
            Vector2 size = new Vector2(120, 5);

            _minX = transform.Position.X - size.X / 2f;
            _maxX = transform.Position.X + size.X / 2f;
        }

        public void SetBackgroundColor(Color newColor) {
            BackgroundRenderer.Color = newColor;
        }

        public void SetHandleColor(Color newColor) {
            HandleRenderer.Color = newColor;
        }
        



        private void ShiftValue(float amount) {
            float x = MathHelper.Clamp(Handle.Position.X + amount, _minX, _maxX);

            Handle.Position = new Vector3(x, Handle.Position.Y, 0);
            Value = (x - _minX) / (_maxX - _minX);

            Resources.Sound_Menu_Move.Play(GameManager.RealSoundVolume);
        }


        public void ForceSetHandle(Transform handle) {
            Handle = handle;
            Handle.Position = new Vector3(transform.Position.X, transform.Position.Y, 0);

            float x = MathHelper.Clamp(Handle.Position.X, _minX, _maxX);
            Value = (x - _minX) / (_maxX - _minX);
        }


        private void OnLeftDown() {
            ShiftValue(-ShiftAmount);
        }

        private void OnRightDown() {
            ShiftValue(ShiftAmount);
        }


        public override void OnDestroy() {
            base.OnDestroy();

            transform.WorldMatrixUpdateAction -= RefreshBounds;
        }

    }
}