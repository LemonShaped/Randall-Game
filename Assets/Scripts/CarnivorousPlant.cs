using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarnivorousPlant : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Collider2D headCollider;

    public Sprite open;
    public Sprite closed;

    public float timeClosed;

    public Vector2 eatenPosition;

    public void Bite(LiquidCharacter player)
    {
        player.rb.MovePosition((Vector2)transform.position + eatenPosition);
        player.Hurt();
        spriteRenderer.sprite = closed;
        headCollider.enabled = false;

        StartCoroutine(Reopen());
    }

    IEnumerator Reopen()
    {
        yield return new WaitForSeconds(timeClosed);

        spriteRenderer.sprite = open;
        headCollider.enabled = true;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.blue.MixWith(Color.white, 0.05f);
        Gizmos.DrawWireSphere(transform.position + (Vector3)eatenPosition, 0.1f);
    }
#endif

}
