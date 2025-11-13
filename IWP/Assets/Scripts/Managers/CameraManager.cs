using DG.Tweening;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using static BattleManager;
using static UnityEditor.Rendering.InspectorCurveEditor;

public class CameraManager : MonoBehaviour
{
    //[SerializeField] private CinemachineCamera cinemachine;

    [SerializeField] private List<Transform> cameraTransforms;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void OnStart()
    {
        //cinemachine.Follow = cameraTransforms[0];
        //cinemachine.LookAt = cameraTransforms[0];
        Camera.main.transform.DOLocalRotateQuaternion(cameraTransforms[0].localRotation, 0.5f);
        Camera.main.transform.DOMove(cameraTransforms[0].position, 0.5f);
    }

    public void ChangeCameraPos(int i)
    {
        Camera.main.transform.DOLocalRotateQuaternion(cameraTransforms[i].localRotation, 1f);
        Camera.main.transform.DOMove(cameraTransforms[i].position, 1f);
    }

    // Update is called once per frame
    public void OnUpdate()
    {
        if (BattleManager.instance.GetCurrState() == BattleStates.P1turn)
        {
            ChangeCameraPos(1);
        }
        else if (BattleManager.instance.GetCurrState() == BattleStates.P2turn)
        {
            ChangeCameraPos(2);
        }
        else if (BattleManager.instance.IsAttackState())
        {
            ChangeCameraPos(0);
        }
    }


}
