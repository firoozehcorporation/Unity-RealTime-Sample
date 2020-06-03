using System;
using System.Collections;
using System.Collections.Generic;
using FiroozehGameService.Core;
using FiroozehGameService.Handlers;
using FiroozehGameService.Models.Enums;
using FiroozehGameService.Models.GSLive;
using FiroozehGameService.Models.GSLive.RT;
using Models;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;
using Action = Models.Action;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class GameController : MonoBehaviour
{
    [Header("Objects")]
    public GameObject me;
    public GameObject enemy;

    public TextMesh enemyMemberName;
    public TextMesh meMemberName;
    
    [Header("Config Values")]
    public float posThreshold = 0.5f;
    public float rotThreshold = 5;
    public float lerpRate = 10;
    public bool synchronizePosition = true;
    public bool synchronizeRotation = true;
    
    
    private Vector3 _mNetworkPosition;
    private Quaternion _mNetworkRotation;
    private Vector3 _oldPosition;
    private Quaternion _oldRotation;

  
    void Start()
    {
        enemy.GetComponent<Renderer>().material.color = Color.red;
        me.GetComponent<Renderer>().material.color = Color.green;

        // setup With Real Values
        _oldPosition = me.transform.position;
        _oldRotation = me.transform.rotation;
        
        
        _mNetworkPosition = enemy.transform.position;
        _mNetworkRotation = enemy.transform.rotation;
        
        StartCoroutine(GetRoomMember());

        RealTimeEventHandlers.NewMessageReceived += NewMessageReceived;
        RealTimeEventHandlers.LeftRoom += LeftRoom;
        RealTimeEventHandlers.RoomMembersDetailReceived += RoomMembersDetailReceived;
    }

    private static IEnumerator GetRoomMember()
    {
        Debug.Log("wait GetRoomMembersDetail request to all member join room ...");
        yield return new WaitForSeconds(2);
        Debug.Log("GetRoomMembersDetail request now");
        // get GetRoomMembersDetail
        GameService.GSLive.RealTime.GetRoomMembersDetail();
    }

    private void RoomMembersDetailReceived(object sender, List<Member> members)
    {
       
        foreach (var member in members)
        {
            Debug.Log("RoomMembersDetailReceived : " + member.Name + "  "+member.User.IsMe);
            // set enemy name
            if (!member.User.IsMe)
            {
                enemyMemberName.text = member.Name;
                enemyMemberName.color = Color.red;
            }
            // set my name
            else
            {
                meMemberName.text = member.Name;
                meMemberName.color = Color.green;
            }
        }
    }


    private void LeftRoom(object sender, Member e)
    {
        Debug.Log("enemy left game!" + e.Name);
        GameService.GSLive.RealTime.LeaveRoom();
        SceneManager.LoadScene("MenuScene");
    }

    private void NewMessageReceived(object sender, MessageReceiveEvent message)
    {
        var data = JsonConvert.DeserializeObject<Data>(message.Message.Data);

        //var rtt = message.MessageInfo.RoundTripTime;
        //Debug.Log("rtt : " + rtt);
        
        switch (data.Action)
        {
            case Action.Translate:
                _mNetworkPosition = new Vector3(data.Position.X, data.Position.Y, data.Position.Z);
                break;
            case Action.Rotate:
                _mNetworkRotation = new Quaternion(data.Rotation.X, data.Rotation.Y, data.Rotation.Z,data.Rotation.W);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    // Update is called once per frame
      

    private void Update()
    {
        // Update Enemy 
        enemy.transform.position = Vector3.Lerp(enemy.transform.position, _mNetworkPosition, Time.deltaTime * lerpRate);
        enemy.transform.rotation = Quaternion.Lerp(enemy.transform.rotation,_mNetworkRotation, Time.deltaTime * lerpRate);

        
        // send to server
        var newPos = me.transform.position;
        var newRot = me.transform.rotation;
            
        if (Vector3.Distance(_oldPosition,newPos) > posThreshold)
        {
            _oldPosition = newPos;
            if (synchronizePosition)
                SendDataToEnemy(me.transform.position,Action.Translate);
            
        }

        if (Quaternion.Angle(_oldRotation,newRot) > rotThreshold)
        {
            _oldRotation = newRot;
            if (synchronizeRotation)
                SendDataToEnemy(me.transform.rotation, Action.Rotate);
        }
    }


    private static void SendDataToEnemy(Vector3 pos,Action action)
    {
        try
        {
            var d = new Data {
                Action = action ,
                Position = new Models.Vector3
                {
                    X = pos.x , Y = pos.y , Z = pos.z
                }
            };
            var jsonData = JsonConvert.SerializeObject(d);
            
            if(GameService.GSLive.IsRealTimeAvailable()) 
                GameService.GSLive.RealTime.SendPublicMessage(jsonData,GProtocolSendType.UnReliable);
        }
        catch (Exception e)
        {
            Debug.LogError("SendDataToEnemy" + e);
        }
           
    }
        
        
    private static void SendDataToEnemy(Quaternion data,Action action)
    {
        try
        {
            var d = new Data {
                Action = action , Rotation = new Models.Quaternion
                {
                    X = data.x , Y = data.y , Z = data.z , W = data.w
                }
            };
            var jsonData = JsonConvert.SerializeObject(d);
            
            if(GameService.GSLive.IsRealTimeAvailable()) 
                GameService.GSLive.RealTime.SendPublicMessage(jsonData,GProtocolSendType.UnReliable);
        }
        catch (Exception e)
        {
            Debug.LogError("SendDataToEnemy" + e);
        }
           
    }
}