using UnityEngine;

public class DoorController : MonoBehaviour
{
    // Drag the Door's Animator component here in the Inspector
    public Animator doorAnimator;
    // Only objects with this tag will trigger the door
    public string playerTag = "Player";
    // The name of the boolean parameter in your Animator Controller
    public string animatorBoolName = "IsOpen";

    // Called when another collider enters this trigger
    private void OnTriggerEnter(Collider other)
    {
        // If the entering object has the specified playerTag
        if (other.CompareTag(playerTag))
        {
            if (doorAnimator != null)
            {
                // Set the specified boolean parameter in the Animator to true
                doorAnimator.SetBool(animatorBoolName, true);
                Debug.Log("Door opening!");
            }
            else
            {
                Debug.LogError("Door Animator not assigned on " + gameObject.name);
            }
        }
    }

    // Called when another collider exits this trigger
    private void OnTriggerExit(Collider other)
    {
        // If the exiting object has the specified playerTag
        if (other.CompareTag(playerTag))
        {
            if (doorAnimator != null)
            {
                // Set the specified boolean parameter in the Animator to false
                doorAnimator.SetBool(animatorBoolName, false);
                Debug.Log("Door closing!");
            }
            else
            {
                Debug.LogError("Door Animator not assigned on " + gameObject.name);
            }
        }
    }
}