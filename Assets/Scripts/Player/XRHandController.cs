using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public enum HandType{
    Left,
    Right
}

public class XRHandController : MonoBehaviour
{
    Collider collider;
    float distanceToSelectedTarget;
    float minDistance;
    public HandType handType;
    public string typeOfHand = ""; 
    public float thumbMoveSpeed = 0.1f;
    private Animator animator;
    private InputDevice inputDevice;
    private XRRayInteractor inputInteractor;
    private ActionBasedContinuousMoveProvider moveProvider;
    Vector3 selectedObjectScale;
    private object selectedObject;
    private float triggerValue;
    private float gripValue;
    private float thumbValue;
    void Start() {
        animator = GetComponent<Animator>();
        inputDevice = GetInputDevice();
        inputInteractor = gameObject.GetComponentInParent<XRRayInteractor>() as XRRayInteractor;
        moveProvider = GameObject.Find("Locomotion System").GetComponent<ActionBasedContinuousMoveProvider>() as ActionBasedContinuousMoveProvider;
    }
    void Update()  {
        AnimateHand();
        distanceToSelectedTarget = Vector3.Distance(inputInteractor.selectTarget.gameObject.transform.position, inputInteractor.gameObject.transform.position);
        selectedObjectScale = inputInteractor.selectTarget.gameObject.transform.localScale / 2;
        minDistance = selectedObjectScale.y * 3;
    }
    
    InputDevice GetInputDevice()
    {
        InputDeviceCharacteristics controllerCharacteristic = InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Controller;
        if (handType == HandType.Left) {
            controllerCharacteristic = controllerCharacteristic | InputDeviceCharacteristics.Left;
            typeOfHand = "Left";
        } else {
            controllerCharacteristic = controllerCharacteristic | InputDeviceCharacteristics.Right;
            typeOfHand = "Right";
        }
        
        List<InputDevice> inputDevices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(controllerCharacteristic, inputDevices);
        return inputDevices[0];
    }
    
	void AnimateHand() {
		inputDevice.TryGetFeatureValue(CommonUsages.trigger, out triggerValue);
        inputDevice.TryGetFeatureValue(CommonUsages.grip, out gripValue);
        inputDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryPressed);
        inputDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out bool secondaryPressed);
        inputDevice.TryGetFeatureValue(CommonUsages.primaryTouch, out bool primaryTouched);
        inputDevice.TryGetFeatureValue(CommonUsages.secondaryTouch, out bool secondaryTouched);
        if (primaryPressed) {
            thumbValue += thumbMoveSpeed;
        } else {
            thumbValue -= thumbMoveSpeed;
        }
        thumbValue = Mathf.Clamp(thumbValue, 0, 1);
        if (inputInteractor.selectTarget == null) {
            moveProvider.moveSpeed = 2;
            gameObject.transform.position = inputInteractor.transform.position;
            animator.SetFloat(typeOfHand + "Grab", 0);
            animator.SetFloat("Point", 0);
			animator.SetFloat("ThreeFingers", gripValue);
            animator.SetFloat("Index", triggerValue);
            animator.SetFloat("Thumb", thumbValue);
            inputInteractor.useForceGrab = primaryPressed;
        } else {
			if (distanceToSelectedTarget < minDistance) {
                moveProvider.moveSpeed = 0;
                gameObject.transform.position = inputInteractor.transform.position;
                inputInteractor.useForceGrab = thumbValue == 1;
                if(typeOfHand == "Right") {
                    gameObject.transform.Translate(-selectedObjectScale.y * (-distanceToSelectedTarget / minDistance + 1), (float)((selectedObjectScale.x - 0.05) * (-distanceToSelectedTarget / minDistance + 1)), -selectedObjectScale.z * (-distanceToSelectedTarget / minDistance + 1));
                } else if (typeOfHand == "Left") {
                    gameObject.transform.Translate(selectedObjectScale.y * (-distanceToSelectedTarget / minDistance + 1), (float)((selectedObjectScale.x - 0.05) * (-distanceToSelectedTarget / minDistance + 1)), -selectedObjectScale.z * (-distanceToSelectedTarget / minDistance + 1));
                }
                animator.SetFloat(typeOfHand + "Grab", -distanceToSelectedTarget / minDistance + 1);
                animator.SetFloat("ThreeFingers", distanceToSelectedTarget / minDistance);
                animator.SetFloat("Point", (gripValue - triggerValue) * distanceToSelectedTarget / minDistance);
                animator.SetFloat("Index", triggerValue * distanceToSelectedTarget / minDistance);
                animator.SetFloat("Thumb", thumbValue * distanceToSelectedTarget / minDistance);
            } else {
                moveProvider.moveSpeed = 0;
                gameObject.transform.position = inputInteractor.transform.position;
                animator.SetFloat(typeOfHand + "Grab", 0);
                animator.SetFloat("ThreeFingers", gripValue);
                animator.SetFloat("Point", gripValue - triggerValue);
                animator.SetFloat("Index", triggerValue);
                animator.SetFloat("Thumb", thumbValue);
                inputInteractor.translateSpeed = 1 + triggerValue;
			}
		}
    }
}