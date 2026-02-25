using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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

    void Start() {
        spriteRenderer.sprite = closed;
        animator.Animate(closed);
    }

    void Update() {
        if (reloadTimeRemaining > 0)
            reloadTimeRemaining -= Time.deltaTime;
    }

    Dictionary<int, IEnumerator> playersWaitingToStartCoroutines = new();

    public void WaitingToStart(LiquidCharacter player) {
        IEnumerator coroutine = WaitingToStart_Coroutine(player);
        StartCoroutine(coroutine);
        playersWaitingToStartCoroutines[player.GetInstanceID()] = coroutine;
    }

    IEnumerator WaitingToStart_Coroutine(LiquidCharacter player) {
        if (!isRunning) {
            isOpen = true;
            animator.Animate(open);
        }

        yield return new WaitForSeconds(activateTime);
        StartCoroutine(Convert(player));
    }

    public void CancelBeforeStarted(LiquidCharacter player) {
        isOpen = false;
        animator.Animate(closed);
        if (playersWaitingToStartCoroutines.Remove(player.GetInstanceID(), out IEnumerator coroutine))
            StopCoroutine(coroutine);
    }

    IEnumerator Convert(LiquidCharacter player) {
        if (isRunning || reloadTimeRemaining > 0)
            yield break;

        reloadTimeRemaining = convertTime + reloadTime;

        player.spriteRenderer.enabled = false;
        isRunning = true;
        isOpen = false;
        animator.Animate(running);

        player.rb.position = (Vector2)transform.position + offset;
        player.rb.constraints |= RigidbodyConstraints2D.FreezePosition;
        player.transform.parent = this.transform;

        yield return new WaitForSeconds(convertTime);
        yield return new WaitForFixedUpdate();

        player.transform.parent = transform.parent;
        player.rb.constraints &= ~RigidbodyConstraints2D.FreezePosition;

        player.CurrentMode = convertInto;
        player.spriteRenderer.enabled = true;
        isRunning = false;
        isOpen = true;
        animator.Animate(open);
    }
}
