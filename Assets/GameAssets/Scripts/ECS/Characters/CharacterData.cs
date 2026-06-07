using Unity.Entities;
using Unity.Mathematics;

public struct CharacterData : IComponentData
{
    public float speed;
    public float health;
    public float maxHealth;
    public float knockbackResistance;
    public float3 moveVector;
    public float3 targetPos;
    public bool isFiring;
}
