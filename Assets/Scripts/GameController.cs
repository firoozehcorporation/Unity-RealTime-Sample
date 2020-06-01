using System;
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

namespace Controllers
{
    public class GameController : MonoBehaviour
    {
        public GameObject me;
        public GameObject enemy;

        private Vector3 _realPosition;
        private Quaternion _realRotation;
        
        
        private Vector3 _oldPosition;
        private Quaternion _oldRotation;
        

        void Start()
        {
            enemy.GetComponent<Renderer>().material.color = Color.red;
            me.GetComponent<Renderer>().material.color = Color.green;

            // setup With Real Values
            _realPosition = enemy.transform.position;
            _realRotation = enemy.transform.rotation;
            
            _oldPosition = me.transform.position;
            _oldRotation = me.transform.rotation;
            
            RealTimeEventHandlers.NewMessageReceived += NewMessageReceived;
            RealTimeEventHandlers.LeftRoom += LeftRoom;
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

            switch (data.Action)
            {
                case Action.Translate:
                    _realPosition = new Vector3(data.Vector3.X, data.Vector3.Y, data.Vector3.Z);
                    break;
                case Action.Rotate:
                    _realRotation = new Quaternion(data.Quaternion.X, data.Quaternion.Y, data.Quaternion.Z,data.Quaternion.W);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        // Update is called once per frame
      

        private void Update()
        {
            
            // Update Enemy 
            enemy.transform.position = Vector3.Lerp(enemy.transform.position,_realPosition,0.1f);
            enemy.transform.rotation = Quaternion.Lerp(enemy.transform.rotation,_realRotation,0.1f);

            
            // send to server
            var newPos = me.transform.position;
            var newRot = me.transform.rotation;
            
            if (_oldPosition != newPos)
            {
                _oldPosition = newPos;
                SendDataToEnemy(newPos, Action.Translate);
            }
            
            if (_oldRotation != newRot)
            {
                _oldRotation = newRot;
                SendDataToEnemy(newRot,Action.Rotate);
            }
        }


        private static void SendDataToEnemy(Vector3 data,Action action)
        {
            try
            {
                var d = new Data {
                    Action = action , Vector3 = new Models.Vector3
                    {
                        X = data.x , Y = data.y , Z = data.z
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
                    Action = action , Quaternion = new Models.Quaternion
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
}