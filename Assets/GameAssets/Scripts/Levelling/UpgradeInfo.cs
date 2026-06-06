using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeInfo", menuName = "Scriptable Objects/UpgradeInfo")]
public class UpgradeInfo : ScriptableObject
{
    public DamageUpgrade damageUpgrade;
    public SpeedUpgrade speedUpgrade;
    public FireRateUpgrade fireRateUpgrade;
    public ExplosionRadiusUpgrade explosionRadiusUpgrade;
    public XPMagnetUpgrade xpMagnetUpgrade;
    public ShockwaveUpgrade shockwaveUpgrade;
    public HealthUpgrade healthUpgrade;

    public BaseUpgrade[] Upgrades => new BaseUpgrade[7] {
        damageUpgrade,
        speedUpgrade,
        fireRateUpgrade,
        explosionRadiusUpgrade,
        xpMagnetUpgrade,
        shockwaveUpgrade,
        healthUpgrade,
    };

    [ContextMenu("Apply Random Upgrade")]
    public void ApplyRandomUpgrade() => GetRandomUpgrade().Apply();

    public BaseUpgrade GetRandomUpgrade()
    {
        var upgrades = Upgrades;
        return upgrades[Random.Range(0, upgrades.Length)];
    }

    public IEnumerable<BaseUpgrade> GetRandomUpgrades(int n)
    {
        var upgrades = Upgrades;

        List<int> choises = new(upgrades.Length);
        for (int i = 0; i < n; i++)
        {
            if (!choises.Any()) choises.AddRange(Enumerable.Range(0, upgrades.Length));
            int idx = Random.Range(0, choises.Count);
            yield return upgrades[choises[idx]];
            choises.RemoveAt(idx);
        }
    }
}
