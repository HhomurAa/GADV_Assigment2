using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // how fast the player moves
    public float MoveSpeed = 1.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
         
    }

    // Update is called once per frame
    void Update()
    {
        //when you press WASD, the player moves in that direction
        Vector2 MoveDirection = Vector2.zero;
        if (Input.GetKey(KeyCode.W))
        {
            MoveDirection += Vector2.up;
        }
        if (Input.GetKey(KeyCode.A))
        {
            MoveDirection += Vector2.left;
        }
        if (Input.GetKey(KeyCode.S))
        {
            MoveDirection += Vector2.down;
        }
        if (Input.GetKey(KeyCode.D))
        {
            MoveDirection += Vector2.right;
        }

        //normalize makes it so that if i move diagonal, the speed will be the same as when i move straight
        if (MoveDirection != Vector2.zero)
        {
            MoveDirection = MoveDirection.normalized;
        }

        //moves in the direction of the key you pressed
        transform.position += (Vector3)(MoveDirection * MoveSpeed * Time.deltaTime);
    }
}
