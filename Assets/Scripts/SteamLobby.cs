using UnityEngine;
using Mirror;
using Steamworks;

public class SteamLobby : MonoBehaviour
{
    public GameObject hostButton = null;

    private NetworkManager networkManager;

    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> lobbyEntered;
    private const string HostAddressKey = "HostAddress";

    private void Start()
    {
        networkManager = GetComponent<NetworkManager>();

        if (!SteamManager.Initialized) { return; }

        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
        lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
    }

    public void HostLobby()
    {
        hostButton.SetActive(false);
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, networkManager.maxConnections);
    }

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        // Nếu tạo Lobby thất bại
        if (callback.m_eResult != EResult.k_EResultOK)
        {
            hostButton.SetActive(true);
            return;
        }

        // Nếu thành công, bắt đầu Host server thông qua Mirror
        networkManager.StartHost();

        // Lưu Steam ID của người tạo (Host) vào dữ liệu của Lobby để người khác có thể tìm thấy và kết nối
        SteamMatchmaking.SetLobbyData(
            new CSteamID(callback.m_ulSteamIDLobby), 
            HostAddressKey, 
            SteamUser.GetSteamID().ToString()
        );
    }

    // 2. Xử lý khi một người chơi nhấn "Join Game" qua Steam (ví dụ: qua danh sách bạn bè)
    private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    // 3. Xử lý khi người chơi đã thực sự vào trong Lobby
    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        // Nếu là Host (Server đã chạy) thì không làm gì thêm
        if (NetworkServer.active) { return; }

        // Lấy địa chỉ IP (Steam ID) của Host từ dữ liệu Lobby
        string hostAddress = SteamMatchmaking.GetLobbyData(
            new CSteamID(callback.m_ulSteamIDLobby), 
            HostAddressKey
        );

        // Kết nối Mirror Client đến địa chỉ của Host
        networkManager.networkAddress = hostAddress;
        networkManager.StartClient();

        // Ẩn nút Host sau khi đã vào phòng
        hostButton.SetActive(false);
    }
}
