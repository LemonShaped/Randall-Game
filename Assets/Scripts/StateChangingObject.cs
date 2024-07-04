using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
using System.Linq;


public class StateChangingObject : MonoBehaviour
{

    public ModesEnum convertInto;
    public float convertTime;

    public Vector2 offset;

    public bool isOpen;

    public bool isRunning;

    public Sprite closed;
    public Sprite open;
    public Sprite[] running;

    public MyAnimator animator;
    public SpriteRenderer spriteRenderer;

    public float activateTime;
    public float reloadTime;
    public float reloadTimeRemaining;

    private void Start()
    {
        spriteRenderer.sprite = closed;
        animator.StartAnimation(new Sprite[]{closed});
    }

    private void Update() {
        if (reloadTimeRemaining > 0)
            reloadTimeRemaining -= Time.deltaTime;
    }


    public Coroutine openWaitingCoroutine = null;

    public IEnumerator Open_WaitingToStart(LiquidCharacter player){
        if (!isRunning){
            isOpen = true;
            animator.StartAnimation(new Sprite[]{open});
        }

        yield return new WaitForSeconds(activateTime);

        StartCoroutine(Convert(player));
    }

    public void Close_LeftBeforeStarted(LiquidCharacter player) {
        isOpen = false;
        animator.StartAnimation(new Sprite[]{closed});
        if (openWaitingCoroutine != null){
            StopCoroutine(openWaitingCoroutine);
            openWaitingCoroutine = null;
        }
    }

    public IEnumerator Convert(LiquidCharacter player) {
        if (isRunning || reloadTimeRemaining > 0)
            yield break;

        reloadTimeRemaining = convertTime + reloadTime;

        player.spriteRenderer.enabled = false;
        isRunning = true;
        isOpen = false;
        animator.StartAnimation(running);

        player.rb.position = (Vector2)transform.position + offset;
        player.rb.constraints |= RigidbodyConstraints2D.FreezePosition;
        player.transform.parent = this.transform;

        yield return new WaitForSeconds(convertTime);

        player.transform.parent = transform.parent;
        player.rb.constraints &= ~RigidbodyConstraints2D.FreezePosition;

        player.CurrentMode = convertInto;
        player.spriteRenderer.enabled = true;
        isRunning = false;
        isOpen = true;
        animator.StartAnimation(new Sprite[]{open});
    }
}
