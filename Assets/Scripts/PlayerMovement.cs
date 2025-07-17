using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // how fast the player moves
    public float MoveSpeed = 1.0f;

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        //when you press WASD, the player moves in that direction
        Vector2 MoveDirection = Vector2.zero;
        if (Input.GetKey(KeyCode.W))
        {
            MoveDirection += Vector2.up;
            transform.rotation = Quaternion.Euler(0, 0, 180); //aprite face up

        }
        if (Input.GetKey(KeyCode.A))
        {
            MoveDirection += Vector2.left;
            transform.rotation = Quaternion.Euler(0, 0, -90); //sprite turn left
        }
        if (Input.GetKey(KeyCode.S))
        {
            MoveDirection += Vector2.down;
            transform.rotation = Quaternion.Euler(0, 0, 0); //sprite turn down

        }
        if (Input.GetKey(KeyCode.D))
        {
            MoveDirection += Vector2.right;
            transform.rotation = Quaternion.Euler(0, 0, 90); //sprite turn right
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
