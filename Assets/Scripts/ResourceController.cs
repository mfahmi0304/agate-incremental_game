using UnityEngine;
using UnityEngine.UI;

public class ResourceController : MonoBehaviour
{
    public AudioSource ButtonUpgrade;
    public Button ResourceButton;
    public Image ResourceImage;
    public Text ResourceDescription;
    public Text ResourceUpgradeCost;
    public Text ResourceUnlockCost; 

    public string unlockCost;
    public string upCost;
    public string descCost;

    private ResourceConfig _config;

    private int _level = 1;
    public bool IsUnlocked { get; private set; } 

    private void Start ()
    {
       ResourceButton.onClick.AddListener (() =>
        {
            if (IsUnlocked)
            {
                UpgradeLevel ();
            }
            else
            {
                UnlockResource ();
            }
        });
    }

    public void SetConfig (ResourceConfig config)
    {
        _config = config;

        unlockCost = _config.UnlockCost.ToString("#,#");
        descCost = GetOutput ().ToString("#,#");

        // ToString("0") berfungsi untuk membuang angka di belakang koma
        ResourceDescription.text = $"{ _config.Name } Lv. { _level }\n+{ descCost }";
        ResourceUnlockCost.text = $"Unlock Cost\n{ unlockCost }";
        ResourceUpgradeCost.text = $"Upgrade Cost\n{ GetUpgradeCost () }";

        SetUnlocked (_config.UnlockCost == 0);
    } 

    public double GetOutput ()
    {
        return _config.Output * _level;
    }

    public double GetUpgradeCost ()
    {
        return _config.UpgradeCost * _level;
    }

    public double GetUnlockCost ()
    {
        return _config.UnlockCost;
    }

     public void UpgradeLevel ()
    {
        double upgradeCost = GetUpgradeCost ();
        if (GameManager.Instance._totalGold < upgradeCost)
        {
            return;
        }
        else{
            if(_level == 10){
                return;
            }
            else{
                GameManager.Instance.AddGold (-upgradeCost);
                _level++;

                upCost = GetUpgradeCost ().ToString("#,#");
                descCost = GetOutput ().ToString("#,#");

                ButtonUpgrade.Play();
                if(_level == 10){
                    ResourceUpgradeCost.text = $"MAX";
                }
                else{
                    ResourceUpgradeCost.text = $"Upgrade Cost\n{ upCost }";
                }
                ResourceDescription.text = $"{ _config.Name } Lv. { _level }\n+{ descCost }";
            }
        }
    }

    public void UnlockResource ()
    {
        double unlockCost = GetUnlockCost ();
        if (GameManager.Instance._totalGold < unlockCost)
        {
            return;
        } 

        SetUnlocked (true);
        GameManager.Instance.ShowNextResource ();

        AchievementController.Instance.UnlockAchievement (AchievementType.UnlockResource, _config.Name);
    }

    public void SetUnlocked (bool unlocked)
    {
        IsUnlocked = unlocked;
        ResourceImage.color = IsUnlocked ? Color.white : Color.grey;
        ResourceUnlockCost.gameObject.SetActive (!unlocked);
        ResourceUpgradeCost.gameObject.SetActive (unlocked);
    }
}