using UnityEngine;
using UnityEngine.UI;

public class Billboard : MonoBehaviour
{
    private GameObject mainCamera;
    void LateUpdate()
    {
        if (mainCamera == null)
        {
            mainCamera = GameObject.FindWithTag("MainCam");
        }

        transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward, mainCamera.transform.rotation * Vector3.up);
        
    }
}
