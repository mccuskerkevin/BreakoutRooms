using SpatialSys.UnitySDK;
using UnityEngine;

/// <summary>
/// This is a utility script that is not essential to the breakout room functionality.
/// This script takes a transform, floatingTransform, and "animates" it up and down simulating a "floating" effect.
/// Additionally, if alwaysFacePlayer is true, the transform will always face the player.
/// </summary>
public class FloatUpDown : MonoBehaviour
{
    public Transform floatingTransform;
    public float lowerBound = 0f; // The lowest Y position
    public float upperBound = 1f; // The highest Y position
    public float speed = 1f;      // Speed of the oscillation
    public bool alwaysFacePlayer = true;

    private bool movingUp = true; // To track direction of movement

    // Store the starting position of the object
    private Vector3 startPosition;
    private Vector3 rotationMask = new Vector3(0f, 1f, 0f);

    void Start()
    {
        // Capture the object's starting position
        startPosition = floatingTransform.position;
    }

    void Update()
    {
        if (!floatingTransform.gameObject.activeSelf)
            return;

        // Move the object upwards if movingUp is true, otherwise move it downwards
        if (movingUp)
        {
            floatingTransform.position += new Vector3(0, speed * Time.deltaTime, 0);

            // Check if the object has reached the upper bound
            if (floatingTransform.position.y >= startPosition.y + upperBound)
                movingUp = false;
        }
        else
        {
            floatingTransform.position -= new Vector3(0, speed * Time.deltaTime, 0);

            // Check if the object has reached the lower bound
            if (floatingTransform.position.y <= startPosition.y + lowerBound)
                movingUp = true;
        }

        if (alwaysFacePlayer)
        {
            //Vector3 lookRotation = Quaternion.LookRotation(floatingTransform.position - SpatialBridge.actorService.localActor.avatar.position).eulerAngles;
            Vector3 lookRotation = Quaternion.LookRotation(floatingTransform.position - SpatialBridge.cameraService.position).eulerAngles;
            floatingTransform.rotation = Quaternion.Euler(Vector3.Scale(lookRotation, rotationMask));
        } 
    }
}