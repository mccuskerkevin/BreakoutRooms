using SpatialSys.UnitySDK;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserManager : MonoBehaviour
{
    public TMP_Dropdown userDropdown;
    public Button teleportButton;
    public Button closeButton;
    public Button adminUIButton;
    public TMP_InputField roomIDText;
    public string serverPropertyKey = "room";
    public GameObject returnTeleporter;

    bool _isAdmin = false;
    int _teleportedActorNumber = -1;

    const byte EVENT_ID = 1;

    private void OnEnable()
    {
        SpatialBridge.actorService.onActorJoined += ActorService_onActorJoined;
        SpatialBridge.actorService.onActorLeft += ActorService_onActorLeft;
        SpatialBridge.networkingService.remoteEvents.onEvent += RemoteEvents_onEvent;
        SpatialBridge.networkingService.onConnectionStatusChanged += NetworkingService_onConnectionStatusChanged;
        userDropdown.onValueChanged.AddListener(OnValueChanged);
        teleportButton.onClick.AddListener(RaiseTeleportUserEvent);
        closeButton.onClick.AddListener(ResetForm);
    }

    private void NetworkingService_onConnectionStatusChanged(ServerConnectionStatus status)
    {
        if (status == ServerConnectionStatus.Connected)
        {            
            if (SpatialBridge.networkingService.GetServerProperties().ContainsKey(serverPropertyKey))
                returnTeleporter.SetActive(true);
            else returnTeleporter.SetActive(false);
        }
    }

    private void OnDisable()
    {
        SpatialBridge.actorService.onActorJoined -= ActorService_onActorJoined;
        SpatialBridge.actorService.onActorLeft -= ActorService_onActorLeft;
        SpatialBridge.networkingService.remoteEvents.onEvent -= RemoteEvents_onEvent;
        SpatialBridge.networkingService.onConnectionStatusChanged -= NetworkingService_onConnectionStatusChanged;
        userDropdown.onValueChanged.RemoveListener(OnValueChanged);
        teleportButton.onClick.RemoveListener(RaiseTeleportUserEvent);
        closeButton.onClick.RemoveListener(ResetForm);
    }

    private void ActorService_onActorJoined(ActorJoinedEventArgs args)
    {        
        _isAdmin = SpatialBridge.actorService.localActor.isSpaceAdministrator || SpatialBridge.actorService.localActor.isSpaceOwner;

        if (_isAdmin)
            GetUsers();
        else
            DisableAdminUI();
    }

    private void ActorService_onActorLeft(ActorLeftEventArgs args)
    {      
        _teleportedActorNumber = args.actorNumber;

        if (_isAdmin)
            GetUsers();
    }

    private void RemoteEvents_onEvent(NetworkingRemoteEventArgs args)
    {
        int actorNumber = (int)args.eventArgs[1];

        if (actorNumber == SpatialBridge.actorService.localActorNumber)
        {            
            SpatialBridge.networkingService.TeleportToBestMatchServer(
            serverProperties: new Dictionary<string, object> { { serverPropertyKey, args.eventArgs[0] } },
            serverPropertiesToMatch: new List<string> { serverPropertyKey }
            );
        }              
    }

    void OnValueChanged(int value)
    {        
        if (value == 0)
        {
            ResetForm();
            return;
        }

        teleportButton.interactable = true;
    }

    void GetUsers()
    {
        userDropdown.ClearOptions();        

        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
        var defaultOption = new TMP_Dropdown.OptionData();
        defaultOption.text = "-- Select a user --";        
        options.Add(defaultOption);

        var actors = SpatialBridge.actorService.actors;

        foreach (var actor in actors)
        {
            if (actor.Value.actorNumber == _teleportedActorNumber)
                continue;
            
            var optionData = new TMP_Dropdown.OptionData();
            optionData.text = $"ID: {actor.Value.actorNumber} {actor.Value.displayName}";
            options.Add(optionData);
        }

        userDropdown.options = options;
    }

    void RaiseTeleportUserEvent()
    {
        Int32.TryParse(userDropdown.options[userDropdown.value].text.Split(' ')[1], out _teleportedActorNumber);

        SpatialBridge.networkingService.remoteEvents.RaiseEventOthers(EVENT_ID, roomIDText.text, _teleportedActorNumber);

        ResetForm();
    }

    void ResetForm()
    {
        teleportButton.interactable = false;
        roomIDText.text = string.Empty; 
        userDropdown.value = 0;
    }

    void DisableAdminUI()
    {
        adminUIButton.gameObject.SetActive(false);
    }
}
