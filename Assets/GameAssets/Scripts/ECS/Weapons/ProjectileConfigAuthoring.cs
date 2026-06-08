using Unity.Entities;
using Unity.Physics;
using UnityEngine;

public class ProjectileConfigAuthoring : MonoBehaviour
{
    class Baker : Baker<ProjectileConfigAuthoring>
    {
        public override void Bake(ProjectileConfigAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);

            AddComponent(entity, new ProjectileConfig
            {
                filter = new CollisionFilter {
                    BelongsTo = 1u << LayerMask.NameToLayer("Projectiles"),
                    CollidesWith = 1u << LayerMask.NameToLayer("Character"),
                    GroupIndex = 0
                },
            });
        }
    }
}
