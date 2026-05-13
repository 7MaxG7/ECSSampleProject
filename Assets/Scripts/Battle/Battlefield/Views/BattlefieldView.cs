using UnityEngine;
using UnityEngine.Tilemaps;

namespace Battle.Battlefield
{
    public class BattlefieldView : MonoBehaviour
    {
        [SerializeField] private Grid _grid;
        [SerializeField] private Tilemap _tilemap;

        public Grid Grid => _grid;

        public void SetTile(Vector3Int cellPosition, TileBase tile)
            => _tilemap.SetTile(cellPosition, tile);
    }
}