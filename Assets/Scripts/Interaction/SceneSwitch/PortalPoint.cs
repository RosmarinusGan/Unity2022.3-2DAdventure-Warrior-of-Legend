using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalPoint : MonoBehaviour, IInteractable
{
    public SceneSwitchDeleSO sceneSwitchDeleSo;
    public SceneSO targetScene;
    public Vector3 targetPlayerPosition;
    
    public void DoInteraction()
    {
        sceneSwitchDeleSo.ActiveDele(targetScene, targetPlayerPosition, true);
    }
}
