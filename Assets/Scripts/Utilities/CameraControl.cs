using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraControl : MonoBehaviour
{
    public CinemachineImpulseSource cameraShake;
    //CameraControl作为订阅者
    public VoidDeleSO cameraShakeDele;
    public VoidDeleSO afterSceneLoadDele;
    
    private CinemachineConfiner2D confiner2D;
    
    private void Awake()
    {
        confiner2D = GetComponent<CinemachineConfiner2D>();
    }
    
    private void OnEnable()
    {
        cameraShakeDele.OnEventDele += OnCameraShake;
        afterSceneLoadDele.OnEventDele += SetCameraBounds;
    }
    
    private void OnDisable()
    {
        cameraShakeDele.OnEventDele -= OnCameraShake;
        afterSceneLoadDele.OnEventDele -= SetCameraBounds;
    }
    
    private void OnCameraShake() => cameraShake.GenerateImpulse();

    private void SetCameraBounds()
    {
        var bound = GameObject.FindGameObjectWithTag("Bounds");
        if (bound == null)
        {
            throw new Exception("Need to define a confiner collider with tag Bounds");
        }

        confiner2D.m_BoundingShape2D = bound.GetComponent<Collider2D>();
        confiner2D.InvalidateCache();//清除之前的碰撞体边界缓存
    }
}
