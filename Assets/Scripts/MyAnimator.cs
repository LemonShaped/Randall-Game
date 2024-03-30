using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;


public class MyAnimator : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;

    [Tooltip("frames per second")]
    public float animationSpeed;

    public Sprite[] sprites;

    [HideInInspector]
    public bool animating;

    [HideInInspector]
    public int i;

    public void Animate(Sprite[] sprites) {
        if (animating) {
            this.sprites = sprites;
            spriteRenderer.sprite = sprites[i];
        }
        else {
            StartAnimation(sprites);
        }
    }


    private void OnDisable() {
        animating = false;
    }

    public async void StartAnimation(Sprite[] sprites) {
        this.sprites = sprites;
        animating = (animationSpeed > 0);
        i = 0;

        while (animating) {
            spriteRenderer.sprite = this.sprites[i];
            await Task.Delay((int)(1000 / animationSpeed));
            i = (i + 1) % this.sprites.Length;
        }
    }

}
