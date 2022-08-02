using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float Facing = 1;
    public float Dmg = 1;
    public float Speed = 5f;
    Animator Animator;
    public enum Direction
    {
        Side,
        Up,
        Down
    }
    public Direction MovementDirection;

    public int PlayerNumber;
    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = new Vector2(Facing, 1);
        Animator = GetComponent<Animator>();
    }
    
    private void FixedUpdate()
    {
        switch (MovementDirection)
        {
            case Direction.Side:
                transform.Translate(Vector2.right*Speed*Facing);
                break;
            
            case Direction.Up:
                transform.Translate(Vector2.right*Speed);
                break;
            
            case Direction.Down:
                transform.Translate(Vector2.right*Speed);
                break;
        }
    }
    private void Update()
    {
        if (Animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
        {
            Destroy(gameObject);
        }
    }
}
