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

                    while(timer > 0) {
                        timer -= Time.deltaTime;
                        yield return new WaitForEndOfFrame();
                        Camera.main.transform.Position = Vector3.SmoothStep(nextRoom.transform.Position, origPos, timer / timer_max);

                        if(!playerTeleported && timer <= timer_max / 2f) {
                            // teleport player to opposite door point
                            GameManager.PlayerTransform.Position = GetOppositeDoorPosition(nextRoom.transform, doorDirection);
                            playerTeleported = true;
                        }
                    }

                    yield return new WaitForEndOfFrame();
                    Camera.main.transform.Position = nextRoom.transform.Position;

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
    }
}