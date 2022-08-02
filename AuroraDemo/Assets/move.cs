using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class move : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Translate(Vector2.right*Time.deltaTime*1);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        print(transform.position.x);
    }
}
