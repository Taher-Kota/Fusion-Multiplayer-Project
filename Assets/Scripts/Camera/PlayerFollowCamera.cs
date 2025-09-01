using Cinemachine;
using Fusion;
using StarterAssets;
using UnityEngine;

public class PlayerFollowCamera : MonoBehaviour
{
    private CinemachineVirtualCamera virtualCamera;

    private void Start()
    {
        NetworkObject networkObject = GetComponent<NetworkObject>();

        if (networkObject.HasStateAuthority)
        {
            virtualCamera = GameObject.Find("PlayerFollowCamera").GetComponent<CinemachineVirtualCamera>();
            virtualCamera.Follow = transform.GetChild(0);

            GetComponent<ThirdPersonController>().enabled = true;
            gameObject.SetActive(false);
        }
    }
}
