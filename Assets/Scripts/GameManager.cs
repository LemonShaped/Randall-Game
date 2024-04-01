using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{

    public Tilemap groundTilemap;

    public List<Fire> fires = new();
    public TileBase fireTile;
    public float fireTimeout;

    void FixedUpdate()
    {
        foreach (Fire fire in fires) {
            fire.remainingTime -= Time.fixedDeltaTime;
            if (fire.remainingTime <= 0)
                SetTile(fire.position, null);
        }
    }

    public void PlaceFire(Vector3Int position) {
        groundTilemap.SetTile(position, fireTile);
        fires.Add(new Fire(position, fireTimeout));
    }

    public void SetTile(Vector3Int position, TileBase tile) {
        if (tile == fireTile)
            PlaceFire(position);
        else {
            groundTilemap.SetTile(position, tile);
            foreach (Fire fire in fires) {
                if (fire.position == position)
                    fires.Remove(fire);
            }
        }
    }

    public class Fire
    {
        public Vector3Int position;
        public float remainingTime;
        public Fire(Vector3Int position, float remainingTime) {
            this.position = position;
            this.remainingTime = remainingTime;
        }
    }

}
