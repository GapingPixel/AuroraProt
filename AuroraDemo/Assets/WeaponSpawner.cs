using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSpawner : MonoBehaviour
{
    float timer;
    public GameObject Pickable;
    // Start is called before the first frame update
    
    void Start()
    {
        timer = Time.time;
    }

    // Update is called once per frame
    [System.Obsolete]
    void Update()
    {
        if (timer <= Time.time) {
            timer = Time.time + Random.RandomRange(11f,14f);
            Instantiate(Pickable,transform.position,transform.rotation);
        }
    }
}
