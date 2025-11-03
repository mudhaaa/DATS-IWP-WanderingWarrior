using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineCamera cinemachine;

    [SerializeField] private List<Transform> cameraTransforms;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void OnStart()
    {
        cinemachine.Follow = cameraTransforms[0];
        cinemachine.LookAt = cameraTransforms[0];
    }

    public void ChangeCameraPos(int i)
    {
        cinemachine.Follow = cameraTransforms[i];
    }

    // Update is called once per frame
    public void OnUpdate()
    {
        
    }


}
