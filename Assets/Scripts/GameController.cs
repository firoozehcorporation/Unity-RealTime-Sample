using Controller;
using FiroozehGameService.Handlers;
using FiroozehGameService.Models.Enums.GSLive;
using FiroozehGameService.Models.GSLive;
using Plugins.GameService.Utils.RealTimeUtil;
using Plugins.GameService.Utils.RealTimeUtil.Classes;
using Plugins.GameService.Utils.RealTimeUtil.Models.CallbackModels;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public class GameController : GameServiceMonoBehaviour
{
    public GameObject mePrefab;
    public Button spawnPlayerBtn;
    public Camera mainCamera;
    public Button SpawnButton; 
    public Text NetworkDetails;
    
    private GameObject _me,_enemy;

    private void Start()
    {
        RealTimeEventHandlers.LeftRoom += LeftRoom;
        RealTimeEventHandlers.Reconnected += Reconnected;
        GsLiveRealtime.Callbacks.OnBeforeInstantiateHandler += OnBeforeInstantiateHandler;
        GsLiveRealtime.Callbacks.OnAfterInstantiateHandler += OnAfterInstantiateHandler;
        GsLiveRealtime.Callbacks.OnDestroyObjectHandler += OnDestroyObjectHandler;
        GsLiveRealtime.Callbacks.OnPropertyEvent += OnPropertyEvent;
        spawnPlayerBtn.onClick.AddListener(SpawnPlayer);
        Debug.Log("GameController Start Called!");
    }

    private void Update()
    {
        NetworkDetails.text = null;
        NetworkDetails.text += "RTT : " + GsLiveRealtime.GetRoundTripTime() + "\r\n";
        NetworkDetails.text += "Packet Lost : " +  GsLiveRealtime.GetPacketLost();
    }

    private void OnDestroy()
    {
        RealTimeEventHandlers.LeftRoom = null;
        GsLiveRealtime.Callbacks.OnBeforeInstantiateHandler = null;
        GsLiveRealtime.Callbacks.OnAfterInstantiateHandler = null;
        GsLiveRealtime.Callbacks.OnDestroyObjectHandler = null;
        GsLiveRealtime.Callbacks.OnPropertyEvent = null;
        spawnPlayerBtn.onClick.RemoveListener(SpawnPlayer);
        Debug.Log("GameController Destroy Called!");

    }

    private void Reconnected(object sender, ReconnectStatus e)
    {
        Debug.Log("Reconnected : " + e);
    }
    
    private static void OnPropertyEvent(object sender, OnPropertyEvent propertyEvent)
    {
        Debug.Log("OnPropertyEvent > Ac : " + propertyEvent.action
                                            + ", Name : " + propertyEvent.propertyName
                                            + ", Data : " + propertyEvent.propertyData
                                            + ", OwnerID : " + propertyEvent.ownerMemberId);
    }


    private static void OnDestroyObjectHandler(object sender, OnDestroyObject onDestroyObject)
    {
        Debug.Log("is Tag : " + onDestroyObject.isTag + ", Name : " + onDestroyObject.objectNameOrTag);
    }

    private void OnAfterInstantiateHandler(object sender, OnAfterInstantiate onAfter)
    {
        Debug.Log("OnAfterInstantiateHandler : " + onAfter.prefabName);

        _enemy = onAfter.gameObject;
        // enable Game Object
        _enemy.SetActive(true);
        _enemy.GetComponent<Renderer>().material.color = Color.red;
        
        // disable Character Movement for enemy
        _enemy.GetComponent<CharacterMovement>().enabled = false;
    }

    private static void OnBeforeInstantiateHandler(object sender, OnBeforeInstantiate onBefore)
    {
        Debug.Log("OnBeforeInstantiateHandler : " + onBefore.prefabName);
    }
    
    

    private void SpawnPlayer()
    {
        var randomX = Random.Range(-5f, 5f);
        var randomZ = Random.Range(-5f, 5f);
        var randomVector = new Vector3(randomX,.5f,randomZ);
        
        Debug.Log("Player Spawn in " + randomVector);
        mainCamera.gameObject.SetActive(false);
        SpawnButton.gameObject.SetActive(false);

        _me = GsLiveRealtime.Instantiate(mePrefab.name, randomVector, Quaternion.identity);

        // Enable Objects for current Player
        _me.GetComponent<PlayerObjectController>().SetActiveObjects(true);
        _me.SetActive(true);
        _me.GetComponent<Renderer>().material.color = Color.green;
        
        Debug.Log("Player Spawn Done!");
    }


    private static void LeftRoom(object sender, Member e)
    {
        Debug.Log("player left game , Name : " + e.Name + ", IsMe : " + e.User.IsMe);
        if(e.User.IsMe) SceneManager.LoadScene("MenuScene");
    }
}