using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Transform player;   //players position
    public Vector3 offset;    //offset from the player, incase I need the camera to be further from the player

    //ensures the camera moves only after the player moves
    private void LateUpdate()
    {
        //update the camera position to be the same as the player
        transform.position = player.position + offset;
    }
}
