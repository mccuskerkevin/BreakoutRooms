using SpatialSys.UnitySDK;
using UnityEngine;

public class ConnectionHandler : MonoBehaviour
{
    public RoomTeleporter[] teleports;

    public void OnEnable()
    {
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

            foreach (var teleport in teleports)
            {
                if (isInRoom)
                {
                    teleport.gameObject.SetActive(teleport.room == 0);
                }
                else
                {
                    teleport.gameObject.SetActive(teleport.room != 0);
                }
            }

            SpatialBridge.actorService.localActor.avatar.Respawn();
        }
        else if(status == ServerConnectionStatus.Connecting)
        {           
            DisableTeleports();
        }
        else
        {
            DisableTeleports();
        }
    }

    void DisableTeleports()
    {
        foreach (var teleport in teleports)
        {
            teleport.gameObject.SetActive(false);
        }
    }
}