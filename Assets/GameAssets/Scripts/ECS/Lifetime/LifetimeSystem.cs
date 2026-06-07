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
        var entityManager = state.EntityManager;

        foreach (var (lifeTimeData, entity) in SystemAPI.Query<RefRW<LifeTimeData>>().WithNone<DeadEntityTag>().WithEntityAccess())
        {
            float newLifeTime = lifeTimeData.ValueRW.lifeTime + deltaTime;
            if (newLifeTime >= lifeTimeData.ValueRW.maxLifeTime)
            {
                newLifeTime = lifeTimeData.ValueRW.maxLifeTime;
                entityManager.AddComponent<DeadEntityTag>(entity);
            }
            lifeTimeData.ValueRW.lifeTime = newLifeTime;
        }
    }
}
