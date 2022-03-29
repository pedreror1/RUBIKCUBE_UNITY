using PEDREROR1.RUBIK.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
namespace PEDREROR1.RUBIK.UI
{
    /// <summary>
    /// The menu Manager IS in charge of managing both the Main Menu as well
    /// as the Pause Menu
    /// from here we can Dinamically update The pause menu to display the desired Content.
    /// </summary>
    public class MenuManager : SingletonComponent<MenuManager>
    {
        #region PARAMETERS

        [SerializeField] private Button ContinueButton;
        [SerializeField] private Text cubeSizeLabel;
        [SerializeField] private PlayableDirector startAnimation, continueGameAnimation, selectCubeAnimation, showMenuAnimation, hideMenuAnimation, GoToMenuAnimation;
        [SerializeField] private Text inGameTimer;
        [SerializeField] private GameObject DynamicObject1, DynamicObject2, DynamicObject3, TimerToggle;
        [SerializeField] private Text DescriptionText, DynamicText1, DynamicText2, DynamicText3;
        private enum PauseMenuState
        {
            PauseMenu = 0,
            ConfirmRestart = 1,
            ConfirmExit = 2,
            Save = 3,
            Win = 4
        }
        private PauseMenuState currentPauseState;

        #endregion

        #region METHODS

        #region ANIMATIONS
        public void playSelectCubeAnimation()
        {
            if (selectCubeAnimation)
            {
                selectCubeAnimation.Play();
            }
        }
        public void PlayContinueGameAnimation()
        {
            if (continueGameAnimation)
            {
                continueGameAnimation.Play();
            }
        }

        public void PlayCubeAnimation()
        {
            if (selectCubeAnimation)
            {
                selectCubeAnimation.Play();
            }
        }
        public void PlayGoToMenuAnimation()
        {
            if (GoToMenuAnimation)
            {
                GoToMenuAnimation.Play();
            }
        }
        private void PlayShowMenuAnimation()
        {
            if (showMenuAnimation)
            {
                showMenuAnimation.Play();
            }
        }
        private void PlayHideMenuAnimation()
        {
            if (hideMenuAnimation)
            {
                hideMenuAnimation.Play();
            }
        }

        public void PlayStartAnimation()
        {
            if (startAnimation)
            {
                startAnimation.Play();
            }
        }

        #endregion


        #region GAME_FLOW

        void Start()
        {
            cubeSizeLabel.text = "3x3";
            if (ContinueButton)
            {
                ContinueButton.interactable = PlayerManager.Instance.load();
            }
            PlayerManager.Instance.UpdateTimerEvnt += UpdateTimer;
            PlayerManager.Instance.ResumeEvnt += Resume;
        }
        public void Pause()
        {
            UpdateMenu(0);
            PlayerManager.Instance.UpdateState(PlayerManager.GameState.Pause);
            PlayShowMenuAnimation();
        }
        public void OnWin()
        {
            UpdateMenu(4);
            PlayShowMenuAnimation();
        }       
        public void Resume()
        {
            PlayHideMenuAnimation();
        }
        public void UpdateCubeSize(int direction)
        {
            if (direction != 1 && direction != -1) return;
            var dimension = PlayerManager.Instance.UpdateDimension(direction);
            cubeSizeLabel.text = $"{dimension}X{dimension}";
        }
        void UpdateTimer(float gameTime)
        {
            if (inGameTimer)
            {
                var formattedTime = System.TimeSpan.FromSeconds(Mathf.Max(0, gameTime));
                inGameTimer.text = string.Format("{0:D2}:{1:D2}:{2:D2}", formattedTime.Hours, formattedTime.Minutes, formattedTime.Seconds);
            }
        }
        #endregion

        #region PAUSE_MENU_LOGIC
        public void UpdateMenu(int newState)
        {
            currentPauseState = (PauseMenuState)newState;
            switch (currentPauseState)
            {
                case PauseMenuState.PauseMenu:
                    DescriptionText.text = "SHOW TIMER";
                    TimerToggle.SetActive(true);
                    DynamicObject3.SetActive(true);
                    DynamicText1.text = "RESTART";
                    DynamicText2.text = "CONTINUE";
                    DynamicText3.text = "EXIT";
                    break;

                case PauseMenuState.ConfirmRestart:
                    DescriptionText.text = "ARE YOU SURE YOU WANT TO RESTART?";
                    TimerToggle.SetActive(false);
                    DynamicObject3.SetActive(false);
                    DynamicText1.text = "YES";
                    DynamicText2.text = "CANCEL";
                    break;

                case PauseMenuState.ConfirmExit:
                    DescriptionText.text = "ARE YOU SURE YOU WANT TO EXIT?";
                    TimerToggle.SetActive(false);
                    DynamicObject3.SetActive(false);
                    DynamicText1.text = "YES";
                    DynamicText2.text = "CANCEL";
                    break;

                case PauseMenuState.Save:
                    DescriptionText.text = "SAVE DATA";
                    TimerToggle.SetActive(false);
                    DynamicObject3.SetActive(false);
                    DynamicText1.text = "YES";
                    DynamicText2.text = "NO";
                    break;

                case PauseMenuState.Win:
                    DescriptionText.text = $"CONGRATULATIONS! Cube Completed in {inGameTimer.text}";
                    TimerToggle.SetActive(false);
                    DynamicObject3.SetActive(false);
                    DynamicText1.text = "Exit";
                    DynamicText2.text = "Continue";
                    break;
            }
        }

        public void PauseDynamicButton1()
        {
            switch (currentPauseState)
            {
                case PauseMenuState.PauseMenu:
                    //GOTO Confirm Restart
                    UpdateMenu(1);
                    break;
                case PauseMenuState.ConfirmRestart:
                    //Reshuffle Cube
                    PlayerManager.Instance.Restart();
                    break;
                case PauseMenuState.ConfirmExit:
                    //if Player has not Won Goto -> Save Screen
                    if (!PlayerManager.Instance.hasWon)
                    {
                        UpdateMenu(3);
                    }
                    //if Player Has won Go Directly to main menu
                    else
                    {
                        PlayGoToMenuAnimation();
                        PlayerManager.Instance.GoBackToMenu();
                    }
                    break;
                case PauseMenuState.Save:
                    //Save Data and go to main Menu
                    PlayerManager.Instance.save();
                    ContinueButton.interactable = PlayerManager.Instance.load();
                    PlayHideMenuAnimation();
                    PlayerManager.Instance.GoBackToMenu();
                    PlayGoToMenuAnimation();
                    break;

                case PauseMenuState.Win:
                    //Go to Main Menu
                    PlayGoToMenuAnimation();
                    PlayerManager.Instance.StopWinAnimation();
                    PlayerManager.Instance.GoBackToMenu();
                    break;
            }
        }
        public void PauseDynamicButton2()
        {
            switch (currentPauseState)
            {
                case PauseMenuState.PauseMenu:
                    //Resume Game
                    Resume();
                    break;

                case PauseMenuState.ConfirmExit:
                    //Go to main Pause Menu
                    UpdateMenu(0);
                    break;

                case PauseMenuState.ConfirmRestart:
                    //Go Back to main Menu
                         UpdateMenu(0);
                    break;

                case PauseMenuState.Save:
                    //Go back to main menu without Saving
                    PlayGoToMenuAnimation();
                    PlayerManager.Instance.GoBackToMenu();
                    break;
                
                case PauseMenuState.Win:
                    //Close pause Menu 
                    Resume();
                    PlayerManager.Instance.StopWinAnimation();
                    break;
            }
        }

        public void PauseDynamicButton3()
        {
            switch (currentPauseState)
            {
                case PauseMenuState.PauseMenu:
                    //Go To Confirm Exit Menu
                    UpdateMenu(2);
                    break;
            }
        }
        public void ToggleTimer(bool isOn)
        {
            inGameTimer.gameObject.SetActive(isOn);
        }
     
        #endregion
      

      
    }
    #endregion
}