using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.AI;

public partial struct EnemySpawnSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EnemySpawnConfig>();
    }

    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        var entityManager = state.EntityManager;
        uint seed = (uint)(SystemAPI.Time.ElapsedTime * 1000f) + 1u;
        var random = Random.CreateFromIndex(seed);

        if (!SystemAPI.TryGetSingletonEntity<EnemySpawnConfig>(out var entity)) return;

        var config = SystemAPI.GetComponentRW<EnemySpawnConfig>(entity).ValueRW;
        config.timeUntilSpawn -= deltaTime;
        if (config.timeUntilSpawn > 0f)
        {
            entityManager.SetComponentData(entity, config);
            return;
        }

        var transformMatrix = SystemAPI.GetComponentRO<LocalToWorld>(entity).ValueRO;
        float2 unitCircle = random.NextFloat2Direction();
        float3 pos = math.transform(transformMatrix.Value, new float3(unitCircle.x, 0f, unitCircle.y));

        if (!NavMesh.SamplePosition(pos, out var hit, 5f, NavMesh.AllAreas))
        {
            entityManager.SetComponentData(entity, config);
            return;
        }
        pos = hit.position;
        pos.y += 0.75f;

        var buffer = entityManager.GetBuffer<EnemySpawnPreset>(entity, true);
        if (!buffer.IsEmpty)
        {
            var preset = buffer[random.NextInt(0, buffer.Length)];
            var enemy = entityManager.Instantiate(preset.entity);
            var enemyTransform = LocalTransform.FromPosition(pos);

            entityManager.AddComponentData(enemy, preset.characterData);
            entityManager.AddComponentData(enemy, preset.enemyData);
            entityManager.AddComponentData(enemy, new DamageData { damageTaken = 0f });

            if (entityManager.HasComponent<LocalTransform>(enemy))
                entityManager.SetComponentData(enemy, enemyTransform);
            else
                entityManager.AddComponentData(enemy, enemyTransform);
        }

        config.timeUntilSpawn = config.baseSpawnInterval;
        config.timeUntilSpawn += random.NextFloat(-config.spawnIntervalRandomness, config.spawnIntervalRandomness);
        config.timeUntilSpawn *= math.pow(config.intervalLevelMultiplier, config.level);
        entityManager.SetComponentData(entity, config);
    }
}
