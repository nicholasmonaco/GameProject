using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace GameProject.Code.Core.Components {
    public class SpriteAnimator : Component {
        public SpriteAnimator(GameObject attached) : base(attached) { }


        public List<(Texture2D, float)> AnimationFrames;
        public bool Playing = false;

        private bool _existing = true;
        private SpriteRenderer _attachedRenderer;


        public void InitAnimation(SpriteRenderer attachedRenderer, int frameCount) {
            _attachedRenderer = attachedRenderer;
            AnimationFrames = new List<(Texture2D, float)>(frameCount);
        }

        public void AddFrame(Texture2D sprite, float frameDuration) {
            AnimationFrames.Add((sprite, frameDuration));
        }

        public void StartAnimating() {
            Playing = true;
            StartCoroutine(Animate());
        }

        public void StartAnimating_Ponging() {
            Playing = true;
            StartCoroutine(Animate_Ponging());
        }


        private IEnumerator Animate() {
            _attachedRenderer.Sprite = AnimationFrames[0].Item1;
            float timer = AnimationFrames[0].Item2;
            int animIndex = 0;

            while (_existing) {
                if (Playing) {
                    //todo anim logic
                    timer -= Time.deltaTime;
                    if(timer <= 0) {
                        //switch animations
                        animIndex = animIndex + 1 >= AnimationFrames.Count ? 0 : animIndex + 1;
                        timer = AnimationFrames[animIndex].Item2;
                        _attachedRenderer.Sprite = AnimationFrames[animIndex].Item1;
                    }
                }
                yield return null;
            }
        }

        private IEnumerator Animate_Ponging() {
            _attachedRenderer.Sprite = AnimationFrames[0].Item1;
            float timer = AnimationFrames[0].Item2;
            int animIndex = 0;
            bool playingForward = true;

            while (_existing) {
                if (Playing) {
                    //todo anim logic
                    timer -= Time.deltaTime;

                    if (playingForward) {
                        if (timer <= 0) {
                            //switch animations
                            if(animIndex + 1 >= AnimationFrames.Count) {
                                animIndex--;
                                playingForward = false;
                            } else {
                                animIndex++;
                            }
                            timer = AnimationFrames[animIndex].Item2;
                            _attachedRenderer.Sprite = AnimationFrames[animIndex].Item1;
                        }
                    } else {
                        if (timer <= 0) {
                            //switch animations
                            if (animIndex - 1 < 0) {
                                animIndex++;
                                playingForward = true;
                            } else {
                                animIndex--;
                            }
                            timer = AnimationFrames[animIndex].Item2;
                            _attachedRenderer.Sprite = AnimationFrames[animIndex].Item1;
                        }
                    }
                }
                yield return null;
            }
        }



        public override void OnDestroy() {
            base.OnDestroy();
            _existing = false;
        }

    }
}