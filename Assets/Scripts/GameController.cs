using System;
using FiroozehGameService.Core;
using FiroozehGameService.Handlers;
using FiroozehGameService.Models.Enums;
using FiroozehGameService.Models.GSLive;
using FiroozehGameService.Models.GSLive.RT;
using Models;
using Newtonsoft.Json;
using UnityEngine;
using Action = Models.Action;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace Controllers
{
    public class GameController : MonoBehaviour
    {
        public GameObject me;
        public GameObject enemy;


        void Start()
        {
            enemy.GetComponent<Renderer>().material.color = Color.red;
            me.GetComponent<Renderer>().material.color = Color.green;
            
            RealTimeEventHandlers.NewMessageReceived += NewMessageReceived;
            RealTimeEventHandlers.LeftRoom += LeftRoom;
        }

       
        private void LeftRoom(object sender, Member e)
        {
            Debug.Log("enemy left game!" + e.Name);
        }

        private void NewMessageReceived(object sender, MessageReceiveEvent message)
        {
            var data = JsonConvert.DeserializeObject<Data>(message.Message.Data);
            //Debug.Log(message.Message.Data);

            switch (data.Action)
            {
                case Action.Translate:
                    var vect3 = new Vector3(data.Vector3.X, data.Vector3.Y, data.Vector3.Z);
                    enemy.transform.position = Vector3.Lerp(enemy.transform.position,vect3,0.1f);
                    break;
                case Action.Rotate:
                    var quaternion = new Quaternion(data.Quaternion.X, data.Quaternion.Y, data.Quaternion.Z,data.Quaternion.W);
                    enemy.transform.rotation = Quaternion.Lerp(enemy.transform.rotation,quaternion,0.1f);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        // Update is called once per frame
        void Update()
        {
            //Move Front/Back
            if (MobileJoystickUi.instance.moveDirection.y != 0)
            {
                var data = transform.forward * (Time.deltaTime * 2.45f * MobileJoystickUi.instance.moveDirection.y);
                me.transform.Translate(data,Space.World);
                SendDataToEnemy(me.transform.position, Action.Translate);
            }

            //Rotate Left/Right
            if (MobileJoystickUi.instance.moveDirection.x != 0)
            {
                var data = new Vector3(0, 14, 0) * (Time.deltaTime * 4.5f * MobileJoystickUi.instance.moveDirection.x);
                me.transform.Rotate(data,Space.Self);
                SendDataToEnemy(me.transform.rotation,Action.Rotate);
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
            var d = new Data {
                Action = action , Quaternion = new Models.Quaternion
                {
                    X = data.x , Y = data.y , Z = data.z , W = data.w
                }
            };
            var jsonData = JsonConvert.SerializeObject(d);
            
            if(GameService.GSLive.IsRealTimeAvailable()) 
                GameService.GSLive.RealTime.SendPublicMessage(jsonData,GProtocolSendType.Reliable);
        }
    }
}