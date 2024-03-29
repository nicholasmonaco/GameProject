﻿// Extensions.cs - Nick Monaco

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using GameProject.Code.Scripts.Util;

namespace GameProject.Code.Core {
    
    /// <summary>
    /// Contains extension methods used throughout other classes.
    /// </summary>
    public static class Extensions {

        public static void Set(this Vector2 value, float x, float y) {
            value.X = x;
            value.Y = y;
        }

        public static void Set(this Vector3 value, float x, float y, float z) {
            value.X = x;
            value.Y = y;
            value.Z = z;
        }

        public static Point ToPoint(this Vector2 value) {
            return new Point((int)value.X, (int)value.Y);
        }

        public static Point ToPoint2D(this Vector3 value) {
            return new Point((int)value.X, (int)value.Y);
        }

        public static Point Div(this Point value, float div) {
            return new Point((int)(value.X / div), (int)(value.Y / div));
        }

        public static Point Mult(this Point value, float mult) {
            return new Point((int)(value.X * mult), (int)(value.Y * mult));
        }

        public static T Last<T>(this List<T> list) {
            return list[list.Count - 1];
        }

        public static T AddReturn<T>(this List<T> list, T element) {
            list.Add(element);
            return element;
        }

        //public static NMatrix ToNMatrix(this Matrix matrix, int size) {
        //    if (size < 1 || size > 4) throw new ArgumentException("Invalid size for NMatrix.");

        //    NMatrix result = new NMatrix(size);
        //    for(int row = 0;row < size; row++) {
        //        for (int column = 0; column < size; column++) {
        //            
        //        }
        //    }
        //}


        public static Quaternion Rotation(this Matrix matrix) {
            Quaternion rot;
            //if(!matrix.Decompose(out _, out rot, out _)) throw new ArgumentException("Cannot decompose matrix to fetch rotation.");
            matrix.Decompose(out _, out rot, out _);

            return rot;
        }

        public static Vector3 Scale(this Matrix matrix) {
            Vector3 scale;
            //if (!matrix.Decompose(out scale, out _, out _)) throw new ArgumentException("Cannot decompose matrix to fetch scale.");
            //matrix.Decompose(out scale, out _, out _);

            float xs = (Math.Sign(matrix.M11 * matrix.M12 * matrix.M13 * matrix.M14) < 0) ? -1 : 1;
            float ys = (Math.Sign(matrix.M21 * matrix.M22 * matrix.M23 * matrix.M24) < 0) ? -1 : 1;
            float zs = (Math.Sign(matrix.M31 * matrix.M32 * matrix.M33 * matrix.M34) < 0) ? -1 : 1;

            scale.X = xs * (float)Math.Sqrt(matrix.M11 * matrix.M11 + matrix.M12 * matrix.M12 + matrix.M13 * matrix.M13);
            scale.Y = ys * (float)Math.Sqrt(matrix.M21 * matrix.M21 + matrix.M22 * matrix.M22 + matrix.M23 * matrix.M23);
            scale.Z = zs * (float)Math.Sqrt(matrix.M31 * matrix.M31 + matrix.M32 * matrix.M32 + matrix.M33 * matrix.M33);

            //Debug.Log($"Scale B: ({scale.X}, {scale.Y}, {scale.Z})");

            return scale;
        }

        public static Vector2 ToVector2(this Vector3 v) {
            return new Vector2(v.X, v.Y);
        }

        public static Vector2 ToVector2(this Vector4 v) {
            return new Vector2(v.X, v.Y);
        }

        public static Vector3 ToVector3(this Vector2 v) {
            return new Vector3(v.X, v.Y, 0);
        }

        public static Vector2 FlipY(this Vector2 v) {
            return new Vector2(v.X, -v.Y);
        }



        public static Vector4 ConvertToIdentity4(this Vector2 v) {
            return new Vector4(v.X, v.Y, 0, 1);
        }

        public static Point InvertPoint(this Point p) {
            return new Point(-p.X, -p.Y);
        }

        
        public static float NextValue(this Random random, float minValue = 0, float maxValue = 1) {
            return (float)random.NextDouble() * (maxValue - minValue) + minValue;
        }

        public static Direction InvertDirection(this Direction direction) {
            if (direction == Direction.None) return Direction.None;

            return (Direction)(((int)direction + 2) % 4);
        }

        public static Point GetDirectionPoint(this Direction direction) {
            switch (direction) {
                default:
                case Direction.None:
                    return new Point(0, 0);
                case Direction.Up:
                    return new Point(0, 1);
                case Direction.Down:
                    return new Point(0, -1);
                case Direction.Left:
                    return new Point(-1, 0);
                case Direction.Right:
                    return new Point(1, 0);
            }
        }


        public static Vector2 Norm(this Vector2 vector) {
            if (vector == Vector2.Zero)
                return Vector2.Zero;
            else {
                return Vector2.Normalize(vector);
            }
        }

        public static Vector3 Norm(this Vector3 vector) {
            if (vector == Vector3.Zero)
                return Vector3.Zero;
            else {
                return Vector3.Normalize(vector);
            }
        }

        /// <summary>
        /// Rotates the direction angle clockwise(?) in a direction.
        /// </summary>
        /// <param name="angle">The angle to rotate, in degrees</param>
        /// <returns>The rotated direction vector.</returns>
        public static Vector2 RotateDirection(this Vector2 vector, float angle) {
            float cos = MathF.Cos(angle);
            float sin = MathF.Sin(angle);
            return new Vector2(cos * vector.X - sin * vector.Y,
                               sin * vector.X + cos * vector.Y).Norm();
        }

        public static Vector2 RotateDirectionNonUnit(this Vector2 vector, float angle) {
            float mag = vector.Length();

            float cos = MathF.Cos(angle);
            float sin = MathF.Sin(angle);
            return new Vector2(cos * vector.X - sin * vector.Y,
                               sin * vector.X + cos * vector.Y).Norm() * mag;
        }


        public static void Play(this SoundEffect sound, float volume) {
            sound.Play(volume * GameManager.MasterVolume * GameManager.SoundVolume, 0, 0);
        }
    }
}
