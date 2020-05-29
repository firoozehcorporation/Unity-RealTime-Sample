using System;
using FiroozehGameService.Core;
using FiroozehGameService.Core.GSLive;
using FiroozehGameService.Handlers;
using FiroozehGameService.Models;
using FiroozehGameService.Models.GSLive;
using FiroozehGameService.Models.GSLive.Command;
using FiroozehGameService.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using LogType = FiroozehGameService.Utils.LogType;

public class MenuController : MonoBehaviour
{
    public GameObject StartMenu;
    public GameObject LoginMenu;
    public Button StartGameBtn;
    public Text Status;
    
    
    public InputField NickName;
    public InputField Email;
    public InputField Password;
    public Button Submit;
    public GameObject SwitchToRegisterOrLogin;
    public Text LoginErr;


    void Start()
    {
        if(GameService.IsAuthenticated()) return;
            
        SetEventListeners();
        ConnectToGamesService();
    }

    // Update is called once per frame
    void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape)) return;
        GameService.Logout();
        Application.Quit();
    }
    
    
    
        /// <summary>
        /// Connect To GameService -> Login Or SignUp
        /// It May Throw Exception
        /// </summary>
        private void ConnectToGamesService () {
        //connecting to GamesService
        Status.text = "Status : Connecting...";
        StartGameBtn.interactable = false;
        SwitchToRegisterOrLogin.GetComponent<Button>().onClick.AddListener(() =>
        {
            if (NickName.IsActive())
            {
                NickName.gameObject.SetActive(false);
                SwitchToRegisterOrLogin.GetComponent<Text>().text = "Dont have an account? Register!";
            }
            else
            {
                NickName.gameObject.SetActive(true);
                SwitchToRegisterOrLogin.GetComponent<Text>().text = "Have an Account? Login!";
            }
        });
        
        // Enable LoginUI
        StartMenu.SetActive(false);
        LoginMenu.SetActive(true);
            
            Submit.onClick.AddListener(async () =>
            {
                try
                {
                    if (NickName.IsActive()) // is SignUp
                    {
                        var nickName = NickName.text.Trim();
                        var email = Email.text.Trim();
                        var pass = Password.text.Trim();

                        if (string.IsNullOrEmpty(nickName)
                            && string.IsNullOrEmpty(email)
                            && string.IsNullOrEmpty(pass))
                            LoginErr.text = "Invalid Input!";
                        else
                            await GameService.SignUp(nickName, email, pass);
                        

                    }
                    else
                    {
                        var email = Email.text.Trim();
                        var pass = Password.text.Trim();

                        if (string.IsNullOrEmpty(email) && string.IsNullOrEmpty(pass))
                            LoginErr.text = "Invalid Input!";
                        else
                        { 
                            await GameService.Login(email, pass);
                            // Disable LoginUI
                            StartMenu.SetActive(true);
                            LoginMenu.SetActive(false);
                        }
                    }
                }
                catch (Exception e)
                {
                    if (e is GameServiceException) LoginErr.text = "GameServiceException : " + e.Message;
                    else LoginErr.text = "InternalException : " + e.Message;
                }
               
            });
        }

        
        private void SetEventListeners()
        {
            RealTimeEventHandlers.SuccessfullyLogined += OnSuccessfullyLogined;
            RealTimeEventHandlers.Error += OnError;
                
            RealTimeEventHandlers.JoinedRoom += OnJoinRoom;
            RealTimeEventHandlers.LeftRoom += LeftRoom;
            LogUtil.LogEventHandler += LogEventHandler;
        }
        
        private static void LogEventHandler(object sender, Log e)
        {
            if(e.Type == LogType.Normal) Debug.Log(e.Txt);
            else Debug.LogError(e.Txt);
        }
        
        private static void LeftRoom(object sender, Member e)
        {
            Debug.Log("LeftRoom : " + e.Name);
        }


        private static void OnJoinRoom(object sender, JoinEvent e)
        {
            Debug.Log("OnJoinRoom : " + e.JoinData.JoinedMember.Name);
            var activeScene = SceneManager.GetActiveScene();
            // Go To GameScene When Joined To Room
            if(activeScene.name == "MenuScene")
                SceneManager.LoadScene("GameScene");
        }

        private void OnError(object sender, ErrorEvent e)
        {
            Status.text = "OnError : " + e.Error;
        }

        private void OnSuccessfullyLogined(object sender, EventArgs e)
        {
            Status.text = "Status : Connected!";
            StartGameBtn.interactable = true;
            StartGameBtn.onClick.AddListener(async () =>
            {
                await GameService.GSLive.RealTime.AutoMatch(new GSLiveOption.AutoMatchOption("GSRealtimeSample"));
                
                Status.color = Color.green;
                Status.text = "MatchMaking...";
                StartGameBtn.interactable = false;
            });
        }
}
