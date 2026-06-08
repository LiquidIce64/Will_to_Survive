using Unity.Entities;
using Unity.Physics;

public struct ProjectileConfig : IComponentData
{
    public CollisionFilter filter;
}
