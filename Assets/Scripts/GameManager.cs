using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;
using UnityEngine.Video;

public class GameManager : MonoBehaviour
{

    public PlayerController player;
    public LayerMask groundLayers;

    public SpriteShapeRenderer[] fireSpriteShapeFrames;
    public float fireFrame = 0;

    public GameObject firePrefab;
    public float fireTimeout;
    public Dictionary<Vector3, (GameObject fireObj, IEnumerator coroutine)> fires = new();

    public GameObject deathScreen;
    public GameObject winScreen;
    public SpriteRenderer door;
    public Sprite closedDoorSprite;

    //private void Awake() {
    //    Debug.Log(winScreen.GetComponent<VideoPlayer>().url);
    //    Debug.Log(System.IO.Path.Combine(Application.streamingAssetsPath));
    //    Debug.Log(winScreen.GetComponent<VideoPlayer>().url);
    //}

    private void Start() {
        StartCoroutine(FireAnimation());
    }

    private IEnumerator FireAnimation() {
        while (true) {
            for (int i = 0; i < fireSpriteShapeFrames.Length; i++) {
                if (i == fireFrame)
                    fireSpriteShapeFrames[i].gameObject.SetActive(true);
                else
                    fireSpriteShapeFrames[i].gameObject.SetActive(false);
            }
            fireFrame = (fireFrame + 1) % 2;
            yield return new WaitForSeconds(0.4f);
        }
    }


    /// <param name="angle">Angle in degrees clockwise from Up</param>
    public void PlaceFire(Vector3 position, float angle) {

        foreach (Vector3 pos in fires.Keys) {
            if (Vector3.Distance(pos, position) < 0.5f) {
                StopCoroutine(fires[pos].coroutine);
                fires[pos] = (fires[pos].fireObj, FireExpiry(pos));
                StartCoroutine(fires[pos].coroutine);
                return;
            }
        }
        GameObject fire = Instantiate(firePrefab, position, Quaternion.AngleAxis(angle, Vector3.forward), transform);
        fires.Add(position, (fire, FireExpiry(position)));
        StartCoroutine(fires[position].coroutine);
    }

    private IEnumerator FireExpiry(Vector3 position) {
        yield return new WaitForSeconds(fireTimeout);
        RemoveFire(position);
    }
    public void RemoveFire(Vector3 position) {
        Destroy(fires[position].fireObj);
        fires.Remove(position);
    }



    public void LevelComplete() {
        door.GetComponent<Spawner>().enabled = false;
        StartCoroutine(CloseDoor());
    }

    private IEnumerator CloseDoor() {
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



    public GroundMaterial GetMaterial(Vector2 position) {
        Collider2D obj = Physics2D.OverlapPoint(position, groundLayers.Including("Water"));
        if (obj != null) {
            if (obj.gameObject.layer == LayerMask.NameToLayer("GroundNonPorous"))
                return GroundMaterial.Stone;
            else if (obj.gameObject.layer == LayerMask.NameToLayer("GroundPorous"))
                return GroundMaterial.Dirt;
            else if (obj.gameObject.layer == LayerMask.NameToLayer("Water"))
                return GroundMaterial.Water;
            else
                throw new NotSupportedException("Unknown ground layer found");
        }
        else
            return GroundMaterial.None;
    }
    public GroundMaterial GetMaterial(int layer) {
        if (layer == LayerMask.NameToLayer("GroundNonPorous"))
            return GroundMaterial.Stone;
        else if (layer == LayerMask.NameToLayer("GroundPorous"))
            return GroundMaterial.Dirt;
        else if (layer == LayerMask.NameToLayer("Water"))
            return GroundMaterial.Water;
        else
            return GroundMaterial.None;
    }

    public void SetMaterial(Mesh area, GroundMaterial material) => throw new NotImplementedException();
    public void SetMaterial(Rect area, GroundMaterial material) => throw new NotImplementedException();
    public void SetMaterial(PhysicsShape2D area, GroundMaterial material) => throw new NotImplementedException();
    public void SetMaterial(Collider2D area, GroundMaterial material) => throw new NotImplementedException();


    public bool IsPorousGround(Vector2 position) =>
        IsPorous(GetMaterial(position));
    public bool IsPorous(GroundMaterial material) {
        switch (material) {
            case GroundMaterial.Dirt:
                return true;
            case GroundMaterial.Stone:
                return false;
            default:
                return false;
        }

    }
}

public enum GroundMaterial
{
    None,
    Dirt,
    Stone,
    Water
}

