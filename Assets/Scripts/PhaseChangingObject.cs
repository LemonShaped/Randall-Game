using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PhaseChangingObject : MonoBehaviour
{

    public ModesEnum convertInto;
    public float convertTime;

    public Vector2 offset;

    private bool inProgress;

    public Sprite[] active;
    public Sprite[] inactive;

    public MyAnimator animator;
    public SpriteRenderer spriteRenderer;

    public float reloadTime;
    public float reloadTimeRemaining;

    private void Start()
    {
        spriteRenderer.sprite = inactive[0];
        animator.StartAnimation(inactive);
    }

    private void Update() {
        if (reloadTimeRemaining > 0)
            reloadTimeRemaining -= Time.deltaTime;
    }

    public void StartChange(LiquidCharacter player) {
        if (inProgress || reloadTimeRemaining > 0)
            return;

        reloadTimeRemaining = convertTime + reloadTime;

        player.spriteRenderer.enabled = false;
        inProgress = true;
        animator.StartAnimation(active);

        StartCoroutine(Convert(player));
    }

    public IEnumerator Convert(LiquidCharacter player)
    {
        player.rb.position = (Vector2)transform.position + offset;
        player.rb.constraints |= RigidbodyConstraints2D.FreezePosition;

        yield return new WaitForSeconds(convertTime);

        player.rb.constraints &= ~RigidbodyConstraints2D.FreezePosition;

        player.CurrentMode = convertInto;
        player.spriteRenderer.enabled = true;
        inProgress = false;
        animator.StartAnimation(inactive);
    }

}
