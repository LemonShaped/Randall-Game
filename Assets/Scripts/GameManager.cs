using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{

    public Tilemap groundTilemap;

    public TileBase fireTile;
    public float fireTimeout;

    public PlayerController player;

    public GameObject deathScreen;
    public GameObject winScreen;
    public SpriteRenderer door;
    public Sprite closedDoorSprite;

    public void SetTile(Vector3Int position, TileBase tile) {
        if (tile == fireTile) {
            groundTilemap.SetTile(position, fireTile);
            StartCoroutine(FireExpiry(position));
        }
        else {
            groundTilemap.SetTile(position, tile);
        }
    }
    public void GetTile(Vector3Int position)
        => groundTilemap.GetTile(position);
    IEnumerator FireExpiry(Vector3Int position)
    {
        yield return new WaitForSeconds(fireTimeout);
        if (groundTilemap.GetTile(position) == fireTile)
            groundTilemap.SetTile(position, null);
    }


    public void LevelComplete() {
        door.GetComponent<Spawner>().enabled = false;
        StartCoroutine(CloseDoor());
    }
    IEnumerator CloseDoor()
    {
        yield return new WaitForSeconds(1.5f);

        door.sprite = closedDoorSprite;
        door.sortingLayerName = "Foreground";
        door.transform.GetChild(0).gameObject.SetActive(false);
        player.gameObject.SetActive(false);

        yield return new WaitForSeconds(1.5f);

        winScreen.SetActive(true);
    }

    public void LevelFailed() {
        deathScreen.SetActive(true);
    }

    public void RestartGame() {
        SceneManager.LoadScene("Level 1");
    }
    public void MainMenu() {
        SceneManager.LoadScene("Main Menu");
    }



}
