using Unity.Entities;
using UnityEngine;

public class XPMagnetAuthoring : MonoBehaviour
{
    class Baker : Baker<XPMagnetAuthoring>
    {
        public override void Bake(XPMagnetAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<XPMagnetTag>(entity);
        }
    }
}
