using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
abstract public class BaseCharacter : MonoBehaviour
{
    public float speed = 200f;
    public float maxHealth = 100f;
    public float damageResistance = 1f;
    public float knockbackResistance = 1f;
    protected BaseWeapon weapon;
    protected Rigidbody rb;
    protected float health;

    public BaseWeapon Weapon => weapon;

    protected void Awake()
    {
        TryGetComponent(out weapon);
        rb = GetComponent<Rigidbody>();
        health = maxHealth;
    }

    protected void Move(Vector2 moveVector)
    {
        moveVector *= speed * Time.deltaTime;
        rb.AddForce(moveVector.x, 0f, moveVector.y);
    }

    public void ApplyKnockback(Vector3 knockback)
    {
        rb.linearVelocity += knockback / knockbackResistance;
    }

    public void ApplyDamage(float damage)
    {
        health = Mathf.Clamp(health - damage / damageResistance, 0f, maxHealth);
        OnDamaged();
        if (health == 0f) OnDeath();
    }

    virtual protected void OnDamaged() { }

    abstract protected void OnDeath();

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out ITriggerable trigger))
            trigger.OnEnter(this);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent(out ITriggerable trigger))
            trigger.OnExit(this);
    }
}
