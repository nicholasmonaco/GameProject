// Transform.cs - Nick Monaco

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using GameProject.Code.Core.Components;

namespace GameProject.Code.Core {
    
    /// <summary>
    /// How position, scale, and rotation are stored and handled.
    /// Closely intwined with GameObjects and can be controlled by a rigidbody.
    /// </summary>
    public class Transform : Component {

        public Action ViewChangeAction = () => { };
        public void ViewChangeAction_Camera() {
            //ViewMatrix = Matrix.CreateLookAt(_worldMatrix.Translation, _worldMatrix.Forward, _worldMatrix.Up);
            ViewMatrix = Matrix.CreateTranslation(-_worldPosition) *
                         Matrix.CreateScale(GameManager.MainCamera.Size, GameManager.MainCamera.Size, 1) * //Note: this is split up to preserve the layerDepth with sb.Draw()
                         Matrix.CreateTranslation(-GameManager.ViewOffset) * 
                         Matrix.CreateRotationZ(-0) * 
                         Matrix.CreateTranslation(GameManager.ViewOffset);
        }



        public Vector3 LocalPosition {
            get { return _localPosition; }
            set {
                if (Parent == null) { 
                    _worldPosition = value;
                } else {
                    _localPosition = value;
                    _worldPosition = ParentPos + _localPosition;    
                }

                ViewChangeAction();
                RecalculateWorldMatrix();
                gameObject.rigidbody2D?.ResetPosition();
            }
        }

        //public Quaternion LocalRotation {
        //    get { return _worldMatrix.Rotation(); }
        //    set {
        //        _worldMatrix = Matrix.CreateFromQuaternion(value * Quaternion.Conjugate(_worldMatrix.Rotation())) * _worldMatrix;
        //    }
        //}

        //public Vector3 LocalScale {
        //    get { return _worldMatrix.Scale(); }
        //    set {
        //        // First, find the difference between current scale and new scale (by comparing the values used above)
        //        // Then change it by that much
        //        //ReScale(diff);

        //        //Debug.Log($"Scale Pre: ({value.X}, {value.Y}, {value.Z})");
        //        _worldMatrix = Matrix.Invert(Matrix.CreateScale(_worldMatrix.Scale())) * _worldMatrix;
        //        _worldMatrix = Matrix.CreateScale(/*value - _transformMatrix.Scale()*/value) * _worldMatrix; //so whats happening is this operation is setting it to 0 somehow
        //        //Debug.Log($"Scale Post: ({LocalScale.X}, {LocalScale.Y}, {LocalScale.Z})");
        //    }
        //}

        public Vector3 Position {
            get { return _worldPosition; }
            set {
                //_worldPosition = value;
                LocalPosition = value - ParentPos;

                //ViewChangeAction(); //These 3 things are handled in LocalPosition
                //RecalculateWorldMatrix();
                //gameObject.rigidbody2D?.ResetPosition();
            }
        }

        public float Rotation {
            get { return _worldRotation; }
            set {
                ////first, we need to find the quaternion representing the difference between the current quaternion and the new one
                //Quaternion diff = value * Quaternion.Conjugate(_worldRotation);
                ////then, we move the world rotation by that quaternion
                //Forward = Vector3.Transform(Forward, diff);
                //Up = Vector3.Transform(Up, diff);

                _worldRotationRad = value * MathEx.Deg2Rad;
                _worldRotation = value;
                ViewChangeAction();
                RecalculateWorldMatrix();
            }
        }

        public float Rotation_Rads => _worldRotationRad;

        public Vector3 Scale {
            get { return _worldScale; }
            set {
                ////first, we need to undo the current scale
                //_worldMatrix = Matrix.CreateWorld(Position, Forward, Up); // By resetting it to a new world matrix, the scale is removed, as position is unaffected by scale
                ////then, we need to apply the new scale
                //_worldMatrix = Matrix.CreateScale(value) * _worldMatrix;

                _worldScale = value;
                RecalculateWorldMatrix();
            }
        }


        //public Vector3 Right => Vector3.Transform(Vector3.Right, _worldMatrix.Rotation());
        //public Vector3 Up => Vector3.Transform(Vector3.Up, _worldMatrix.Rotation());
        //public Vector3 Forward => Vector3.Transform(Vector3.Forward, _worldMatrix.Rotation());

        //public Vector3 Forward { 
        //    get { return _worldMatrix.Forward; }
        //    set { 
        //        _worldMatrix = Matrix.CreateWorld(_worldMatrix.Translation, value, _up);
        //        RecreateWorldAndView();
        //    }
        //}

        //public Vector3 Up {
        //    get { return _up; }
        //    set {
        //        _up = value;
        //        _worldMatrix = Matrix.CreateWorld(_worldMatrix.Translation, _worldMatrix.Forward, _up);
        //        RecreateWorldAndView();
        //    }
        //}

        //public Vector3 LookAtDirection {
        //    get { return _worldMatrix.Forward; }
        //    set {
        //        _worldMatrix = Matrix.CreateWorld(_worldMatrix.Translation, value, _up);
        //        RecreateWorldAndView();
        //    }
        //}

        //public Matrix World {
        //    get { return _worldMatrix; }
        //    set {
        //        _worldMatrix = value;
        //        ViewChangeAction();
        //    }
        //}

        //private void RecreateWorldAndView() {
        //    _up = _worldMatrix.Up;

        //    _worldMatrix = Matrix.CreateWorld(_worldMatrix.Translation, _worldMatrix.Forward, _up);
        //    ViewChangeAction();
        //}

        private void RecalculateWorldMatrix() {
            WorldMatrix = Matrix.CreateScale(_worldScale) * 
                          Matrix.CreateRotationZ(_worldRotationRad) * 
                          Matrix.CreateTranslation(_worldPosition);

            WorldMatrixUpdateAction();
        }
        
        public Matrix RecalculateWorldMatrix_Renderer() {
            return Matrix.CreateTranslation(-RenderOffsets) * 
                   Matrix.CreateScale(_worldScale) * 
                   Matrix.CreateRotationZ(_worldRotationRad) * 
                   Matrix.CreateTranslation(RenderOffsets) * 
                   Matrix.CreateTranslation(_worldPosition);
        }


        // Variables
        public Transform Parent = null;
        public List<Transform> _children;

        private Vector3 ParentPos => Parent == null ? Vector3.Zero : Parent.Position;

        // This is representative of world space for this single transform
        public Matrix WorldMatrix { get; private set; } = Matrix.Identity;
        public Matrix ViewMatrix { get; private set; } = Matrix.Identity;

        //private Vector3 _up = Vector3.Up;//i think this stuff can be replaced with quaternions. do it


        // V3
        private Vector3 _worldPosition = Vector3.Zero; //World position is ParentPos + _localPos
        private float _worldRotation = 0;
        private float _worldRotationRad = 0;
        private Vector3 _worldScale = Vector3.One;

        private Vector3 _localPosition = Vector3.Zero; //_localPos is the distance from ParentPos

        public Vector3 RenderOffsets = Vector3.Zero;

        public Action WorldMatrixUpdateAction = () => { };


        // End variables




        // Constructors

        public Transform(GameObject attach) : base(attach) {
            // In this case, we can assume that the Transform will have no parents by default.

            // This matrix will represent where the Transform will be in world space. 
            // By default, it will be facing forward.
            transform = this;
            gameObject = attach;

            _children = new List<Transform>();

            Position = Vector3.Zero;
            Rotation = 0;
            Scale = Vector3.One;

            //WorldMatrixUpdateAction = RecalculateWorldMatrix;
            RecalculateWorldMatrix();
        }

        public Transform(GameObject attach, Transform parent) : base(attach) {
            transform = this;
            gameObject = attach;

            Parent = parent;
            _children = new List<Transform>();

            Position = Vector3.Zero;
            Rotation = 0;
            Scale = Vector3.One;

            //WorldMatrixUpdateAction = RecalculateWorldMatrix;
            RecalculateWorldMatrix();
        }

        public Transform(GameObject attach, GameObject parent) : this(attach, parent.transform) { }

        public Transform(GameObject attach, Vector3 position, float rotation, Vector3 scale) : this(attach) {
            Position = position;
            Rotation = rotation;
            Scale = scale;
        }

        public Transform(GameObject attach, Transform parent, Vector3 position, float rotation, Vector3 scale) : this(attach, parent) {
            Position = position;
            Rotation = rotation;
            Scale = scale;
        }



        public override void OnDestroy() {
            foreach(Transform t in _children) {
                GameObject.Destroy(t.gameObject);
            }
        }




        // Transforms from local space to world space
        public Vector3 TransformPoint(Vector3 point) {
            return ParentPos + point;
        }

        // Transforms from world space to local space
        public Vector3 InverseTransformPoint(Vector3 point) {

            return point - ParentPos;
        }

        // Transforms from local space to world space
        public Quaternion TransformRotation(Quaternion localRotation) {
            Transform t = this;
            Quaternion world = localRotation;
            while(t.Parent != null) {
                world = t.WorldMatrix.Rotation() * world;
            }

            return world;
        }

        // Transforms from local space to world space
        public Vector3 TransformScale(Vector3 point) {
            Transform t = this;
            Vector3 pos = point;
            while (t.Parent != null) {
                Matrix m = Matrix.CreateScale(pos);
                m = Matrix.CreateScale(t.WorldMatrix.Scale()) * m;

                pos = m.Scale();

                t = t.Parent;
            }

            return pos;
        }

        // Transforms from world space to local space
        public Quaternion InverseTransformRotation(Quaternion worldRotation) {
            Transform t = this;
            Quaternion local = worldRotation;
            while (t.Parent != null) {
                //local = t._rotation.Conjugate * local;
                local = Quaternion.Multiply(Quaternion.Conjugate(t.WorldMatrix.Rotation()), local);
            }

            return local;
        }


        
    }
}
