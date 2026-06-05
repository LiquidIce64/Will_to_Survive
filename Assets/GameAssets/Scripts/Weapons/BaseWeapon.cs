using UnityEngine;

abstract public class BaseWeapon : MonoBehaviour
{
    public float damage = 1f;
    public float knockback = 1f;
    public float selfKnockback = 0f;
    public float range = 1f;
    public float fireRate = 1f;
    public float horizontalSpread = 10f;
    protected float _cooldown = 0f;
    protected BaseCharacter owner;

    public BaseCharacter Owner => owner;

    public float RemainingCooldown => _cooldown;

    protected Vector3 GetDirectionToPos(Vector3 pos)
    {
        Vector3 vec = pos - owner.transform.position;
        vec.y = 0f;
        return vec.normalized;
    }

    public bool IsTarget(BaseCharacter character)
    {
        if (owner is Player) return character != owner;

        return character is Player;
    }

    public void Use(Vector3 targetPos)
    {
        if (_cooldown > 0f) return;
        _cooldown = 1f / fireRate;
        OnUse(targetPos);
    }

    abstract protected void OnUse(Vector3 targetPos);

    protected void Awake()
    {
        owner = GetComponent<BaseCharacter>();
    }

    protected void FixedUpdate()
    {
        if (_cooldown > 0f) _cooldown -= Time.fixedDeltaTime;
    }
}
