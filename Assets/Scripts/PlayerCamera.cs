using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Transform player;   //players position
    public Vector3 offset;    //offset from the player

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //ensures the camera moves only after the player moves
    private void LateUpdate()
    {
        //update the camera position to be the same as the player
        transform.position = player.position + offset;
    }
}
