using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PEDREROR1.RUBIK.Utilities;
using System.Linq;
using System;
using PEDREROR1.RUBIK.UI;

namespace PEDREROR1.RUBIK
{
    /// <summary>
    /// The Player Manager Is in charge of the Whole Game Flow Managment
    /// It also comunnicates between the Input Manager, Cublets Objects and Slice Objects
    /// </summary>
    [RequireComponent(typeof(SaveManager))]
    public class PlayerManager : SingletonComponent<PlayerManager>
    {
#region PARAMETERS
        [SerializeField]
        private CameraController cameraController;
        [SerializeField]
        CubeGenerator cubeGenerator;
        public Vector2 shuffleAmmount = new Vector2(10, 70);
        public float animationSpeed = 1f;
        public bool debug = false;
        public Cublet[,,] cubeMatrix;
        public enum GameState
        {
            Menu = 0,
            Pause = 1,
            Playing = 2,
            Shuffling = 3,
            Ending = 4
        }
        [HideInInspector]
        public GameState currentState;
        [HideInInspector]
        public List<Slice> slices = new List<Slice>();
        public Transform GetCubeTransform => cubeGenerator ? cubeGenerator.transform : null;
        public Action ResumeEvnt, CubeGeneratedEvnt;
        public Action<float> UpdateTimerEvnt;
        public int Dimension { get; private set; }
        public bool hasWon { get; set; }
        public bool hasCublet => currentCublet != null;

        public Vector3 GetCenter
        {
            get { return new Vector3((Dimension - 1) / 2f, (Dimension - 1) / 2f, (Dimension - 1) / 2f); }
            private set { }
        }

        private SaveManager saveManager { get;  set; }
        private Coroutine CO_Timer, CO_WinAnimation, CO_Shuffle;
        private WaitForSeconds timerDelay = new WaitForSeconds(1f);
        private SaveData saveData;
        private bool canRotate;
        private Cublet currentCublet;
        private Vector3 currentFace;
        private float gameTime;
        private bool shuffleOnStart;
        private Stack<KeyValuePair<Slice, int>> movementList = new Stack<KeyValuePair<Slice, int>>();
        private bool isAnimating;
        private bool checkWiningCondition = true;
#endregion
#region METHODS

        #region SETUP
        private void Awake()
        {
            shuffleOnStart = true;
            saveManager = GetComponent<SaveManager>();
            Dimension = 3;
            UpdateState(GameState.Menu);
        }
        public int UpdateDimension(int dir)
        {
            return Dimension = Mathf.Min(Mathf.Max(Dimension + dir, 2), 6);
        }

        public void toggleShuffleOnStart(bool shuffle)
        {
            shuffleOnStart = shuffle;
        }
        #endregion
        #region GAMEMANAGMENT

        public void UpdateState(GameState newState)
        {
            currentState = newState;
            if (currentState == GameState.Playing && checkWiningCondition)
            {
                if (CO_Timer == null)
                {
                    CO_Timer = StartCoroutine(Timer_CO());
                }
            }
            else
            {
                StopTimer();
            }
        }
        public void UpdateState(int newState)
        {
            currentState = (GameState)newState;

            if (currentState == GameState.Playing)
            {
                if (CO_Timer == null)
                {
                    CO_Timer = StartCoroutine(Timer_CO());
                }
            }
            else
            {
                StopTimer();
            }
        }
        public void TryRotate(Vector3 direction)
        {
            if (!isAnimating && canRotate && currentCublet)
            {
                isAnimating = true;
                canRotate = false;
                var newMovement = currentCublet.Rotate(direction, currentFace);
                if (newMovement.Key != null)
                {
                    movementList.Push(newMovement);
                }
            }
        }
        public void TryRotate(Slice slice, int direction, float speed = 1)
        {
            if (!isAnimating)
            {
                isAnimating = true;
                var newMovement = slice.TryRotate(direction, speed);

                if (newMovement.Key != null)
                {
                    movementList.Push(newMovement);
                }
            }
        }

        public void TryUndo()
        {
            if (movementList.Count > 0 && !isAnimating)
            {
                isAnimating = true;
                var lastMovement = movementList.Pop();
                lastMovement.Key.TryRotate(-lastMovement.Value);
            }
        }

        public void UpdateCurrentCublet(RaycastHit hitObject)
        {
            if (hitObject.transform.TryGetComponent<Cublet>(out currentCublet))
            {
                currentFace = hitObject.normal;
            }
        }
        public void RemoveCurrentCublet()
        {
            currentCublet = null;
            canRotate = true;
        }

        public void UpdateSlices(Vector3 rotationAngle, Slice changedSlice)
        {
            isAnimating = false;
            var slicesTOUpdate = slices.Where(slice => slice.RotationAngle != rotationAngle).ToList();
            slicesTOUpdate.ForEach(slice =>
            {
                slice.updateSlice(changedSlice);
            });
        }
        public IEnumerator Shuffle_CO(SaveData SD = null, bool isPaused = false)
        {
            float startTime = Time.time;
            if (SD != null)
            {
                foreach (var movement in SD.movements)
                {
                    if (movement.Key >= 0 && movement.Key - 1 < slices.Count)
                    {
                        TryRotate(slices[movement.Key], movement.Value, 10);
                        while (isAnimating)
                        {
                            yield return null;
                        }
                    }
                }
                MenuManager.Instance.playSelectCubeAnimation();
                isAnimating = false;
            }
            else
            {
                int Rotations = UnityEngine.Random.Range(Mathf.Max((int)shuffleAmmount.x, 10),
                                                         Mathf.Min((int)shuffleAmmount.y, 100));
                for (int i = 0; i < Rotations; i++)
                {
                    TryRotate(slices[UnityEngine.Random.Range(0, slices.Count)], UnityEngine.Random.Range(-100, 100) > 0 ? 1 : -1, 15);
                    while (isAnimating)
                    {
                        yield return null;
                    }
                }
                if (isPaused)
                {
                    ResumeEvnt.Invoke();
                    gameTime = 0;
                    UpdateTimerEvnt.Invoke(gameTime);
                }

                isAnimating = false;
            }
            if (Time.time - startTime < 2f)
                yield return new WaitForSeconds(2 - (Time.time - startTime));

            checkWiningCondition = true;
            UpdateState(GameState.Playing);
        }
        private void StopTimer(bool reset = false)
        {
            if (CO_Timer != null)
            {
                StopCoroutine(CO_Timer);
                CO_Timer = null;
            }
            if (reset)
            {
                gameTime = 0;
                UpdateTimerEvnt.Invoke(gameTime);
            }
        }
        IEnumerator Timer_CO()
        {
            while (currentState == GameState.Playing)
            {
                yield return timerDelay;
                gameTime++;
                UpdateTimerEvnt?.Invoke(gameTime);
            }
        }
        #endregion
        #region LIFECYCLE
        public void StartGame()
        {
            hasWon = false;
            checkWiningCondition = false;
            if (shuffleOnStart)
            {
                if (currentState != GameState.Shuffling)
                {
                    UpdateState(GameState.Shuffling);
                    cubeGenerator.GenerateCube();
                    StartCoroutine(Shuffle_CO(null));
                }
            }
            else
            {
                cubeGenerator.GenerateCube();
                UpdateState(GameState.Playing);
            }
        }
        public void Continue()
        {
            if (saveData == null)
            {
                cubeGenerator.GenerateCube();
                UpdateState(GameState.Playing);
            }
            else
            {
                if (currentState != GameState.Shuffling)
                {
                    gameTime = saveData.timer;
                    Dimension = saveData.dimensions;
                    UpdateState(GameState.Shuffling);
                    cubeGenerator.GenerateCube();
                    StartCoroutine(Shuffle_CO(saveData));
                }
            }
        }

        public void Win()
        {
            checkWiningCondition = true;
            StopTimer();
            CO_WinAnimation = StartCoroutine(WinAnimation_CO());
        }
        public void StopWinAnimation()
        {
            if (CO_WinAnimation != null)
            {
                checkWiningCondition = false;
                StopCoroutine(CO_WinAnimation);
                UpdateState(GameState.Playing);
            }
        }
        public IEnumerator WinAnimation_CO(float duration = 10f)
        {
            UpdateState(GameState.Ending);
            hasWon = true;
            MenuManager.Instance.OnWin();
            if (cameraController)
            {
                while (duration > 0)
                {
                    yield return new WaitForEndOfFrame();
                    cameraController.Rotate(new Vector2(0, 1f));
                    cameraController.CalculateZoom(extraZoom: -0.1f);
                    duration -= Time.deltaTime;
                }
            }
            CO_WinAnimation = null;
        }
        public void Restart()
        {
            hasWon = false;
            if (cubeGenerator && currentState != GameState.Shuffling)
            {
                movementList.Clear();
                UpdateState(GameState.Shuffling);
                cubeGenerator.DestroyCube();
                cubeGenerator.GenerateCube();
                StartCoroutine(Shuffle_CO(null, true));
            }
        }

        public void Resume()
        {
            if (currentState == GameState.Pause)
            {
                UpdateState(GameState.Playing);
            }
        }
        public void GoBackToMenu()
        {
            StopTimer(true);
            movementList.Clear();
            UpdateTimerEvnt.Invoke(gameTime);
            UpdateState(GameState.Menu);
            if (cubeGenerator)
            {
                cubeGenerator.DestroyCube();
            }
        }
        public void CheckWinningCondition()
        {
            if (currentState != GameState.Playing || !checkWiningCondition) return;
            foreach (var slice in slices)
            {
                if (slice.sliceFaceType != Slice.FaceType.innerFace)
                {
                    if (!slice.CheckFace())
                    {
                        return;
                    }
                }
            }
            CO_WinAnimation = StartCoroutine(WinAnimation_CO());
        }
        #endregion
        #region DATA_MANAGMENT
        public void Save()
        {
            if (saveManager)
            {
                if (movementList.Count > 0)
                    saveManager.Save(movementList, Dimension, gameTime);
            }
        }
        public bool Load()
        {
            if (saveManager)
            {
                saveData = saveManager.LoadData();
            }
            return saveData != null;
        }
        #endregion
        #endregion
    }
}