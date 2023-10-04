using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;

public class Tips : MonoBehaviour
{
    public GameObject tipsSprite;
    public Transform playerTransform;
    private Animator animator;
    private bool isInteract;
    private PlayerInputControl playerInputControl;
    private IInteractable interactable;

    private void Awake()
    {
        animator = tipsSprite.GetComponent<Animator>();
        playerInputControl = new PlayerInputControl();
    }

    private void OnEnable()
    {
        playerInputControl.Enable();
        InputSystem.onActionChange += OnActionChange;
        playerInputControl.Gameplay.Interact.started += OnInteract;
    }
    
    private void OnDisable()
    {
        playerInputControl.Disable();
        isInteract = false;
    }

    private void OnInteract(InputAction.CallbackContext obj) => interactable.DoInteraction();
    
    private void OnActionChange(object obj, InputActionChange inputActionChangeEnum)
    {
        if (inputActionChangeEnum == InputActionChange.ActionStarted)
        {
            var d = ((InputAction)obj).activeControl.device;
            switch (d)
            {
                case Keyboard:
                    animator.Play("KeyBoard");
                    break;
                case DualShockGamepad:
                    animator.Play("PS");
                    break;
            }
        }
    }
    
    private void Update()
    {
        tipsSprite.GetComponent<SpriteRenderer>().enabled = isInteract;
        tipsSprite.transform.localScale = playerTransform.localScale;
    }
    
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Interactable"))
        {
            isInteract = true;
            interactable = other.GetComponent<IInteractable>();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        isInteract = false;
    }
}
