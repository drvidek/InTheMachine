using System;
using System.Collections.Generic;
using UnityEngine;
using QKit;

public class CashManager : MonoBehaviour
{
    [SerializeField] private Meter cash;

	public float Cash => cash.Value;

    public Action<int> onCashChange;


	#region Singleton + Awake
	private static CashManager _singleton;
	public static CashManager main
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
				Debug.LogWarning("CashManager instance already exists, destroy duplicate!");
				Destroy(value);
			}
		}
	}

	private void Awake()
	{
		main = this;
	}

	private void OnDisable()
	{
		if (main == this)
			_singleton = null;
	}
	#endregion


	private void Start()
    {
        cash.SetNewBounds(0, 99999);
    }

    public void IncreaseCashBy(float amount)
    {
        cash.Adjust(amount, false);
        onCashChange?.Invoke((int)cash.Value);
    }

    public void ChargeCash(float amount)
    {
        cash.Adjust(-amount);
        onCashChange?.Invoke((int)cash.Value);
    }

    public bool TryToBuy(float cost)
    {
        return CheckFundsAgainst(cost);
    }

    private bool CheckFundsAgainst(float cost)
    {
        if (cash.TryToAdjust(-cost))
        {
            onCashChange?.Invoke((int)cash.Value);
            return true;
        }
        return false;
    }
}
