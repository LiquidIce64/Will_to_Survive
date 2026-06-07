using System.Linq;
using UnityEngine;

public class UpgradeMenuManager : MonoBehaviour
{
    public UpgradeInfo upgradeInfo;

    public UpgradeCardUI card1;
    public UpgradeCardUI card2;
    public UpgradeCardUI card3;

    private void CloseMenuInstant()
    {
        gameObject.SetActive(false);
    }
    private void Start()
    {
        gameObject.SetActive(true);
        CloseMenuInstant();
    }

    public void OpenMenu()
    {
        gameObject.SetActive(true);
        Time.timeScale = 0f;

        var upgrades = upgradeInfo.GetRandomUpgrades(3).ToList();

        card1.SetUpgrade(upgrades[0], this);
        card2.SetUpgrade(upgrades[1], this);
        card3.SetUpgrade(upgrades[2], this);
    }

    public void CloseMenu()
    {
        Time.timeScale = 1f;
        gameObject.SetActive(false);
    }
}