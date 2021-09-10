using System;
using FiroozehGameService.Core;
using FiroozehGameService.Core.GSLive;
using FiroozehGameService.Handlers;
using FiroozehGameService.Models;
using FiroozehGameService.Models.GSLive.Command;
using Plugins.GameService.Utils.RealTimeUtil.Classes;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : GameServiceMonoBehaviour
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


    private void Awake()
    {
        Application.targetFrameRate = 50;
    }

    void Start()
    {
        if(GameService.IsAuthenticated())
        {
            Debug.Log("IsAuthenticated before!");
            Status.text = "Status : Connected!";
            StartGameBtn.interactable = true;
            StartGameBtn.onClick.AddListener(() =>
            {
                GameService.GSLive.RealTime().AutoMatch(new GSLiveOption.AutoMatchOption("GSRealtimeSample-Test-New"));
                
                Status.color = Color.green;
                Status.text = "MatchMaking...";
                StartGameBtn.interactable = false;
            });
        }
        else
        {
            Debug.Log("First Init!");
            SetEventListeners();
            ConnectToGamesService();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape)) return;
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
                            await GameService.LoginOrSignUp.SignUp(nickName, email, pass);
                          
                        

                    }
                    else
                    {
                        var email = Email.text.Trim();
                        var pass = Password.text.Trim();

                        if (string.IsNullOrEmpty(email) && string.IsNullOrEmpty(pass))
                            LoginErr.text = "Invalid Input!";
                        else
                            await GameService.LoginOrSignUp.Login(email, pass);
                        
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
            RealTimeEventHandlers.AutoMatchUpdated += AutoMatchUpdated;
            RealTimeEventHandlers.JoinedRoom += OnJoinRoom;
        }

        private static void AutoMatchUpdated(object sender, AutoMatchEvent e)
        {
           Debug.Log("AutoMatchUpdated -> Status : " + e.Status);
           foreach (var player in e.Players)
               Debug.Log("AutoMatchUpdated -> player : " + player.Name);
        }


        private static void OnJoinRoom(object sender, JoinEvent e)
        {
            Debug.Log("OnJoinRoom : " + e.JoinData.JoinedMember.Name);
            var activeScene = SceneManager.GetActiveScene();
            // Go To GameScene When Joined To Room
            if (activeScene.name == "MenuScene")
            {
                Debug.Log("GameScene Load...");
                SceneManager.LoadScene("GameScene");
            }
        }

        private void OnError(object sender, ErrorEvent e)
        {
            Status.text = "OnError : " + e.Error;
        }

        private void OnSuccessfullyLogined(object sender, EventArgs e)
        {
            StartMenu.SetActive(true);
            LoginMenu.SetActive(false);
            
            Status.text = "Status : Connected!";
            StartGameBtn.interactable = true;
            StartGameBtn.onClick.AddListener(() =>
            {
                GameService.GSLive.RealTime().AutoMatch(new GSLiveOption.AutoMatchOption("GSRealtimeSample-Test-New"));
                
                Status.color = Color.green;
                Status.text = "MatchMaking...";
                StartGameBtn.interactable = false;
            });
        }
}
