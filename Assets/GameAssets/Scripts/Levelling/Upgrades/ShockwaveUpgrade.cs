using System;
using UnityEngine;

[Serializable]
public class ShockwaveUpgrade : BaseUpgrade
{
    [SerializeField] private GameObject shockwavePrefab;

    override public void Apply()
    {
        GameObject.Instantiate(shockwavePrefab);
    }
}
