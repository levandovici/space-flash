using UnityEngine.Advertisements;
using UnityEngine.UI;
using UnityEngine;
using System;

public class RewardedAdButton : MonoBehaviour
{
    [SerializeField]
    private Button _showAdButton;

    [SerializeField]
    private AdsManager.EReward _reward = AdsManager.EReward.coins_5000;



    public event Action<AdsManager.EReward> OnShowAd;



    public bool CanShowAd
    {
        set
        {
            _showAdButton.gameObject.SetActive(value);

            _showAdButton.interactable = value;
        }
    }



    void Awake()
    {
        CanShowAd = false;

        _showAdButton.onClick.AddListener(() => OnShowAd.Invoke(_reward));
    }



    void OnDestroy()
    {
        _showAdButton.onClick.RemoveAllListeners();
    }
}
