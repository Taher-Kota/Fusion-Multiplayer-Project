using Fusion;
using Photon.Voice.Unity;
using StarterAssets;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerStats : NetworkBehaviour
{
    //public static PlayerStats Instance;
    [Networked (OnChanged = nameof(OnNameChanged))] private string PlayerName { get; set; }
    [SerializeField] private TMP_Text nameText;

    static void OnNameChanged(Changed<PlayerStats> change)
    {
        change.Behaviour.UpdateNameUI();
    }

    //private void Start()
    //{
    //    if (this.HasStateAuthority)
    //    {
            
    //    }
    //}

    private void OnEnable()
    {
        if (this.HasStateAuthority)
        {
            // Only local player will get to set their name
            SetPlayerName(PlayerNameCanvas.Instance.playerName);
        }
    }

    private void SetPlayerName(string newName)
    {
        RPC_SetPlayerName(newName);
    }

    private void RPC_SetPlayerName(string newName)
    {
        PlayerName = newName;
        UpdateNameUI();
    }

    private void UpdateNameUI()
    {
        if (nameText != null)
        {
            nameText.text = PlayerName;
            gameObject.name = PlayerName;
        }
    }
}
