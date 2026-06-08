using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class EnemySpawnAuthoring : MonoBehaviour
{
    [SerializeField] private float baseSpawnInterval = 2f;
    [SerializeField] private float spawnIntervalRandomness = 1f;
    [SerializeField] private float intervalLevelMultiplier = 0.9f;
    [SerializeField] private float spawnCheckRadius = 0.7f;
    [SerializeField] private GameObject[] enemyPresets;

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Handles.color = Color.yellow;
        Handles.matrix = transform.localToWorldMatrix;
        Handles.DrawWireDisc(Vector3.zero, Vector3.up, 1f);
    }
#endif

    class Baker : Baker<EnemySpawnAuthoring>
    {
        public override void Bake(EnemySpawnAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponentObject(entity, authoring);

            var buffer = AddBuffer<EnemySpawnPreset>(entity);
            foreach (var preset in authoring.enemyPresets)
            {
                if (!preset.TryGetComponent(out EnemyDataValues enemyData)) continue;
                if (!preset.TryGetComponent(out WeaponDataValues weaponData)) continue;
                DependsOn(preset);
                Entity projectile = (weaponData.projectilePrefab != null) ? GetEntity(weaponData.projectilePrefab, TransformUsageFlags.Dynamic) : Entity.Null;
                buffer.Add(new EnemySpawnPreset {
                    entity = GetEntity(preset, TransformUsageFlags.Dynamic),
                    characterData = new CharacterData
                    {
                        speed = enemyData.speed,
                        health = enemyData.maxHealth,
                        maxHealth = enemyData.maxHealth,
                        knockbackResistance = enemyData.knockbackResistance,
                        moveVector = float3.zero,
                        targetPos = float3.zero,
                        isFiring = false,
                    },
                    enemyData = new EnemyData
                    {
                        seed = 1u,
                        state = NPCState.Follow,
                        maxAttackDistance = enemyData.maxAttackDistance,
                        attackDistance = enemyData.attackDistance,
                        minAttackDistance = enemyData.minAttackDistance,
                        aimingTime = enemyData.aimingTime,
                        remainingAimTime = enemyData.aimingTime,
                        xpDrop = enemyData.xpDrop,
                    },
                    weaponData = new WeaponData
                    {
                        projectilePrefab = projectile,
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
                    },
                });
            }

            AddComponent(entity, new EnemySpawnConfig
            {
                seed = (uint)UnityEngine.Random.Range(1, int.MaxValue),
                spawnCheckRadius = authoring.spawnCheckRadius,
                timeUntilSpawn = 0f,
                baseSpawnInterval = authoring.baseSpawnInterval,
                spawnIntervalRandomness = authoring.spawnIntervalRandomness,
                intervalLevelMultiplier = authoring.intervalLevelMultiplier,
                level = 0,
            });
        }
    }
}
