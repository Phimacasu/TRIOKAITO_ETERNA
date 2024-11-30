using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class MenuController : MonoBehaviourPunCallbacks
{
    [SerializeField] private string Versionname = "0.1";
    [SerializeField] private GameObject ConnectPanel;

    [SerializeField] private InputField JoinGameInput;
    [SerializeField] private InputField CreateGameInput;

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.GameVersion = Versionname;
        Debug.Log("Connecting to Photon...");
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby(TypedLobby.Default);
        Debug.Log("Connected to Master Server.");
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby.");
        ConnectPanel.SetActive(true);
    }

    public void CreateGame()
    {
        PhotonNetwork.CreateRoom(CreateGameInput.text, new RoomOptions() { MaxPlayers = 4 }, null);
        Debug.Log("Creating Room: " + CreateGameInput.text);
    }

    public void JoinGame()
    {
        PhotonNetwork.JoinRoom(JoinGameInput.text);
        Debug.Log("Joining Room: " + JoinGameInput.text);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Successfully joined room. Loading game scene...");
        PhotonNetwork.LoadLevel("Game");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"Create Room Failed: {message}");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"Join Room Failed: {message}");
    }
}
