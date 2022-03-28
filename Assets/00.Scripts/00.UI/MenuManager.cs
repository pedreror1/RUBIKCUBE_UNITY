using PEDREROR1.RUBIK.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
namespace PEDREROR1.RUBIK.UI
{
    public class MenuManager : SingletonComponent<MenuManager>
    {
        public Button ContinueButton;
        public Text cubeSizeLabel;
        [SerializeField] private PlayableDirector startAnimation, continueGameAnimation, selectCubeAnimation, showMenuAnimation, hideMenuAnimation, GoToMenuAnimation;

        public Text inGameTimer;

        public GameObject DynamicObject1, DynamicObject2, DynamicObject3, TimerToggle;
        public Text DescriptionText, DynamicText1, DynamicText2, DynamicText3;

        enum PauseMenuState
        {
            PauseMenu = 0,
            ConfirmRestart = 1,
            ConfirmExit = 2,
            Save = 3
        }
        PauseMenuState currentPauseState;

        public void StartGame()
        {
            if (startAnimation)
            {
                startAnimation.Play();
            }


        }
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
            showMenu();
        }

        private void showMenu()
        {
            if (showMenuAnimation)
            {
                showMenuAnimation.Play();
            }
        }

        public void Resume()
        {
            hideMenu();
        }

        private void hideMenu()
        {
            if (hideMenuAnimation)
            {
                hideMenuAnimation.Play();
            }
        }

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
            }
        }

        public void PauseDynamicButton1()
        {
            switch (currentPauseState)
            {
                case PauseMenuState.PauseMenu:
                    UpdateMenu(1);
                    break;
                case PauseMenuState.ConfirmRestart:
                    PlayerManager.Instance.Restart();
                    break;
                case PauseMenuState.ConfirmExit:
                    UpdateMenu(3);
                    break;
                case PauseMenuState.Save:
                    PlayerManager.Instance.save();
                    ContinueButton.interactable = PlayerManager.Instance.load();
                    hideMenu();
                    PlayerManager.Instance.GoBackToMenu();
                    PlayGoToMenuAnimation();

                    break;
            }
        }
        public void PauseDynamicButton2()
        {
            switch (currentPauseState)
            {
                case PauseMenuState.PauseMenu:
                    Resume();
                    break;

                case PauseMenuState.ConfirmExit:
                    UpdateMenu(0);
                    break;
                case PauseMenuState.ConfirmRestart:
                    if (PlayerManager.Instance.currentState != PlayerManager.GameState.Shuffling)
                        UpdateMenu(0);
                    break;

                case PauseMenuState.Save:
                    PlayGoToMenuAnimation();
                    PlayerManager.Instance.GoBackToMenu();

                    break;
            }
        }

        public void PauseDynamicButton3()
        {
            switch (currentPauseState)
            {
                case PauseMenuState.PauseMenu:
                    UpdateMenu(2);
                    break;

            }
        }

        public void UpdateCubeSize(int direction)
        {
            if (direction != 1 && direction != -1) return;

            var dimension = PlayerManager.Instance.UpdateDimension(direction);
            cubeSizeLabel.text = $"{dimension}X{dimension}";

        }

        public void ToggleTimer(bool isOn)
        {
            inGameTimer.gameObject.SetActive(isOn);
        }
        void UpdateTimer()
        {
            if (inGameTimer)
            {
                var formattedTime = System.TimeSpan.FromSeconds(Mathf.Max(0, PlayerManager.Instance.gameTime));
                inGameTimer.text = string.Format("{0:D2}:{1:D2}:{2:D2}", formattedTime.Hours, formattedTime.Minutes, formattedTime.Seconds);

            }
        }
    }
}