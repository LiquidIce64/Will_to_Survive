using UnityEngine;

public class XPOrb : MonoBehaviour, ITriggerable
{
    [SerializeField] private float speed = 1f;
    private Vector3 pos;
    private float posLerp = 0f;
    public float xp = 1f;

    public void OnEnter() => enabled = true;

    private void Start()
    {
        pos = transform.position;
    }

    private void Update()
    {
        posLerp += speed * Time.deltaTime;
        transform.position = Vector3.Lerp(pos, Player.Instance.transform.position, posLerp * posLerp);
        if (posLerp >= 1f)
        {
            Player.Instance.playerXP.AddXP(xp);
            Destroy(gameObject);
        }
    }
}
