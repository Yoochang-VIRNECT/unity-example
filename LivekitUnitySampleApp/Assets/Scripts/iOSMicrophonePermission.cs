using UnityEngine;
using System.Collections;

public class iOSMicrophonePermission : MonoBehaviour
{
    IEnumerator RequestMicrophonePermission()
    {
        // iOS에서는 Application.RequestUserAuthorization 사용
        yield return Application.RequestUserAuthorization(UserAuthorization.Microphone);
        
        if (Application.HasUserAuthorization(UserAuthorization.Microphone))
        {
            Debug.Log("마이크 권한 승인됨");
        }
        else
        {
            Debug.Log("마이크 권한 거부됨");
        }
    }
    
    void Start()
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            StartCoroutine(RequestMicrophonePermission());
        }
    }
}