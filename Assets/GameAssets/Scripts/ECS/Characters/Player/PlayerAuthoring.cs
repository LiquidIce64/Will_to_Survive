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

            var controller = FindAnyObjectByType<PlayerController>();
            DependsOn(controller);
            AddComponentObject(entity, controller);

            AddComponent(entity, new CharacterData
            {
                speed = controller.speed,
                health = controller.maxHealth,
                maxHealth = controller.maxHealth,
                knockbackResistance = controller.knockbackResistance,
                moveVector = float3.zero,
                targetPos = float3.zero,
                isFiring = false,
            });
            AddComponent(entity, new DamageData { damageTaken = 0f });
            AddComponent<PlayerTag>(entity);
        }
    }
}
