using UnityEngine;

public class XPBar : MonoBehaviour
{
    private static XPBar instance;

    [SerializeField] private RectTransform bar;

    public static XPBar Instance => instance;

    private void Awake()
    {
        instance = this;
    }

    public void SetValue(float value)
    {
        bar.localScale = new Vector3(value, 1f, 1f);
    }
}
