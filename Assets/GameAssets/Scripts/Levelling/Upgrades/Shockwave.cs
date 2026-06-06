using UnityEngine;

public class Shockwave : MonoBehaviour
{
    private static Shockwave instance;
    public float damage = 1f;
    public float knockback = 1f;
    public float range = 1f;
    public float fireRate = 1f;
    protected float _cooldown = 0f;

    public static Shockwave Instance => instance;

    protected void Awake()
    {
        instance = this;
    }

    protected void FixedUpdate()
    {
        _cooldown -= Time.fixedDeltaTime;
        if (_cooldown > 0f) return;
        _cooldown = 1f / fireRate;

        foreach (Collider c in Physics.OverlapSphere(
            transform.position, range, LayerMask.GetMask("Character")
        ))
        {
            if (!c.gameObject.TryGetComponent(out Enemy character)) continue;

            Vector3 characterDir = character.transform.position - transform.position;
            characterDir.y = 0f;
            characterDir.Normalize();

            character.ApplyDamage(damage);
            character.ApplyKnockback(characterDir * knockback);
        }
    }
}
