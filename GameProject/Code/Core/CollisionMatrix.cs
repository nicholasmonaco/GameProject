using System;
using System.Collections.Generic;
using System.Text;

namespace GameProject.Code.Core {
    public static class CollisionMatrix {
        public static bool DoLayersInteract(uint layer, uint otherLayer) {
            return DoLayersInteract((LayerID)layer, (LayerID)otherLayer);
        }

        public static bool DoLayersInteract(LayerID layer, LayerID otherLayer) {
            return (_collisionMatrix[_indexDict[layer]] & otherLayer) != 0;
        }


        public static void SetLayerIgnore(bool ignore, LayerID layer, LayerID otherLayer) {
            if (ignore) { // Unset
                _collisionMatrix[_indexDict[layer]] &= ~otherLayer;
                _collisionMatrix[_indexDict[otherLayer]] &= ~layer;
            } else { // Set
                _collisionMatrix[_indexDict[layer]] |= otherLayer;
                _collisionMatrix[_indexDict[otherLayer]] |= layer;
            }
        }

        public static void SetLayerIgnore(LayerID layer, LayerID otherLayer) {
            SetLayerIgnore(true, layer, otherLayer);
        }

        private static Dictionary<LayerID, int> _indexDict = new Dictionary<LayerID, int>(32) {
            { LayerID.Default, 0 },
            { LayerID.IgnoreRaycast, 1 },
            { LayerID.Unnamed_2, 2 },
            { LayerID.Unnamed_3, 3 },
            { LayerID.Unnamed_4, 4 },
            { LayerID.Unnamed_5, 5 },
            { LayerID.Unnamed_6, 6 },
            { LayerID.Unnamed_7, 7 },
            { LayerID.Player, 8 },
            { LayerID.Enemy, 9 },
            { LayerID.Pickup, 10 },
            { LayerID.Item, 11 },
            { LayerID.Wall, 12 },
            { LayerID.EdgeWall, 13 },
            { LayerID.Door, 14 },
            { LayerID.Bullet_Good, 15 },
            { LayerID.Bullet_Evil, 16 },
            { LayerID.Familiar, 17 },
            { LayerID.Obstacle, 18 },
            { LayerID.Hole, 19 },
            { LayerID.ShopItem, 20 },
            { LayerID.Special, 21 },
            { LayerID.Enemy_Flying, 22 },
            { LayerID.Damage, 23 },
            { LayerID.Unnamed_24, 24 },
            { LayerID.Unnamed_25, 25 },
            { LayerID.Unnamed_26, 26 },
            { LayerID.Unnamed_27, 27 },
            { LayerID.Unnamed_28, 28 },
            { LayerID.Unnamed_29, 29 },
            { LayerID.Unnamed_30, 30 },
            { LayerID.Unnamed_31, 31 }
        };


        private static LayerID[] _collisionMatrix = new LayerID[32] {
            LayerID.Max, LayerID.Max, LayerID.Max, LayerID.Max,
            LayerID.Max, LayerID.Max, LayerID.Max, LayerID.Max,
            LayerID.Max, LayerID.Max, LayerID.Max, LayerID.Max,
            LayerID.Max, LayerID.Max, LayerID.Max, LayerID.Max,
            LayerID.Max, LayerID.Max, LayerID.Max, LayerID.Max,
            LayerID.Max, LayerID.Max, LayerID.Max, LayerID.Max,
            LayerID.Max, LayerID.Max, LayerID.Max, LayerID.Max,
            LayerID.Max, LayerID.Max, LayerID.Max, LayerID.Max
        };
        
    }
}
