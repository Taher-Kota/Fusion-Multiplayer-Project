using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;

public class SessionCanvas : MonoBehaviour
{
    public static SessionCanvas Instance;

    [Header("Entry Panel")]
    [SerializeField] private GameObject entryPanel, loadingText;
    public GameObject joiningRoomTxt;
    [SerializeField] private Button createBtn, refreshBtn;

    [Header("Create Session Panel")]
    [SerializeField] private GameObject createRoomPanel;
    [SerializeField] private Button createRoomBtn, createPanelBackBtn;
    [SerializeField] private TMP_InputField roomNameInput, createPanelPasswordInput, maxPlayerInput;
    [SerializeField] private Toggle privateSessionToggle;
    [SerializeField] private GameObject createSessionEmptyFieldsWarning, maxPlayerWarning, sameNameWarning;

    [Header("Join Session Panel")]
    [SerializeField] private GameObject joinSessionPanel;
    [SerializeField] private TMP_InputField joinPanelPasswordInput;
    [SerializeField] private Button joinRoomBtn, joinPanelBackBtn;
    [SerializeField] private GameObject joinSessionEmptyFieldsWarning, wrongPasswordWarning;

    public Action OnRefreshSession;

    private SessionInfo currentClickSessionInfo;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        createBtn.onClick.AddListener(ActivateCreateRoomPanel);
        refreshBtn.onClick.AddListener(RefreshSession);
        createRoomBtn.onClick.AddListener(TryCreateSession);
        createPanelBackBtn.onClick.AddListener(OnCreatePanelBackBtnClick);
        joinPanelBackBtn.onClick.AddListener(OnJoinPanelBackBtn);
        joinRoomBtn.onClick.AddListener(JoinSession);
    }

    private void Start()
    {
        FusionConnection.Instance.OnConnectedToLobby += OnConnectedToLobby;
    }

    private void OnConnectedToLobby()
    {
        loadingText.SetActive(false);
    }

    private void JoinSession()
    {
        if (string.IsNullOrWhiteSpace(joinPanelPasswordInput.text))
        {
            joinSessionEmptyFieldsWarning.SetActive(true);
            return;
        }

        bool joined = FusionConnection.Instance.TryJoinSession(currentClickSessionInfo, joinPanelPasswordInput.text);
        if (joined)
        {
            joiningRoomTxt.SetActive(true);
            transform.root.gameObject.SetActive(false);
        }
        else
        {
            wrongPasswordWarning.SetActive(true);
        }
    }

    private void OnJoinPanelBackBtn()
    {
        entryPanel.SetActive(true);
        joinSessionPanel.SetActive(false);
    }

    private void OnCreatePanelBackBtnClick()
    {
        entryPanel.SetActive(true);
        createRoomPanel.SetActive(false);
    }

    public void ActivateButtons()
    {
        createBtn.interactable = true;
        refreshBtn.interactable = true;
    }

    private void ActivateCreateRoomPanel()
    {
        createRoomPanel.SetActive(true);
        entryPanel.SetActive(false);
    }

    private void TryCreateSession()
    {
        Task<bool> success = CreateSession();
        createRoomBtn.interactable = false;

        if (success.IsCompleted)
        {
            createRoomBtn.interactable = true;
        }
    }

    private async Task<bool> CreateSession()
    {
        if (string.IsNullOrWhiteSpace(roomNameInput.text))
        {
            createSessionEmptyFieldsWarning.SetActive(true);
            return false;
        }
        if (privateSessionToggle.isOn && string.IsNullOrWhiteSpace(createPanelPasswordInput.text))
        {
            createSessionEmptyFieldsWarning.SetActive(true);
            return false;
        }

        if (!int.TryParse(maxPlayerInput.text, out int maxPlayerCount))
        {
            maxPlayerWarning.SetActive(true);
            return false;
        }

        if (maxPlayerCount <= 0 || maxPlayerCount > 6)
        {
            maxPlayerWarning.SetActive(true);
            return false;
        }

        bool success;
        string sessionName = roomNameInput.text;
        if (privateSessionToggle.isOn)
        {
            string sessionKey = createPanelPasswordInput.text;
            success = await FusionConnection.Instance.CreateSession(sessionName, true, maxPlayerCount, sessionKey);
        }
        else
        {
            success = await FusionConnection.Instance.CreateSession(sessionName, false, maxPlayerCount);
        }

        if (!success)
        {
            sameNameWarning.SetActive(true);
            return false;
        }
        else
        {
            gameObject.SetActive(false); 
        }
        return true;
    }


    private void RefreshSession()
    {
        FusionConnection.Instance.RefreshSessionUI();
        refreshBtn.interactable = false;
        StartCoroutine(enableRefreshBtn());
    }

    private IEnumerator enableRefreshBtn()
    {
        yield return new WaitForSeconds(2f);
        refreshBtn.interactable = true;
    }

    public void ActivateJoinPanel(SessionInfo currentClickSessionInfo)
    {
        this.currentClickSessionInfo = currentClickSessionInfo;
        joinSessionPanel.SetActive(true);
        entryPanel.SetActive(false);
    }
}
