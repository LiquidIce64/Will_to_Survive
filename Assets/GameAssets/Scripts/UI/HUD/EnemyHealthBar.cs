using UnityEngine;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private RectTransform bar;

    private void Update()
    {
        transform.rotation = Camera.main.transform.rotation;
    }

    public void SetHealth(float healthPercent)
    {
        bar.localScale = new Vector3(healthPercent, 1f, 1f);
    }
}
