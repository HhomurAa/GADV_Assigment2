using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class ConeFill : MonoBehaviour
{
    public Image FillImage;
    public Image BorderImage;

    public float FillSpeed = 1f; //fill speed according to delayed attack

    private float CurrentFill = 0f; //0 = f, 1 = full
    private bool Filling = false;

    public System.Action OnFillComplete; //callback to notify when full

    public Transform target; //enemy this cone belongs to

    public void StartFilling()
    {
        if (FillImage == null || BorderImage == null) return;

        //set pivot and anchor to bottom right
        FillImage.type = Image.Type.Filled;
        FillImage.fillMethod = Image.FillMethod.Vertical;
        FillImage.fillOrigin = (int)Image.OriginVertical.Top; //fill from bottom up
        FillImage.fillAmount = 0f;

        CurrentFill = 0f;
        Filling = true;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return; //target is your enemy
        }

        if (!Filling || FillImage == null)
        {
            return;
        }
        CurrentFill += Time.deltaTime * FillSpeed;
        CurrentFill = Mathf.Clamp01(CurrentFill); //makes sure it stays between 0 and 1 

        //scale from tip
        FillImage.fillAmount = CurrentFill;

        if (CurrentFill >= 1f)
        {
            Filling = false;
            OnFillComplete?.Invoke(); //notify enemy.cs
        }
    }

    public void SetAttackRange(float Range)
    {
        float MinWidth = 200;
        float MinHeight = 200;

        if (FillImage != null)
        {
            var rt = FillImage.rectTransform;
            rt.sizeDelta = new Vector2(MinWidth, Mathf.Max(Range, MinHeight));
        }

        if (BorderImage != null)
        {
            var rt = BorderImage.rectTransform;
            rt.sizeDelta = new Vector2(MinWidth, Mathf.Max(Range, MinHeight));
        }

    }

    public void ResetFill()
    {
        CurrentFill = 0f;
        Filling = false;

        if (FillImage != null)
        {
            FillImage.fillAmount = 0f;
        }
    }
}