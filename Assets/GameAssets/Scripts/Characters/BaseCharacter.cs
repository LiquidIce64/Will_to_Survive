using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
abstract public class BaseCharacter : MonoBehaviour
{
    [SerializeField] protected float speed = 200f;
    [SerializeField] protected float maxHealth = 100f;
    [SerializeField] protected float damageResistance = 1f;
    [SerializeField] protected float knockbackResistance = 1f;
    protected Rigidbody rb;
    protected float health;

    protected void Awake()
    {
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
}
