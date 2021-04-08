using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using GameProject.Code.Core.Components;

namespace GameProject.Code.Core.Particles {
    public class MainModule : IParticleModule {
        public bool Enabled { get; set; }
        public ParticleSystem AttachedSystem { get; set; }


        public float Duration;
        public bool Looping;
        public bool Prewarm;

        public ValueCurve_Float StartLifetime;
        public ValueCurve_Vector3 StartSpeed;
        public ValueCurve_Vector3 StartSize;
        public ValueCurve_Vector3 StartRotation;
        public ValueCurve_Color StartColor;

        public ParticleSimulationSpace SimulationSpace;

        public bool PlayOnAwake;

        public int MaxParticles;



        #region Initializers

        public void Initialize() {
            Initialize(5, true, false, 5, new Vector3(1, 0, 0), Vector3.One, Vector3.Zero, Color.White, ParticleSimulationSpace.Local, true, 200);
        }

        public void Initialize(float duration = 5, bool looping = true, bool prewarm = false,
                               float startLifetime = 5, ParticleSimulationSpace simulationSpace = ParticleSimulationSpace.Local,
                               bool playOnAwake = true, int maxParticles = 200) {

            Initialize(duration, looping, prewarm, startLifetime, 
                       new Vector3(1, 0, 0), Vector3.One, Vector3.Zero, Color.White, 
                       simulationSpace, playOnAwake, maxParticles);
        }

        public void Initialize(Vector3 startSpeed, Vector3 startSize, Vector3 startRotation,
                               Color startColor, 
                               float duration = 5, bool looping = true, bool prewarm = false,
                               float startLifetime = 5, ParticleSimulationSpace simulationSpace = ParticleSimulationSpace.Local,
                               bool playOnAwake = true, int maxParticles = 200) {

            Initialize(duration, looping, prewarm, startLifetime,
                       startSpeed, startSize, startRotation, startColor,
                       simulationSpace, playOnAwake, maxParticles);
        }

        public void Initialize(float duration, bool looping, bool prewarm,
                               float startLifetime, Vector3 startSpeed, Vector3 startSize, Vector3 startRotation,
                               Color startColor, ParticleSimulationSpace simulationSpace, bool playOnAwake, int maxParticles) {

            Initialize(duration, looping, prewarm,
                       (startLifetime, startLifetime), (startSpeed, startSpeed), (startSize, startSize), (startRotation, startRotation),
                       (startColor, startColor), simulationSpace, playOnAwake, maxParticles);
        }

        public void Initialize(float duration, bool looping, bool prewarm, 
                               (float, float) startLifetime, (Vector3, Vector3) startSpeed, (Vector3, Vector3) startSize, (Vector3, Vector3) startRotation,
                               (Color, Color) startColor, ParticleSimulationSpace simulationSpace, bool playOnAwake, int maxParticles) {

            Duration = duration;
            Looping = looping;
            Prewarm = prewarm;
            PlayOnAwake = playOnAwake;
            MaxParticles = maxParticles;

            StartLifetime = new ValueCurve_Float(startLifetime.Item1, startLifetime.Item2);
            StartSpeed = new ValueCurve_Vector3(startSpeed.Item1, startSpeed.Item2, InterpolationBehaviour.ComponentIndependent);
            StartSize = new ValueCurve_Vector3(startSize.Item1, startSize.Item2, InterpolationBehaviour.ComponentIndependent);
            StartRotation = new ValueCurve_Vector3(startRotation.Item1, startRotation.Item2, InterpolationBehaviour.ComponentIndependent);
            StartColor = new ValueCurve_Color(startColor.Item1, startColor.Item2);

            SimulationSpace = simulationSpace;
        }

        public void Initialize(float duration, bool looping, bool prewarm,
                               ValueCurve_Float startLifetime, ValueCurve_Vector3 startSpeed, ValueCurve_Vector3 startSize, ValueCurve_Vector3 startRotation,
                               ValueCurve_Color startColor, ParticleSimulationSpace simulationSpace, bool playOnAwake, int maxParticles) {

            Duration = duration;
            Looping = looping;
            Prewarm = prewarm;
            PlayOnAwake = playOnAwake;
            MaxParticles = maxParticles;

            StartLifetime = startLifetime;
            StartSpeed = startSpeed;
            StartSize = startSize;
            StartRotation = startRotation;
            StartColor = startColor;

            SimulationSpace = simulationSpace;
        }

        #endregion

    }

    public enum ParticleSimulationSpace {
        Local,
        World
    }
}
