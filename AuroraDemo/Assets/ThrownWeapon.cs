using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrownWeapon : MonoBehaviour
{
    //[System.NonSerialized]
    public bool Right;
    public int PlayerNumber;
    public float Dmg = 0.5f;
    public bool IsWeapon;

    public float Speed = 5000.0f;
    public Rigidbody2D rb;
    public Transform Player;
    // Start is called before the first frame update
    void Start()
    {
        if (IsWeapon) {
        //Physics2D.IgnoreLayerCollision(0,3,true);
        /*Physics2D.IgnoreCollision(Player.collider2D, GetComponent<Collider2D>(),true);
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), Player.GetComponent<Collider2D>(),true);*/
            switch (Right) {
                case false:
                rb.AddForce( (transform.right*-1)*Speed);
                transform.localScale = new Vector2(-1, 1);
                break;

                case true:
                rb.AddForce(transform.right*Speed);
                transform.localScale = new Vector2(1, 1);
                break;
            }
            Destroy(gameObject,3);
        } else {
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0;
            Destroy(gameObject,8);
        }
    }

    

    void OnTriggerEnter2D (Collider2D col) {
        if (col.gameObject.CompareTag("Collision") ) {
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0;
        }
        if (!IsWeapon) {
         if (col.gameObject.CompareTag("Player"))
            {
                PlayerNumber = col.gameObject.GetComponent<Player>().PlayerNumber;
            } 
        }
    }

    
   

}
