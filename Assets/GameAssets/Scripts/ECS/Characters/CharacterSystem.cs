using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

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
        var entityManager = state.EntityManager;

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
                entityManager.AddComponent<DeadEntityTag>(entity);
        }
    }
}
