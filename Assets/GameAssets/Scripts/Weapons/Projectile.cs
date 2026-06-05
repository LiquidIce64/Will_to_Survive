using UnityEngine;

public class Projectile : MonoBehaviour, ITriggerable
{
    public Vector3 initialVelocity = Vector3.forward;
    [SerializeField] private float lifetime = 2f;
    [SerializeField] private bool explodeOnImpact = true;
    [SerializeField] private bool damageOnImpact = true;
    public ProjectileWeapon weapon;
    private Rigidbody rb;
    private bool exploded = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearVelocity = transform.rotation * initialVelocity;
    }

    private bool IsTarget(BaseCharacter character)
    {
        if (weapon.Owner is Player) return character != weapon.Owner;

        return character is Player;
    }

    private void Explode()
    {
        if (exploded) return;
        exploded = true;
        if (weapon.range > 0f)
        {
            foreach (Collider c in Physics.OverlapSphere(
                transform.position, weapon.range, LayerMask.GetMask("Character")
            ))
            {
                if (!c.gameObject.TryGetComponent(out BaseCharacter character)) continue;
                if (!IsTarget(character)) continue;

                Vector3 characterDir = character.transform.position - transform.position;
                float knockbackMult = 2f - characterDir.magnitude / weapon.range;
                characterDir.Normalize();

                character.ApplyDamage(weapon.damage);
                character.ApplyKnockback(weapon.knockback * knockbackMult * characterDir);
            }
        }
        Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        lifetime -= Time.fixedDeltaTime;
        if (lifetime < 0f) Explode();
    }

    public void OnEnter(BaseCharacter character)
    {
        if (weapon.IsTarget(character))
        {
            if (damageOnImpact)
            {
                var knockbackDir = rb.linearVelocity;
                knockbackDir.y = 0f;
                knockbackDir.Normalize();
                character.ApplyDamage(weapon.damage);
                character.ApplyKnockback(knockbackDir * weapon.knockback);
            }
            if (explodeOnImpact) Explode();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (explodeOnImpact) Explode();
    }
}
