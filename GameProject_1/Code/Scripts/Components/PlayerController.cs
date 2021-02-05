using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using GameProject.Code.Core;
using GameProject.Code.Core.Components;

namespace GameProject.Code.Scripts.Components {
    public class PlayerController : Component {

        // Components
        private Rigidbody2D _playerRB;
        // End components

        // Public values
        public bool CanMove = true;
        public bool FreezeMovement = false;
        // End public values

        // Private values
        private float _maxSpeed = 105;
        private float _acceleration = 800;
        private float _shotRate = 2;

        private Vector2 _moveVec = Vector2.Zero;
        private float _shootDelayTimer = 0;
        private bool _shooting = false;
        // End private values

        public PlayerController(GameObject attached) : base(attached) { }


        public override void PreAwake() {
            base.PreAwake();

            _playerRB = GetComponent<Rigidbody2D>();
            _playerRB.Drag = 5f;

            // Input
            Input.OnShoot_Down += OnShootDown;
            Input.OnShoot_Released += OnShootUp;
            // End input

            GameManager.Player = this;
        }



        public override void Update() {
            _moveVec = Input.MovementDirection;

            Shoot();
        }

        public override void FixedUpdate() {
            if (FreezeMovement) {
                _playerRB.Velocity = Vector2.Zero;
            } else {
                if (!CanMove) return;
                _playerRB.Velocity += _moveVec * _acceleration * Time.fixedDeltaTime;

                if (_playerRB.Velocity.Length() > _maxSpeed) {
                    _playerRB.Velocity += -_playerRB.Velocity * (_playerRB.Velocity.Length() - _maxSpeed) * Time.fixedDeltaTime;
                }
            }
        }


        private void Shoot() {
            if(_shootDelayTimer <= 0) {
                if (_shooting) {
                    ShootLogic();
                    _shootDelayTimer = 1 / _shotRate;
                }
            } else {
                _shootDelayTimer -= Time.deltaTime;
            }
        }

        private void ShootLogic() {
            Vector2 aimDir = Input.AimDirection;
            Debug.Log("Bullet spawned");
        }



        private void OnShootDown() {
            _shooting = true;
        }

        private void OnShootUp() {
            _shooting = false;
        }

    }
}
