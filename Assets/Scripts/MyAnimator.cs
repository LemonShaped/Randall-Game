using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class MyAnimator : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;

    [Tooltip("frames per second")]
    public float animationSpeed;

    public Sprite[] frames;

    private bool animating;

    private int i;

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();

        animating = false;
    }

    public void StartAnimation(Sprite[] sprites) {
        if (sprites.Length == 0)
            sprites = new Sprite[1]{null};

        frames = sprites;

        if (animating || animationSpeed == 0) {
            spriteRenderer.sprite = frames[0]; // update sprite instantly after frames change because the animation will take long to switch
        }
        else {
            StartCoroutine(Animate());
        }
    }

    private IEnumerator Animate() {
        animating = true;
        i = 0;

        while (animating) {
            i = (i + 1) % frames.Length;
            spriteRenderer.sprite = frames[i];
            yield return new WaitForSeconds(1 / animationSpeed);
        }
    }

    private void OnDisable() {
        animating = false;
        frames = new Sprite[1]{null};
    }

}
