using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

[UpdateInGroup(typeof(PhysicsSystemGroup))]
[UpdateBefore(typeof(PhysicsSimulationGroup))]
public partial struct CharacterSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PhysicsVelocity>();
        state.RequireForUpdate<CharacterData>();
        state.RequireForUpdate<DamageData>();
    }

    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        foreach (var (transform, velocity, character, damage, entity) in SystemAPI.Query<
            RefRW<LocalTransform>, RefRW<PhysicsVelocity>,
            RefRW<CharacterData>, RefRW<DamageData>
        >().WithNone<DeadEntityTag>().WithEntityAccess())
        {
            velocity.ValueRW.Linear += deltaTime * character.ValueRO.speed * character.ValueRO.moveVector;

            var viewDir = character.ValueRO.targetPos - transform.ValueRO.Position;
            viewDir.y = 0;
            viewDir = math.normalizesafe(viewDir, new float3(0f, 0f, 1f));
            transform.ValueRW.Rotation = quaternion.LookRotation(viewDir, new float3(0f, 1f, 0f));

            character.ValueRW.health = math.clamp(character.ValueRO.health - damage.ValueRO.damageTaken, 0f, character.ValueRO.maxHealth);
            damage.ValueRW.damageTaken = 0f;
            if (character.ValueRW.health == 0f)
                ecb.AddComponent(entity, new DeadEntityTag());

            if (SystemAPI.HasComponent<WeaponData>(entity))
            {
                var weaponData = SystemAPI.GetComponentRW<WeaponData>(entity);
                if (weaponData.ValueRO.cooldown > 0f)
                {
                    weaponData.ValueRW.cooldown -= deltaTime;
                    continue;
                }
                if (!character.ValueRO.isFiring) continue;
                weaponData.ValueRW.cooldown = 1f / weaponData.ValueRO.fireRate;

                var random = Random.CreateFromIndex((uint)(SystemAPI.Time.ElapsedTime * 1000f) + 1u);
                var spread = weaponData.ValueRO.horizontalSpread / 360f * math.PI;
                var newTransform = transform.ValueRO.RotateY(random.NextFloat(-spread, spread));

                Entity projectile;
                if (weaponData.ValueRO.projectilePrefab != Entity.Null)
                {
                    projectile = ecb.Instantiate(weaponData.ValueRO.projectilePrefab);
                    ecb.SetComponent(projectile, newTransform);
                    ecb.SetComponent(projectile, new PhysicsVelocity {
                        Angular = float3.zero,
                        Linear = newTransform.TransformDirection(weaponData.ValueRO.projectileVelocity)
                    });
                }
                else
                {
                    projectile = ecb.CreateEntity();
                    ecb.AddComponent(projectile, newTransform);
                }
                ecb.AddComponent(projectile, weaponData.ValueRO.projectileData);
                ecb.AddComponent(projectile, new LifeTimeData { lifeTime = 0f, maxLifeTime = weaponData.ValueRO.projectileLifeTime });
                if (weaponData.ValueRO.projectileLifeTime == 0f) ecb.AddComponent(projectile, new DeadEntityTag());

                velocity.ValueRW.Linear -= weaponData.ValueRO.selfKnockback;
            }
        }
    }
}
