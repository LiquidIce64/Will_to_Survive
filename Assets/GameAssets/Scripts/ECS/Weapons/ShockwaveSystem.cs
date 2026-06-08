using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

public partial struct ShockwaveSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ShockwaveData>();
    }

    public void OnUpdate(ref SystemState state)
    {
        if (!SystemAPI.TryGetSingleton(out ProjectileConfig config)) return;
        if (!SystemAPI.TryGetSingletonEntity<PlayerTag>(out Entity player)) return;
        if (!SystemAPI.TryGetSingletonRW<ShockwaveData>(out var shockwaveData)) return;

        var deltaTime = SystemAPI.Time.DeltaTime;
        if (shockwaveData.ValueRO.cooldown > 0f)
        {
            shockwaveData.ValueRW.cooldown -= deltaTime;
            return;
        }
        shockwaveData.ValueRW.cooldown = 1f / shockwaveData.ValueRO.fireRate;

        var transform = SystemAPI.GetComponent<LocalTransform>(player);
        var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld;
        var hits = new NativeList<DistanceHit>(64, Allocator.Temp);
        if (!physicsWorld.OverlapSphere(transform.Position, shockwaveData.ValueRO.range, ref hits, config.filter)) return;
        foreach (var hit in hits)
        {
            var character = hit.Entity;
            if (character.Equals(player)) continue;
            if (!SystemAPI.TryGetComponent(character, out CharacterData characterData)) continue;

            SystemAPI.GetComponentRW<DamageData>(character).ValueRW.damageTaken += shockwaveData.ValueRO.damage;

            var knockbackDir = SystemAPI.GetComponent<LocalTransform>(character).Position - transform.Position;
            float knockbackMult = 2f - math.length(knockbackDir) / shockwaveData.ValueRO.range;
            knockbackDir = math.normalizesafe(knockbackDir, float3.zero);
            var knockback = knockbackMult * shockwaveData.ValueRO.knockback * knockbackDir;
            if (math.all(math.isfinite(knockback)))
                SystemAPI.GetComponentRW<PhysicsVelocity>(character).ValueRW.Linear += knockback / characterData.knockbackResistance;
        }
    }
}
