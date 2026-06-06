using System;
using UnityEngine;

[Serializable]
public class ShockwaveUpgrade : BaseUpgrade
{
    [SerializeField] private GameObject shockwavePrefab;

    override public void Apply()
    {
        if (Shockwave.Instance != null)
            Shockwave.Instance.fireRate *= 2;
        else
            GameObject.Instantiate(shockwavePrefab, Player.Instance.transform);
    }
}
