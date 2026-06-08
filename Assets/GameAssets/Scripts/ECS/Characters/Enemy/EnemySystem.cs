using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial struct EnemySystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<CharacterData>();
        state.RequireForUpdate<EnemyData>();
    }

    private void ResetAimTime(ref EnemyData enemyData, ref Random random)
    {
        float multiplier = 1f + random.NextFloat(-enemyData.aimingTimeRandomness, enemyData.aimingTimeRandomness);
        enemyData.remainingAimTime = enemyData.aimingTime * multiplier;
        enemyData.seed = random.NextUInt();
    }

    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        if (!SystemAPI.TryGetSingletonEntity<PlayerTag>(out var player)) return;
        var playerPos = SystemAPI.GetComponentRO<LocalTransform>(player).ValueRO.Position;

        foreach (var (transform, characterData, enemyData, entity) in SystemAPI.Query<
            RefRO<LocalTransform>, RefRW<CharacterData>, RefRW<EnemyData>
        >().WithEntityAccess())
        {
            if (SystemAPI.HasComponent<DeadEntityTag>(entity))
            {
                if (SystemAPI.TryGetSingleton(out XPOrbConfig config))
                {
                    var xp = enemyData.ValueRO.xpDrop;
                    var newTransform = config.prefabTransform.Translate(transform.ValueRO.Position);
                    Entity orb = ecb.Instantiate(config.prefab);
                    ecb.SetComponent(orb, newTransform);
                    ecb.AddComponent(orb, new XPOrbData { xp = xp });
                    ecb.AddComponent(orb, new XPOrbAnimData { posLerp = 0f, startPos = newTransform.Position });
                    ecb.SetComponentEnabled<XPOrbAnimData>(orb, false);
                }
                ecb.DestroyEntity(entity);
                continue;
            }

            characterData.ValueRW.targetPos = playerPos;
            var playerDir = playerPos - transform.ValueRO.Position;
            playerDir.y = 0f;
            float distance = math.length(playerDir);
            playerDir = math.normalizesafe(playerDir, float3.zero);
            var random = Random.CreateFromIndex(enemyData.ValueRO.seed);

            switch (enemyData.ValueRO.state)
            {
                case NPCState.Follow:
                    ResetAimTime(ref enemyData.ValueRW, ref random);
                    characterData.ValueRW.moveVector = playerDir;
                    if (distance <= enemyData.ValueRO.attackDistance)
                        enemyData.ValueRW.state = NPCState.Attack;
                    break;
                case NPCState.Attack:
                    characterData.ValueRW.moveVector = float3.zero;
                    if (enemyData.ValueRO.remainingAimTime > 0f)
                        enemyData.ValueRW.remainingAimTime -= deltaTime;
                    else
                    {
                        //weapon.Use();
                    }
                    if (distance > enemyData.ValueRO.maxAttackDistance)
                        enemyData.ValueRW.state = NPCState.Follow;
                    else if (distance < enemyData.ValueRO.minAttackDistance)
                        enemyData.ValueRW.state = NPCState.Retreat;
                    break;
                case NPCState.Retreat:
                    ResetAimTime(ref enemyData.ValueRW, ref random);
                    characterData.ValueRW.moveVector = -playerDir;
                    if (distance >= enemyData.ValueRO.attackDistance)
                        enemyData.ValueRW.state = NPCState.Attack;
                    break;
            }
        }
    }
}
