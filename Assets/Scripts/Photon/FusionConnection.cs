using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;
using Cinemachine;
using Photon.Voice.Fusion;
using System.Threading.Tasks;

public class FusionConnection : MonoBehaviour, INetworkRunnerCallbacks
{
    public static FusionConnection Instance;
    public NetworkRunner runner;
    [SerializeField] private NetworkObject playerPrefab;
    private NetworkObject player;
    private List<SessionInfo> sessionInfoList { get; set; }
    [SerializeField] private GameObject sessionPrefab;
    [SerializeField] private Transform sessionContent;
    [SerializeField] private GameObject nameCanvas;
    public GameInputs gameInputs;
    public Action OnPunch , OnConnectedToLobby;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        sessionInfoList = new List<SessionInfo>();
    }

    private void Start()
    {
        // only connect to lobby if runner exists and we are not host
        if (runner != null && !runner.IsSharedModeMasterClient)
        {
            ConnectToLobby();
        }

    }

    public bool IsSessionPrivate(SessionInfo sessionInfo)
    {
        if (sessionInfo.Properties.TryGetValue("isPrivate", out SessionProperty value))
        {
            if (value == 1)
            {
                return true;
            }
        }
        return false;
    }

    public bool TryJoinSession(SessionInfo sessionInfo, string passKey)
    {
        if (sessionInfo.Properties.TryGetValue("password", out SessionProperty value))
        {
            if (value != null)
            {
                if (value == passKey)
                {
                    JoinSession(sessionInfo.Name);
                    return true;
                }
            }
        }
        return false;
    }

    public async void JoinSession(string sessionName)
    {
        if (runner == null)
        {
            runner = gameObject.AddComponent<NetworkRunner>();
        }

        await runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = sessionName,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
        });

        nameCanvas.SetActive(true);
        PlayerNameCanvas.Instance.ActivateButton();
        PlayerNameCanvas.Instance.OnNameSubmit += OnNameSubmit;
    }

    public void RefreshSessionUI()
    {
        foreach (Transform child in sessionContent)
        {
            Destroy(child.gameObject);
        }

        foreach (SessionInfo session in sessionInfoList)
        {
            if (session.IsVisible)
            {
                GameObject newSession = Instantiate(sessionPrefab, sessionContent);
                if (newSession.TryGetComponent<SessionPanel>(out SessionPanel script))
                {
                    if (IsSessionPrivate(session))
                    {
                        script.SetSessionData(session , "Private");
                    }
                    else
                    {
                        script.SetSessionData(session , "Public");
                    }
                }
                if (session.IsOpen == false || session.PlayerCount >= session.MaxPlayers)
                {
                    script.ToggleJoinButton(false);
                }
                else
                {
                    script.ToggleJoinButton(true);
                }
            }

        }
    }

    private void OnNameSubmit()
    {
        player?.gameObject.SetActive(true);
    }

    //public async void ConnectToRunner()
    //{
    //    if (runner == null)
    //    {
    //        runner = gameObject.AddComponent<NetworkRunner>();
    //    }

    //    await runner.StartGame(new StartGameArgs()
    //    {
    //        GameMode = GameMode.Shared,
    //        SessionName = "practiceRoom",
    //        PlayerCount = 2,
    //        SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
    //    });

    //    PlayerNameCanvas.Instance.ActivateButton();
    //    player = runner.Spawn(playerPrefab);
    //    runner.SetPlayerObject(runner.LocalPlayer, player);
    //}

    public async void ConnectToLobby()
    {
        if (runner == null)
        {
            runner = gameObject.AddComponent<NetworkRunner>();
            runner.ProvideInput = true;
        }

        var result = await runner.JoinSessionLobby(SessionLobby.Shared);

        if (!result.Ok)
        {
            Debug.LogError($"Failed to join lobby: {result.ShutdownReason}");
        }

        SessionCanvas.Instance.ActivateButtons();
        OnConnectedToLobby?.Invoke();
        Invoke("RefreshSessionUI", 1f);
    }

    public async Task<bool> CreateSession(string sessionName, bool isPrivate, int maxPlayers = 2, string sessionKey = null)
    {
        foreach(SessionInfo session in sessionInfoList)
        {
            if (session.Name == sessionName)
                return false;
        }

        Dictionary<string, SessionProperty> sessionProperties;
        if (sessionKey == null)
        {
            sessionProperties = new Dictionary<string, SessionProperty>{
               { "isPrivate", Convert.ToInt32(isPrivate)},
            };
        }
        else
        {
            sessionProperties = new Dictionary<string, SessionProperty>{
               { "password", sessionKey },
               { "isPrivate", Convert.ToInt32(isPrivate)},
            };
        }

        if (runner == null)
        {
            runner = gameObject.AddComponent<NetworkRunner>();
            runner.ProvideInput = true;
        }

        var result = await runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = sessionName,
            SessionProperties = sessionProperties,
            PlayerCount = maxPlayers,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
        });

        if (!result.Ok)
        {
            Debug.LogError($"CreateSession failed: {result.ShutdownReason}");
            return false;
        }

        nameCanvas.SetActive(true);
        PlayerNameCanvas.Instance.ActivateButton();
        PlayerNameCanvas.Instance.OnNameSubmit += OnNameSubmit;
        return true;
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        sessionInfoList.Clear();
        sessionInfoList.AddRange(sessionList);
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        PlayerInputData data = new PlayerInputData();

        data.Move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        data.Look = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        data.Jump = Input.GetKey(KeyCode.Space);
        data.Sprint = Input.GetKey(KeyCode.LeftShift);
        data.Talk = Input.GetKey(KeyCode.T);
        data.AnalogMovement = false; // or true if using joystick
        input.Set(data);
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if ((runner.IsSharedModeMasterClient || player == runner.LocalPlayer) && this.player == null)
        {
            this.player = runner.Spawn(playerPrefab, Vector3.zero, Quaternion.identity, player);
            runner.SetPlayerObject(runner.LocalPlayer, this.player);
            gameInputs = new GameInputs();
            gameInputs.Enable();
        }
    }


    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnDisconnectedFromServer(NetworkRunner runner) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
}
