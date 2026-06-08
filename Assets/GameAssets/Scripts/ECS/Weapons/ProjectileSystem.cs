using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

[UpdateInGroup(typeof(PhysicsSystemGroup))]
[UpdateAfter(typeof(PhysicsSimulationGroup))]
public partial struct ProjectileSystem : ISystem
{
    private ComponentLookup<ProjectileData> _projectileLookup;
    private ComponentLookup<DamageData> _damageLookup;
    private ComponentLookup<PhysicsVelocity> _velocityLookup;

    public void OnCreate(ref SystemState state)
    {
        _projectileLookup = state.GetComponentLookup<ProjectileData>(true);
        _damageLookup = state.GetComponentLookup<DamageData>(true);
        _velocityLookup = state.GetComponentLookup<PhysicsVelocity>(false);

        state.RequireForUpdate<ProjectileData>();
        state.RequireForUpdate<LifeTimeData>();
    }

    public void OnUpdate(ref SystemState state)
    {
        if (!SystemAPI.TryGetSingleton(out ProjectileConfig config)) return;

        float deltaTime = SystemAPI.Time.DeltaTime;
        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var simulation = SystemAPI.GetSingleton<SimulationSingleton>();
        var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld;

        _projectileLookup.Update(ref state);
        _velocityLookup.Update(ref state);
        _damageLookup.Update(ref state);

        state.Dependency = new CalculateCollisionsJob {
            PhysicsWorld = physicsWorld,
            projectileLookup = _projectileLookup,
            damageLookup = _damageLookup,
            velocityLookup = _velocityLookup,
        }.Schedule(simulation, state.Dependency);

        foreach (var (transform, projectile, entity) in SystemAPI.Query<
            RefRO<LocalTransform>, RefRO<ProjectileData>
        >().WithPresent<DeadEntityTag>().WithEntityAccess())
        {
            var hits = new NativeList<DistanceHit>(10, Allocator.Temp);
            if (physicsWorld.OverlapSphere(transform.ValueRO.Position, projectile.ValueRO.radius, ref hits, config.filter))
                foreach (var hit in hits)
                {
                    var character = hit.Entity;
                    var isPlayer = SystemAPI.HasComponent<PlayerTag>(character);
                    if (projectile.ValueRO.playerOwned == isPlayer) continue;

                    if (!SystemAPI.TryGetComponent(character, out DamageData damageData)) continue;
                    damageData.damageTaken += projectile.ValueRO.damage;

                    var characterVel = SystemAPI.GetComponentRW<PhysicsVelocity>(character).ValueRW;
                    var knockbackDir = SystemAPI.GetComponent<LocalTransform>(character).Position - transform.ValueRO.Position;
                    float knockbackMult = 2f - math.length(knockbackDir) / projectile.ValueRO.radius;
                    knockbackDir = math.normalizesafe(knockbackDir, float3.zero);
                    characterVel.Linear += knockbackMult * projectile.ValueRO.knockback * knockbackDir;
                }
        }
    }
}

[BurstCompile]
public struct CalculateCollisionsJob : ICollisionEventsJob
{
    [ReadOnly] public PhysicsWorld PhysicsWorld;
    [ReadOnly] public ComponentLookup<ProjectileData> projectileLookup;
    [ReadOnly] public ComponentLookup<DamageData> damageLookup;
    public ComponentLookup<PhysicsVelocity> velocityLookup;

    public EntityCommandBuffer ecb;

    public void Execute(CollisionEvent collisionEvent)
    {
        Entity projectile, other;
        if (projectileLookup.HasComponent(collisionEvent.EntityA))
        {
            projectile = collisionEvent.EntityA;
            other = collisionEvent.EntityB;
        }
        else
        {
            projectile = collisionEvent.EntityB;
            other = collisionEvent.EntityA;
            if (!projectileLookup.HasComponent(projectile)) return;
        }
        var projectileData = projectileLookup[projectile];

        if (projectileData.damageOnImpact && damageLookup.HasComponent(other))
        {
            var damageData = damageLookup[other];
            damageData.damageTaken += projectileData.damage;
            var projectileDir = velocityLookup[projectile].Linear;
            projectileDir.y = 0f;
            projectileDir = math.normalizesafe(projectileDir, float3.zero);
            var characterVel = velocityLookup[other];
            characterVel.Linear += projectileDir * projectileData.knockback;
            velocityLookup[other] = characterVel;
        }

        if (projectileData.explodeOnImpact)
            ecb.AddComponent(projectile, new DeadEntityTag());
    }
}

