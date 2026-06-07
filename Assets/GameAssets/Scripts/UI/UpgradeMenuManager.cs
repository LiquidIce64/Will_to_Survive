using System.Linq;
using UnityEngine;

public class UpgradeMenuManager : MonoBehaviour
{
    public UpgradeInfo upgradeInfo;

    public UpgradeCardUI card1;
    public UpgradeCardUI card2;
    public UpgradeCardUI card3;

    public GameObject panel;
    public GameObject button;

    private void Start()
    {
        Player.Instance.playerXP.levelGained.AddListener(UpdateButton);
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
        Player.Instance.playerXP.AvailableUpgrades--;
        UpdateButton();
    }

    public void UpdateButton()
    {
        button.SetActive(Player.Instance.playerXP.AvailableUpgrades > 0);
    }
}