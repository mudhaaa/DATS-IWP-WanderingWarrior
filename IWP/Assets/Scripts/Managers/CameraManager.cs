using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using
    static BattleManager;

public class CameraManager : MonoBehaviour
{
    //[SerializeField] private CinemachineCamera cinemachine;
    [Header("Fake Cinemachine")]
    [SerializeField] private List<Transform> cameraTransforms;
    [SerializeField] private List<Vector3> OGcameraPositions;
    [SerializeField] private int currIndex;

    [Header("Shake")]
    private Vector3 originalPos;
    private Vector3 shakeOffset;
    [SerializeField] private float shakeAmplitude = 0.1f;
    [SerializeField] private float shakeDuration = 0.1f;
    private float shakeRemainingTime = 0.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void OnStart()
    {
        Camera.main.transform.DOLocalRotateQuaternion(cameraTransforms[0].localRotation, 0.5f);
        Camera.main.transform.DOMove(cameraTransforms[0].position, 0.5f);
        Camera.main.transform.LookAt(cameraTransforms[0]);
        
        foreach(Transform t in cameraTransforms)
        {
            OGcameraPositions.Add(t.localPosition);
        }

        currIndex = 0;
    }

    public void ChangeCameraPos(int i)
    {
        Camera.main.transform.DOLocalRotateQuaternion(cameraTransforms[i].localRotation, .5f);
        Camera.main.transform.DOMove(cameraTransforms[i].position, .5f);
        currIndex = i;
    }

    // Update is called once per frame
    public void OnUpdate()
    {
        if (BattleManager.instance.GetCurrState() == BattleStates.P1turn || BattleManager.instance.GetCurrState() == BattleStates.P1winRound)
        {
            ChangeCameraPos(1);
        }
        else if (BattleManager.instance.GetCurrState() == BattleStates.P2turn || BattleManager.instance.GetCurrState() == BattleStates.P2winRound)
        {
            ChangeCameraPos(2);
        }
        else if (BattleManager.instance.IsAttackState() || 
                 BattleManager.instance.GetCurrState() == BattleStates.Enhancement || 
                 BattleManager.instance.GetCurrState() == BattleStates.StatusAktion)
        {
            ChangeCameraPos(0);
        }
        else if(BattleManager.instance.GetCurrState() == BattleStates.AktionAnimation)
        {
            ChangeCameraPos(currIndex);
        }
    }

    public void OnLateUpdate()
    {
        CameraShake();
    }
    void CameraShake()
    {
        if (shakeRemainingTime > 0)
        {
            shakeRemainingTime -= Time.deltaTime;
            shakeOffset = Random.insideUnitSphere * shakeAmplitude;
        }
        else
        {
            shakeOffset = Vector3.zero;
        }

        if (OGcameraPositions.Count > 0) cameraTransforms[currIndex].localPosition = OGcameraPositions[currIndex] + shakeOffset;
    }

    public void ActivateShake(float d, float a)
    {
        shakeAmplitude = a;
        shakeRemainingTime = d;

        Debug.Log("Activating Shake!");
    }
}
