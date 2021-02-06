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


        public override void OnTriggerEnter2D(Collider2D other) {
            if(other.gameObject.Layer == (int)LayerID.Player && !GameManager.Map.ChangingRooms) {
                Debug.Log("Player entered door");
                StartCoroutine(RoomTransition(DoorDirection, CameraMoveStyle.Slide));
            }
        }

        private IEnumerator RoomTransition(Direction doorDirection, CameraMoveStyle camMoveStyle) {
            GameManager.Map.ChangingRooms = true;

            Point additive;
            switch (doorDirection) {
                default:
                case Direction.Up:
                    additive = new Point(0, -1);
                    break;
                case Direction.Down:
                    additive = new Point(0, 1);
                    break;
                case Direction.Left:
                    additive = new Point(1, 0);
                    break;
                case Direction.Right:
                    additive = new Point(-1, 0);
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
                    float timer_max = 0.7f; // Time the camera slides
                    float timer = timer_max;
                    Vector3 origPos = Camera.main.transform.Position;

                    while(timer > 0) {
                        timer -= Time.deltaTime;
                        yield return null;
                        Camera.main.transform.Position = Vector3.Lerp(nextRoom.transform.Position, origPos, timer / timer_max);
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

            // teleport player to opposite door point
            GameManager.PlayerTransform.Position = GetOppositeDoorPosition(nextRoom.transform, doorDirection);

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
            switch (doorDirection) {
                default:
                case Direction.Down:
                    return nextRoom.TransformPoint(new Vector3(0, 80, 0)); // Up door
                case Direction.Up:
                    return nextRoom.TransformPoint(new Vector3(0, -80, 0)); // Down door
                case Direction.Left:
                    return nextRoom.TransformPoint(new Vector3(120, 0, 0)); // Right door
                case Direction.Right:
                    return nextRoom.TransformPoint(new Vector3(-120, 0, 0)); // Left door
            }
        }
    }
}