using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimateHandController : MonoBehaviour
{
    public InputActionReference gripInputActionReference;
    public InputActionReference triggerInputActionReference;

    private Animator _handAnimator;
    private float _gripValue;
    private float _triggerValue;

    void Start()
    {
        _handAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        animateGrip();
        animateTrigger();
    }

    private void animateGrip() {
        _gripValue = gripInputActionReference.action.ReadValue<float>();
        _handAnimator.SetFloat("Grip", _gripValue);
    }

    private void animateTrigger() {
        _triggerValue = triggerInputActionReference.action.ReadValue<float>();
        _handAnimator.SetFloat("Trigger", _triggerValue);
    }
}
