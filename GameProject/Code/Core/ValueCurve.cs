using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameProject.Code.Core {
    public struct ValueCurve_Float {
        public float Min { get; private set; }
        public float Max { get; private set; }
        public InterpolationBehaviour InterpolationBehaviour;

        public ValueCurve_Float(float min, float max, InterpolationBehaviour interpolationBehaviour = InterpolationBehaviour.Lerp) {
            Min = min;
            Max = max;
            InterpolationBehaviour = interpolationBehaviour;
        }

        public ValueCurve_Float(float value) {
            Min = value;
            Max = value;
            InterpolationBehaviour = InterpolationBehaviour.Minimum;
        }


        public float GetValue() {
            switch (InterpolationBehaviour) {
                default:
                case InterpolationBehaviour.Minimum: return Min;
                case InterpolationBehaviour.Maximum: return Max;
                case InterpolationBehaviour.Average: return (Min + Max) / 2f;
                case InterpolationBehaviour.Lerp: return GetLerp(GameManager.DeltaRandom.NextValue());
                case InterpolationBehaviour.Smoothstep: return GetSmoothstep(GameManager.DeltaRandom.NextValue());
            }
        }

        public float GetAverage() {
            return (Min + Max) / 2f;
        }

        public float GetLerp(float t) {
            return MathHelper.Lerp(Min, Max, t);
        }

        public float GetSmoothstep(float t) {
            return MathHelper.SmoothStep(Min, Max, t);
        }
    }


    public struct ValueCurve_Vector3 {
        public Vector3 Min { get; private set; }
        public Vector3 Max { get; private set; }
        public InterpolationBehaviour InterpolationBehaviour;

        public ValueCurve_Vector3(Vector3 min, Vector3 max, InterpolationBehaviour interpolationBehaviour = InterpolationBehaviour.Lerp) {
            Min = min;
            Max = max;
            InterpolationBehaviour = interpolationBehaviour;
        }

        public ValueCurve_Vector3(Vector3 value) {
            Min = value;
            Max = value;
            InterpolationBehaviour = InterpolationBehaviour.Minimum;
        }


        public Vector3 GetValue() {
            switch (InterpolationBehaviour) {
                default:
                case InterpolationBehaviour.Minimum: return Min;
                case InterpolationBehaviour.Maximum: return Max;
                case InterpolationBehaviour.Average: return (Min + Max) / 2f;
                case InterpolationBehaviour.Lerp: return GetLerp(GameManager.DeltaRandom.NextValue());
                case InterpolationBehaviour.Smoothstep: return GetSmoothstep(GameManager.DeltaRandom.NextValue());
                case InterpolationBehaviour.ComponentIndependent: return GetComponentIndependent(GameManager.DeltaRandom.NextValue(), GameManager.DeltaRandom.NextValue(), GameManager.DeltaRandom.NextValue());
            }
        }

        public Vector3 GetLerp(float t) {
            return Vector3.Lerp(Min, Max, t);
        }

        public Vector3 GetSmoothstep(float t) {
            return Vector3.SmoothStep(Min, Max, t);
        }

        public Vector3 GetComponentIndependent(float x, float y, float z) {
            return new Vector3(MathHelper.Lerp(Min.X, Max.X, x),
                               MathHelper.Lerp(Min.Y, Max.Y, y),
                               MathHelper.Lerp(Min.Z, Max.Z, z));
        }
    }


    public struct ValueCurve_Vector2 {
        public Vector2 Min { get; private set; }
        public Vector2 Max { get; private set; }
        public InterpolationBehaviour InterpolationBehaviour;

        public ValueCurve_Vector2(Vector2 min, Vector2 max, InterpolationBehaviour interpolationBehaviour = InterpolationBehaviour.Lerp) {
            Min = min;
            Max = max;
            InterpolationBehaviour = interpolationBehaviour;
        }

        public ValueCurve_Vector2(Vector2 value) {
            Min = value;
            Max = value;
            InterpolationBehaviour = InterpolationBehaviour.Minimum;
        }


        public Vector2 GetValue() {
            switch (InterpolationBehaviour) {
                default:
                case InterpolationBehaviour.Minimum: return Min;
                case InterpolationBehaviour.Maximum: return Max;
                case InterpolationBehaviour.Average: return (Min + Max) / 2f;
                case InterpolationBehaviour.Lerp: return GetLerp(GameManager.DeltaRandom.NextValue());
                case InterpolationBehaviour.Smoothstep: return GetSmoothstep(GameManager.DeltaRandom.NextValue());
                case InterpolationBehaviour.ComponentIndependent: return GetComponentIndependent(GameManager.DeltaRandom.NextValue(), GameManager.DeltaRandom.NextValue());
            }
        }

        public Vector2 GetLerp(float t) {
            return Vector2.Lerp(Min, Max, t);
        }

        public Vector2 GetSmoothstep(float t) {
            return Vector2.SmoothStep(Min, Max, t);
        }

        public Vector2 GetComponentIndependent(float x, float y) {
            return new Vector2(MathHelper.Lerp(Min.X, Max.X, x),
                               MathHelper.Lerp(Min.Y, Max.Y, y));
        }
    }


    public struct ValueCurve_Color {
        public Color Min { get; private set; }
        public Color Max { get; private set; }
        public InterpolationBehaviour InterpolationBehaviour;

        public ValueCurve_Color(Color min, Color max, InterpolationBehaviour interpolationBehaviour = InterpolationBehaviour.Lerp) {
            Min = min;
            Max = max;
            InterpolationBehaviour = interpolationBehaviour;
        }

        public ValueCurve_Color(Color value) {
            Min = value;
            Max = value;
            InterpolationBehaviour = InterpolationBehaviour.Minimum;
        }


        public Color GetValue() {
            switch (InterpolationBehaviour) {
                default:
                case InterpolationBehaviour.Minimum: return Min;
                case InterpolationBehaviour.Maximum: return Max;
                case InterpolationBehaviour.Average: return GetLerp(0.5f);
                case InterpolationBehaviour.Lerp: return GetLerp(GameManager.DeltaRandom.NextValue());
                case InterpolationBehaviour.Smoothstep: return GetSmoothstep(GameManager.DeltaRandom.NextValue());
                case InterpolationBehaviour.ComponentIndependent:
                    return GetComponentIndependent(GameManager.DeltaRandom.NextValue(), GameManager.DeltaRandom.NextValue(), GameManager.DeltaRandom.NextValue(), GameManager.DeltaRandom.NextValue());
            }
        }

        public Color GetLerp(float t) {
            return Color.Lerp(Min, Max, t);
        }

        public Color GetSmoothstep(float t) {
            Vector4 frac = new Vector4(t);

            Vector4 result;

            Vector4 unpackedMin = Min.ToVector4() / 255f;
            Vector4 unpackedMax = Max.ToVector4() / 255f;
            //are these making them be 0-255 or 0-1 ?

            result = (frac - unpackedMin) / (unpackedMax - unpackedMin);
            result = result * result * (new Vector4(3.0f) - 2.0f * result);
            return new Color(result);
        }

        public Color GetComponentIndependent(float r, float g, float b, float a) {
            Vector4 result;

            Vector4 unpackedMin = Min.ToVector4();
            Vector4 unpackedMax = Max.ToVector4();

            result = new Vector4(MathHelper.Lerp(unpackedMin.X, unpackedMax.X, r),
                                 MathHelper.Lerp(unpackedMin.Y, unpackedMax.Y, g),
                                 MathHelper.Lerp(unpackedMin.Z, unpackedMax.Z, b),
                                 MathHelper.Lerp(unpackedMin.W, unpackedMax.W, a));

            return new Color(result);
        }
    }




    public enum InterpolationBehaviour {
        Minimum,
        Maximum,
        Average,
        Lerp,
        Smoothstep,
        ComponentIndependent
    } 
}
