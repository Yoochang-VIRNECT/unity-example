using com.virnect.dataprovider;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateRoom : MonoBehaviour
{
    [SerializeField] private LivekitSamples livekitSamples;
    [SerializeField] private GameObject createRoomPanel;
    [SerializeField] private Button _createRoomButton;
    [SerializeField] private Button _joinRoomButton;
    [SerializeField] private Login login;

    public string sessionId;
    public string wss;
    public string token;
    public int collaborationId;

    private void Start()
    {
        _createRoomButton.onClick.AddListener(CrateRoomRequest);

        _joinRoomButton.onClick.AddListener(JoinRoomRequest);
    }

    private async void CrateRoomRequest()
    {
        var response = await PlatformServerAdapter.Livekit.CreateRoomLivekit("테스트방", /*login.Content.contentUUID*/"fed9d2d1-29ad-4975-8ad2-0d0585e172df", login.UserInfo.uuid, login.Workspace.uuid, "2.9.0.1", "MOBILE");

        wss = response.wss;
        token = response.token;
        sessionId = response.sessionId;

        livekitSamples.url = wss;
        livekitSamples.token = token;

        createRoomPanel.SetActive(false);
    }

    private async void JoinRoomRequest()
    {
        var roomList = await PlatformServerAdapter.Livekit.GetRoomLiveKitList(login.Workspace.uuid);
        if (roomList.contents.Count == 0)
        {
            return;
        }
        collaborationId = roomList.contents[0].collaborationId;

        var response = await PlatformServerAdapter.Livekit.JoinRoomLivekit(login.Workspace.uuid, collaborationId, "2.9.0.1", /*login.Content.serialNumber*/"3ced2d30-135d-463e-802c-97b4f3acdfc2", login.UserInfo.uuid, "MEMBER", "MOBILE");
        wss = response.wss;
        token = response.token;
        sessionId = response.sessionId;

        livekitSamples.url = wss;
        livekitSamples.token = token;

        createRoomPanel.SetActive(false);
    }
}
