using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using GameProject.Code.Scripts.Util;
using GameProject.Code.Scripts.Components.Entity;
using GameProject.Code.Prefabs;
using GameProject.Code.Pipeline;
using GameProject.Code.Prefabs.MapGen;
using GameProject.Code.Prefabs.Enemies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using GameProject.Code.Scenes;

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

        public static Point ObstacleTilemapSize = new Point(13, 7);
        public static Vector2 ObstacleTilemapSize_F = new Vector2(13, 7);
        public static Vector2 ObstacleTileSize = new Vector2(26, 28);
        public static Vector2 ObstacleTileOffset = new Vector2(1, 1);


        public Point GridPos;

        public LevelID TextureID = 0;
        public int[] CornerTextureIDs;
        public Dictionary<Direction, DoorController> Doors;
        private Dictionary<Direction, GameObject> _doorFillers;

        public TileMap<ObstacleID> ObstacleTilemap;
        public List<Collider2D> ObstacleColliders { get; private set; }

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
            Entities = new List<AbstractEntity>();

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
                    wallCornerRend.Material.BatchID = BatchID.Room;

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

            ObstacleColliders = null;

            if (RoomType == RoomType.Item) GenerateItem();
            else if (RoomType == RoomType.Boss) SpawnBoss();

            // If no room is found
            if (data.RoomID == -99999) {
                CreateEmptyObstacleMap();
                return; 
            }

            
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

                    pickup.transform.Position = GetClosestOpenPoint(transform.Position.ToVector2()).ToVector3();

                    AbstractEntity pickupEntity = pickup.GetComponent<AbstractEntity>();
                    pickupEntity.ExtraOnDestroy = () => {
                        Entities.Remove(pickupEntity);
                    };

                    Entities.Add(pickupEntity);
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
            for (int y = 0; y < ObstacleTilemapSize.Y; y++) {
                for (int x = 0; x < ObstacleTilemapSize.X; x++) {
                    EntityID ent = GetRealEntityID(rawEntityMap[x, y]);
                    int entID = (int)ent;

                    if (ent == EntityID.None) continue;

                    //Vector2 offset = new Vector2(13f, 7f) * -13;
                    Vector2 offset = ObstacleTilemapSize_F * -ObstacleTileSize.X / 2f;

                    // If it's an enemy
                    if (entID >= 301 && entID < 500) {
                        AbstractEnemy enemy = Instantiate(AbstractEnemy.GetEnemyFromID(ent)).GetComponent<AbstractEnemy>();
                        enemy.transform.Parent = transform;
                        //enemy.transform.LocalPosition = ((new Vector2(x, y) * (new Vector2(26, 28) + new Vector2(2))) + Vector2.One + offset).ToVector3();
                        enemy.transform.LocalPosition = ((new Vector2(x, y) * (ObstacleTileSize + 2 * ObstacleTileOffset)) + ObstacleTileOffset + offset).ToVector3();

                        enemy.OnDeathFlag = () => {
                            Enemies.Remove(enemy);
                        };
                        enemy.OnDeathFlag += CheckClear;

                        Enemies.Add(enemy);

                    } else { // If it isn't an enemy
                        AbstractEntity entity = Instantiate(AbstractEntity.GetEntityFromID(ent)).GetComponent<AbstractEntity>();
                        entity.transform.Parent = transform;
                        entity.transform.LocalPosition = ((new Vector2(x, y) * (ObstacleTileSize + 2 * ObstacleTileOffset)) + ObstacleTileOffset + offset).ToVector3();

                        entity.ExtraOnDestroy = () => {
                            Entities.Remove(entity);
                        };

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


        private void CreateEmptyObstacleMap() {
            int[,] rawObstacleMap = new int[ObstacleTilemapSize.X, ObstacleTilemapSize.Y];

            for (int y = 0; y < ObstacleTilemapSize.Y; y++) {
                for (int x = 0; x < ObstacleTilemapSize.X; x++) {
                    rawObstacleMap[x, y] = 0;
                }
            }

            SetObstacleTiles(rawObstacleMap);
        }


        public void SetObstacleTiles(int[,] rawObstacleMap) {
            // Note: If for some reason we use tilemaps somewhere else, this is the way to do it.

            gameObject.Layer = LayerID.Obstacle; //this is probably a bad way to do this
            GameObject obstacleMapHolder = Instantiate<GameObject>(transform.Position, transform);
            obstacleMapHolder.Name = "Obstacle Map Holder";
            obstacleMapHolder.transform.Position = transform.Position;
            //Debug.Log($"holderpos: {obstacleMapHolder.transform.Position}");
            obstacleMapHolder.Layer = LayerID.Obstacle;

            ObstacleColliders = new List<Collider2D>();

            ObstacleTilemap = obstacleMapHolder.AddComponent<TileMap<ObstacleID>>();
            
            ObstacleTilemap.TileChangeAction = (tile, parentMap) => {
                tile.TileRenderer.Sprite = GetCorrectObstacleSprite(tile.Data);
                tile.TileRenderer.Material.BatchID = BatchID.Room;

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

                    ObstacleColliders.Add(newTileCollider);

                    //maybe add a method here to see what we need to add to it
                    parentMap.ColliderMap[tile.TilemapPos.X, tile.TilemapPos.Y] = newTileCollider;

                }else if (ObstacleDamaging(tile.Data)) {
                    Vector2 pos = new Vector2(parentMap.transform.Position.X + tile.TileRenderer.SpriteOffset.X,
                                              parentMap.transform.Position.Y + tile.TileRenderer.SpriteOffset.Y);

                    Collider2D newTileCollider = obstacleMapHolder.AddComponent<CircleCollider2D>(pos, 9.85f, false);

                    newTileCollider.IsTrigger = true;

                    newTileCollider.OnTriggerStay2D_Direct += PlayerController.DamagePlayer;

                    ObstacleColliders.Add(newTileCollider);

                    parentMap.ColliderMap[tile.TilemapPos.X, tile.TilemapPos.Y] = newTileCollider;
                }
            };

            ObstacleID[,] realMap = new ObstacleID[13, 7];
            for(int y = 0; y < ObstacleTilemapSize.Y; y++) {
                for(int x = 0; x < ObstacleTilemapSize.X; x++) {
                    realMap[x, y] = GetRealObstacleID(rawObstacleMap[x, 6-y]);
                }
            }

            // Dimensions of the tilemap (in tiles) times half of the width of a tile
            //Vector2 mapOffset = new Vector2(13, 7) * -13;
            Vector2 mapOffset = ObstacleTilemapSize_F * -ObstacleTileSize.X / 2f;

            SetMapAction += () => {
                //ObstacleTilemap.SetMap(realMap, ObstacleTilemapSize.X, ObstacleTilemapSize.Y, new Vector2(26, 28), Vector2.One, mapOffset);
                ObstacleTilemap.SetMap(realMap, ObstacleTilemapSize.X, ObstacleTilemapSize.Y, ObstacleTileSize, Vector2.One, mapOffset);

                //Debug.Log($"Transform: {ObstacleTilemap.transform.Position} ");
                SetMapAction = () => { };
            };
        }

        public Action SetMapAction = () => { };

        public static bool ObstacleCollidable(ObstacleID id) {
            return (int)id >= 1 && (int)id <= 31;
        }

        private static bool ObstacleDamaging(ObstacleID id) {
            return (int)id >= 41 && (int)id <= 43;
        }

        public static bool ObstacleSolid(ObstacleID id) {
            return (int)id >= 11 && (int)id <= 31;
        }

        public ObstacleID GetObstacleAtPos(Vector2 position) {
            //Vector2 mapOffset = new Vector2(13, 7) * -13;
            //Vector2 tilepoint = (position - transform.Position.ToVector2() - mapOffset) / new Vector2(28, 30);

            Vector2 mapOffset = ObstacleTilemapSize_F * -ObstacleTileSize.X / 2f;
            Vector2 tilepoint = (position - transform.Position.ToVector2() - mapOffset) / (ObstacleTileSize + ObstacleTileOffset * 2);
            Point p = tilepoint.ToPoint();
            return ObstacleTilemap.GetTile(p);
        }

        public ObstacleID GetObstacleAtGridPos(Point point) {
            return ObstacleTilemap.GetTile(point);
        }

        
        public Point GetGridPos(Vector3 position) {
            return GetGridPos(position.ToVector2());
        }
        
        public Point GetGridPos(Vector2 position) {
            Vector2 mapOffset = ObstacleTilemapSize_F * -ObstacleTileSize.X / 2f;
            Vector2 tilepoint = (position - transform.Position.ToVector2() - mapOffset) / (ObstacleTileSize + ObstacleTileOffset * 2);
            return tilepoint.ToPoint();
        }

        public Point GetCeilGridPos(Vector2 position) {
            Vector2 mapOffset = ObstacleTilemapSize_F * -ObstacleTileSize.X / 2f;
            Vector2 tilepoint = (position - transform.Position.ToVector2() - mapOffset) / (ObstacleTileSize + ObstacleTileOffset * 2);
            return new Point((int)MathF.Ceiling(tilepoint.X), (int)MathF.Ceiling(tilepoint.Y));
        }

        public Vector2 GetWorldPos(Point gridPos) {
            return ObstacleTilemap.GetWorldPosFromGridPos(gridPos);
        }

        public void RefreshTile(Point gridPos) {
            ObstacleTilemap.RefreshTile(gridPos);
        }

        public void ChangeTile(Point gridPos, ObstacleID newID) {
            ObstacleTilemap.ChangeTile(newID, gridPos.X, gridPos.Y);
            RefreshTile(gridPos);
        }


        public Point GetClosestOpenTilePoint(Point attemptPoint) {
            bool openFound = false;
            int counterMax = ObstacleTilemapSize.X * ObstacleTilemapSize.Y;
            int counter = 0;

            Queue<Point> points = new Queue<Point>();
            points.Enqueue(attemptPoint);

            List<Point> checkedPoints = new List<Point>();
            checkedPoints.Add(attemptPoint);

            Point point;
            Point lastObstacleOpen = attemptPoint;

            do {
                point = points.Dequeue();
                counter++;

                if(GetObstacleAtGridPos(point) == ObstacleID.None) {
                    lastObstacleOpen = point;
                    // Also check that there is no entity there
                    foreach (AbstractEntity entity in Entities) {
                        if (GetGridPos(entity.transform.Position) == point) goto NotOpen;
                    }

                    openFound = true;
                    break;
                }

                NotOpen:

                if (counter >= counterMax) return lastObstacleOpen;

                // Add neighboring positions to queue
                Point testpoint = point + new Point(1, 0);
                if (testpoint.X < ObstacleTilemapSize.X && !checkedPoints.Contains(testpoint)) {
                    points.Enqueue(testpoint);
                    checkedPoints.Add(testpoint);
                }

                testpoint = point - new Point(1, 0);
                if (testpoint.X >= 0 && !checkedPoints.Contains(testpoint)) {
                    points.Enqueue(testpoint);
                    checkedPoints.Add(testpoint);
                }

                testpoint = point + new Point(0, 1);
                if (testpoint.Y < ObstacleTilemapSize.Y && !checkedPoints.Contains(testpoint)) {
                    points.Enqueue(testpoint);
                    checkedPoints.Add(testpoint);
                }

                testpoint = point - new Point(0, 1);
                if (testpoint.Y >= 0 && !checkedPoints.Contains(testpoint)) {
                    points.Enqueue(testpoint);
                    checkedPoints.Add(testpoint);
                }

            } while (!openFound);

            return point;
        }

        public Vector2 GetClosestOpenPoint(Vector2 position) {
            return GetWorldPos(GetClosestOpenTilePoint(GetGridPos(position)));
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
            if(GameManager.WorldRandom.NextValue(0, 10) >= 6.5f) {
                Loot = new List<Pickup>(0);
                return;
            }

            float lootRNG = GameManager.WorldRandom.NextValue(0, 100);
            int lootCount;
            if (lootRNG <= 100 && lootRNG > 25) {
                lootCount = 1;
            } else if (lootRNG > 7) {
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
            //todo - improve
            float rng = GameManager.WorldRandom.NextValue(0, 100);

            if (rng > 95) return Pickup.Coin_5;
            else if (rng > 75) return Pickup.Coin;
            else if (rng > 55) return Pickup.Heart_Whole;
            else if (rng > 40) return Pickup.Heart_Half;
            else if (rng > 25) return Pickup.Bomb;
            else if (rng > 10) return Pickup.BonusHeart;
            else return Pickup.Key;
        }

        private void GenerateItem() {
            GameObject itemObj = Instantiate(new Prefab_WorldItem(WorldItem.GetRandomItem(ItemPool.Item)));
            itemObj.transform.Parent = transform;
            itemObj.transform.LocalPosition = Vector3.Zero;

            //WorldItem item = Instantiate(itemObj).GetComponent<WorldItem>();

            //Debug.Log($"Spawning item at {GridPos}");
        }

        private void SpawnBoss() {
            AbstractEnemy boss = Instantiate(new Prefab_TestBoss()).GetComponent<AbstractEnemy>();
            boss.transform.Parent = transform;
            boss.transform.LocalPosition = Vector3.Zero;

            boss.OnDeathFlag = () => {
                Enemies.Remove(boss);

                //debug - end game
                (GameManager.CurrentScene as GameScene).Win();
            };
            boss.OnDeathFlag += CheckClear;

            Enemies.Add(boss);
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

            ObstacleColliders = null;
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