using UnityEngine;

public class Chest : MonoBehaviour
{
    [SerializeField] private GameObject ItemToDrop;

    public void Open()
    {
        //drop the ability item
        if (ItemToDrop != null)
        {
            Instantiate(ItemToDrop, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);

        Debug.Log("Chest opened! Ability dropped.");
    }
}
