using System.Linq;
using UnityEngine;

public class UpgradeMenuManager : MonoBehaviour
{
    private static UpgradeMenuManager instance;

    public UpgradeInfo upgradeInfo;

    public UpgradeCardUI card1;
    public UpgradeCardUI card2;
    public UpgradeCardUI card3;

    public GameObject panel;
    public GameObject button;

    public static UpgradeMenuManager Instance => instance;

    private void Awake()
    {
        instance = this;
    }

    public void OpenMenu()
    {
        panel.SetActive(true);
        Time.timeScale = 0f;

        var upgrades = upgradeInfo.GetRandomUpgrades(3).ToList();

        card1.SetUpgrade(upgrades[0], this);
        card2.SetUpgrade(upgrades[1], this);
        card3.SetUpgrade(upgrades[2], this);
    }

    public void CloseMenu()
    {
        Time.timeScale = 1f;
        panel.SetActive(false);
        PlayerController.Instance.playerXP.AvailableUpgrades--;
        UpdateButton();
    }

    public void UpdateButton()
    {
        button.SetActive(PlayerController.Instance.playerXP.AvailableUpgrades > 0);
    }
}