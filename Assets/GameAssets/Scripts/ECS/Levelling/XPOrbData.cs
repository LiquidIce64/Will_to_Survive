using Unity.Entities;
using Unity.Mathematics;

public struct XPOrbData : IComponentData
{
    public float xp;
}

public struct XPOrbAnimData : IComponentData, IEnableableComponent
{
    public float posLerp;
    public float3 startPos;
}
