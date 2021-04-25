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
    [AnimatableComponent]
    public class Transform : Component {

        private static Action EmptyAction = () => { };


        public Action ViewChangeAction = EmptyAction;
        public void ViewChangeAction_Camera() {
            //ViewMatrix = Matrix.CreateLookAt(_worldMatrix.Translation, _worldMatrix.Forward, _worldMatrix.Up);
            ViewMatrix = Matrix.CreateTranslation(-_worldPosition) *
                         Matrix.CreateScale(GameManager.MainCamera.Size, GameManager.MainCamera.Size, 1) * //Note: this is split up to preserve the layerDepth with sb.Draw()
                         Matrix.CreateTranslation(-GameManager.ViewOffset) * 
                         Matrix.CreateRotationZ(-0) * 
                         Matrix.CreateTranslation(GameManager.ViewOffset);
        }

        public bool UIParentFlag = false;





        [AnimatableValue]
        public virtual Vector3 LocalPosition {
            get { return _localPosition; }
            set {
                if (Parent == null) { 
                    _worldPosition = value;
                } else {
                    _localPosition = value;
                    _worldPosition = ParentPos + (_localPosition * ParentScale);    
                }

                ViewChangeAction();
                RecalculateWorldMatrix();
                gameObject.rigidbody2D?.ResetPosition();

                UpdateChildren();
            }
        }


        [AnimatableValue]
        public Vector3 Position {
            get { return _worldPosition; }
            set {
                //_worldPosition = value;
                LocalPosition = (value * ParentScale) - ParentPos;

                //ViewChangeAction(); //These 3 things are handled in LocalPosition
                //RecalculateWorldMatrix();
                //gameObject.rigidbody2D?.ResetPosition();
            }
        }



        [AnimatableValue]
        public Vector3 LocalScale {
            get { return _localScale; }
            set {
                if (Parent == null) {
                    _worldScale = value;
                } else {
                    _localScale = value;
                    _worldScale = ParentScale * _localScale;
                }

                ViewChangeAction();
                RecalculateWorldMatrix();
                gameObject.rigidbody2D?.ResetPosition();

                UpdateChildren();
            }
        }


        [AnimatableValue]
        public Vector3 Scale {
            get { return _worldScale; }
            set {
                LocalScale = value / ParentScale;

                //_worldScale = value;
                //RecalculateWorldMatrix();
            }
        }



        public Quaternion LocalRotation {
            get { return _localRotation; }
            set {
                if (Parent == null) {
                    _worldRotation = value;
                } else {
                    _localRotation = value;
                    _worldRotation = Quaternion.Concatenate(ParentRotation, _localRotation);
                }

                ViewChangeAction();
                RecalculateWorldMatrix();
                gameObject.rigidbody2D?.ResetPosition();

                UpdateChildren();
            }
        }


        public Quaternion Rotation {
            get { return _worldRotation; }
            set {
                // It's either this or the other way around.
                LocalRotation = Quaternion.Concatenate(value, Quaternion.Inverse(ParentRotation));
            }
        }



        [AnimatableValue]
        public float Rotation2D {
            get { return _worldRotation2D; }
            set {
                _worldRotationRad2D = value * MathEx.Deg2Rad;
                _worldRotation2D = value;

                Rotation = Quaternion.CreateFromAxisAngle(new Vector3(0, 0, 1), _worldRotationRad2D); //maybe the z has to be -1 instead
            }
        }

        public float Rotation_Rads2D => _worldRotationRad2D;
        




        protected virtual void RecalculateWorldMatrix() {
            //WorldMatrix = Matrix.CreateScale(_worldScale) * 
            //              Matrix.CreateRotationZ(_worldRotationRad2D) * 
            //              Matrix.CreateTranslation(_worldPosition);


            WorldMatrix = Matrix.CreateScale(_worldScale) *
                          Matrix.CreateFromQuaternion(_worldRotation) *
                          Matrix.CreateTranslation(_worldPosition);

            WorldMatrixUpdateAction();
        }

        public void UpdateChildren() {
            foreach(Transform t in _children) {
                t.LocalPosition = t.LocalPosition;
            }
        }



        // Variables
        private Transform _parent = null;
        public Transform Parent {
            get { return _parent; }
            set {
                _parent?._children.Remove(this);
                _parent = value;
                value?._children.Add(this);
            }
        }


        public List<Transform> _children;

        protected Vector3 ParentPos => Parent == null ? Vector3.Zero : Parent.Position;
        protected Vector3 ParentScale => Parent == null ? Vector3.One : Parent.Scale;
        protected Quaternion ParentRotation => Parent == null ? Quaternion.Identity : Parent.Rotation;



        // This is representative of world space for this single transform
        public Matrix WorldMatrix { get; protected set; } = Matrix.Identity;
        public Matrix ViewMatrix { get; private set; } = Matrix.Identity;



        // V3
        protected Vector3 _worldPosition = Vector3.Zero; //World position is ParentPos + _localPos
        protected Vector3 _localPosition = Vector3.Zero; //_localPos is the distance from ParentPos

        protected float _worldRotation2D = 0;
        protected float _worldRotationRad2D = 0;
        protected Quaternion _worldRotation;
        protected Quaternion _localRotation;

        protected Vector3 _worldScale = Vector3.One;
        protected Vector3 _localScale = Vector3.One;


        public Action WorldMatrixUpdateAction = () => { };

        // End variables




        #region Constructors

        public Transform(GameObject attach) : base(attach) {
            // In this case, we can assume that the Transform will have no parents by default.

            // This matrix will represent where the Transform will be in world space. 
            // By default, it will be facing forward.
            transform = this;
            gameObject = attach;

            _children = new List<Transform>();

            Position = Vector3.Zero;
            Rotation2D = 0;
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
            Rotation2D = 0;
            Scale = Vector3.One;

            //WorldMatrixUpdateAction = RecalculateWorldMatrix;
            RecalculateWorldMatrix();
        }

        public Transform(GameObject attach, GameObject parent) : this(attach, parent.transform) { }

        public Transform(GameObject attach, Vector3 position, float rotation, Vector3 scale) : this(attach) {
            Position = position;
            Rotation2D = rotation;
            Scale = scale;
        }

        public Transform(GameObject attach, Transform parent, Vector3 position, float rotation, Vector3 scale) : this(attach, parent) {
            Position = position;
            Rotation2D = rotation;
            Scale = scale;
        }

        #endregion




        public override void OnDestroy() {
            if(Parent != null) {
                Parent._children.Remove(this);
            }

            while(_children.Count > 0) {
                GameObject.Destroy(_children[0].transform.gameObject);
            }
        }




        /// <summary>
        /// Transforms a point from local space into world space.
        /// </summary>
        /// <param name="point">The point to transform</param>
        /// <returns>The point's position in world space</returns>
        public Vector3 TransformPoint(Vector3 point) {
            return Position + point;
        }

        /// <summary>
        /// Transforms a point from world space in to local space.
        /// </summary>
        /// <param name="point">The point to transform</param>
        /// <returns>The point's position in local space</returns>
        public Vector3 InverseTransformPoint(Vector3 point) {
            return point - Position;
        }


        
    }
}
