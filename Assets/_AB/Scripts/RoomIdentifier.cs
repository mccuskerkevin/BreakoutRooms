using SpatialSys.UnitySDK;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoomIdentifier : MonoBehaviour
{
    TextMeshPro _roomText;
    public void OnEnable()
    {
        _roomText = GetComponent<TextMeshPro>();
        SpatialBridge.networkingService.onConnectionStatusChanged += HandleConnectionStatusChanged;
    }

    public void OnDisable()
    {
        if (SpatialBridge.networkingService != null)
        {
            SpatialBridge.networkingService.onConnectionStatusChanged -= HandleConnectionStatusChanged;
        }
    }

    private void HandleConnectionStatusChanged(ServerConnectionStatus status)
    {        
        if (status == ServerConnectionStatus.Connected)
        {
            var serverProperties = SpatialBridge.networkingService.GetServerProperties();

            bool isInRoom = serverProperties != null && serverProperties.ContainsKey("room");
            string text = "YOU ARE IN ROOM {0}";
            if(isInRoom)
                text = string.Format(text, serverProperties.GetValueOrDefault("room"));
            else
                text = string.Format(text, 0);

            _roomText.SetText(text);
        }
    }
}
