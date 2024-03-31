using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PhaseChangingObject : MonoBehaviour
{

    public ModesEnum convertInto;
    //public float timeUntilActivate;
    public float convertTime;
    //public float rechargeTime;

    public Vector2 offset;

    private bool inProgress;

    public Sprite[] active;
    public Sprite[] inactive;

    public MyAnimator animator;
    public SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer.sprite = inactive[0];
        animator.StartAnimation(inactive);
        
    }

    public void StartChange(PlayerController player) {
        if (inProgress)
            return;

        player.spriteRenderer.enabled = false;
        inProgress = true;
        animator.StartAnimation(active);

        StartCoroutine(Convert(player));
    }

    public IEnumerator Convert(PlayerController player)
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
