using System;
using UnityEngine;

[Serializable]
public class XPMagnetUpgrade : BaseUpgrade
{
    [SerializeField] private float multiplier = 1.5f;

    override public void Apply()
    {
        Player.Instance.xpMagnet.radius *= multiplier;
    }
}
