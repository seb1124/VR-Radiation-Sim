using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem; // Include the new Input System

public class SmoothTurnXR : MonoBehaviour
{
    public InputActionProperty rightJoystickAction;  // Reference to the right joystick input action
    public float turnSpeed = 45.0f;                  // Speed of the smooth turning
    public float deadZone = 0.1f;                    // Deadzone to prevent unwanted turning from small joystick movements

    private void OnEnable()
    {
        // Enable the input action when the script is enabled
        rightJoystickAction.action.Enable();
    }

    private void OnDisable()
    {
        // Disable the input action when the script is disabled
        rightJoystickAction.action.Disable();
    }

    private void Update()
    {
        // Get the joystick X input from the right controller
        float rightJoystickX = rightJoystickAction.action.ReadValue<Vector2>().x;

        // Only apply turning if the input is beyond the deadzone
        if (Mathf.Abs(rightJoystickX) > deadZone)
        {
            // Apply smooth rotation to the XR Origin rig (the player camera)
            transform.Rotate(Vector3.up, rightJoystickX * turnSpeed * Time.deltaTime);
        }
    }
}
