using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
#if UNITY_IOS
using UnityEngine.iOS;
#endif

public class PermissionCheck : MonoBehaviour
{
    private int sdkVersion;
    private List<string> permissions = new List<string>() { Permission.Camera, Permission.Microphone, };

    private int grantedPermissionCount = 0;

    const string BLUETOOTH_CONNECT = "android.permission.BLUETOOTH_CONNECT";
    const string BLUETOOTH = "android.permission.BLUETOOTH";
    const string READ_MEDIA_VIDEO = "android.permission.READ_MEDIA_VIDEO";
    const string READ_MEDIA_IMAGES = "android.permission.READ_MEDIA_IMAGES";
    const string READ_MEDIA_AUDIO = "android.permission.READ_MEDIA_AUDIO";

    private void Awake()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass buildVersion = new AndroidJavaClass("android.os.Build$VERSION");
        sdkVersion = buildVersion.GetStatic<int>("SDK_INT");
#endif
// Android Audio Manager 설정 필요
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaObject audioManager = new AndroidJavaObject("android.media.AudioManager");
        // STREAM_VOICE_CALL 모드로 설정
        audioManager.Call("setMode", 3); // AudioManager.MODE_IN_COMMUNICATION
#endif

        SetPermissionList();
        RequestPermission();
    }

    public void RequestPermission()
    {
        StartCoroutine(nameof(RequestPermissionCoroutine));
    }

    private void SetPermissionList()
    {
        if (sdkVersion >= 33)
        {
            permissions.Add(READ_MEDIA_VIDEO);
            permissions.Add(READ_MEDIA_IMAGES);
            permissions.Add(READ_MEDIA_AUDIO);
        }
        else
        {
            permissions.Add(Permission.ExternalStorageRead);

            if (sdkVersion <= 29)
            {
                permissions.Add(Permission.ExternalStorageWrite);
            }
        }

        if (sdkVersion >= 31)
        {
            permissions.Add(BLUETOOTH_CONNECT);
        }
        else
        {
            permissions.Add(BLUETOOTH);
        }

        foreach (var device in WebCamTexture.devices)
        {
            Debug.Log($"{device.name}");
        }

        foreach (var device in Microphone.devices)
        {
            Debug.Log($"{device}");
        }
    }

    private IEnumerator RequestPermissionCoroutine()
    {
        var callbacks = new PermissionCallbacks();
        callbacks.PermissionDenied += PermissionCallbacks_PermissionDenied;
        callbacks.PermissionGranted += PermissionCallbacks_PermissionGranted;
        callbacks.PermissionDeniedAndDontAskAgain += PermissionCallbacks_PermissionDeniedAndDontAskAgain;

        foreach (var permission in permissions)
        {
            Debug.Log(permission);
            if (!Permission.HasUserAuthorizedPermission(permission))
            {
                Permission.RequestUserPermission(permission, callbacks);
            }

            yield return new WaitForSeconds(0.2f);
            yield return new WaitUntil(() => Application.isFocused);
        }

        yield return new WaitForSeconds(1.0f);
    }

    private void PermissionCallbacks_PermissionDeniedAndDontAskAgain(string permission)
    {
        if (IsCameraPermission(permission) || IsStoragePermission(permission))
        {
            StopAllCoroutines();
        }
    }

    private void PermissionCallbacks_PermissionGranted(string permission)
    {

    }

    private void PermissionCallbacks_PermissionDenied(string permission)
    {
        if (IsCameraPermission(permission) || IsStoragePermission(permission))
        {
            StopAllCoroutines();
        }
    }

    private bool IsCameraPermission(string permission)
    {
        if (permission.Equals(Permission.Camera))
            return true;
        return false;
    }

    private bool IsStoragePermission(string permission)
    {
        if (permission.Equals(Permission.ExternalStorageWrite) || permission.Equals(Permission.ExternalStorageRead))
            return true;
        else if (permission.Equals(READ_MEDIA_VIDEO) || permission.Equals(READ_MEDIA_IMAGES) || permission.Equals(READ_MEDIA_AUDIO))
            return true;
        return false;
    }
}
