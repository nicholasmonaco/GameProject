using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using GameProject.Code.Core.UI;
using GameProject.Code.Scripts.Util;
using GameProject.Code.Scripts.Components.UI;
using GameProject.Code.Prefabs;

namespace GameProject.Code.Scripts.Components {
    public class DoorController : Component {

        enum CameraMoveStyle { Slide, Instant }

        public DoorController(GameObject attached) : base(attached) { }


        public Direction DoorDirection;
        private DoorType _doorType = DoorType.Normal;
        
        public SpriteRenderer FrameRenderer;
        public SpriteRenderer InsideRenderer;
        public SpriteRenderer DoorRenderer;
        private SpriteRenderer _secondaryRenderer;

        public Collider2D DoorCollider { private get; set; }
        public Collider2D FillerCollider { private get; set; }


        private DoorState _curState = DoorState.Open;
        private bool _closed = false;



        public void InitDoor(Vector3 localPos, float rotation, Direction doorDirection) {
            transform.LocalPosition = localPos;
            transform.Rotation2D = rotation;

            DoorDirection = doorDirection;
        }

        public void SwitchDoorType(DoorType newType) {
            switch (_doorType) {
                default:
                    break;
                case DoorType.Boss:
                    Destroy(_secondaryRenderer.gameObject);
                    break;
            }

            _doorType = newType;
            FrameRenderer.Sprite = Resources.Sprites_DoorFrames[newType];

            switch (newType) {
                default:
                    break;
                case DoorType.Boss:
                    GameObject eyes = Instantiate<GameObject>(transform.Position, transform);
                    _secondaryRenderer = eyes.AddComponent<SpriteRenderer>();
                    _secondaryRenderer.DrawLayer = FrameRenderer.DrawLayer;
                    _secondaryRenderer.OrderInLayer = 27;
                    _secondaryRenderer.Material.BatchID = BatchID.Room;
                    eyes.transform.Rotation2D = transform.Rotation2D;
                    eyes.transform.LocalPosition += (DoorDirection.GetDirectionPoint().ToVector2() * new Vector2(12, 12)).ToVector3();

                    SpriteAnimator anim = eyes.AddComponent<SpriteAnimator>();
                    anim.InitAnimation(_secondaryRenderer, Resources.Sprites_BossDoorEyeAnim.Count);
                    for(int i = 0; i < Resources.Sprites_BossDoorEyeAnim.Count; i++) {
                        anim.AddFrame(Resources.Sprites_BossDoorEyeAnim[i], 0.4f); //duration of 0.4 seconds per frame
                    }
                    anim.StartAnimating_Ponging();

                    GameObject glow = Instantiate<GameObject>(transform.Position, transform);
                    SpriteRenderer _glowRend = glow.AddComponent<SpriteRenderer>();
                    _glowRend.Sprite = Resources.Sprite_BossDoorGlow;
                    _glowRend.DrawLayer = FrameRenderer.DrawLayer;
                    _glowRend.OrderInLayer = 27;
                    _glowRend.Material.BatchID = BatchID.Room;
                    glow.transform.Rotation2D = transform.Rotation2D;
                    glow.transform.LocalPosition += (DoorDirection.GetDirectionPoint().ToVector2() * new Vector2(-38)).ToVector3();

                    InsideRenderer.Sprite = Resources.Sprite_Door_Inside_Boss;

                    break;
            }
        }


        public void OpenDoor() {
            if (_curState != DoorState.Closed) return;

            StartCoroutine(Open_C());            
        }

        public void CloseDoor() {
            if (_curState != DoorState.Open) return;

            StartCoroutine(Close_C());
        }

        private IEnumerator Open_C() {
            //play opening animation
            float frameDur = 0.05f;

            for(int i = 1; i < Resources.Sprites_DoorCloseFrames.Count; i++) {
                yield return new WaitForSeconds(frameDur);
                DoorRenderer.Sprite = Resources.Sprites_DoorCloseFrames[i];
            }

            yield return new WaitForSeconds(frameDur);
            DoorRenderer.Sprite = Resources.Sprite_Invisible;


            //enable real door collider
            DoorCollider.Enabled = true;
            //disable blocking collider
            FillerCollider.Enabled = false;

            _curState = DoorState.Open;
        }

        private IEnumerator Close_C() {
            //disable real door collider
            DoorCollider.Enabled = false;
            //enable blocking collider
            FillerCollider.Enabled = true;

            _curState = DoorState.Closed;


            //play closing animation
            float frameDur = 0.05f;

            for (int i = Resources.Sprites_DoorCloseFrames.Count - 1; i >= 0; i--) {
                yield return new WaitForSeconds(frameDur);
                DoorRenderer.Sprite = Resources.Sprites_DoorCloseFrames[i];
            }

            Resources.Sound_Doors_Close.Play(GameManager.RealSoundVolume);
        }



        public override void OnTriggerStay2D(Collider2D other) {
            if(other.gameObject.Layer == LayerID.Player && !GameManager.Map.ChangingRooms) {
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

            // delete all bullets
            while(GameManager.BulletHolder._children.Count > 0) {
                Destroy(GameManager.BulletHolder._children[0]);
            }

            // load entered room
            Room nextRoom = GameManager.Map.LoadRoom(GameManager.Map.CurrentGridPos + additive);

            RoomType lastRoomType = GameManager.Map.CurrentRoom.RoomType;

            // stop player movement
            GameManager.Player.FreezeMovement = true;

            //kinda debug, kinda not
            CameraMoveStyle moveStyle = nextRoom.RoomType == RoomType.Boss ? CameraMoveStyle.Instant : CameraMoveStyle.Slide;
            //

            // slide camera one room distance in the direction if TransitionType is slide
            // teleport camera one room distance in the direction if TransitionType is teleport
            switch (moveStyle) {
                case CameraMoveStyle.Slide:
                    float timer_max = 0.3f; // Time the camera slides
                    float timer = timer_max;
                    bool playerTeleported = false;
                    Vector3 origPos = Camera.main.transform.Position;
                    Point mult = ((new Point(additive.X * -1, additive.Y * -1)) - GameManager.Map.CurrentGridPos) * MinimapController.MinimapIconSize;
                    Vector3 origMMPos = GameManager.Minimap.transform.LocalPosition;
                    Vector3 nextMMPos = mult.ToVector2().ToVector3() * GameManager.Minimap.transform.LocalScale;

                    while (timer > 0 && !GameManager.LevelResetting) {
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

                    if (GameManager.LevelResetting) {
                        GameManager.Player.FreezeMovement = false;
                        GameManager.Map.ChangingRooms = false;

                        yield break;
                    }

                    yield return new WaitForEndOfFrame();
                    Camera.main.transform.Position = nextRoom.transform.Position;
                    GameManager.Minimap.transform.LocalPosition = nextMMPos;

                    break;
                default:
                case CameraMoveStyle.Instant:
                    yield return null;
                    Camera.main.transform.Position = nextRoom.transform.Position;
                    GameManager.PlayerTransform.Position = GetOppositeDoorPosition(nextRoom.transform, doorDirection);
                    break;
            }


            // Versus screen check
            bool versusScreenActive = false;
            if (nextRoom.RoomType == RoomType.Boss) {
                //do boss cutscene
                /*yield return */
                versusScreenActive = true;
                StartCoroutine(VersusScreen());
            }


            // switch music (if applicable)
            if (nextRoom.RoomType != lastRoomType) { //replace this with a more intuitive version later
                if (nextRoom.RoomType == RoomType.Item) {
                    GameManager.ActivateRoomSong(Resources.Music_Store);
                }else if (nextRoom.RoomType == RoomType.Boss) {
                    GameManager.ActivateRoomSong(Resources.Music_MagmaChambers);
                } else {
                    GameManager.DeactivateRoomSong();
                }
            }

            // unload last room
            GameManager.Map.UnloadCurrentRoom();

            // close doors of next room
            nextRoom.CheckClear();
            if (nextRoom.Beaten == false) {
                nextRoom.CloseDoors();
            }


            if(GameManager.LevelResetting == false) {
                // set real current room position
                GameManager.Map.SetCurrentRoomInDirection(additive);

                // update minimap
                GameManager.Minimap.ShiftDirection(GameManager.Map.CurrentGridPos);
            }


            // resume player movement
            if (!versusScreenActive) {
                GameManager.Player.FreezeMovement = false;
            }
            
            // reset checker flag
            GameManager.Map.ChangingRooms = false;
        }

        private IEnumerator VersusScreen() {
            //stop gametime
            float origTimeScale = Time.EntityTimeScale;
            Time.EntityTimeScale = 0;

            GameManager.Player.FreezeMovement = true;

            //add input checker
            bool skip = false;
            Action inputCheck = () => { skip = true; };
            Input.OnAnyKey_Down += inputCheck;

            //black overlay active
            Panel backroundPanel = Instantiate<Prefab_Panel>(Vector3.Zero, GameManager.MainCanvas.transform).GetComponent<Panel>();
            backroundPanel.transform.LocalPosition = Vector3.Zero;
            backroundPanel.SetColor(new Color(49, 15, 77));
            backroundPanel.SetOpacity(1);

            GameObject vsText = Instantiate<GameObject>(Vector3.Zero, backroundPanel.transform);
            vsText.transform.LocalPosition = Vector3.Zero;
            //vsText.transform.UIParentFlag = true;
            TextRenderer tr = vsText.AddComponent<TextRenderer>();
            tr.SetFont(GameFont.Debug);
            tr.SpriteScale *= 0.23f;
            tr.Text = "ENEMY APPROACHES";
            tr.Material.BatchID = backroundPanel.PanelRenderer.Material.BatchID;
            tr.DrawLayer = backroundPanel.PanelRenderer.DrawLayer;
            tr.OrderInLayer = backroundPanel.PanelRenderer.OrderInLayer + 1;


            //prepare while loop variables
            bool displayUp = false;

            Panel topBar = Instantiate<Prefab_Panel>(Vector3.Zero, GameManager.MainCanvas.transform).GetComponent<Panel>();
            topBar.rectTransform.VerticalAlignment = VerticalStick.Top;
            topBar.rectTransform.LocalPosition = Vector3.Zero;
            topBar.rectTransform.LocalScale *= new Vector3(1, 0.35f, 1);
            topBar.SetColor(Color.Black);
            topBar.SetOpacity(1);
            topBar.AddToOrder(1);

            Panel bottomBar = Instantiate<Prefab_Panel>(Vector3.Zero, GameManager.MainCanvas.transform).GetComponent<Panel>();
            bottomBar.rectTransform.VerticalAlignment = VerticalStick.Bottom;
            bottomBar.rectTransform.LocalPosition = Vector3.Zero;
            bottomBar.rectTransform.LocalScale *= new Vector3(1, 0.35f, 1);
            bottomBar.SetColor(Color.Black);
            bottomBar.SetOpacity(1);
            bottomBar.AddToOrder(1);

            Transform playerImage;
            Transform bossImage;
            Transform versusText;


            //play ambience
            //GameManager.ActivateOverlayRoomSong(Resources.VersusAmbience);

            //enter while loop: While input not pressed
            while (!skip) {
                if (!displayUp) {
                    //slide in top and bottom bar things
                    //slide in player imge, boss image, versus text, play ambience
                }

                yield return null;
            }

            //once input pressed:
            //remove input checker
            Input.OnAnyKey_Down -= inputCheck;

            

            //fade in white overlay really fast


            //once all white, remove all graphics
            Destroy(backroundPanel.gameObject);
            Destroy(topBar.gameObject);
            Destroy(bottomBar.gameObject);
            //Destroy(playerImage.gameObject);
            //Destroy(bossImage.gameObject);
            //Destroy(versusText.gameObject);

            //hold for one frame
            yield return new WaitForEndOfFrame();

            //fade white back to invisible


            //disable versus ambience
            GameManager.DeactivateOverlayRoomSong();

            //resume time
            Time.EntityTimeScale = origTimeScale;

            // Resume player movement
            GameManager.Player.FreezeMovement = false;
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

    public enum DoorState {
        Closed,
        Open,
        Locked,
        Locked_Double,
        Bricked,
        Bricked_Double,
        MoneyLock,
        Hidden,
        Destroyed
    }
}