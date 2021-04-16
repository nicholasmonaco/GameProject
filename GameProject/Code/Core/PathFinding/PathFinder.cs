// PathFinder.cs - Nick Monaco
// A component that allows a GameObject to access pathfinding while in a room.
// Logic and code for the A* pathfinding algorithm from:
// https://dotnetcoretutorials.com/2020/07/25/a-search-pathfinding-algorithm-in-c/

using GameProject.Code.Core;
using GameProject.Code.Core.Components;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using GameProject.Code.Scripts.Util;
using GameProject.Code.Scripts.Components;
using Microsoft.Xna.Framework.Graphics;

namespace GameProject.Code.Core.PathFinding {
    public class PathFinder : Component {
        public PathFinder(GameObject attached) : base(attached) { }


        public static Room CurrentRoom => GameManager.Map.CurrentRoom;
        public static TileMap<ObstacleID> CurrentTilemap => GameManager.Map.CurrentRoom.ObstacleTilemap;


        public bool FindPath(Vector3 position, out Vector2 moveDirection) {
            return FindPath(position.ToVector2(), out moveDirection);
        }



        public bool FindPath(Vector2 targetPosition, out Vector2 moveDirection) {
            if(CurrentTilemap == null) {
                moveDirection = Vector2.Zero;
                return false;
            }

            #region Setup Map
            Vector2 offset = Room.ObstacleTileSize / 2f;

            PathNode start = new PathNode();
            start.Position = CurrentRoom.GetGridPos(transform.Position.ToVector2() + offset);

            PathNode finish = new PathNode();
            finish.Position = CurrentRoom.GetGridPos(targetPosition + offset);

            start.SetDistance(finish.X, finish.Y);

            List<PathNode> activeTiles = new List<PathNode>();
            activeTiles.Add(start);

            List<PathNode> visitedTiles = new List<PathNode>();


            // Debug Drawing
            if (Debug.ShowPathfinding) {
                _debugTileDrawPoints = new List<(Point, Color)>();
                _debugTileDrawPoints.Add((start.Position, Color.SkyBlue));
                _debugTileDrawPoints.Add((finish.Position, Color.Pink));
            }
            //
            #endregion


            #region Main Pathfinding
            while (activeTiles.Any()) {
                PathNode checkNode = activeTiles.OrderBy(node => node.CostDistance).First();

                if (checkNode.SamePosition(finish)) { // Checking if position is the same
                    // Destination reached!

                    moveDirection = Vector2.Zero;

                    // Go up the linked list to find the direction the caller will currently move
                    PathNode recursiveNode = checkNode;

                    if(recursiveNode.Parent != null) {
                        while (recursiveNode.Parent.Parent != null) { 
                            recursiveNode = recursiveNode.Parent;
                            if(Debug.ShowPathfinding) _debugTileDrawPoints.Add((recursiveNode.Position, Color.ForestGreen));
                        }

                        Vector2 dirToStartTileCenter = (GameManager.Map.CurrentRoom.ObstacleTilemap.GetWorldPosFromGridPos(start.Position) - transform.Position.ToVector2()).Norm();
                        moveDirection = (recursiveNode.Position - start.Position).ToVector2(); // This should always be a unit vector

                        moveDirection = (dirToStartTileCenter + moveDirection * 9f) / 10f;
                        moveDirection = moveDirection.Norm();
                    }

                    return true;
                }

                visitedTiles.Add(checkNode);
                activeTiles.Remove(checkNode);

                List<PathNode> walkableNodes = GetWalkableTiles(checkNode, finish);

                foreach(PathNode walkableNode in walkableNodes) {
                    // If already visited node, this path is done.
                    if (visitedTiles.Any(node => node.SamePosition(walkableNode))) continue;

                    // This tile's been seen before, but has other potential paths
                    if(activeTiles.Any(node => node.SamePosition(walkableNode))) {
                        PathNode existingNode = activeTiles.First(node => node.SamePosition(walkableNode));

                        if(existingNode.CostDistance > checkNode.CostDistance) {
                            activeTiles.Remove(existingNode);
                            activeTiles.Add(walkableNode);
                        }
                    } else {
                        // This tile has never been seen before
                        activeTiles.Add(walkableNode);
                    }
                }
            }

            // No path found.
            moveDirection = Vector2.Zero;
            return false;
            #endregion
        }




        public static List<PathNode> GetWalkableTiles(PathNode currentTile, PathNode targetTile) {
            List<PathNode> borderingTiles = new List<PathNode>(4);

            // Add horizontal bordering nodes
            for (int x = -1; x <= 1; x += 2) {
                borderingTiles.Add(new PathNode {
                    X = currentTile.X + x,
                    Y = currentTile.Y,
                    Parent = currentTile,
                    Cost = currentTile.Cost + 1
                });
            }

            // Add vertical bordering nodes
            for (int y = -1; y <= 1; y += 2) {
                borderingTiles.Add(new PathNode {
                    X = currentTile.X,
                    Y = currentTile.Y + y,
                    Parent = currentTile,
                    Cost = currentTile.Cost + 1
                });
            }

            // Set distance of each node
            foreach(PathNode node in borderingTiles) {
                node.SetDistance(targetTile.X, targetTile.Y);
            }

            // Determine upper bounds of the tilemap - getting index-1
            int maxX = CurrentTilemap.MapSize.X - 1;
            int maxY = CurrentTilemap.MapSize.Y - 1;

            // Return checked and clamped final list
            return borderingTiles.Where(node => node.X >= 0 && node.X <= maxX)
                                 .Where(node => node.Y >= 0 && node.Y <= maxY)
                                 .Where(node => CurrentTilemap.GetTile(node.X, node.Y) == ObstacleID.None)
                                 .ToList();
        }




        private List<(Point, Color)> _debugTileDrawPoints = new List<(Point, Color)>();


        public override void DebugDraw(SpriteBatch sb) {
            Vector2 halfTilesize = Room.ObstacleTileSize / 2f;

            foreach((Point, Color) data in _debugTileDrawPoints) {
                Vector2 center = GameManager.Map.CurrentRoom.ObstacleTilemap.GetWorldPosFromGridPos(data.Item1);
                Vector2[] points = {
                    center - halfTilesize,
                    center + new Vector2(-halfTilesize.X, halfTilesize.Y),
                    center + halfTilesize,
                    center + new Vector2(halfTilesize.X, -halfTilesize.Y),
                    center - halfTilesize
                };

                for (int i = 0; i < points.Length - 1; i++) {
                    DrawLine(sb, points[i], points[i + 1], data.Item2);
                }

                DrawLine(sb, points[points.Length-1], points[0], data.Item2);
            }

            _debugTileDrawPoints.Clear();
        }

        private static void DrawLine(SpriteBatch sb, Vector2 start, Vector2 end, Color color) {
            Vector2 edge = end - start;
            float angle = (float)MathF.Atan2(edge.Y, edge.X);

            sb.Draw(Resources.Sprite_Pixel,
                    start,
                    null,
                    color,
                    angle,
                    Vector2.Zero,
                    new Vector2(edge.Length(), 0.5f).FlipY(),
                    SpriteEffects.None,
                    0.9f);
        }
    }
}