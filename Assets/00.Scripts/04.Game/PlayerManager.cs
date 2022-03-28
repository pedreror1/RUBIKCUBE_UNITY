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
        public CubeGenerator cubeGenerator;
        public enum GameState
        {
            Menu = 0,
            Pause = 1,
            Playing = 2,
            Shuffling = 3,
            Ending = 4
        }
        public GameState currentState;
        public int dimension;
        bool canRotate;
        public Cublet currentCublet;
        Vector3 currentDirection;
        Vector3 currentFace;
        public List<Slice> Slices;
        public Cublet[,,] cubeMatrix;

        public float gameTime;

        public Stack<KeyValuePair<Slice, int>> MovementList = new Stack<KeyValuePair<Slice, int>>();

        public float animationSpeed = 1f;
        public bool isPlaying => currentState == GameState.Playing;

        private bool isAnimating { get; set; }

        public SaveManager saveManager { get; private set; }

        Coroutine CO_Timer;
        WaitForSeconds TimerDelay = new WaitForSeconds(1f);

        public Action UpdateTimerEvnt, ResumeEvnt, CubeGeneratedEvnt;

        public SaveData saveData;
        [SerializeField] CameraController _camera;
        public Vector2 ShuffleAmmount = new Vector2(10, 70);
        public void UpdateState(GameState newState)
        {
            currentState = newState;
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

        private void Awake()
        {
            saveManager = GetComponent<SaveManager>();
            dimension = 3;
            UpdateState(GameState.Menu);

        }

        public int UpdateDimension(int dir)
        {
            return dimension = Mathf.Min(Mathf.Max(dimension + dir, 2), 6);

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

                var newMovement = slice.Rotate(direction, speed);
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
                lastMovement.Key.Rotate(-lastMovement.Value);
            }
        }
        public bool hasCublet => currentCublet != null;
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
        public void UpdateDirection(Vector3 newDirection)
        {
            currentDirection = newDirection;
            //tryRotate
        }
        //TODO move to CUbe
        public void UpdateSlices(Vector3 rotationAngle, Slice changedSlice)
        {
            isAnimating = false;
            var slicesTOUpdate = Slices.Where(slice => slice.RotationAngle != rotationAngle).ToList();
            slicesTOUpdate.ForEach(slice =>
            {
                slice.updateSlice(changedSlice);
            });

        }

        //DATA MANAGMENT
        public void save()
        {
            if (saveManager)
            {
                if (MovementList.Count > 0)
                    saveManager.Save(MovementList, dimension, gameTime);
            }
        }
        public bool shuffleOnStart;

        public void StartGame()
        {
            if (shuffleOnStart)
            {
                if (currentState != GameState.Shuffling)
                {
                    UpdateState(GameState.Shuffling);
                    cubeGenerator.generateCube();
                    StartCoroutine(Shuffle(null));
                }
            }
            else
            { cubeGenerator.generateCube(); UpdateState(GameState.Playing); }
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
                    dimension = saveData.dimensions;
                    UpdateState(GameState.Shuffling);
                    cubeGenerator.generateCube();
                    StartCoroutine(Shuffle(saveData));
                }
            }
        }
        public IEnumerator Shuffle(SaveData SD = null, bool isPaused = false)
        {

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
                int Rotations = UnityEngine.Random.Range((int)ShuffleAmmount.x,(int) ShuffleAmmount.y);
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
                    UpdateTimerEvnt.Invoke();

                }
                
                isAnimating = false;
            }
            UpdateState(GameState.Playing);
        }

        public void Win()
        {
            StopTimer();
            StartCoroutine(WinAnimation());
        }

        public IEnumerator WinAnimation(float duration = 10f)
        {
            UpdateState(GameState.Ending);


            if (_camera)
            {
                while (duration > 0)
                {
                    yield return new WaitForEndOfFrame();
                    _camera.Rotate(new Vector2(0, 1f));
                    _camera.CalculateZoom(-0.1f);
                    duration -= Time.deltaTime;
                }
            }
        }
        public void Restart()
        {
            if (cubeGenerator && currentState!= GameState.Shuffling)
            {
                MovementList.Clear();
                UpdateState(GameState.Shuffling);
                cubeGenerator.DestroyCube();
                cubeGenerator.generateCube();
                StartCoroutine(Shuffle(null, true));

            }
        }

        private void Update()
        {
            
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
            UpdateTimerEvnt.Invoke();
            UpdateState(GameState.Menu);
            if (cubeGenerator)
            {
                cubeGenerator.DestroyCube();
            }
        }

        private void StopTimer(bool reset=false)
        {
            if (CO_Timer != null)
            {
                StopCoroutine(CO_Timer);

                CO_Timer = null;
               
            }
            if (reset)
            {
                gameTime = 0;
                UpdateTimerEvnt.Invoke();
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

        IEnumerator Timer_CO()
        {
            while (currentState == GameState.Playing)
            {
                yield return TimerDelay;
                gameTime++;
                UpdateTimerEvnt?.Invoke();
                
            }
        }

        public void CheckWinningCondition()
        {
            if (currentState != GameState.Playing) return;
            foreach (var slice in Slices)
            {
                if(slice.sliceFaceType!= Slice.FaceType.innerFace)
                {
                    if(!slice.CheckFace())
                    {
                        return ;
                    }
                }
            }

            StartCoroutine(WinAnimation());
        }
    }
}