// Input.cs - Nick Monaco

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GameProject.Code.Core {

    /// <summary>
    /// Handles all input detection, providing event actions for when certain buttons are pressed or released.
    /// </summary>
    public static class Input {

        private static Action _emptyAction = () => { };

        // State variables
        private static KeyboardState _keyboardState;
        private static KeyboardState _lastKeyboardState;

        private static MouseState _mouseState;
        private static MouseState _lastMouseState;
        // End state variables

        // Controls
        public static Keys Movement_Up = Keys.W;
        public static Keys Movement_Down = Keys.S;
        public static Keys Movement_Left = Keys.A;
        public static Keys Movement_Right = Keys.D;

        public static Keys Aim_Up = Keys.Up;
        public static Keys Aim_Down = Keys.Down;
        public static Keys Aim_Left = Keys.Left;
        public static Keys Aim_Right = Keys.Right;

        public static Keys Act_Shoot = Keys.Space;
        public static Keys Act_Interact = Keys.E;
        public static Keys Act_Special = Keys.Q;
        public static Keys Act_Shift = Keys.LeftShift;

        public static Keys UI_Escape = Keys.Escape;
        public static Keys UI_Tab = Keys.Tab;

        public static Keys Util_Reset = Keys.R;
        // End controls

        // Direct input properties
        private static Vector2 _movementDirection;
        public static Vector2 MovementDirection {
            get { return _movementDirection; }
            private set { _movementDirection = value; }
        }

        private static Vector2 _aimDirection;
        public static Vector2 AimDirection {
            get { return _aimDirection; }
            private set { _aimDirection = value; }
        }

        private static Vector2 _mousePosition;
        public static Vector2 MousePosition {
            get { return _mousePosition; }
            private set { _mousePosition = value; }
        }

        public static Vector2 MouseWorldPosition { get {
            Matrix transformMat = GameManager.ViewMatrix *
                                  Matrix.CreateScale(1, -1, 1) *
                                  Matrix.CreateTranslation(GameManager.Resolution.X / 2, GameManager.Resolution.Y / 2, 0);

            return Vector3.Transform(MousePosition.ToVector3(), Matrix.Invert(transformMat)).ToVector2(); } 
        }
        // End direct input properties

        public static Vector2 ScreenPointToWorld(Vector2 screenPos) {
            Matrix transformMat = GameManager.ViewMatrix *
                                  Matrix.CreateScale(1, -1, 1) *
                                  Matrix.CreateTranslation(GameManager.Resolution.X / 2, GameManager.Resolution.Y / 2, 0);

            return Vector3.Transform(screenPos.ToVector3(), Matrix.Invert(transformMat)).ToVector2();
        }



        public static void Start() {
            _lastKeyboardState = Keyboard.GetState();
            _keyboardState = Keyboard.GetState();

            _movementDirection = Vector2.Zero;
            _aimDirection = Vector2.Zero;
            _mousePosition = Vector2.Zero;
        }

        public static void Update() {
            // State retrieval
            _lastKeyboardState = _keyboardState;
            _keyboardState = Keyboard.GetState();

            _lastMouseState = _mouseState;
            _mouseState = Mouse.GetState();
            // End state retrieval

            //---

            // Setting variables
            _movementDirection = new Vector2(_keyboardState.IsKeyDown(Movement_Left) ? -1 : (_keyboardState.IsKeyDown(Movement_Right) ? 1 : 0),
                                             _keyboardState.IsKeyDown(Movement_Down) ? -1 : (_keyboardState.IsKeyDown(Movement_Up) ? 1 : 0));

            _aimDirection = new Vector2(_keyboardState.IsKeyDown(Aim_Left) ? -1 : (_keyboardState.IsKeyDown(Aim_Right) ? 1 : 0),
                                        _keyboardState.IsKeyDown(Aim_Down) ? -1 : (_keyboardState.IsKeyDown(Aim_Up) ? 1 : 0));

            _mousePosition = new Vector2(_mouseState.X, _mouseState.Y);
            // End setting variables

            //---

            // Checking for event handlers
            bool localVal, lastLocalVal;

            // Shoot
            // Change this based on settings later
            //localVal = _keyboardState.IsKeyDown(Act_Shoot);
            //lastLocalVal = _lastKeyboardState.IsKeyDown(Act_Shoot);
            //if (localVal && !lastLocalVal) {
            //    OnShoot_Down();
            //} else if (!localVal && lastLocalVal) {
            //    OnShoot_Released();
            //}

            localVal = _mouseState.LeftButton == ButtonState.Pressed;
            lastLocalVal = _lastMouseState.LeftButton == ButtonState.Released;
            if (localVal && !lastLocalVal) {
                OnShoot_Down();
            } else if (!localVal && lastLocalVal) {
                OnShoot_Released();
            }

            // Interact
            localVal = _keyboardState.IsKeyDown(Act_Interact);
            lastLocalVal = _lastKeyboardState.IsKeyDown(Act_Interact);
            if (localVal && !lastLocalVal) {
                OnInteract_Down();
            } else if (!localVal && lastLocalVal) {
                OnInteract_Released();
            }

            // Special
            localVal = _keyboardState.IsKeyDown(Act_Special);
            lastLocalVal = _lastKeyboardState.IsKeyDown(Act_Special);
            if (localVal && !lastLocalVal) {
                OnSpecial_Down();
            } else if (!localVal && lastLocalVal) {
                OnSpecial_Released();
            }

            // Shift
            localVal = _keyboardState.IsKeyDown(Act_Shift);
            lastLocalVal = _lastKeyboardState.IsKeyDown(Act_Shift);
            if (localVal && !lastLocalVal) {
                OnShift_Down();
            } else if (!localVal && lastLocalVal) {
                OnShift_Released();
            }

            // Escape
            localVal = _keyboardState.IsKeyDown(UI_Escape);
            lastLocalVal = _lastKeyboardState.IsKeyDown(UI_Escape);
            if (localVal && !lastLocalVal) {
                OnEscape_Down();
            } else if (!localVal && lastLocalVal) {
                OnEscape_Released();
            }

            // Tab
            localVal = _keyboardState.IsKeyDown(UI_Tab);
            lastLocalVal = _lastKeyboardState.IsKeyDown(UI_Tab);
            if (localVal && !lastLocalVal) {
                OnTab_Down();
            } else if (!localVal && lastLocalVal) {
                OnTab_Released();
            }

            // Reset
            localVal = _keyboardState.IsKeyDown(Util_Reset);
            lastLocalVal = _lastKeyboardState.IsKeyDown(Util_Reset);
            if (localVal && !lastLocalVal) {
                OnReset_Down();
            } else if (!localVal && lastLocalVal) {
                OnReset_Released();
            }

            // Mouse Left
            localVal = _mouseState.LeftButton == ButtonState.Pressed;
            lastLocalVal = _lastMouseState.LeftButton == ButtonState.Pressed;
            if (localVal && !lastLocalVal) {
                OnMouseLeft_Down();
            } else if (!localVal && lastLocalVal) {
                OnMouseLeft_Released();
            }

            // Mouse Right
            localVal = _mouseState.RightButton == ButtonState.Pressed;
            lastLocalVal = _lastMouseState.RightButton == ButtonState.Pressed;
            if (localVal && !lastLocalVal) {
                OnMouseRight_Down();
            } else if (!localVal && lastLocalVal) {
                OnMouseRight_Released();
            }

            // End checking for event handlers



            // Fullscreen detection
            localVal = _keyboardState.IsKeyDown(Keys.F);
            lastLocalVal = _lastKeyboardState.IsKeyDown(Keys.F);
            if(localVal && !lastLocalVal) {
                OnFullscreenToggle();
            }
        }



        // Event handler methods

        public static Action OnShoot_Down = _emptyAction;
        public static Action OnShoot_Released = _emptyAction;
        public static Action OnInteract_Down = _emptyAction;
        public static Action OnInteract_Released = _emptyAction;
        public static Action OnSpecial_Down = _emptyAction;
        public static Action OnSpecial_Released = _emptyAction;
        public static Action OnShift_Down = _emptyAction;
        public static Action OnShift_Released = _emptyAction;

        public static Action OnEscape_Down = _emptyAction;
        public static Action OnEscape_Released = _emptyAction;
        public static Action OnTab_Down = _emptyAction;
        public static Action OnTab_Released = _emptyAction;

        public static Action OnReset_Down = _emptyAction;
        public static Action OnReset_Released = _emptyAction;

        public static Action OnMouseLeft_Down = _emptyAction;
        public static Action OnMouseLeft_Released = _emptyAction;
        public static Action OnMouseRight_Down = _emptyAction;
        public static Action OnMouseRight_Released = _emptyAction;

        public static Action OnFullscreenToggle = _emptyAction;

        // End event handler methods
    }
}
