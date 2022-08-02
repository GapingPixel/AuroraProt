using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageObject : MonoBehaviour
{
    public int PlayerNumber;
    public float Dmg = 1;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject,0.1f);
    }

    
}
