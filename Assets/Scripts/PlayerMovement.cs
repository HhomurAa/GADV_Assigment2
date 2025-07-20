using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.U2D;

public class PlayerMovement : MonoBehaviour
{
    // how fast the player moves
    public float MoveSpeed = 1.0f;

    public Transform Player;


    private Vector2 MoveDirection;

    void Update()
    {
        MoveDirection = Vector2.zero; //reset at start of frame

        //movement input
        if (Input.GetKey(KeyCode.W)) MoveDirection += Vector2.up;
        if (Input.GetKey(KeyCode.S)) MoveDirection += Vector2.down;
        if (Input.GetKey(KeyCode.A)) MoveDirection += Vector2.left;
        if (Input.GetKey(KeyCode.D)) MoveDirection += Vector2.right;

        //apply rotation when character turns
        if (MoveDirection != Vector2.zero)
        {
            MoveDirection = MoveDirection.normalized;

            // Move the entire object(this includes the sprite and everything else)
            transform.position += (Vector3)(MoveDirection * MoveSpeed * Time.deltaTime);

            // Rotate just the sprite (child)
            float angle = Mathf.Atan2(MoveDirection.y, MoveDirection.x) * Mathf.Rad2Deg + 90f;
            if (Player != null)
                Player.localRotation = Quaternion.Euler(0, 0, angle); // use localRotation to avoid global shift
        }
    }
}
    

