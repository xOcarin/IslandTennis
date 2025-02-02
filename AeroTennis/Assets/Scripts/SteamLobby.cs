using System.Collections;
using UnityEngine;
using Mirror;
using Steamworks;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SteamLobby : NetworkBehaviour
{
    // Callbacks
    protected Callback<LobbyCreated_t> LobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> JoinRequest;
    protected Callback<LobbyEnter_t> LobbyEntered;

    // Vars
    public ulong CurrentLobbyID;
    private const string HostAddressKey = "HostAddress";
    private NetworkManager manager;

    // Game objects
    public GameObject HostButton;
    public Text LobbyNameText;
    public GameObject LeaveButton;
    public GameObject MainButton;
    public GameObject player1spawn;
    public GameObject controlimg;

    private void Start()
    {
        if (!SteamManager.Initialized) return;

        manager = GetComponent<NetworkManager>();
        LobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        JoinRequest = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequest);
        LobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);

        // Initialize UI
        HostButton.SetActive(true);
        LobbyNameText.gameObject.SetActive(false);
        LeaveButton.SetActive(false); 
    }

    public void HostLobby()
    {
        controlimg.SetActive(false);
        print("host clicked");
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, manager.maxConnections);
        HostButton.SetActive(false);
        LeaveButton.SetActive(true);
        MainButton.SetActive(false);
        
        
    }

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK) return;

        Debug.Log("Lobby created successfully");

        manager.StartHost();

        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey, SteamUser.GetSteamID().ToString());

        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name",
            SteamFriends.GetPersonaName() + "'s Lobby");
    }

    private void OnJoinRequest(GameLobbyJoinRequested_t callback)
    {
        Debug.Log("Request to join lobby");
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        // Everyone
        HostButton.SetActive(false);
        CurrentLobbyID = callback.m_ulSteamIDLobby;
        LobbyNameText.gameObject.SetActive(true);
        controlimg.SetActive(false);
        LobbyNameText.text = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name");

        // Clients
        if (NetworkServer.active) return;

        manager.networkAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey);

        manager.StartClient();
    }

    // This method can be called when you want to reset the lobby state, such as after disconnecting
    public void ResetLobby()
    {
        // Reset UI
        HostButton.SetActive(true);
        LobbyNameText.gameObject.SetActive(false);
        LeaveButton.SetActive(false);
        Cursor.visible = true;
        MainButton.SetActive(true);
        controlimg.SetActive(true);
        
        // Clear any cached lobby ID
        CurrentLobbyID = 0;
        
        manager.StopClient();
        manager.StopServer();
        
    }


    public void goToMainMenu()
    {
        manager.StopClient();
        manager.StopServer();
        SceneManager.LoadScene("MainMenu");
    }
    
    
}