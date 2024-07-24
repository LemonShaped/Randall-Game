using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.Video;

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

    //private void Awake() {
    //    Debug.Log(winScreen.GetComponent<VideoPlayer>().url);
    //    Debug.Log(System.IO.Path.Combine(Application.streamingAssetsPath));
    //    Debug.Log(winScreen.GetComponent<VideoPlayer>().url);
    //}

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
        if (!winScreen.GetComponent<VideoPlayer>().isPlaying)
            winScreen.GetComponent<VideoPlayer>().Play();

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
