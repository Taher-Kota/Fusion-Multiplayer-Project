using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class ChatSystem : NetworkBehaviour
{

    [SerializeField] private GameObject chatMessangerCanvas, chatDisplayCanvas;
    [SerializeField] private TMP_InputField chatInput;
    [SerializeField] private TextMeshProUGUI chatBoxText;
    private static TextMeshProUGUI MyChatBody;
    private string playerName;

    [Networked (OnChanged = nameof(OnMessageUpdate))] private NetworkString<_256> newMessage { get; set; }


    private void Start()
    {
        if (!HasStateAuthority) return;
        playerName = transform.root.name;
        FusionConnection.Instance.gameInputs.Chat.SendChat.performed += SendChat_performed;
        FusionConnection.Instance.gameInputs.Chat.OpenChat.performed += OpenChatBox_performed;
        chatDisplayCanvas.gameObject.SetActive(true);
        MyChatBody = chatBoxText;
    }

    private static void OnMessageUpdate(Changed<ChatSystem> change)
    {
        MyChatBody.text += change.Behaviour.newMessage.ToString();
    }

    private void SendChat_performed(InputAction.CallbackContext obj)
    {
        if (string.IsNullOrWhiteSpace(chatInput.text)) return;
        newMessage = "\n" + playerName + " : " + chatInput.text;
        chatInput.text = "";
    }


    private void OpenChatBox_performed(InputAction.CallbackContext obj)
    {
        chatMessangerCanvas.SetActive(!chatMessangerCanvas.activeInHierarchy);

        if (chatMessangerCanvas.activeInHierarchy)
            chatInput.Select();
    }

}
