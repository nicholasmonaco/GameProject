using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using GameProject.Code.Scripts.Util;
using GameProject.Code.Scripts.Components.Entity;
using GameProject.Code.Prefabs;
using GameProject.Code.Pipeline;
using GameProject.Code.Prefabs.MapGen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

namespace GameProject.Code.Scripts.Components {
    public class Room : Component {
        public Room(GameObject attached) : base(attached) { }

        //epic music riff is the background of Division Ruine

        // Each room has:
        //  Texture ID
        //  Corner texture ids
        //  Doors/door fillers
        //  Hole/Obstacle layout
        //  Enemies
        //  Entities
        //  Prespawned Pickups
        //  Loot pickups
        //  Entered
        //  Beaten
        //  RevealedOnMinimap
        //  MinimapIcon

        public Point ObstacleTilemapSize = new Point(13, 7);
        public static Vector2 ObstacleTileSize = new Vector2(26, 26);


        public Point GridPos;

        public LevelID TextureID = 0;
        public int[] CornerTextureIDs;
        public Dictionary<Direction, DoorController> Doors;
        private Dictionary<Direction, GameObject> _doorFillers;

        public TileMap<ObstacleID> ObstacleTilemap;

        public List<AbstractEnemy> Enemies;
        public List<AbstractEntity> Entities; //this includes pickups that are prespawned
        public List<Pickup> Loot;

        public bool Entered = false;
        public bool Beaten = false;
        public bool RevealedOnMinimap = false;

        public RoomType RoomType = RoomType.Normal;
        public RoomStyle RoomStyle = RoomStyle.QuarantineLevel_01;




        public void GenerateRoom() {
            Doors = new Dictionary<Direction, DoorController>(4);
            _doorFillers = new Dictionary<Direction, GameObject>(4);
            RoomType = RoomType.Normal;

            Enemies = new List<AbstractEnemy>();

            // Make base room stuff
            for (int x = -1; x < 2; x += 2) {
                for (int y = -1; y < 2; y += 2) {
                    GameObject wallCorner = Instantiate<GameObject>();
                    wallCorner.Name = "WallCorner";
                    wallCorner.transform.Parent = transform;
                    wallCorner.transform.LocalPosition = new Vector3(x * 117, y * 78, 0);
                    //wallCorner.transform.Scale = new Vector3(-x, y, 0);
                    wallCorner.Layer = LayerID.EdgeWall;

                    SpriteRenderer wallCornerRend = wallCorner.AddComponent<SpriteRenderer>();
                    wallCornerRend.Sprite = GetRandomCornerTexture(RoomStyle.QuarantineLevel_01);
                    wallCornerRend.DrawLayer = DrawLayer.ID[DrawLayers.Background];
                    wallCornerRend.OrderInLayer = 21;
                    wallCornerRend.SpriteScale = new Vector2(-x, y);

                    //Vector2[] cornerPolygonPoints = new Vector2[] { new Vector2(-110, 70),
                    //                                                new Vector2(97, 70),
                    //                                                new Vector2(97, 55),
                    //                                                new Vector2(100, 28),
                    //                                                new Vector2(-67, 28) };
                    //wallCorner._components.Add(new PolygonCollider2D(wallCorner, cornerPolygonPoints, false));

                    //cornerPolygonPoints = new Vector2[] { new Vector2(-110, 70),
                    //                                      new Vector2(-67, 28),
                    //                                      new Vector2(-67, -61),
                    //                                      new Vector2(-92, -58),
                    //                                      new Vector2(-110, -58) };
                    //wallCorner._components.Add(new PolygonCollider2D(wallCorner, cornerPolygonPoints, false));

                    RectCollider2D cornerColl_1 = wallCorner.AddComponent<RectCollider2D>(234 * 0.87f, 40, 0, 46 * y);
                    RectCollider2D cornerColl_2 = wallCorner.AddComponent<RectCollider2D>(35, 94, 83 * x, -20*y);
                }
            }

            

        }

        public void ResetType(RoomType type, RoomStyle style, DoorType doorType) {
            RoomType = type;
            RoomStyle = style;
            
            //change corner pictures
            
            //change door sprites
            foreach(Direction d in Doors.Keys) {
                if (Resources.Sprites_DoorFrames.ContainsKey(doorType)) {
                    Doors[d].SwitchDoorType(doorType);
                    GameManager.Map.RoomGrid[GridPos + d.GetDirectionPoint()].Doors[d.InvertDirection()].SwitchDoorType(doorType);
                } else {
                    Doors[d].SwitchDoorType(DoorType.Normal);
                    GameManager.Map.RoomGrid[GridPos + d.GetDirectionPoint()].Doors[d.InvertDirection()].SwitchDoorType(DoorType.Normal);
                }
            }
        }

        public void SetDoor(Direction dir, bool isDoor) {
            //Direction dir = (Direction)i;
            float rotation = 0;
            Vector3 pos = new Vector3(206, 128, 0);
            switch (dir) {
                default:
                case Direction.Up:
                    pos.X = 0;
                    break;
                case Direction.Down:
                    pos.X = 0;
                    pos.Y *= -1;
                    rotation = 180;
                    break;
                case Direction.Right:
                    pos.Y = 0;
                    rotation = 270;
                    break;
                case Direction.Left:
                    pos.Y = 0;
                    pos.X *= -1;
                    rotation = 90;
                    break;
            }

            // Make sure to not make a door if there is literally no room in that direction at all.
            //if (GameManager.Map.RoomAtGridCoords(GridPos + gridDir)) {
            if (isDoor) {
                if (Doors.ContainsKey(dir)) return;

                // Generate door if relevant
                DoorController door = Instantiate<Prefab_Door>(Vector3.Zero, transform).GetComponent<DoorController>();
                door.InitDoor(pos, rotation, dir);

                if (_doorFillers.ContainsKey(dir)) {
                    Destroy(_doorFillers[dir]);
                    _doorFillers.Remove(dir);
                }
                Doors.Add(dir, door);

            } else {
                if (_doorFillers.ContainsKey(dir)) return;
                
                GameObject doorFiller = Instantiate<GameObject>(Vector3.Zero, transform);
                doorFiller.Name = "Door Filler";
                doorFiller.transform.Parent = transform;
                doorFiller.transform.LocalPosition = pos;
                doorFiller.transform.Rotation = rotation;

                //Vector2[] doorBounds = new Vector2[] { new Vector2(-20, 3.5f), new Vector2(20, 3.5f), new Vector2(11, -22), new Vector2(-11, -22) };
                //PolygonCollider2D pc = doorFiller._components.AddReturn(new PolygonCollider2D(doorFiller, doorBounds, false)) as PolygonCollider2D;
                RectCollider2D fillerCollider = doorFiller.AddComponent<RectCollider2D>(30, 35, 0, -6.5f);
                fillerCollider.IsTrigger = false;
                doorFiller.Layer = LayerID.EdgeWall;

                if (Doors.ContainsKey(dir)) {
                    Destroy(Doors[dir].gameObject);
                    Doors.Remove(dir);
                } 
                _doorFillers.Add(dir, doorFiller);
            }
        }


        
        public void LoadLayout() {
            // Get room layout data
            RoomData data = GetRandomRoomData(Doors.ContainsKey(Direction.Up),
                                              Doors.ContainsKey(Direction.Down),
                                              Doors.ContainsKey(Direction.Left),
                                              Doors.ContainsKey(Direction.Right));

            if (RoomType == RoomType.Item) GenerateItem();

            // If no room is found
            if (data.RoomID == -99999) return;

            
            SetObstacleTiles(data.ObstacleData);

            SetMapAction += () => { SetEntities(data.EntityData); };

            if(RoomType == RoomType.Normal) GenerateRandomLoot();
        }

        /// <summary>
        /// Loads a room layout appropriate for the current room, dependent on level.
        /// </summary>
        /// <param name="upDoor">If the room needs a up door</param>
        /// <param name="downDoor">If the room needs a down door</param>
        /// <param name="leftDoor">If the room needs a left door</param>
        /// <param name="rightDoor">If the room needs a right door</param>
        /// <returns>A random room layout with at least the nessecary doors</returns>
        private RoomData GetRandomRoomData(bool upDoor, bool downDoor, bool leftDoor, bool rightDoor) {
            if (!Resources.CurRooms_All.ContainsKey(RoomType)) {
                RoomData failData = new RoomData();
                failData.RoomID = -99999;
                return failData;
            }

            HashSet<int> sharedKeys = new HashSet<int>(Resources.CurRooms_All[RoomType].Keys);

            if (upDoor) sharedKeys.IntersectWith(Resources.CurRooms_Up[RoomType].Keys);
            if (downDoor) sharedKeys.IntersectWith(Resources.CurRooms_Down[RoomType].Keys);
            if (leftDoor) sharedKeys.IntersectWith(Resources.CurRooms_Left[RoomType].Keys);
            if (rightDoor) sharedKeys.IntersectWith(Resources.CurRooms_Right[RoomType].Keys);


            if(sharedKeys.Count != 0) {
                int[] keyArr = new int[sharedKeys.Count];
                sharedKeys.CopyTo(keyArr);

                return Resources.CurRooms_All[RoomType][keyArr[GameManager.WorldRandom.Next(0, keyArr.Length)]];
            } else {
                RoomData failData = new RoomData();
                failData.RoomID = -99999;
                return failData;
            }
            
        }

        public void CheckClear() {
            if(Beaten == false && Enemies.Count == 0) {
                Beaten = true;
                StartCoroutine(Clear_C());
            }
        }

        private IEnumerator Clear_C() {
            yield return new WaitForSeconds(0.5f);

            //drop loot
            if(Loot != null && Loot.Count > 0) {
                foreach (Pickup p in Loot) {
                    GameObject pickup = Instantiate(new Prefab_PickupGeneric(p));
                    pickup.transform.Parent = transform;
                    pickup.transform.LocalPosition = Vector3.Zero; //change this later to find the nearest clear spot(s) from the center of the room
                }
            }

            //open doors
            foreach(DoorController door in Doors.Values) {
                door.OpenDoor();
            }
        }

        public void CloseDoors() {
            foreach (DoorController door in Doors.Values) {
                door.CloseDoor();
            }
        }


        public void SetEntities(int[,] rawEntityMap) {
            for (int y = 0; y < 7; y++) {
                for (int x = 0; x < 13; x++) {
                    EntityID ent = GetRealEntityID(rawEntityMap[x, y]);
                    int entID = (int)ent;

                    if (ent == EntityID.None) continue;

                    Vector2 offset = new Vector2(13f, 7f) * -13;

                    // If it's an enemy
                    if (entID >= 301 && entID < 500) {
                        AbstractEnemy enemy = Instantiate(AbstractEnemy.GetEnemyFromID(ent)).GetComponent<AbstractEnemy>();
                        enemy.transform.Parent = transform;
                        //enemy.transform.LocalPosition = (new Vector2(x, y) * (ObstacleTileSize + Vector2.One) + offset).ToVector3(); //these will probably need a +2 on each
                        enemy.transform.LocalPosition = ((new Vector2(x, y) * (new Vector2(26, 28) + new Vector2(2))) + Vector2.One + offset).ToVector3();

                        enemy.OnDeathFlag = () => {
                            Enemies.Remove(enemy);
                        };
                        enemy.OnDeathFlag += CheckClear;

                        Enemies.Add(enemy);

                    } else { // If it isn't an enemy
                        AbstractEntity entity = Instantiate(AbstractEntity.GetEntityFromID(ent)).GetComponent<AbstractEntity>();
                        entity.transform.Parent = transform;
                        entity.transform.LocalPosition = ((new Vector2(x, y) * (new Vector2(26, 28) + new Vector2(2))) + Vector2.One + offset).ToVector3();

                        Entities.Add(entity);
                    }
                }
            }

        }

        private static EntityID GetRealEntityID(int rawID) {
            // Do this after the entities actually exist
            switch (rawID) {
                case 20:
                    return EntityID.CaveChaser;
                case 21:
                    return EntityID.CaveChaser_Armed;
                case 22:
                    return EntityID.CaveChaser_Buckshot;
                case 23:
                    return EntityID.CaveChaser_Omega;
                case 24:
                    return EntityID.Drone_Attack;
                case 25:
                    return EntityID.Drone_Bugged;
                case 26:
                case 27:
                    return EntityID.CaveChaser;
                case 0:
                default:
                    return EntityID.None;
                //case 
            }
        }



        public void SetObstacleTiles(int[,] rawObstacleMap) {
            // Note: If for some reason we use tilemaps somewhere else, this is the way to do it.

            gameObject.Layer = LayerID.Obstacle; //this is probably a bad way to do this
            GameObject obstacleMapHolder = Instantiate<GameObject>(transform.Position, transform);
            obstacleMapHolder.transform.Position = transform.Position;
            Debug.Log($"holderpos: {obstacleMapHolder.transform.Position}");
            obstacleMapHolder.Layer = LayerID.Obstacle;

            ObstacleTilemap = obstacleMapHolder.AddComponent<TileMap<ObstacleID>>();
            
            ObstacleTilemap.TileChangeAction = (tile, parentMap) => {
                tile.TileRenderer.Sprite = GetCorrectObstacleSprite(tile.Data);

                if (parentMap.ColliderMap[tile.TilemapPos.X, tile.TilemapPos.Y] != null) {
                    parentMap.ColliderMap[tile.TilemapPos.X, tile.TilemapPos.Y].Destroy();
                    parentMap.ColliderMap[tile.TilemapPos.X, tile.TilemapPos.Y] = null;
                }

                //need to change this later to give it other properties
                if (ObstacleCollidable(tile.Data)) {
                    //Collider2D newTileCollider = parentMap.gameObject.AddComponent<RectCollider2D>(tile.TileRenderer);

                    Collider2D newTileCollider = obstacleMapHolder.AddComponent<RectCollider2D>(
                        tile.TileRenderer.Sprite.Width * tile.TileRenderer.SpriteScale.X,
                        tile.TileRenderer.Sprite.Height * tile.TileRenderer.SpriteScale.Y,
                        parentMap.transform.Position.X + tile.TileRenderer.SpriteOffset.X,
                        parentMap.transform.Position.Y + tile.TileRenderer.SpriteOffset.Y,
                        false
                    );

                    newTileCollider.Bounds.ResolveCorners();

                    //maybe add a method here to see what we need to add to it
                    parentMap.ColliderMap[tile.TilemapPos.X, tile.TilemapPos.Y] = newTileCollider;

                }else if (ObstacleDamaging(tile.Data)) {
                    Vector2 pos = new Vector2(parentMap.transform.Position.X + tile.TileRenderer.SpriteOffset.X,
                                              parentMap.transform.Position.Y + tile.TileRenderer.SpriteOffset.Y);

                    Collider2D newTileCollider = obstacleMapHolder.AddComponent<CircleCollider2D>(pos, 9.85f, false);

                    newTileCollider.IsTrigger = true;

                    newTileCollider.OnTriggerStay2D_Direct += PlayerController.DamagePlayer;

                    parentMap.ColliderMap[tile.TilemapPos.X, tile.TilemapPos.Y] = newTileCollider;
                }
            };

            ObstacleID[,] realMap = new ObstacleID[13, 7];
            for(int y = 0; y < 7; y++) {
                for(int x = 0; x < 13; x++) {
                    realMap[x, y] = GetRealObstacleID(rawObstacleMap[x, 6-y]);
                }
            }

            Vector2 mapOffset = new Vector2(13, 7) * -13;

            SetMapAction += () => {
                ObstacleTilemap.SetMap(realMap, ObstacleTilemapSize.X, ObstacleTilemapSize.Y, new Vector2(26, 28), Vector2.One, mapOffset);
                Debug.Log($"Transform: {ObstacleTilemap.transform.Position} ");
                SetMapAction = () => { };
            };
        }

        public Action SetMapAction = () => { };

        private static bool ObstacleCollidable(ObstacleID id) {
            return (int)id >= 1 && (int)id <= 31;
        }

        private static bool ObstacleDamaging(ObstacleID id) {
            return (int)id >= 41 && (int)id <= 43;
        }

        public static bool ObstacleSolid(ObstacleID id) {
            return (int)id >= 11 && (int)id <= 31;
        }

        public ObstacleID GetObstacleAtPos(Vector2 position) {
            Vector2 mapOffset = new Vector2(13, 7) * -13;
            Vector2 tilepoint = (position - transform.Position.ToVector2() - mapOffset) / new Vector2(28, 30);
            Point p = tilepoint.ToPoint();
            return ObstacleTilemap.GetTile(p);
        }


        private static Texture2D GetCorrectObstacleSprite(ObstacleID id) {
            if (Resources.Sprites_GlobalObstacles.ContainsKey(id)) {
                return Resources.Sprites_GlobalObstacles[id];
            } else if(Resources.Sprites_Obstacles[GameManager.CurLevelID].ContainsKey(id)) {
                return Resources.Sprites_Obstacles[GameManager.CurLevelID][id];
            } else {
                return Resources.Sprite_Invisible;
            }
        }

        private static ObstacleID GetRealObstacleID(int rawID) {
            switch (rawID) {
                default:
                case 0:
                    return ObstacleID.None;
                case 2:
                    return ObstacleID.Hole;
                case 3:
                    return (ObstacleID)GameManager.WorldRandom.Next(11, 17); // Get a random rock (change later to replace randomly based on chance)
                case 4:
                    return ObstacleID.Wall;
                case 5:
                    return (ObstacleID)GameManager.WorldRandom.Next(41, 44); // Get a random spike
            }
        }


        private void GenerateRandomLoot() {
            float lootRNG = GameManager.WorldRandom.NextValue(0, 100);
            int lootCount;
            if (lootRNG <= 70 && lootRNG > 30) {
                lootCount = 1;
            } else if (lootRNG > 10) {
                lootCount = 2;
            } else if (lootRNG > 1) {
                lootCount = 3;
            } else {
                lootCount = 4;
            }

            Loot = new List<Pickup>(lootCount);
            for(int i = 0; i < lootCount; i++) {
                Loot.Add(GetRandomLootPickup());
            }
        }

        private Pickup GetRandomLootPickup() {
            //todo
            return Pickup.Coin;
        }

        private void GenerateItem() {
            GameObject itemObj = Instantiate(new Prefab_WorldItem(WorldItem.GetRandomItem(ItemPool.Item)));
            itemObj.transform.Parent = transform;
            itemObj.transform.LocalPosition = Vector3.Zero;

            //WorldItem item = Instantiate(itemObj).GetComponent<WorldItem>();

            Debug.Log($"Spawning item at {GridPos}");
        }

        
        




        public void ClearDoors() {
            foreach(DoorController d in Doors.Values) {
                Destroy(d.gameObject);
            }
            Doors.Clear();

            foreach(GameObject filler in _doorFillers.Values) {
                Destroy(filler);
            }
            _doorFillers.Clear();
        }

        public override void OnDestroy() {
            ClearDoors();
        }



        public void FillEmptyDoorSlots() {
            List<Direction> possible = new List<Direction>() { Direction.Up, Direction.Down, Direction.Left, Direction.Right };
            foreach(Direction d in Doors.Keys) {
                possible.Remove(d);
            }

            foreach(Direction d in possible) {
                SetDoor(d, false);
            }
        }

        public void GenerateExtraDoors() {
            List<Direction> possible = new List<Direction>() { Direction.Up, Direction.Down, Direction.Left, Direction.Right };
            
            foreach(Direction d in Doors.Keys) {
                possible.Remove(d);
            }

            foreach(Direction d in possible) {
                SetDoor(d, GameManager.WorldRandom.Next(0, 3) == 0 ? false : true);
            }
        }

        public void RegenerateFalseDoors() {
            List<Direction> possible = new List<Direction>() { Direction.Up, Direction.Down, Direction.Left, Direction.Right };

            foreach (Direction d in Doors.Keys) {
                if (!possible.Contains(d)) {
                    SetDoor(d, GameManager.WorldRandom.Next(0, 2) == 0 ? false : true);
                }
            }
        }

        public void ForceSetDoors(int doorCount) {
            //change this to genereate between doorCount and 4 rooms
            int leftToGen = doorCount;
            List<Direction> possible = new List<Direction>() { Direction.Up, Direction.Down, Direction.Left, Direction.Right };

            while (leftToGen > 0) {
                int index = GameManager.WorldRandom.Next(0, possible.Count);
                Direction d = possible[index];
                possible.RemoveAt(index);

                SetDoor(d, true);

                leftToGen--;
            }
        }

        public bool CanGenerateInAnyDirection() {
            foreach(Direction dir in Doors.Keys) {
                if(GameManager.Map.RoomGrid.ContainsKey(GridPos + dir.GetDirectionPoint())) {
                    return true;
                }
            }

            return false;
        }

        public void DeleteUnconnectedDoors() {
            foreach(Direction dir in Doors.Keys) {
                if (!GameManager.Map.RoomAtGridCoords(GridPos + dir.GetDirectionPoint())) {
                    Destroy(Doors[dir].gameObject);
                    Doors.Remove(dir);

                    
                    //roll to see if we want a connector there if there's also a door in the other room?
                }else if (!GameManager.Map.RoomGrid[GridPos + dir.GetDirectionPoint()].Doors.ContainsKey(dir.InvertDirection())) {
                    Destroy(Doors[dir].gameObject);
                    Doors.Remove(dir);
                }
            }

            HashSet<Direction> possible = new HashSet<Direction>() { Direction.Up, Direction.Down, Direction.Left, Direction.Right };
            foreach(Direction dir in possible) {
                if (!Doors.ContainsKey(dir)) {
                    SetDoor(dir, false);
                }
            }
        }


        public static Texture2D GetRandomCornerTexture(RoomStyle style) {
            List<Texture2D> list = Resources.Sprites_RoomCorners[style];
            return list[GameManager.WorldRandom.Next(0, list.Count)];
        }

    }
}