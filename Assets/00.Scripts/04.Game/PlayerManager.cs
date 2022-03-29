using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PEDREROR1.RUBIK.Utilities;
using System.Linq;
using System;
using PEDREROR1.RUBIK.UI;

namespace PEDREROR1.RUBIK
{
    [RequireComponent(typeof(SaveManager))]
    public class PlayerManager : SingletonComponent<PlayerManager>
    {

        [SerializeField]
        private CameraController _camera;
        [SerializeField]
        CubeGenerator cubeGenerator;
        public Vector2 ShuffleAmmount = new Vector2(10, 70);
        public float animationSpeed = 1f;
        public bool DEBUG = false;
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
        public List<Slice> Slices = new List<Slice>();

        private bool canRotate;
        private Cublet currentCublet;
        private Vector3 currentFace;
        private float gameTime;
        private bool shuffleOnStart;
        private Stack<KeyValuePair<Slice, int>> MovementList = new Stack<KeyValuePair<Slice, int>>();


        public bool isPlaying => currentState == GameState.Playing;

        private bool isAnimating { get; set; }

        public SaveManager saveManager { get; private set; }

        Coroutine CO_Timer, CO_WinAnimation, CO_SHUFFLE;
        WaitForSeconds TimerDelay = new WaitForSeconds(1f);

        public Action ResumeEvnt, CubeGeneratedEvnt;
        public Action<float> UpdateTimerEvnt;

        public SaveData saveData;



        public Transform GetCubeTransform => cubeGenerator ? cubeGenerator.transform : null;


        public int Dimension { get; private set; }
        public bool hasWon { get; set; }
        public bool hasCublet => currentCublet != null;


        public Vector3 GetCenter
        {
            get { return new Vector3((Dimension - 1) / 2f, (Dimension - 1) / 2f, (Dimension - 1) / 2f); }
            private set { }
        }

        private bool CheckWiningCondition = true;

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
            if (currentState == GameState.Playing && CheckWiningCondition)
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
                    MovementList.Push(newMovement);
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
                    MovementList.Push(newMovement);
                }
            }
        }

        public void TryUndo()
        {
            if (MovementList.Count > 0 && !isAnimating)
            {
                isAnimating = true;
                var lastMovement = MovementList.Pop();
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
            var slicesTOUpdate = Slices.Where(slice => slice.RotationAngle != rotationAngle).ToList();
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
                    if (movement.Key >= 0 && movement.Key - 1 < Slices.Count)
                    {
                        TryRotate(Slices[movement.Key], movement.Value, 10);
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
                int Rotations = UnityEngine.Random.Range(Mathf.Max((int)ShuffleAmmount.x, 10),
                                                         Mathf.Min((int)ShuffleAmmount.y, 100));
                for (int i = 0; i < Rotations; i++)
                {
                    TryRotate(Slices[UnityEngine.Random.Range(0, Slices.Count)], UnityEngine.Random.Range(-100, 100) > 0 ? 1 : -1, 15);
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
            if (Time.time - startTime < 3f)
                yield return new WaitForSeconds(3 - (Time.time - startTime));

            CheckWiningCondition = true;
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
                yield return TimerDelay;
                gameTime++;
                UpdateTimerEvnt?.Invoke(gameTime);
            }
        }
        #endregion
        #region LIFECYCLE
        public void StartGame()
        {
            hasWon = false;
            CheckWiningCondition = false;
            if (shuffleOnStart)
            {
                if (currentState != GameState.Shuffling)
                {
                    UpdateState(GameState.Shuffling);
                    cubeGenerator.generateCube();
                    StartCoroutine(Shuffle_CO(null));
                }
            }
            else
            {
                cubeGenerator.generateCube();
                UpdateState(GameState.Playing);
            }
        }
        public void Continue()
        {
            if (saveData == null)
            {
                cubeGenerator.generateCube();
                UpdateState(GameState.Playing);
            }
            else
            {
                if (currentState != GameState.Shuffling)
                {
                    gameTime = saveData.timer;
                    Dimension = saveData.dimensions;
                    UpdateState(GameState.Shuffling);
                    cubeGenerator.generateCube();
                    StartCoroutine(Shuffle_CO(saveData));
                }
            }
        }

        public void Win()
        {
            CheckWiningCondition = true;
            StopTimer();
            CO_WinAnimation = StartCoroutine(WinAnimation_CO());
        }
        public void StopWinAnimation()
        {
            if (CO_WinAnimation != null)
            {
                CheckWiningCondition = false;
                StopCoroutine(CO_WinAnimation);
                UpdateState(GameState.Playing);
            }
        }
        public IEnumerator WinAnimation_CO(float duration = 10f)
        {
            UpdateState(GameState.Ending);
            hasWon = true;
            MenuManager.Instance.OnWin();
            if (_camera)
            {
                while (duration > 0)
                {
                    yield return new WaitForEndOfFrame();
                    _camera.Rotate(new Vector2(0, 1f));
                    _camera.CalculateZoom(extraZoom: -0.1f);
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
                MovementList.Clear();
                UpdateState(GameState.Shuffling);
                cubeGenerator.DestroyCube();
                cubeGenerator.generateCube();
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
            MovementList.Clear();
            UpdateTimerEvnt.Invoke(gameTime);
            UpdateState(GameState.Menu);
            if (cubeGenerator)
            {
                cubeGenerator.DestroyCube();
            }
        }
        public void CheckWinningCondition()
        {
            if (currentState != GameState.Playing || !CheckWiningCondition) return;
            foreach (var slice in Slices)
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
        public void save()
        {
            if (saveManager)
            {
                if (MovementList.Count > 0)
                    saveManager.Save(MovementList, Dimension, gameTime);
            }
        }
        public bool load()
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