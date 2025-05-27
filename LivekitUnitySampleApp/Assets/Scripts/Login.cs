using com.virnect.dataprovider;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    [SerializeField] private TMP_InputField _idInputField;
    [SerializeField] private TMP_InputField _pwInputField;
    [SerializeField] private Button _loginButton;

    public UserInfo UserInfo;
    public WorkspaceInfo Workspace;
    public ContentInfo Content;

    private void Start()
    {
        PlatformServerAdapter.State.PrivateServerURL = "https://192.168.0.178:8073";
        PlatformServerAdapter.State.Server = TargetServer.Private;

        _loginButton.onClick.AddListener(() =>
        {
            if (string.IsNullOrEmpty(_idInputField.text) || string.IsNullOrEmpty(_pwInputField.text))
            {
                return;
            }
            ServerLogin();
        });
    }

    private async void ServerLogin()
    {
        string id = _idInputField.text;
        string pw = _pwInputField.text;

        var result = await PlatformServerAdapter.Account.Login(id, pw);

        UserInfo = await PlatformServerAdapter.Account.GetMyUserInfo();

        var workspaceList = await PlatformServerAdapter.Workspace.GetMyAccessibleWorkspaceList(UserInfo.uuid, product: "VIEW");
        Workspace = workspaceList.contents[0];

        var contentsInfoList = await PlatformServerAdapter.Contents.GetContentsInfoList(UserInfo.uuid, Workspace.uuid);
        Content = contentsInfoList.contentInfo[0];

    }
}
