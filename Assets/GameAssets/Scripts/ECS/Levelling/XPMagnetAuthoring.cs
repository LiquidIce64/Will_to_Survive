using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class XPMagnetAuthoring : MonoBehaviour
{
    [SerializeField] private GameObject xpOrbPrefab;
    [SerializeField] private float pickupSpeed;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.lightGreen;
        Gizmos.DrawWireSphere(transform.position, transform.localScale.x);
    }

    class Baker : Baker<XPMagnetAuthoring>
    {
        public override void Bake(XPMagnetAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            Entity prefab = GetEntity(authoring.xpOrbPrefab, TransformUsageFlags.Dynamic);
            DependsOn(authoring.xpOrbPrefab);

            var orbTransform = authoring.xpOrbPrefab.transform;
            var prefabTransform = LocalTransform.FromPositionRotationScale(
                orbTransform.position, orbTransform.rotation, orbTransform.localScale.x);

            AddComponent<XPMagnetTag>(entity);
            AddComponent(entity, new XPOrbConfig
            {
                prefab = prefab,
                prefabTransform = prefabTransform,
                pickupSpeed = authoring.pickupSpeed,
                layer = LayerMask.NameToLayer("XP orbs"),
                xpGained = 0f,
            });
        }
    }
}
