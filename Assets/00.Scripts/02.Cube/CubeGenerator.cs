using PEDREROR1.RUBIK.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace PEDREROR1.RUBIK
{
    /// <summary>
    /// THE CUBEGENERATOR class is in charged Of Creating and Destroying The Rubik Cube
    /// </summary>
    public class CubeGenerator : MonoBehaviour
    {
#region PARAMETERS

        [Tooltip("Reference To the Cublet PRefab")]
        public Cublet cublet;
        [Tooltip("DEBUG OPTION")]
        [SerializeField] bool createCubeOnAwake;
        int currentSlice = 0;
        #endregion

#region METHODS
        private void Awake()
        {
            if (createCubeOnAwake)
            {
                GenerateCube();
            }
        }
        public void GenerateCube()
        {
            if (!cublet || !Application.isPlaying || PlayerManager.Instance.cubeMatrix != null) return;
            currentSlice = 0;
            PlayerManager.Instance.cubeMatrix = new Cublet[PlayerManager.Instance.Dimension, PlayerManager.Instance.Dimension, PlayerManager.Instance.Dimension];
            SetupXSlices();
            SetupYSlices();
            SetupZSlices();
            PlayerManager.Instance.CubeGeneratedEvnt?.Invoke();
        }
        private Slice CreateSlice(int currentSlice)
        {
            var newSlice = new GameObject($"Slice_{currentSlice}").AddComponent<Slice>();
            newSlice.transform.position = PlayerManager.Instance.GetCenter;
            newSlice.transform.parent = transform;
            PlayerManager.Instance.slices.Add(newSlice);
            return newSlice;
        }
        private void SetupZSlices()
        {
            for (int z = 0; z < PlayerManager.Instance.Dimension; z++)
            {
                Slice newSlice = CreateSlice(currentSlice);
                for (int y = 0; y < PlayerManager.Instance.Dimension; y++)
                {
                    for (int x = 0; x < PlayerManager.Instance.Dimension; x++)
                    {
                        if (PlayerManager.Instance.cubeMatrix[x, y, z] != null)
                        {
                            newSlice.AddCublet(new CubletData(PlayerManager.Instance.cubeMatrix[x, y, z]));
                        }
                    }
                }
                if (z == 0)
                    newSlice.Setup(Slice.FaceType.Front);
                else if (z == PlayerManager.Instance.Dimension - 1)
                    newSlice.Setup(Slice.FaceType.Back);
                else
                    newSlice.Setup(Slice.FaceType.innerFace);
                currentSlice++; ;
            }
        }

        private void SetupYSlices()
        {
            for (int y = 0; y < PlayerManager.Instance.Dimension; y++)
            {
                Slice newSlice = CreateSlice(currentSlice);
                for (int z = 0; z < PlayerManager.Instance.Dimension; z++)
                {
                    for (int x = 0; x < PlayerManager.Instance.Dimension; x++)
                    {
                        if (PlayerManager.Instance.cubeMatrix[x, y, z] != null)
                        {
                            newSlice.AddCublet(new CubletData(PlayerManager.Instance.cubeMatrix[x, y, z]));

                        }
                    }
                }
                if (y == 0)
                    newSlice.Setup(Slice.FaceType.Down);
                else if (y == PlayerManager.Instance.Dimension - 1)
                    newSlice.Setup(Slice.FaceType.Top);
                else
                    newSlice.Setup(Slice.FaceType.innerFace);

                currentSlice++; ;
            }
        }

        private void SetupXSlices()
        {
            for (int x = 0; x < PlayerManager.Instance.Dimension; x++)
            {
                Slice newSlice = CreateSlice(currentSlice);
                for (int y = 0; y < PlayerManager.Instance.Dimension; y++)
                {
                    for (int z = 0; z < PlayerManager.Instance.Dimension; z++)
                    {
                        if (x == 0 || y == 0 || z == 0 || x == PlayerManager.Instance.Dimension - 1 || y == PlayerManager.Instance.Dimension - 1 || z == PlayerManager.Instance.Dimension - 1)
                        {
                            Vector3 position = new Vector3(x, y, z);
                            {
                                PlayerManager.Instance.cubeMatrix[x, y, z] = Instantiate(cublet, position, Quaternion.identity);
                                PlayerManager.Instance.cubeMatrix[x, y, z].setPositionAndName(position);
                                PlayerManager.Instance.cubeMatrix[x, y, z].setParent(transform);
                                PlayerManager.Instance.slices[currentSlice].AddCublet(new CubletData(PlayerManager.Instance.cubeMatrix[x, y, z]));
                            }
                        }
                    }
                }
                if (x == 0)
                    newSlice.Setup(Slice.FaceType.Left);
                else if (x == PlayerManager.Instance.Dimension - 1)
                    newSlice.Setup(Slice.FaceType.Right);
                else
                    newSlice.Setup(Slice.FaceType.innerFace);
                currentSlice++; ;
            }
        }

        public void DestroyCube()
        {
            if (!Application.isPlaying || transform.childCount <= 0) return;

            foreach (var item in PlayerManager.Instance.cubeMatrix)
            {
                if (item)
                    item.DestroyCublet();
            }
            PlayerManager.Instance.cubeMatrix = null;
            for (int i = 0; i < PlayerManager.Instance.slices.Count; i++)
            {
                PlayerManager.Instance.slices[i].Destroy();
            }

            PlayerManager.Instance.slices.Clear();
        }
        #endregion
    }
}