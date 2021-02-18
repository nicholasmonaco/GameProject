// QuaternionTest.cs - Nick Monaco
// Math and logic of quaternions from:
// https://www.cprogramming.com/tutorial/3d/quaternions.html
// https://www.3dgep.com/understanding-quaternions/
// Conversion between quaterions and Euler angles from:
// https://en.wikipedia.org/wiki/Conversion_between_quaternions_and_Euler_angles
// Extra method logic and code frameworks from:
// https://www.euclideanspace.com/maths/algebra/realNormedAlgebra/quaternions/code/index.htm

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameProject.Code.Core {
    
    /// <summary>
    /// A test class used to test and understand the inner workings of quaternions.
    /// </summary>
    public class QuaternionTest {

        public static QuaternionTest Identity => new QuaternionTest(1, 0, 0, 0);

        public float w;
        public float x;
        public float y;
        public float z;


        public QuaternionTest(float w, float x, float y, float z) {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public QuaternionTest(Vector4 values) {
            w = values.X;
            x = values.Y;
            y = values.Z;
            z = values.W;
        }


        // Operators

        public static QuaternionTest operator *(QuaternionTest a, QuaternionTest b) => new QuaternionTest(
            a.w * b.x + a.x * b.w + a.y * b.z - a.z * b.y,
            a.w * b.y - a.x * b.z + a.y * b.w + a.z * b.x,
            a.w * b.z + a.x * b.y - a.y * b.x + a.z * b.w,
            a.w * b.w - a.x * b.x - a.y * b.y - a.z * b.z);

        //public static Vector3 operator *(Quaternion q, Vector3 v) => new Vector3(
        //    Matri
        //    );

        // End operators


        // Methods

        public Matrix RotationMatrix => new Matrix(
                new Vector4(1 - 2 * (MathF.Pow(y, 2) - MathF.Pow(z, 2)), 2 * (x* y - w* z),  2 * (x* z + w* y), 0),
                new Vector4(2*(x* y + w* z), 1 - 2*(MathF.Pow(x,2)-MathF.Pow(z,2)), 2 * (y* z + w* x), 0),
                new Vector4(2*(x* z - w* y), 2 * (y* z - w* x), 1 - 2 * (MathF.Pow(x, 2) - MathF.Pow(y, 2)), 0),
                new Vector4(0, 0, 0, 1));

        public QuaternionTest Conjugate => new QuaternionTest(w, -x, -y, -z);
        public QuaternionTest Inverse => new QuaternionTest(w, -x, -y, -z);
        //public Vector3 Up => Rotation * Vector3.Up;

        public void ForceNormalize() {
            float magnitude = MathF.Sqrt(MathF.Pow(x, 2) + MathF.Pow(y, 2) + MathF.Pow(z, 2) + MathF.Pow(w, 2));
            w /= magnitude;
            x /= magnitude;
            y /= magnitude;
            z /= magnitude;
        }

        public void Normalize() {
            // This is a more efficient version of normalization, as sometimes it's so close to being normalized that it isn't needed
        }

        public static QuaternionTest Euler(float x, float y, float z) {
            return Euler(new Vector3(x, y, z));
        }

        public static QuaternionTest Euler(Vector3 eulers) {
            Vector3 radEulers = eulers * MathEx.Deg2Rad;
            QuaternionTest q = Identity;

            float cy = MathF.Cos(radEulers.Z * 0.5f);
            float sy = MathF.Sin(radEulers.Z * 0.5f);
            float cp = MathF.Cos(radEulers.Y * 0.5f);
            float sp = MathF.Sin(radEulers.Y * 0.5f);
            float cr = MathF.Cos(radEulers.Z * 0.5f);
            float sr = MathF.Sin(radEulers.Z * 0.5f);

            q.w = cr * cp * cy + sr * sp * sy;
            q.x = sr * cp * cy - cr * sp * sy;
            q.y = cr * sp * cy + sr * cp * sy;
            q.z = cr * cp * sy - sr * sp * cy;

            return q;
        }

        public Vector3 GetEulerAngles() {
            Vector3 angles = Vector3.Zero;

            // X
            float sinr_cosp = 2 * (w * x + y * z);
            float cosr_cosp = 1 - 2 * (x * x + y * y);
            angles.X = MathF.Atan2(sinr_cosp, cosr_cosp);

            // Y
            float sinp = 2 * (w * y - z * x);
            if(MathF.Abs(sinp) >= 1) {
                angles.Y = MathF.CopySign(MathF.PI / 2, sinp); // This if is to handle out of range issues
            } else {
                angles.Y = MathF.Asin(sinp);
            }

            // Z
            float siny_cosp = 2 * (w * z + x * y);
            float cosy_cosp = 1 - 2 * (y * y + z * z);
            angles.Z = MathF.Atan2(siny_cosp, cosy_cosp);

            return angles * MathEx.Rad2Deg;
        }

        

        // This internal (commented out) parts of this method should be used in stuff like lerp, which will probably be implemented later.
        public Matrix RotateTo(QuaternionTest other) {
            //Vector3 axisOfRotation = Vector3.Zero;

            QuaternionTest qT = other * this.Conjugate; // Transforming quaternion
            //float angle = 2 * MathF.Acos(qT.w);
            //float sqrt = MathF.Sqrt(1 - qT.w * qT.w);
            //if (sqrt <= 0.001f) return Matrix.Identity; // Don't perform the rotation
            //axisOfRotation.X = qT.x / sqrt;
            //axisOfRotation.Y = qT.y / sqrt;
            //axisOfRotation.Z = qT.z / sqrt;

            // Now, this * qT = other
            // So, we know how much we need to move this by in order to turn it into other
            //Quaternion local_rotation = Identity;
            //local_rotation.w = MathF.Cos(angle / 2);
            //local_rotation.x = axisOfRotation.X * MathF.Sin(angle / 2);
            //local_rotation.y = axisOfRotation.Y * MathF.Sin(angle / 2);
            //local_rotation.z = axisOfRotation.Z * MathF.Sin(angle / 2);

            //Quaternion total = Identity * local_rotation;

            // Note: This is slightly optimized, as we only work with unit quaternions
            // This is what we use to rotate points
            Matrix rotationMatrix = new Matrix(
                new Vector4(1 - 2 * (MathF.Pow(qT.y, 2) - MathF.Pow(qT.z, 2)), 2 * (qT.x * qT.y - qT.w * qT.z),  2 * (qT.x * qT.z + qT.w * qT.y), 0),
                new Vector4(2*(qT.x * qT.y + qT.w * qT.z), 1 - 2*(MathF.Pow(qT.x,2)-MathF.Pow(qT.z,2)), 2 * (qT.y * qT.z + qT.w * qT.x), 0),
                new Vector4(2*(qT.x * qT.z - qT.w * qT.y), 2 * (qT.y * qT.z - qT.w * qT.x), 1 - 2 * (MathF.Pow(qT.x, 2) - MathF.Pow(qT.y, 2)), 0),
                new Vector4(0, 0, 0, 1));

            w = other.w;
            x = other.x;
            y = other.y;
            z = other.z;

            return rotationMatrix;
        }


        // End methods
    }
}
