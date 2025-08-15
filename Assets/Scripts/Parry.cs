using UnityEngine;
using System.Collections;

public class Parry : MonoBehaviour
{
    public bool IsParrying { get; private set; } = false;

    [SerializeField] private float ParryDuration = 0.3f;
    [SerializeField] private float ParryCooldown = 2f;
    [SerializeField] private GameObject ParrySprite;

    private float cooldownTimer = 0f;
    private HMEBar hmeBar;


    private void Start()
    {
        cooldownTimer = ParryCooldown; //start full

        if (ParrySprite != null)
            ParrySprite.SetActive(false);

        hmeBar = FindFirstObjectByType<HMEBar>();
        if (hmeBar != null)
            hmeBar.SetParryCooldown(ParryCooldown);
    }

    private void Update()
    {
        //increment cooldown if not full
        if (cooldownTimer < ParryCooldown)
        {
            cooldownTimer += Time.deltaTime;
            cooldownTimer = Mathf.Clamp(cooldownTimer, 0f, ParryCooldown);
        }

        //update slider
        if (hmeBar != null)
            hmeBar.UpdateParryCooldown(cooldownTimer);

        //F to parry
        if (Input.GetKeyDown(KeyCode.F) && !IsParrying && cooldownTimer >= ParryCooldown)
        {
            StartCoroutine(ParryCoroutine());
        }
    }

    private IEnumerator ParryCoroutine()
    {
        IsParrying = true;

        if (ParrySprite != null)
            ParrySprite.SetActive(true);

        yield return new WaitForSeconds(ParryDuration);

        if (ParrySprite != null)
            ParrySprite.SetActive(false);

        IsParrying = false;

        cooldownTimer = 0f;
    }
}