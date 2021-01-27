using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameProject.Code.Core {
    public class Transform : Component {
        private Vector2 _position;
        private float _yRotation;
        private Vector2 _scale;

        private Vector2 _localPosition;
        private float _localYRotation;
        private Vector2 _localScale;


        public Vector2 Position { 
            get { return _position; } 
            set {
                _localPosition = InverseTransformPoint(value);
                _position = value;
            } 
        }

        public float YRotation {
            get { return _yRotation; }
            set {
                _localYRotation = InverseTransformRotation(value);
                _yRotation = value;
            }
        }

        public Vector2 Scale {
            get { return _scale; }
            set {
                _localScale = InverseTransformPoint(value);
                _scale = value;
            }
        }

        public Vector2 LocalPosition {
            get { return _localPosition; }
            set {
                _position = TransformPoint(value);
                _localPosition = value;
            }
        }

        public float LocalYRotation {
            get { return _localYRotation; }
            set {
                _yRotation = TransformRotation(value);
                _localYRotation = value;
            }
        }

        public Vector2 LocalScale {
            get { return _localScale; }
            set {
                _scale = TransformPoint(value);
                _localScale = value;
            }
        }


        public Transform Parent = null;
        public List<Transform> _children;


        public Transform(GameObject attach) : base(attach) {
            Position = Vector2.Zero;
            YRotation = 0;
            Scale = Vector2.One;

            // In this case, we can assume that the Transform will have no parents by default.

            // This matrix will represent where the Transform will be in world space. 
            // By default, it will be facing forward on all 3 axes. (I think)
            Matrix transformMatrix = new Matrix(new Vector4(1, 0, 0, 0),
                                                new Vector4(0, 1, 0, 0),
                                                new Vector4(0, 0, 1, 0),
                                                new Vector4(0, 0, 0, 1));
        }

        public Transform(GameObject attach, Transform parent) : base(attach) {
            Parent = parent;

            LocalPosition = Vector2.Zero;
            LocalYRotation = 0;
            LocalScale = Vector2.One;
        }

        public Transform(GameObject attach, Vector2 position, float yRotation, Vector2 scale) : this(attach) {
            Position = position;
            YRotation = yRotation;
            Scale = scale;
        }

        public Transform(GameObject attach, Transform parent, Vector2 position, float yRotation, Vector2 scale) : base(attach) {
            Parent = parent;
        }





        // Transforms from local space to world space
        public Vector2 TransformPoint(Vector2 point) {
            



            Transform t = this;
            Vector2 pos = point;
            while (t.Parent != null){
                pos *= t._localScale;
                pos += t._localPosition;
                t = t.Parent;
            }
            
            return pos;
        }

        // Transforms from world space to local space
        public Vector2 InverseTransformPoint(Vector2 point) {
            Transform t = this;
            Vector2 pos = point;
            while (t.Parent != null) {
                pos /= t._localScale;
                pos -= t._localPosition; // I think this one might not work
                t = t.Parent;
            }

            return pos;
        }

        // Transforms from local rotation to world rotation
        public float TransformRotation(float rotation) {
            return 0;
        }

        // Transforms from world rotation to local rotation
        public float InverseTransformRotation(float rotation) {
            return 0;
        }
    }
}
