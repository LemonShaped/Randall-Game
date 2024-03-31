using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class MyAnimator : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;

    [Tooltip("frames per second")]
    public float animationSpeed;

    public Sprite[] frames;

    private bool animating;

    private int i;

    private new Coroutine animation;

    public void StartAnimation(Sprite[] sprites) {
        frames = sprites;
        // switch out the frames. if animation in progress it will start using these new ones.

        if (frames.Length <= 1) {
            StopAnimation();
            if (frames.Length == 1)
                spriteRenderer.sprite = frames[0];
            return;
        }

        if (animating) {
            spriteRenderer.sprite = frames[i]; // update sprite instantly after frames change because the animation will take long to switch
        }
        else {
            StopAnimation();
            animation = StartCoroutine(Animate());
        }
    }

    private IEnumerator Animate() {
        animating = (animationSpeed > 0);
        i = 0;

        while (animating) {
            i = (i + 1) % frames.Length;
            spriteRenderer.sprite = frames[i];
            yield return new WaitForSeconds(1f / animationSpeed);

        }
        frames = new Sprite[0];
    }

    public void StopAnimation()
    {
        animating = false;
        //if (animation is not null)
        //    StopCoroutine(animation);
    }

    private void OnDisable() {
        StopAnimation();
    }

}
