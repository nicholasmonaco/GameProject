// Scene.cs - Nick Monaco

using System;
using System.Collections.Generic;
using System.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using GameProject.Code.Core.Components;

namespace GameProject.Code.Core {
    
    /// <summary>
    /// A container for everything that is present in the game.
    /// </summary>
    public abstract class Scene {
        public static Action EmptyAction = () => { };


        public List<GameObject> GameObjects; //change this to use guids and be made of a GameObjectReferencer class, so that we can just set the gameobject reference in there to be null when we want it to be destroyed
        public List<Collider2D> Collider2Ds;

        private List<(Coroutine, Guid)> _coroutines;
        private Action _coroutineQueue = () => { };

        private Action _instantiateList = () => { };
        private Action _removeList = EmptyAction;

        protected bool _updating = true;

        public static Dictionary<BatchID, Effect> ShaderDictionary;



        public Scene() {
            
        }



        public virtual void LoadContent(ContentManager content) { }

        public virtual void UnloadContent() {
            foreach ((Coroutine, Guid) c in _coroutines) {
                c.Item1.Finished = true;
            }

            while (GameObjects.Count > 0) {
                GameObject.Destroy(GameObjects[0]);
                //GameObjects.RemoveAt(0);
            }

            Collider2Ds.Clear();

            _coroutineQueue = EmptyAction;
            _instantiateList = EmptyAction;
        }


        public virtual void Init() {
            GameObjects = new List<GameObject>();
            Collider2Ds = new List<Collider2D>();
            _coroutines = new List<(Coroutine, Guid)>();
        }

        public virtual void Awake() {
            // Instantiate initial gameobjects
            //_instantiateList();
            //_instantiateList = () => { };

            // Handle GameObjects
            foreach (GameObject g in GameObjects) {
                if (g.Enabled) {
                    g.Awake();
                    g.OnEnable();
                }
            }
        }

        public virtual void Start() {
            // Handle GameObjects
            foreach (GameObject g in GameObjects) {
                if(g.Enabled) g.Start();
            }
        }

        public virtual void Update() {
            if (_updating) {
                // Instantiate new GameObjects
                _instantiateList();
                _instantiateList = () => { };

                // Handle GameObjects
                foreach (GameObject g in GameObjects) {
                    if (g.Enabled && g._everAwaked) g.Update();
                }
            }

            // Handle coroutines
            _coroutineQueue();
            _coroutineQueue = () => { };

            if (_removeList != EmptyAction) _removeList();
            _removeList = EmptyAction;

            foreach ((Coroutine, Guid) routine in _coroutines) {
                // Update the coroutine
                routine.Item1.Update();
                // If the coroutine is finished, remove it from the list.
                if (routine.Item1.Finished) _removeList += () => { _coroutines.Remove(routine); };
            }

            _removeList();
        }

        public virtual void FixedUpdate() {
            if (!_updating) return;

            // Handle GameObjects
            foreach (GameObject g in GameObjects) {
                if (g.Enabled && g._everAwaked) g.FixedUpdate();
            }
        }

        public virtual void LateUpdate() {
            if (_updating) {
                // Handle GameObjects
                foreach (GameObject g in GameObjects) {
                    if (g.Enabled && g._everAwaked) g.LateUpdate();
                }
            }

            // Handle coroutines
            if (_removeList != EmptyAction) _removeList();
            _removeList = EmptyAction;

            foreach ((Coroutine, Guid) routine in _coroutines) {
                // Update the coroutine
                routine.Item1.LateUpdate();
                // If the coroutine is finished, remove it from the list.
                if (routine.Item1.Finished) _removeList += () => { _coroutines.Remove(routine); };
            }

            _removeList();
        }

        public virtual void Draw(SpriteBatch sb) {
            // Handle GameObjects
            //foreach (GameObject g in GameObjects) {
            //    if (g.Enabled) g.Draw(sb);
            //}


            Matrix viewMatrix = GameManager.MainCamera.ViewMatrix *
                                Matrix.CreateScale(1, -1, 1) *
                                Matrix.CreateTranslation(GameManager.Resolution.X / 2, GameManager.Resolution.Y / 2, 0);


            Dictionary<BatchID, List<IGameDrawable>> renderers = new Dictionary<BatchID, List<IGameDrawable>>();
            for(int i = 0; i < Material.ShaderCount; i++) { renderers.Add((BatchID)i, new List<IGameDrawable>()); }

            foreach(GameObject g in GameObjects) {
                if (!g.Enabled) continue;
                foreach(Component c in g._components) {
                    if(c is IGameDrawable drawable) {
                        renderers[drawable.Material.BatchID].Add(drawable);
                    }
                }
            }

            foreach(KeyValuePair<BatchID, List<IGameDrawable>> kvp in renderers) {
                if (kvp.Value.Count == 0) continue;

                sb.Begin(sortMode: SpriteSortMode.FrontToBack,
                         blendState: BlendState.AlphaBlend,
                         samplerState: SamplerState.PointClamp,
                         rasterizerState: RasterizerState.CullNone,
                         transformMatrix: viewMatrix,
                         effect: ShaderDictionary[kvp.Key]);

                foreach (IGameDrawable drawable in kvp.Value) {
                    drawable.Draw(sb);
                }

                sb.End();
            }
        }


        public static void InitShaders() {
            ShaderDictionary = new Dictionary<BatchID, Effect>(Material.ShaderCount);

            for (int i = 0; i < Material.ShaderCount; i++) {
                // This uses the default generated shader file, not sure why a new instance of a SpriteEffect doesn't work
                ShaderDictionary.Add((BatchID)i, Resources.Effect_Base);
            }
        }

        public static void ResetShaders() {
            ShaderDictionary.Clear();

            for (int i = 0; i < Material.ShaderCount; i++) {
                ShaderDictionary.Add((BatchID)i, Resources.Effect_Base);
            }
        }

        public static void ChangeShader(BatchID id, Effect newShader) {
            ShaderDictionary[id] = newShader;
        }

        public static void ChangeShader(BatchID id, Effect newShader, out Effect oldShader) {
            oldShader = ShaderDictionary[id];
            ShaderDictionary[id] = newShader;
        }




        public void PhysicsUpdate() {
            if (_updating) {
                // Do internal physics updates here
                Action _triggerActions = () => { };
                Action _collisionActions = () => { };

                //foreach (GameObject go in GameObjects) {
                //    if (!go.Enabled) continue;
                //    //search for rigidbody in children
                //    if (go.rigidbody2D != null) {
                //        go.rigidbody2D._PhysicsUpdate();

                //        // Now, queue up trigger and collision events
                //        _triggerActions += go.rigidbody2D.CallTriggerEvents;
                //        _collisionActions += go.rigidbody2D.CallCollisionEvents;
                //    }
                //}

                // Mark 2
                for(int i = 0; i < GameObjects.Count; i++) {
                    GameObject go = GameObjects[i];                    
                    if (!go.Enabled || go.rigidbody2D == null) continue;

                    //do physics logic
                    Rigidbody2D localRB = go.rigidbody2D;

                    foreach (Collider2D localCollider in localRB.Subcolliders) {
                        if (localCollider.Enabled == false) continue;

                        for (int j = 0; j < GameObjects.Count; j++) {
                            GameObject otherGO = GameObjects[j];
                            if (otherGO.Enabled == false || CollisionMatrix.DoLayersInteract(go.Layer, otherGO.Layer) == false) continue;

                            foreach (Collider2D collider in otherGO.collider2Ds) {
                                if (collider.Enabled == false) continue;
                                //if (Subcolliders.Contains(collider)) continue;

                                bool entered = false;
                                if (!localCollider.Entered.ContainsKey(collider)) localCollider.Entered.Add(collider, false);
                                else { entered = localCollider.Entered[collider]; }

                                Vector2 otherVelocity = collider.AttachedRigidbody == null ? Vector2.Zero : collider.AttachedRigidbody.Velocity;

                                // Use a switch depending on what type of shape match up it is
                                CollisionResult2D result;

                                AbstractBounds localBounds = localCollider.Bounds;
                                AbstractBounds otherBounds = collider.Bounds;
                                BoundsType localBoundType = localBounds.BoundsType;
                                BoundsType otherBoundType = otherBounds.BoundsType;

                                switch (localBoundType) {
                                    case BoundsType.Rectangle:
                                        switch (otherBoundType) {
                                            case BoundsType.Rectangle:
                                            case BoundsType.Polygon:
                                                //rectangle polygon/rectangle
                                                result = PolygonBounds.DetectPolygonCollision(localBounds as PolygonBounds, otherBounds as PolygonBounds, localRB.Velocity, otherVelocity);
                                                break;
                                            case BoundsType.Circle:
                                                //rectangle circle
                                                result = AbstractBounds.DetectCircleRectangleCollision(otherBounds as CircleBounds, localBounds as PolygonBounds, otherVelocity, localRB.Velocity);
                                                break;
                                            default:
                                                result = new CollisionResult2D();
                                                result.Intersecting = false;
                                                result.WillIntersect = false;
                                                result.MinimumTranslationVector = Vector2.Zero;
                                                break;
                                        }
                                        break;
                                    case BoundsType.Polygon:
                                        switch (otherBoundType) {
                                            case BoundsType.Rectangle:
                                            case BoundsType.Polygon:
                                                //polygon polygon
                                                result = PolygonBounds.DetectPolygonCollision(localBounds as PolygonBounds, otherBounds as PolygonBounds, otherVelocity, localRB.Velocity);
                                                break;
                                            case BoundsType.Circle:
                                                //polygon circle
                                                result = AbstractBounds.DetectDualTypeCollision(otherBounds as CircleBounds, localBounds as PolygonBounds, otherVelocity, localRB.Velocity);
                                                break;
                                            default:
                                                result = new CollisionResult2D();
                                                result.Intersecting = false;
                                                result.WillIntersect = false;
                                                result.MinimumTranslationVector = Vector2.Zero;
                                                break;
                                        }
                                        break;
                                    case BoundsType.Circle:
                                        switch (otherBoundType) {
                                            case BoundsType.Rectangle:
                                                //circle rectangle
                                                result = AbstractBounds.DetectCircleRectangleCollision(localBounds as CircleBounds, otherBounds as PolygonBounds, localRB.Velocity, otherVelocity);
                                                break;
                                            case BoundsType.Polygon:
                                                //circle polygon
                                                result = AbstractBounds.DetectDualTypeCollision(localBounds as CircleBounds, otherBounds as PolygonBounds, localRB.Velocity, otherVelocity);
                                                break;
                                            case BoundsType.Circle:
                                                //circle circle
                                                result = CircleBounds.DetectCircleCollision(localBounds as CircleBounds, otherBounds as CircleBounds, localRB.Velocity, otherVelocity);
                                                break;
                                            default:
                                                result = new CollisionResult2D();
                                                result.Intersecting = false;
                                                result.WillIntersect = false;
                                                result.MinimumTranslationVector = Vector2.Zero;
                                                break;
                                        }
                                        break;
                                    default:
                                        result = new CollisionResult2D();
                                        result.Intersecting = false;
                                        result.WillIntersect = false;
                                        result.MinimumTranslationVector = Vector2.Zero;
                                        break;
                                }


                                bool willCollide = false;
                                bool nonTriggerCollision = false;
                                Vector2 pushbackVec = Vector2.Zero;

                                if (result.WillIntersect) {
                                    bool notTrigger = !(collider.IsTrigger || localCollider.IsTrigger);
                                    if (notTrigger) pushbackVec += result.MinimumTranslationVector;

                                    if (!willCollide) willCollide = true;
                                    if (notTrigger && !nonTriggerCollision) nonTriggerCollision = true;
                                }

                                //Debug.Log($"will intersect: {result.WillIntersect} | intersecting: {result.Intersecting}");

                                //okay hear me out
                                //instead of doing this this way, let's store a bool for each other collider in each collider that represents if it's been entered
                                //if true, it's colliding
                                //if false, it isn't
                                //as long as it's true, we are staying, but until its false then we cant exit


                                #region Collision Event Handlers

                                if (!entered && result.WillIntersect) {
                                    if (!nonTriggerCollision) {
                                        _triggerActions += () => {
                                            localRB.OnTriggerEnter2D_Direct(collider);
                                            localCollider.OnTriggerEnter2D_Direct(collider);
                                            collider.OnTriggerEnter2D_Direct(localCollider);
                                            //Debug.Log("Trigger enter");
                                        };
                                    } else {
                                        _collisionActions += () => {
                                            localRB.OnCollisionEnter2D_Direct(collider);
                                            localCollider.OnCollisionEnter2D_Direct(collider);
                                            collider.OnCollisionEnter2D_Direct(localCollider);
                                            //Debug.Log("Collision enter");
                                        };
                                    }
                                } else if (entered && !result.WillIntersect) {
                                    if (!nonTriggerCollision) {
                                        _triggerActions += () => {
                                            localRB.OnTriggerExit2D_Direct(collider);
                                            localCollider.OnTriggerExit2D_Direct(collider);
                                            collider.OnTriggerExit2D_Direct(localCollider);
                                            //Debug.Log("Trigger exit");
                                        };
                                    } else {
                                        _collisionActions += () => {
                                            localRB.OnCollisionExit2D_Direct(collider);
                                            localCollider.OnCollisionExit2D_Direct(collider);
                                            collider.OnCollisionExit2D_Direct(localCollider);
                                            //Debug.Log("Collision exit");
                                        };
                                    }
                                } else if (entered) {
                                    if (!nonTriggerCollision) {
                                        _triggerActions += () => {
                                            localRB.OnTriggerStay2D_Direct(collider);
                                            localCollider.OnTriggerStay2D_Direct(collider);
                                            collider.OnTriggerStay2D_Direct(localCollider);
                                            //Debug.Log("Trigger stay");
                                        };
                                    } else {
                                        _collisionActions += () => {
                                            localRB.OnCollisionStay2D_Direct(collider);
                                            localCollider.OnCollisionStay2D_Direct(collider);
                                            collider.OnCollisionStay2D_Direct(localCollider);
                                            //Debug.Log("Collision stay");
                                        };
                                    }
                                }

                                #endregion



                                Rigidbody2D otherRB = otherGO.rigidbody2D;

                                // Now, queue up trigger and collision events
                                _triggerActions += localRB.CallTriggerEvents;
                                _collisionActions += localRB.CallCollisionEvents;

                                if(otherRB != null) {
                                    _triggerActions += otherRB.CallTriggerEvents;
                                    _collisionActions += otherRB.CallCollisionEvents;
                                }


                                // Apply collision logic                                
                                if(otherRB == null || (otherRB.Mass == 0 && localRB.Mass == 0)) {
                                    localRB.ApplyCollision(willCollide, nonTriggerCollision, pushbackVec);
                                } else {
                                    float totalMass = localRB.Mass + otherRB.Mass; 

                                    localRB.ApplyCollision(willCollide, nonTriggerCollision, pushbackVec * (localRB.Mass / totalMass));
                                    otherGO.rigidbody2D.ApplyCollision(willCollide, nonTriggerCollision, pushbackVec * (otherRB.Mass / totalMass));
                                }

                                localCollider.Entered[collider] = result.WillIntersect;
                                collider.Entered[localCollider] = result.WillIntersect;
                            }
                        }

                    }
                    //end physics logic


                    if (go.rigidbody2D != null) GameObjects[i].rigidbody2D.UpdateValues();
                }

                //if (GameObjects[GameObjects.Count - 1].rigidbody2D != null) GameObjects[GameObjects.Count - 1].rigidbody2D.UpdateValues();


                _triggerActions();
                _collisionActions();
                // End internal physics updates
            }

            // Handle coroutines
            if (_removeList != EmptyAction) _removeList();
            _removeList = EmptyAction;

            foreach ((Coroutine, Guid) routine in _coroutines) {
                // Update the coroutine
                routine.Item1.FixedUpdate();
                // If the coroutine is finished, remove it from the list.
                if (routine.Item1.Finished) _removeList += () => { _coroutines.Remove(routine); };
            }

            _removeList();
        }

        public Coroutine StartCoroutine(IEnumerator routine, Guid id) {
            Coroutine coroutine = new Coroutine(routine);

            _coroutineQueue += () => { _coroutines.Add((coroutine, id)); };

            // This StepThrough makes it so the first section of code in the coroutine (before the first yield) happens immediately
            coroutine.StepThrough(); 

            return coroutine;
        }

        public Coroutine StartCoroutine(IEnumerator routine) {
            return StartCoroutine(routine, Guid.Empty);
        }

        public void RemoveRelatedCoroutines(Guid id) {
            List<(Coroutine, Guid)> found = new List<(Coroutine, Guid)>();

            foreach((Coroutine, Guid) routinePair in _coroutines){
                if (id == routinePair.Item2) found.Add(routinePair);
            }

            foreach((Coroutine, Guid) routinePair in found) {
                //_coroutines[i].Item1.Finished = true;
                _removeList += () => { _coroutines.Remove(routinePair); };
            }
        }


        //public void RemoveGameObject(GameObject obj) {
        //    GameObjects.Remove(obj);
        //}


        public GameObject Instantiate(GameObject obj) {
            _instantiateList += () => {
                GameManager.CurrentScene.GameObjects.Add(obj);
                if (obj.Enabled) {
                    obj.Awake();
                    obj.OnEnable();
                    obj.Start();
                }
            };

            return obj;
        }

        public virtual void ResetScene() { }


        public static void LoadScene(Scene scene, ContentManager content) {
            ResetShaders();

            scene.Init();
            scene.LoadContent(content);
            scene.Awake();
            scene.Start();
        }

        public static void UnloadScene(Scene scene) {
            scene.UnloadContent();
        }


        protected void ActivateAction() {
            if(GameManager.CurrentUIIndex >= 0 && GameManager.CurrentUIIndex <= GameManager.UILayoutMembers.Count && GameManager.UILayoutMembers.Count > 0) {
                GameManager.UILayoutMembers[GameManager.CurrentUIIndex].DoActivate();
            }
        }

    }
}
