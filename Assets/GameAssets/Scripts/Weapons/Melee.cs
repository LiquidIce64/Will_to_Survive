using UnityEngine;

public class Melee : BaseWeapon
{
    [SerializeField] protected float sweepAngle = 45f;

    override protected void OnUse(Vector3 targetPos)
    {
        Vector3 targetDirection = GetDirectionToPos(targetPos);

        foreach (Collider c in Physics.OverlapSphere(
            owner.transform.position, range, LayerMask.GetMask("Character")
        ))
        {
            if (!c.gameObject.TryGetComponent(out BaseCharacter character)) continue;
            if (!IsTarget(character)) continue;

            Vector3 characterDir = GetDirectionToPos(character.transform.position);
            if (Vector3.Angle(targetDirection, characterDir) > sweepAngle) continue;

            character.ApplyDamage(damage);
            character.ApplyKnockback(characterDir * knockback);
        }
    }
}
