using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameProject.Code.Core {
    public class Transform : Component {

        public Vector3 Position { 
            get { return new Vector3(_transformMatrix.M14, _transformMatrix.M24, _transformMatrix.M34); } 
            set {
                // First, find the difference between current position and new position (by comparing the values used above)
                // Then move it by that much
                Vector3 diff = value - new Vector3(_transformMatrix.M14, _transformMatrix.M24, _transformMatrix.M34);
                Move(diff);

                //inverse transform point would be used here
            } 
        }

        public Vector3 Rotation { //setting this is gonna be hard if not impossible unless we use quaternions
            get { return _rotation; }
            set {
                _localRotation = InverseTransformPoint(value);
                _rotation = value;
            }
        }

        public Vector3 Scale {
            get { return new Vector3(_transformMatrix.M11, _transformMatrix.M22, _transformMatrix.M33); }
            set {
                // First, find the difference between current scale and new scale (by comparing the values used above)
                // Then change it by that much
                Vector3 diff = value - new Vector3(_transformMatrix.M11, _transformMatrix.M22, _transformMatrix.M33);
                ReScale(diff);

                //inverse transform point would be used here
            }
        }

        public Vector3 LocalPosition { // right now, i think the transformation matrix is stored in local space, so we need to figure this out in accordance with that.
            get { return _localPosition; }
            set {
                _position = TransformPoint(value);
                _localPosition = value;
            }
        }

        public Vector3 LocalRotation {
            get { return _localRotation; }
            set {
                _rotation = TransformPoint(value);
                _localRotation = value;
            }
        }

        public Vector3 LocalScale {
            get { return _localScale; }
            set {
                _scale = TransformPoint(value);
                _localScale = value;
            }
        }


        // Variables
        public Transform Parent = null;
        public List<Transform> _children;

        private Matrix _transformMatrix;
        // End variables


        public void Move(Vector3 moveVec) {
            // Translate
            Matrix translationMatrix = new Matrix(new Vector4(1, 0, 0, moveVec.X),
                                                  new Vector4(0, 1, 0, moveVec.Y),
                                                  new Vector4(0, 0, 1, moveVec.Z),
                                                  new Vector4(0, 0, 0, 1));

            _transformMatrix *= translationMatrix;
            //reposition all children
        }

        public void ReScale(Vector3 scaleVec) {
            Matrix scaleMatrix = new Matrix(new Vector4(scaleVec.X, 0, 0, 0),
                                            new Vector4(0, scaleVec.Y, 0, 0),
                                            new Vector4(0, 0, scaleVec.Z, 0),
                                            new Vector4(0, 0, 0, 1));

            _transformMatrix *= scaleMatrix;
            //rescale all children
        }

        public void Rotate(Vector3 eulers) {
            Matrix rotationMatrix_X = new Matrix(new Vector4(1, 0, 0, 0),
                                                 new Vector4(0, MathF.Cos(eulers.X), -MathF.Sin(eulers.X), 0),
                                                 new Vector4(0, MathF.Sin(eulers.X), MathF.Cos(eulers.X), 0),
                                                 new Vector4(0, 0, 0, 1));

            Matrix rotationMatrix_Y = new Matrix(new Vector4(MathF.Cos(eulers.Z), 0, MathF.Sin(eulers.Y), 0),
                                                 new Vector4(0, 1, 0, 0),
                                                 new Vector4(-MathF.Sin(eulers.Y), 0, MathF.Cos(eulers.Y), 0),
                                                 new Vector4(0, 0, 0, 1));

            Matrix rotationMatrix_Z = new Matrix(new Vector4(MathF.Cos(eulers.Z), -MathF.Sin(eulers.Z), 0, 0),
                                                 new Vector4(MathF.Sin(eulers.Z), MathF.Cos(eulers.Z), 0, 0),
                                                 new Vector4(0, 0, 1, 0),
                                                 new Vector4(0, 0, 0, 1));

            _transformMatrix *= rotationMatrix_X;
            _transformMatrix *= rotationMatrix_Y;
            _transformMatrix *= rotationMatrix_Z;
            //This one is going to cause problems unless we always do it individually. Which isn't an awful idea but y'know.

            //rotate all children
        }



        // Constructors

        public Transform(GameObject attach) : base(attach) {
            Position = Vector3.Zero;
            Rotation = Vector3.Zero;
            Scale = Vector3.One;

            // In this case, we can assume that the Transform will have no parents by default.

            // This matrix will represent where the Transform will be in world space. 
            // By default, it will be facing forward on all 3 axes. (I think)
            _transformMatrix = new Matrix(new Vector4(1, 0, 0, 0),
                                          new Vector4(0, 1, 0, 0),
                                          new Vector4(0, 0, 1, 0),
                                          new Vector4(0, 0, 0, 1));
        }

        public Transform(GameObject attach, Transform parent) : base(attach) {
            Parent = parent;

            LocalPosition = Vector3.Zero;
            LocalRotation = Vector3.Zero;
            LocalScale = Vector3.One;
        }

        public Transform(GameObject attach, Vector3 position, Vector3 rotationEulers, Vector3 scale) : this(attach) {
            Position = position;
            Rotation = rotationEulers;
            Scale = scale;
        }

        public Transform(GameObject attach, Transform parent, Vector3 position, Vector3 rotationEulers, Vector3 scale) : base(attach) {
            Parent = parent;
        }





        // Transforms from local space to world space
        public Vector3 TransformPoint(Vector3 point) {
            Transform t = this;
            Vector3 pos = point;
            while (t.Parent != null){
                Matrix m = Matrix.Identity;
                m.M14 = pos.X;
                m.M24 = pos.Y;
                m.M34 = pos.Z;
                m = t._transformMatrix * m;

                pos.Set(m.M14, m.M24, m.M34);

                t = t.Parent;
            }
            
            return pos;
        }

        // Transforms from world space to local space
        public Vector3 InverseTransformPoint(Vector3 point) {
            Transform t = this;
            Vector3 pos = point;
            while (t.Parent != null) {
                Matrix m = Matrix.Identity;
                m.M14 = pos.X;
                m.M24 = pos.Y;
                m.M34 = pos.Z;
                m = Matrix.Invert(t._transformMatrix) * m; // I think this should work, we'll see.
                
                pos.Set(m.M14, m.M24, m.M34);

                t = t.Parent;
            }

            return pos;
        }

    }
}
