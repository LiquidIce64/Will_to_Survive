using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;

[UpdateInGroup(typeof(PhysicsSystemGroup))]
[UpdateAfter(typeof(PhysicsSimulationGroup))]
[BurstCompile]
partial struct ProjectileCollisionSystem : ISystem
{
    private ComponentLookup<DeadEntityTag> _deadEntityLookup;
    private ComponentLookup<ProjectileData> _projectileLookup;
    private ComponentLookup<CharacterData> _characterLookup;
    private ComponentLookup<DamageData> _damageLookup;
    private ComponentLookup<PhysicsVelocity> _velocityLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _deadEntityLookup = state.GetComponentLookup<DeadEntityTag>(true);
        _projectileLookup = state.GetComponentLookup<ProjectileData>(true);
        _characterLookup = state.GetComponentLookup<CharacterData>(true);
        _damageLookup = state.GetComponentLookup<DamageData>(false);
        _velocityLookup = state.GetComponentLookup<PhysicsVelocity>(false);

        state.RequireForUpdate<ProjectileData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var simulation = SystemAPI.GetSingleton<SimulationSingleton>();
        var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld;

        _deadEntityLookup.Update(ref state);
        _projectileLookup.Update(ref state);
        _characterLookup.Update(ref state);
        _velocityLookup.Update(ref state);
        _damageLookup.Update(ref state);

        state.Dependency = new ProcessTriggerEventsJob
        {
            PhysicsWorld = physicsWorld,
            player = SystemAPI.GetSingletonEntity<PlayerTag>(),
            deadEntityLookup = _deadEntityLookup,
            projectileLookup = _projectileLookup,
            characterLookup = _characterLookup,
            damageLookup = _damageLookup,
            velocityLookup = _velocityLookup,
            ecb = ecb.AsParallelWriter(),
        }.Schedule(simulation, state.Dependency);

        state.CompleteDependency();
    }
}

[BurstCompile]
public partial struct ProcessTriggerEventsJob : ITriggerEventsJob
{
    [ReadOnly] public PhysicsWorld PhysicsWorld;
    [ReadOnly] public Entity player;
    [ReadOnly] public ComponentLookup<DeadEntityTag> deadEntityLookup;
    [ReadOnly] public ComponentLookup<ProjectileData> projectileLookup;
    [ReadOnly] public ComponentLookup<CharacterData> characterLookup;
    public ComponentLookup<DamageData> damageLookup;
    public ComponentLookup<PhysicsVelocity> velocityLookup;

    public EntityCommandBuffer.ParallelWriter ecb;

    public void Execute(TriggerEvent triggerEvent)
    {
        if (deadEntityLookup.HasComponent(triggerEvent.EntityA) ||
            deadEntityLookup.HasComponent(triggerEvent.EntityB)) return;

        Entity projectile, other;
        if (projectileLookup.HasComponent(triggerEvent.EntityA))
        {
            projectile = triggerEvent.EntityA;
            other = triggerEvent.EntityB;
        }
        else
        {
            projectile = triggerEvent.EntityB;
            other = triggerEvent.EntityA;
            if (!projectileLookup.HasComponent(projectile)) return;
        }
        var projectileData = projectileLookup[projectile];

        if (characterLookup.HasComponent(other))
        {
            if (other.Equals(player) == projectileData.playerOwned) return;
            if (projectileData.damageOnImpact)
            {
                var damageData = damageLookup[other];
                damageData.damageTaken += projectileData.damage;
                damageLookup[other] = damageData;
                var projectileDir = velocityLookup[projectile].Linear;
                projectileDir.y = 0f;
                projectileDir = math.normalizesafe(projectileDir, float3.zero);
                var characterVel = velocityLookup[other];
                characterVel.Linear += projectileData.knockback / characterLookup[other].knockbackResistance * projectileDir;
                if (math.all(math.isfinite(characterVel.Linear))) velocityLookup[other] = characterVel;
            }
        }

        if (projectileData.explodeOnImpact)
            ecb.AddComponent<DeadEntityTag>(triggerEvent.BodyIndexA + triggerEvent.BodyIndexB << 16, projectile);
    }
}
