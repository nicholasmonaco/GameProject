using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using GameProject.Code.Core.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameProject.Code.Core.Components {
    public class ParticleSystem : Component, IGameDrawable {
        public ParticleSystem(GameObject attached) : base(attached) {
            CurrentTime = 0;

            Material = new Material(this);

            // Initializing modules
            _modules = new IParticleModule[10];

            Main = new MainModule();
            EmissionModule = new EmissionModule();
            Shape = new ShapeModule();
            ColorOverLifetimeModule = new ColorOverLifetimeModule();

            SizeOverLifetimeModule = new SizeOverLifetimeModule();

            TextureSheetAnimationModule = new TextureSheetAnimationModule();


            _modules[0] = Main;
            _modules[1] = EmissionModule;
            _modules[2] = Shape;
            _modules[3] = ColorOverLifetimeModule;
            //colorBySpeed
            //rotationOverLifetime
            _modules[6] = SizeOverLifetimeModule;
            //sizeBySpeed
            //velocityOverLifetime
            _modules[9] = TextureSheetAnimationModule;

            foreach (IParticleModule p in _modules) {
                if (p != null) {
                    p.AttachedSystem = this;
                    p.Initialize();
                }
            }
        }

        public override void PreAwake() {
            base.PreAwake();

            CurrentTime = 0;
            IsEmitting = Main.PlayOnAwake;
            _spawnTimer = 0;

            if (Sprite == null) Sprite = Resources.Sprite_Pixel;

            RelativePosition = Main.SimulationSpace == ParticleSimulationSpace.Local ? -transform.Position : Vector3.Zero;

            if (IsEmitting) {
                Play();
            }

            particles = new Particle[Main.MaxParticles];
            _freeParticles = new Queue<Particle>(Main.MaxParticles);

            for(int i = 0; i < particles.Length; i++) {
                Particle p = new Particle();
                p.RemainingLifetime = 0;
                particles[i] = p;

                _freeParticles.Enqueue(p);
            }

            if (Main.Prewarm) {
                Simulate(Main.StartLifetime.Min / 2f); //theres some magic value to this, but idk what it is at the moment
            }
        }



        public Material Material { get; set; }
        private Particle[] particles;
        private Queue<Particle> _freeParticles;
        public int ParticleCount = 0;
        public int MaxParticleCount => Main.MaxParticles;
        public bool IsEmitting { get; set; }
        public float CurrentTime { get; private set; }


        private ParticleSystemState _currentState = ParticleSystemState.Stopped;
        public bool IsPaused => _currentState == ParticleSystemState.Paused;
        public bool IsPlaying => _currentState == ParticleSystemState.Playing;
        public bool IsStopped => _currentState == ParticleSystemState.Stopped;

        public ParticleSystemStopBehaviour StopBehaviour = ParticleSystemStopBehaviour.Finish;

        public Vector3 RelativePosition = Vector3.Zero;


        #region Modules
        private IParticleModule[] _modules;

        public MainModule Main { get; private set; }
        public ShapeModule Shape { get; private set; }
        public EmissionModule EmissionModule { get; private set; }
        public ColorOverLifetimeModule ColorOverLifetimeModule { get; private set; }

        public SizeOverLifetimeModule SizeOverLifetimeModule { get; private set; }
        public TextureSheetAnimationModule TextureSheetAnimationModule { get; private set; }
        #endregion

        #region Private Variables
        private float _spawnTimer = 0;
        #endregion


        #region Rendering Information

        [AnimatableValue]
        public Texture2D Sprite {
            get => Material.Texture as Texture2D;
            set { if(Material != null) Material.Texture = value; }
        }

        public int DrawLayer {
            get { return _drawLayer; }
            set {
                _drawLayer = value;
                _realDrawOrder = (_drawLayer * 10000 + _orderInLayer) / 500000f;
            }
        }

        public int OrderInLayer {
            get { return _orderInLayer; }
            set {
                _orderInLayer = value;
                _realDrawOrder = (_drawLayer * 10000 + _orderInLayer) / 500000f;
            }
        }

        private int _drawLayer = 0;
        private int _orderInLayer = 0;
        private float _realDrawOrder = 0;

        #endregion


        public override void Update() {
            if (GameManager.Paused) return;

            if (IsPlaying) {
                //update system timer
                if (CurrentTime > Main.Duration) {
                    CurrentTime -= Main.Duration;
                    if (!Main.Looping) {
                        Stop();
                    } else {
                        CurrentTime += Time.deltaTime;
                    }
                } else {
                    CurrentTime += Time.deltaTime;
                }
            }


            if (IsPlaying) {
                //spawn more particles
                _spawnTimer -= Time.deltaTime;

                int burstCount;
                EmissionModule.BurstCheck(CurrentTime, out burstCount);

                if(_spawnTimer <= 0) {
                    int count = (int) ((-_spawnTimer) % (1f / EmissionModule.RateOverTime)); //i think this math is wrong
                    count++;
                    count += burstCount;
                    ForceEmit(count);

                    _spawnTimer += 1f / EmissionModule.RateOverTime;
                }
            }


            if (!IsPaused) {
                //update particles
                foreach (Particle p in particles) {
                    if (p.Alive) {
                        UpdateParticle(p, Time.deltaTime);

                        if (!p.Alive) {
                            _freeParticles.Enqueue(p);
                            ParticleCount--;
                        }
                    }
                }
            }
        }

        private void UpdateParticle(Particle particle, float deltaTime) {
            // Standard updates
            particle.Position += particle.Velocity3D * deltaTime;
            particle.Rotation += particle.AngularVelocity * deltaTime;

            // Lifetime update
            particle.RemainingLifetime -= deltaTime;

            // Module updates
            ColorOverLifetimeModule.UpdateParticle(particle);
            SizeOverLifetimeModule.UpdateParticle(particle);
        }


        public override void Draw(SpriteBatch sb) {
            Vector2 spriteOrigin;
            if (TextureSheetAnimationModule.Enabled) {
                spriteOrigin = TextureSheetAnimationModule.TileSize.ToVector2() / 2f;
            } else {
                spriteOrigin = new Vector2(Sprite.Width / 2f, Sprite.Height / 2f);
            }

            Vector2 localSpace;
            float rotation;
            Vector2 scaleFlip;

            if(Main.SimulationSpace == ParticleSimulationSpace.Local) {
                localSpace = transform.Position.ToVector2();
                rotation = transform.Rotation_Rads;
                scaleFlip = transform.Scale.ToVector2();
            } else {
                localSpace = Vector2.Zero;
                rotation = 0;
                scaleFlip = Vector2.One;
            }
            
            scaleFlip = scaleFlip.FlipY();


            foreach (Particle p in particles) {
                if (!p.Alive) continue;

                if(p.ParticleType == ParticleType.Sprite) {
                    //sprite draw
                    sb.Draw(Sprite,
                            localSpace + p.Position.ToVector2(),
                            TextureSheetAnimationModule.Enabled ? TextureSheetAnimationModule?.GetCurrentSpriteRect(p.LifetimeRatio) : null,
                            p.Color,
                            rotation + p.Rotation,
                            spriteOrigin,
                            scaleFlip * p.Scale,
                            SpriteEffects.None,
                            _realDrawOrder);

                } else if(p.ParticleType == ParticleType.Model) {
                    //model draw
                }
            }
        }



        #region Utility Methods

        public void Clear() {
            foreach(Particle p in particles) {
                p.RemainingLifetime = 0;
            }
        }

        public void Emit(int count) {
            if(_currentState == ParticleSystemState.Playing) {
                ForceEmit(count);
            }
        }

        public void ForceEmit(int count) {
            for(int i = 0; i < count; i++) {
                if (_freeParticles.Count == 0) break;

                ParticleCount++;

                Particle p = _freeParticles.Dequeue();

                //p.Seed = GameManager.DeltaRandom.Next();
                p.StartLifetime = Main.StartLifetime.GetValue();
                p.RemainingLifetime = p.StartLifetime;
                p.Position = Shape.GetPosition();
                p.Rotation3D = Main.StartRotation.GetValue();
                p.Scale3D = Main.StartSize.GetValue();
                p.Velocity3D = Main.StartSpeed.GetValue();
                //p.AngularVelocity3D = Main.StartAngularVelocity.GetValue();

                p.StartColor = Main.StartColor.GetValue();
                p.Color = p.StartColor;
            }
        }

        public int GetParticles(out Particle[] particles) {
            particles = new Particle[this.particles.Length];
            this.particles.CopyTo(particles, 0);
            return particles.Length;
        }

        public void SetParticles(Particle[] particles) {
            this.particles = new Particle[particles.Length];

            for(int i=0;i<particles.Length;i++) {
                this.particles[i] = particles[i];
            }
        }

        public bool IsAlive() {
            return ParticleCount > 0 && _currentState != ParticleSystemState.Stopped; 
        }

        public void Play() {
            _currentState = ParticleSystemState.Playing;
        }

        public void Pause() {
            _currentState = ParticleSystemState.Paused;
        }        

        public void Stop() {
            _currentState = ParticleSystemState.Stopped;
            _spawnTimer = 0;
        }

        public void Simulate(float seconds) {
            //spawn more particles
            int count = (int)(seconds * EmissionModule.RateOverTime); 
            //count++;
            ForceEmit(count);

            // update the particles
            foreach (Particle p in particles) {
                if (p.Alive) {
                    UpdateParticle(p, seconds % p.StartLifetime);

                    if (!p.Alive) {
                        _freeParticles.Enqueue(p);
                        ParticleCount--;
                    }
                }
            }
        }

        #endregion



        public override void OnDestroy() {
            //_freeParticles.Clear();

            if(particles != null) {
                for (int i = 0; i < particles.Length; i++) {
                    particles[i] = null;
                }
            }

            if(_modules != null) {
                for (int i = 0; i < _modules.Length; i++) {
                    _modules[i] = null;
                }
            }

            base.OnDestroy();
        }
    }

    public enum ParticleSystemState {
        Playing,
        Paused,
        Stopped
    }

    public enum ParticleSystemStopBehaviour {
        Finish,
        Clear
    }

    public enum ParticleModuleID {
        Main = 0,
        Emission = 1,
        Shape = 2,
        ColorOverLifetime = 3,
        ColorBySpeed = 4,
        RotationOverLifetime = 5,
        SizeOverLifetime = 6,
        SizeBySpeed = 7,
        VelocityOverLifetime = 8,
        TextureSheetAnimation = 9
    }
}