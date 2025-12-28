using DG.Tweening;
using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.Mathematics;
using UnityEngine;
using static BattleManager;

public class CameraManager : MonoBehaviour
{
    //[SerializeField] private CinemachineCamera cinemachine;
    [Header("Fake Cinemachine")]
    [SerializeField] private List<Transform> cameraTransforms;

    [Header("Shake")]
    [SerializeField] private float shakeAmplitude = 2.0f;  // Strength of shake
    [SerializeField] private float shakeFrequency = 2.0f;  // Speed of shake
    [SerializeField] private float shakeDuration = 0.2f;   // Duration of shake
    private CinemachineBasicMultiChannelPerlin noise;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void OnStart()
    {
        Camera.main.transform.DOLocalRotateQuaternion(cameraTransforms[0].localRotation, 0.5f);
        Camera.main.transform.DOMove(cameraTransforms[0].position, 0.5f);
        Camera.main.transform.LookAt(cameraTransforms[0]);
    }

    public void ChangeCameraPos(int i)
    {
        Camera.main.transform.DOLocalRotateQuaternion(cameraTransforms[i].localRotation, .5f);
        Camera.main.transform.DOMove(cameraTransforms[i].position, .5f);
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
    }

    public void OnLateUpdate()
    {
        //CameraShake();
    }

    //void CameraShake()
    //{
    //    if (shakeTimer > 0)
    //    {
    //        shakeTimer -= Time.unscaledDeltaTime;
    //        if (shakeTimer <= 0)
    //        {
    //            StopShake();
    //        }
    //    }
    //}
    //public void ActivateShake(float amp, float freq)
    //{
    //    if (!cameraShakeToggle) return;

    //    StopShake();

    //    shakeTimer = shakeDuration;

    //    if (noise != null)
    //    {
    //        noise.m_AmplitudeGain = amp;
    //        noise.m_FrequencyGain = freq;
    //    }
    //}
    //public void StopShake()
    //{
    //    if (noise != null)
    //    {
    //        noise.m_AmplitudeGain = 0f;
    //        noise.m_FrequencyGain = 0f;
    //    }
    //}
}
