using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class PlayerAuthoring : MonoBehaviour
{
    class Baker : Baker<PlayerAuthoring>
    {
        public override void Bake(PlayerAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            var controller = FindAnyObjectByType<PlayerController>();
            DependsOn(controller);
            AddComponentObject(entity, controller);

            AddComponent(entity, new CharacterData
            {
                speed = controller.speed,
                health = controller.maxHealth,
                maxHealth = controller.maxHealth,
                knockbackResistance = controller.knockbackResistance,
                moveVector = float3.zero,
                targetPos = float3.zero,
                isFiring = false,
            });
            AddComponent(entity, new DamageData { damageTaken = 0f });
            AddComponent<PlayerTag>(entity);

            if (authoring.gameObject.TryGetComponent(out WeaponDataValues weaponData))
            {
                AddComponent(entity, new WeaponData
                {
                    projectilePrefab = GetEntity(weaponData.projectilePrefab, TransformUsageFlags.Dynamic),
                    projectileData = new ProjectileData
                    {
                        explodeOnImpact = weaponData.projectileData.explodeOnImpact,
                        damageOnImpact = weaponData.projectileData.damageOnImpact,
                        damage = weaponData.projectileData.damage,
                        radius = weaponData.projectileData.radius,
                        knockback = weaponData.projectileData.knockback,
                        playerOwned = weaponData.projectileData.playerOwned,
                    },
                    projectileVelocity = weaponData.projectileVelocity,
                    projectileLifeTime = weaponData.projectileLifeTime,
                    selfKnockback = weaponData.selfKnockback,
                    fireRate = weaponData.fireRate,
                    horizontalSpread = weaponData.horizontalSpread,
                    cooldown = 1f / weaponData.fireRate,
                });
            }
        }
    }
}
