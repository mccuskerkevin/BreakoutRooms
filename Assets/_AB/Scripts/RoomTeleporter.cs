using System.Collections;
using System.Collections.Generic;
using SpatialSys.UnitySDK;
using TMPro;
using UnityEngine;

public class RoomTeleporter : MonoBehaviour
{
    [Tooltip("Room 0 is always the main/starting room")]
    public int room;

    [Tooltip("If true, the Spatial Interactable component is used for teleporting. Otherwise, a Spatial Trigger Event compnent is used.")]
    public bool useInteractable = true;

    [Tooltip("If true, a TextMeshPro element hovers over the teleporter displaying the text in the Display Text field.")]
    public bool displayLabel = true;

    [Tooltip("The text displayed over the teleporter if the display label is enabled.")]
    public string displayText = "Teleport to room {0}";

    [HideInInspector]
    public SpatialInteractable interactable;
    [HideInInspector]
    public SpatialTriggerEvent triggerEvent;
    [HideInInspector]
    public TextMeshPro teleportLabel;

    void OnEnable()
    {        
        if(useInteractable)
        {
            interactable.onInteractEvent += HandleInteract;
            interactable.interactText = string.Format(displayText, room);
            triggerEvent.gameObject.SetActive(false);
        } 
        else
        {
            triggerEvent.onEnterEvent += HandleInteract;
            interactable.gameObject.SetActive(false);
        }

        if (displayLabel)
            teleportLabel.SetText(string.Format(displayText, room));
        else
            teleportLabel.gameObject.SetActive(false);        
    }

    void OnDisable()
    {
        if(useInteractable)
        {
            if (interactable == null)
                return;
            interactable.onInteractEvent -= HandleInteract;
        }
        else
        {
            if (triggerEvent == null)
                return;
            triggerEvent.onEnterEvent -= HandleInteract;
        }
        
    }

    void HandleInteract()
    {
        // ideally we want to return to the "main" room or room 0
        // but if no one is in that room the server instance does not exist
        // and the user will be placed into an open server
        if (room == 0)
            SpatialBridge.networkingService.TeleportToBestMatchServer();
        else
        {
            SpatialBridge.networkingService.TeleportToBestMatchServer(
                serverProperties: new Dictionary<string, object> { { "room", room } },
                maxParticipants: SpatialBridge.networkingService.serverMaxParticipantCount,
                serverPropertiesToMatch: new List<string> { "room" }
            );
        }
    }
}
