// Input.cs - Nick Monaco

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GameProject.Code.Core {
    public static class Input {

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
        // End direct input properties



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

            // Shoot
            bool localVal = _keyboardState.IsKeyDown(Act_Shoot);
            bool lastLocalVal = _lastKeyboardState.IsKeyDown(Act_Shoot);
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
        }



        // Event handler methods

        public static void OnShoot_Down() { }
        public static void OnShoot_Released() { }
        public static void OnInteract_Down() { }
        public static void OnInteract_Released() { }
        public static void OnSpecial_Down() { }
        public static void OnSpecial_Released() { }
        public static void OnShift_Down() { }
        public static void OnShift_Released() { }

        public static void OnEscape_Down() { }
        public static void OnEscape_Released() { }
        public static void OnTab_Down() { }
        public static void OnTab_Released() { }

        public static void OnMouseLeft_Down() { }
        public static void OnMouseLeft_Released() { }
        public static void OnMouseRight_Down() { }
        public static void OnMouseRight_Released() { }

        // End event handler methods
    }
}
