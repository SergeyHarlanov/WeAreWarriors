using System;
using System.Collections;
using System.Collections.Generic;
using Assets.SimpleLocalization.Scripts;
using GAME.Scripts.Signals;
using TMPro;
using UnityEngine;
using Zenject;

public class CoinsCollected : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _coinsCollectedText;

    private SignalBus _signalBus;
    private int _currentCollectCoint;
    

    private void OnEnable()
    {
        UpdateCoinsCollectedText(new CoinCollectedBattleSignal() { TotalCoin = 0});
        _signalBus.Subscribe<LocalizationCloneObjectsSignal>(OnLocalization);
    }

    private void OnDisable()
    {
        _signalBus.Unsubscribe<LocalizationCloneObjectsSignal>(OnLocalization);
    }

    public void Init(SignalBus signalBus)
    {
        _signalBus = signalBus;
    }

    public void UpdateCoinsCollectedText(CoinCollectedBattleSignal args)
    {
        _currentCollectCoint = args.TotalCoin;
        OnLocalization();
    }

    private void OnLocalization()
    {
        _coinsCollectedText.text = LocalizationManager.Localize("game_screen_CollecCoin")+ " " + _currentCollectCoint;
    }
}
