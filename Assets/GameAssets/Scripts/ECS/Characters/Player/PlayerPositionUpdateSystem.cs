using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(TransformSystemGroup))]
public partial struct PlayerPositionUpdateSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        if (Camera.main == null) return;
        var player = SystemAPI.GetSingletonEntity<PlayerTag>();
        var transform = SystemAPI.GetComponentRO<LocalTransform>(player);
        Camera.main.transform.parent.position = transform.ValueRO.Position;
    }
}
