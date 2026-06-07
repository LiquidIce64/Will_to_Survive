using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class PlayerAuthoring : MonoBehaviour
{
    class Baker : Baker<PlayerAuthoring>
    {
        public override void Bake(PlayerAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            DependsOn(PlayerController.Instance);
            AddComponentObject(entity, PlayerController.Instance);

            AddComponent(entity, new CharacterData
            {
                speed = PlayerController.Instance.speed,
                health = PlayerController.Instance.maxHealth,
                maxHealth = PlayerController.Instance.maxHealth,
                knockbackResistance = PlayerController.Instance.knockbackResistance,
                moveVector = float3.zero,
                targetPos = float3.zero,
                isFiring = false,
            });
            AddComponent(entity, new DamageData { damageTaken = 0f });
            AddComponent<PlayerTag>(entity);
        }
    }
}
