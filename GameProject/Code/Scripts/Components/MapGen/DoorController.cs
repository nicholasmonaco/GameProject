using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Microsoft.Xna.Framework;
using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using GameProject.Code.Scripts.Util;

namespace GameProject.Code.Scripts.Components {
    public class DoorController : Component {

        enum CameraMoveStyle { Slide, Instant }

        public DoorController(GameObject attached) : base(attached) { }


        public Direction DoorDirection;
        private DoorType _doorType = DoorType.Normal;
        public SpriteRenderer DoorRenderer;
        private SpriteRenderer _secondaryRenderer;


        public void SwitchDoorType(DoorType newType) {
            switch (_doorType) {
                default:
                    break;
                case DoorType.Boss:
                    Destroy(_secondaryRenderer.gameObject);
                    break;
            }

            _doorType = newType;
            DoorRenderer.Sprite = Resources.Sprites_DoorFrames[newType];

            switch (newType) {
                default:
                    break;
                case DoorType.Boss:
                    GameObject eyes = Instantiate<GameObject>(transform.Position, transform);
                    _secondaryRenderer = eyes.AddComponent<SpriteRenderer>();
                    _secondaryRenderer.DrawLayer = DoorRenderer.DrawLayer;
                    _secondaryRenderer.OrderInLayer = 27;
                    eyes.transform.Rotation = transform.Rotation;
                    eyes.transform.LocalPosition += (DoorDirection.GetDirectionPoint().ToVector2() * new Vector2(12, 12)).ToVector3();

                    SpriteAnimator anim = eyes.AddComponent<SpriteAnimator>();
                    anim.InitAnimation(_secondaryRenderer, Resources.Sprites_BossDoorEyeAnim.Count);
                    for(int i=0;i< Resources.Sprites_BossDoorEyeAnim.Count; i++) {
                        anim.AddFrame(Resources.Sprites_BossDoorEyeAnim[i], 0.4f); //duration of 0.4 seconds per frame
                    }
                    anim.StartAnimating_Ponging();
                    break;
            }
        }


        public override void OnTriggerStay2D(Collider2D other) {
            if(other.gameObject.Layer == (int)LayerID.Player && !GameManager.Map.ChangingRooms) {
                StartCoroutine(RoomTransition(DoorDirection, CameraMoveStyle.Slide));
            }
        }

        private IEnumerator RoomTransition(Direction doorDirection, CameraMoveStyle camMoveStyle) {
            GameManager.Map.ChangingRooms = true;

            Point additive;
            switch (doorDirection) {
                default:
                case Direction.Up:
                    additive = new Point(0, 1);
                    break;
                case Direction.Down:
                    additive = new Point(0, -1);
                    break;
                case Direction.Left:
                    additive = new Point(-1, 0);
                    break;
                case Direction.Right:
                    additive = new Point(1, 0);
                    break;
            }

            // load entered room
            Room nextRoom = GameManager.Map.LoadRoom(GameManager.Map.CurrentGridPos + additive);

            // stop player movement
            GameManager.Player.FreezeMovement = true;

            // slide camera one room distance in the direction if TransitionType is slide
            // teleport camera one room distance in the direction if TransitionType is teleport
            switch (camMoveStyle) {
                case CameraMoveStyle.Slide:
                    float timer_max = 0.3f; // Time the camera slides
                    float timer = timer_max;
                    bool playerTeleported = false;
                    Vector3 origPos = Camera.main.transform.Position;
                    Point mult = ((new Point(additive.X * -1, additive.Y * -1)) - GameManager.Map.CurrentGridPos) * MinimapController.MinimapIconSize;
                    Vector3 origMMPos = GameManager.Minimap.transform.LocalPosition;
                    Vector3 nextMMPos = mult.ToVector2().ToVector3() * GameManager.Minimap.transform.LocalScale;

                    while (timer > 0) {
                        timer -= Time.deltaTime;
                        yield return new WaitForEndOfFrame();
                        Camera.main.transform.Position = Vector3.SmoothStep(nextRoom.transform.Position, origPos, timer / timer_max);
                        GameManager.Minimap.transform.LocalPosition = Vector3.SmoothStep(nextMMPos, origMMPos, timer / timer_max);

                        if(!playerTeleported && timer <= timer_max / 2f) {
                            // teleport player to opposite door point
                            GameManager.PlayerTransform.Position = GetOppositeDoorPosition(nextRoom.transform, doorDirection);
                            playerTeleported = true;
                        }
                    }

                    yield return new WaitForEndOfFrame();
                    Camera.main.transform.Position = nextRoom.transform.Position;
                    GameManager.Minimap.transform.LocalPosition = nextMMPos;

                    break;
                default:
                case CameraMoveStyle.Instant:
                    yield return null;
                    Camera.main.transform.Position = nextRoom.transform.Position;
                    break;
            }


            // unload last room
            GameManager.Map.UnloadCurrentRoom();

            // set real current room position
            GameManager.Map.SetCurrentRoomInDirection(additive);

            // update minimap
            GameManager.Minimap.ShiftDirection(GameManager.Map.CurrentGridPos);

            // resume player movement
            GameManager.Player.FreezeMovement = false;

            // reset checker flag
            GameManager.Map.ChangingRooms = false;
        }


        private static Vector3 GetOppositeDoorPosition(Transform nextRoom, Direction doorDirection) {
            // This door direction is the door we will be coming throuh in the next room (i think)
            switch (doorDirection) {
                default:
                case Direction.Down:
                    return nextRoom.TransformPoint(new Vector3(0, 90, 0)); // Up door
                case Direction.Up:
                    return nextRoom.TransformPoint(new Vector3(0, -90, 0)); // Down door
                case Direction.Left:
                    return nextRoom.TransformPoint(new Vector3(168, 0, 0)); // Right door
                case Direction.Right:
                    return nextRoom.TransformPoint(new Vector3(-168, 0, 0)); // Left door
            }
        }


        //public override void OnDestroy() {
        //    base.OnDestroy();
        //    Debug.Log("Destroyed door");
        //}
    }
}