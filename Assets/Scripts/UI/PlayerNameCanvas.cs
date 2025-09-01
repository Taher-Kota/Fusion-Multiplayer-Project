using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNameCanvas : MonoBehaviour
{
    public static PlayerNameCanvas Instance;
    [SerializeField] private Button submitBtn;
    [SerializeField] private TMP_InputField inputField;
    public Action OnNameSubmit;
    public string playerName;
    [SerializeField] private GameObject joiningRoomText;

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        submitBtn.onClick.AddListener(ChangePlayerName);
    }

    public void ActivateButton()
    {
        if(joiningRoomText.activeInHierarchy) joiningRoomText.SetActive(false);
        submitBtn.interactable = true;
    }

    private void ChangePlayerName()
    {
        if (string.IsNullOrWhiteSpace(inputField.text))
        {
            return;
        }
        playerName = inputField.text;
        OnNameSubmit?.Invoke();
        submitBtn.gameObject.SetActive(false);
        inputField.gameObject.SetActive(false);
    }
}
