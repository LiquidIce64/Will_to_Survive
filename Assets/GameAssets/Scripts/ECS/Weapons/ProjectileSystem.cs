using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

public partial struct ProjectileSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ProjectileData>();
    }

    public void OnUpdate(ref SystemState state)
    {
        if (!SystemAPI.TryGetSingleton(out ProjectileConfig config)) return;

        var ecbSingleton = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld;

        foreach (var (transform, projectile, entity) in SystemAPI.Query<
            RefRO<LocalTransform>, RefRO<ProjectileData>
        >().WithAll<DeadEntityTag>().WithEntityAccess())
        {
            var hits = new NativeList<DistanceHit>(10, Allocator.Temp);
            if (physicsWorld.OverlapSphere(transform.ValueRO.Position, projectile.ValueRO.radius, ref hits, config.filter))
                foreach (var hit in hits)
                {
                    var character = hit.Entity;
                    if (!SystemAPI.TryGetComponent(character, out CharacterData characterData)) continue;

                    var isPlayer = SystemAPI.HasComponent<PlayerTag>(character);
                    if (projectile.ValueRO.playerOwned == isPlayer) continue;

                    SystemAPI.GetComponentRW<DamageData>(character).ValueRW.damageTaken += projectile.ValueRO.damage;

                    var knockbackDir = SystemAPI.GetComponent<LocalTransform>(character).Position - transform.ValueRO.Position;
                    float knockbackMult = 2f - math.length(knockbackDir) / projectile.ValueRO.radius;
                    knockbackDir = math.normalizesafe(knockbackDir, float3.zero);
                    var knockback = knockbackMult * projectile.ValueRO.knockback * knockbackDir;
                    if (math.all(math.isfinite(knockback)))
                        SystemAPI.GetComponentRW<PhysicsVelocity>(character).ValueRW.Linear += knockback / characterData.knockbackResistance;
                }
            ecb.DestroyEntity(entity);
        }
    }
}
