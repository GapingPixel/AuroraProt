using UnityEngine;

public class BigDamageObject : MonoBehaviour
{
    public int PlayerNumber;
    public float Dmg = 2;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject,0.2f);
    }
}