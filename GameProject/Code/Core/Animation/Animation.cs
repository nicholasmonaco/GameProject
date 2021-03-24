using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameProject.Code.Core.Components;

namespace GameProject.Code.Core.Animation {
    public class Animation : StateData {

        public AnimationController Controller { get; private set; }
        private GameObject gameObject => Controller.gameObject;
        private Transform transform => Controller.transform;

        public int FrameDuration { get; private set; } = 60; //60 updates per second
        public int CurrentFrameIndex { get; private set; } = 0;


        private bool Looping = false;
        private bool _doneLooping = false;
        public int Loops { get; private set; } = 0;

        private List<AnimationData> NodeLists;



        public Animation(AnimationController controller) {
            Controller = controller;
        }

        public Animation(AnimationController controller, bool looping, List<AnimationData> nodeLists) : this(controller) {
            Looping = looping;
            SetNodeLists(nodeLists);
        }



        public void SetNodeLists(List<AnimationData> nodeLists) {
            NodeLists = new List<AnimationData>(nodeLists);
        }


        public override void Update(float progress = 1) {
            if (_doneLooping) return;

            for(int i = 0; i < NodeLists.Count; i++) {
                NodeLists[i].Update(CurrentFrameIndex);
            }

            CurrentFrameIndex++;
            if(CurrentFrameIndex >= FrameDuration) {
                if (!Looping) _doneLooping = true;
                CurrentFrameIndex = 0;
                Loops++;
            }
        }

        public override void OnStateReset() {
            Loops = 0;
            _doneLooping = false;
            CurrentFrameIndex = 0;
        }


        public override IEnumerator Delayed_Coroutine(StateData otherState) {
            yield return new WaitForSeconds(TransitionData);
        }

        public override IEnumerator Lerp_Coroutine(StateData otherState) {
            yield return new WaitForSeconds(TransitionData);
        }

        public override IEnumerator SmoothStep_Coroutine(StateData otherState) {
            yield return new WaitForSeconds(TransitionData);
        }


        public override void Destroy() {
            Controller = null;

            foreach (AnimationData data in NodeLists) {
                data.Destroy();
            }

            NodeLists.Clear();
            NodeLists = null;
        }
    }



    //public enum AnimationMoveStyle {
    //    Instant,
    //    AfterEnd,
    //    Lerp,
    //    SmoothStep
    //}


    public class AnimationData {
        private SpriteRenderer _spriteRenderer;
        public int CurrentFrame { get; private set; }
        public List<(int, Texture2D)> Framestamps;

        public AnimationData(SpriteRenderer renderer, List<(int, Texture2D)> framestamps) {
            _spriteRenderer = renderer;
            Framestamps = new List<(int, Texture2D)>(framestamps);

            CurrentFrame = int.MaxValue;
        }

        public void Update(int frame) {
            if (frame == 0) CurrentFrame = 0;
            else if (CurrentFrame >= Framestamps.Count) return;

            (int, Texture2D) framestamp = Framestamps[CurrentFrame];
            if (framestamp.Item1 == frame) {
                _spriteRenderer.Sprite = framestamp.Item2;

                CurrentFrame++;
            }
        }

        public void Destroy() {
            _spriteRenderer = null;
            Framestamps.Clear();
        }
    }



    // The stuff below is the concept for how to make animation work with anything, not just sprites.

    #region Full Animation Prototype

    //public struct AnimationData<T> where T : Component {
    //    public Guid ComponentID;
    //    private int _currentFrame;
    //    public List<int> Framestamps;
    //    public Dictionary<int, Dictionary<string, (object, AnimationMoveStyle)>> ActionFrameData;

    //    public AnimationData(T component, List<int> framestamps, Dictionary<int, Dictionary<string, (object, AnimationMoveStyle)>> frameData) {
    //        ComponentID = component.GUID;
    //        Framestamps = new List<int>(framestamps);

    //        _currentFrame = 0;

    //        ActionFrameData = new Dictionary<int, Dictionary<string, (object, AnimationMoveStyle)>>(frameData);
    //    }

    //    public Dictionary<string, object> Update(int frame) {
    //        if (frame == 0) _currentFrame = 0;


    //        Dictionary<string, object> retDict = new Dictionary<string, object>();

    //        if (Framestamps[_currentFrame] == frame) {
    //            //update values to the real ones

    //            _currentFrame++;
    //        } else {
    //            //look through the update types and partially change them that way
    //            int next = _currentFrame + 1 >= Framestamps.Count ? 0 : _currentFrame + 1;
    //            float frac = (float)frame / (Framestamps[next] - Framestamps[_currentFrame]); //lerp value to use, if needed


    //            foreach(KeyValuePair<string, (object, AnimationMoveStyle)> data in ActionFrameData[_currentFrame]) {
    //                object o;

    //                switch (data.Value.Item1) {
    //                    case float f:
    //                        o = MathF.le
    //                        break;

    //                    default:
    //                        throw new Exception("Property/Field type unable to matched.");
    //                }

    //                retDict.Add(data.Key, o);
    //            }
    //        }
    //    }
    //}

    //public struct AnimationNode<T> where T : struct {
    //    public delegate T ReturnT(T value1, T value2, float t);

    //    public int LastFramestamp;
    //    public int Framestamp;
    //    public AnimationMoveStyle MoveStyle;
    //    public T DataValue;

    //    public AnimationNode(int lastFramestamp, int framestamp, T data, AnimationMoveStyle moveStyle) {
    //        Framestamp = framestamp;
    //        LastFramestamp = lastFramestamp;
    //        MoveStyle = moveStyle;
    //        DataValue = data;

    //        switch (data) {
    //            case float f:
    //                Lerp = Lerp_Float;
    //                break;
    //        }

    //    }

    //    public ReturnT Lerp;
    //    public ReturnT SmoothStep;



    //    public static float Lerp_Float(T value1, T value2, float lerpValue) {
    //        return MathHelper.Lerp((float)value1, (float)value2, lerpValue);
    //    }

    //}



    //animframe count = the frames of each full cycle of the animaiton
    //a list of components that you want to be affected by the animation
    //for each component, a list of property adjustments, framestamps that it should be at that value at, and the movetowards type

    #endregion
}
