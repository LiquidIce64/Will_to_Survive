using UnityEngine;
using UnityEngine.UI;

public class UpgradeCardUI : MonoBehaviour
{
    [SerializeField] private Image cardImage;

    [SerializeField] private Sprite damageCard;
    [SerializeField] private Sprite attackSpeedCard;
    [SerializeField] private Sprite xpMagnetCard;
    [SerializeField] private Sprite explosionRadiusCard;
    [SerializeField] private Sprite healthCard;
    [SerializeField] private Sprite walkSpeedCard;
    [SerializeField] private Sprite shockwaveCard;

    private BaseUpgrade upgrade;
    private UpgradeMenuManager menuManager;

    public void SetUpgrade(BaseUpgrade newUpgrade, UpgradeMenuManager manager)
    {
        upgrade = newUpgrade;
        menuManager = manager;

        if (upgrade is DamageUpgrade)
            cardImage.sprite = damageCard;
        else if (upgrade is FireRateUpgrade)
            cardImage.sprite = attackSpeedCard;
        else if (upgrade is XPMagnetUpgrade)
            cardImage.sprite = xpMagnetCard;
        else if (upgrade is ExplosionRadiusUpgrade)
            cardImage.sprite = explosionRadiusCard;
        else if (upgrade is HealthUpgrade)
            cardImage.sprite = healthCard;
        else if (upgrade is SpeedUpgrade)
            cardImage.sprite = walkSpeedCard;
        else if (upgrade is ShockwaveUpgrade)
            cardImage.sprite = shockwaveCard;
    }

    public void OnTakeClicked()
    {
        upgrade.Apply();
        menuManager.CloseMenu();
    }
}