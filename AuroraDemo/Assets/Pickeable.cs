using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickeable : MonoBehaviour
{
    public int Creator = 0;
    

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            Creator = col.gameObject.GetComponent<Player>().PlayerNumber;
        } 
    }
}
