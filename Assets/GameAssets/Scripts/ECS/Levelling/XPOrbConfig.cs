using Unity.Entities;
using Unity.Transforms;

public struct XPOrbConfig : IComponentData
{
    public Entity prefab;
    public LocalTransform prefabTransform;
    public float pickupSpeed;
    public int layer;
    public float xpGained;
}
