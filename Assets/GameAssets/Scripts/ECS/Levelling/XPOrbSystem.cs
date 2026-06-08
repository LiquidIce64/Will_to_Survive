using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

partial struct XPOrbSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<XPOrbConfig>();
        state.RequireForUpdate<XPMagnetTag>();
        state.RequireForUpdate<XPOrbData>();
    }

    public void OnUpdate(ref SystemState state)
    {
        if (!SystemAPI.TryGetSingletonEntity<XPMagnetTag>(out var magnet)) return;
        var config = SystemAPI.GetComponent<XPOrbConfig>(magnet);
        var magnetRadius = SystemAPI.GetComponent<LocalTransform>(magnet).Scale;
        var magnetPos = SystemAPI.GetComponent<LocalToWorld>(magnet).Position;

        var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld;
        var hits = new NativeList<DistanceHit>(10, Allocator.Temp);
        uint layerMask = 1u << config.layer;
        var filter = new CollisionFilter { BelongsTo = layerMask, CollidesWith = layerMask, GroupIndex = 0 };
        if (physicsWorld.OverlapSphere(magnetPos, magnetRadius, ref hits, filter))
            foreach (var hit in hits)
            {
                var entity = hit.Entity;
                if (SystemAPI.HasComponent<XPOrbAnimData>(entity))
                    SystemAPI.SetComponentEnabled<XPOrbAnimData>(entity, true);
            }

        float deltaLerp = SystemAPI.Time.DeltaTime * config.pickupSpeed;
        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        foreach (var (transform, orbData, animData, entity) in SystemAPI.Query<
            RefRW<LocalTransform>, RefRO<XPOrbData>, RefRW<XPOrbAnimData>
        >().WithEntityAccess())
        {
            var lerp = animData.ValueRO.posLerp + deltaLerp;
            if (lerp >= 1f)
            {
                config.xpGained += orbData.ValueRO.xp;
                ecb.DestroyEntity(entity);
                continue;
            }
            animData.ValueRW.posLerp = lerp;
            transform.ValueRW.Position = math.lerp(animData.ValueRO.startPos, magnetPos, lerp * lerp);
        }

        ecb.SetComponent(magnet, config);
    }
}
