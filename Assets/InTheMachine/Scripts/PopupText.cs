using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using QKit;

public class PopupText : MonoBehaviour
{

	[SerializeField] private GameObject popupBox;
	[SerializeField] private TextMeshProUGUI popupLabel;

	private float defaultPopupTime = 6f;
	private Alarm popupAlarm;

	public bool PopupActive => popupAlarm.IsPlaying;

	#region Singleton + Awake
	private static PopupText _singleton;
	public static PopupText main
	{
		get => _singleton;
		private set
		{
			if (_singleton == null)
			{
				_singleton = value;
			}
			else if (_singleton != value)
			{
				Debug.LogWarning("PopupText instance already exists, destroy duplicate!");
				Destroy(value);
			}
		}
	}

	private void Awake()
	{
		main = this;
	}
    #endregion

    private void Start()
    {
		popupAlarm = Alarm.Get(0f, false, false);
		popupAlarm.onComplete += () =>
		{
			popupBox.SetActive(false);

		};
    }

    public void DisplayAbilityText(Player.Ability ability)
	{
		string upgradeType = "";
		string upgradeName = "";
		string upgradeInstruction = "";

		switch (ability)
		{
			case Player.Ability.Gun:
				upgradeType = "OBTAINED:";
				upgradeName = "POWERPAK";
				upgradeInstruction = "PRESS Y FOR AIR CANNON";
				break;
			case Player.Ability.Flight:
                upgradeType = "POWERPAK UPGRADED:";
                upgradeName = "JETPAK";
                upgradeInstruction = "PRESS B IN AIR";
                break;
			case Player.Ability.Tractor:
                upgradeType = "OBTAINED:";
                upgradeName = "TRACTOR BEAM";
                upgradeInstruction = "HOLD Y DURING JETPAK";
                break;
			case Player.Ability.Boost:
                upgradeType = "OBTAINED:";
                upgradeName = "BOOST";
                upgradeInstruction = "PRESS A TO DODGE";
                break;
			default:
				break;
		}

		popupLabel.text =
			upgradeType + "\n" +
			upgradeName + "\n" +
			"\n" +
			upgradeInstruction;

		popupBox.SetActive(true);

        popupAlarm.ResetAndPlay(defaultPopupTime);
    }

    public void DisplayPowerupText(PowerUp.Type type)
    {
        string upgradeType = "";
        string upgradeName = "";
        string upgradeAmount = "";

		switch (type)
		{
			case PowerUp.Type.Power:
                upgradeType = "UPGRADE FOUND";
                upgradeName = "MAX POWER";
                upgradeAmount = "INCREASED +10";
                break;
			default:
				break;
		}

		popupLabel.text =
			upgradeType + "\n" +
            "\n" +
            upgradeName + "\n" +
            upgradeAmount;

        popupBox.SetActive(true);

        popupAlarm.ResetAndPlay(defaultPopupTime);
    }
}
