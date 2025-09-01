using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SessionPanel : MonoBehaviour
{
    public static SessionPanel Instance;
    [SerializeField] private TextMeshProUGUI sessionNameTxt;
    [SerializeField] private TextMeshProUGUI playerCountTxt;
    [SerializeField] private TextMeshProUGUI publicPrivateTxt;
    [SerializeField] private Button joinButton;
    private SessionInfo sessionInfo;
    
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        joinButton.onClick.AddListener(JoinSession);
    }

    public void SetSessionData(SessionInfo sessionInfo , string privateOrPublic)
    {
        this.sessionInfo = sessionInfo;
        sessionNameTxt.text = sessionInfo.Name;
        playerCountTxt.text = sessionInfo.PlayerCount.ToString() + "/" + sessionInfo.MaxPlayers.ToString();
        publicPrivateTxt.text = privateOrPublic;
    }

    private void JoinSession()
    {
        bool isPrivate = FusionConnection.Instance.IsSessionPrivate(sessionInfo);

        if (isPrivate)
        {
            SessionCanvas.Instance.ActivateJoinPanel(sessionInfo);
        }
        else
        {
            FusionConnection.Instance.JoinSession(sessionInfo.Name);
        SessionCanvas.Instance.joiningRoomTxt.SetActive(true);
            SessionCanvas.Instance.gameObject.SetActive(false);
        }
    }

    public void ToggleJoinButton(bool toggle)
    {
        joinButton.interactable = toggle;
    }
}
