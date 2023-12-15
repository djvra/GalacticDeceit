using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class CameraController : NetworkBehaviour
{
    public GameObject cameraHolder;
    public Vector3 offset;


    public override void OnNetworkSpawn() 
    { 
        cameraHolder.SetActive(IsOwner);
    }

    // public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    public void Update()
    {
        if(SceneManager.GetActiveScene().name == "SampleScene") 
        {
            if (IsOwner)
            {
                if (cameraHolder != null)
                {
                    Camera childCamera = cameraHolder.GetComponentInChildren<Camera>();
                    if (childCamera != null)
                    {
                        childCamera.enabled = true;
                    }
                }
            }           
        } else {
            if (IsOwner)
            {
                if (cameraHolder != null)
                {
                    Camera childCamera = cameraHolder.GetComponentInChildren<Camera>();
                    if (childCamera != null)
                    {
                        childCamera.enabled = false;
                    }
                }
            }
        }
    }
    
}