using Unity.Entities;

public partial struct LifetimeSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<LifeTimeData>();
    }

    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        foreach (var (lifeTimeData, entity) in SystemAPI.Query<RefRW<LifeTimeData>>().WithNone<DeadEntityTag>().WithEntityAccess())
        {
            float newLifeTime = lifeTimeData.ValueRW.lifeTime + deltaTime;
            if (newLifeTime >= lifeTimeData.ValueRW.maxLifeTime)
            {
                newLifeTime = lifeTimeData.ValueRW.maxLifeTime;
                ecb.AddComponent<DeadEntityTag>(entity);
            }
            lifeTimeData.ValueRW.lifeTime = newLifeTime;
        }
    }
}
