using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class MyAnimator : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;

    [Tooltip("frames per second")]
    public float animationSpeed;

    public Sprite[] frames;

    bool animating;

    int i;

    void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();

        animating = false;
    }

    public void Animate(Sprite sprite) {
        Animate(new Sprite[] { sprite });
    }
    public void Animate(Sprite[] sprites) {
        if (sprites.Length == 0)
            sprites = new Sprite[] { null };

        frames = sprites;

        if (animating || animationSpeed == 0) {
            spriteRenderer.sprite = frames[0]; // update sprite instantly after frames change because the animation will take long to switch
        }
        else {
            StartCoroutine(Animation());
        }
    }

    IEnumerator Animation() {
        animating = true;
        i = 0;

        while (animating) {
            i = (i + 1) % frames.Length;
            spriteRenderer.sprite = frames[i];
            yield return new WaitForSeconds(1 / animationSpeed);
        }
    }

    void OnDisable() {
        animating = false;
        frames = new Sprite[] { null };
    }

}
