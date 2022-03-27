 using System.Collections.Generic;  
using UnityEngine;

namespace PEDREROR1.RUBIK
{
    public class CubeGenerator : MonoBehaviour
    {
        // Start is called before the first frame update

       
        public Cublet cublet;
        

        

        Transform _transform;

        public Vector3 getCenter
        {
           
            get 
            {
                    var center = (PlayerManager.Instance.dimension - 1 )    /    2f; 
                    return new Vector3(center, center, center); 
            }
            private set { }
        }

        [SerializeField] bool CreateCubeOnAwake;

        private void Awake()
        {
            _transform = transform;
            if (CreateCubeOnAwake)
            {
                generateCube();
            }
        }
        public void generateCube()
        {
            if (!cublet || !Application.isPlaying || PlayerManager.Instance.cubeMatrix!=null) return;
            PlayerManager.Instance.Slices = new List<Slice>();
            int currentSlice = 0;
            PlayerManager.Instance.cubeMatrix = new Cublet[PlayerManager.Instance.dimension, PlayerManager.Instance.dimension, PlayerManager.Instance.dimension];
            for (int x = 0; x < PlayerManager.Instance.dimension; x++)
            {
                var newSlice = new GameObject($"Slice_{currentSlice}").AddComponent<Slice>();
                newSlice.transform.position = getCenter;
                newSlice.transform.parent = _transform;
                PlayerManager.Instance.Slices.Add(newSlice);
                for (int y = 0; y < PlayerManager.Instance.dimension; y++)
                {
                    for (int z = 0; z < PlayerManager.Instance.dimension; z++)
                    {
                        if (x == 0 || y == 0 || z == 0 || x == PlayerManager.Instance.dimension - 1 || y == PlayerManager.Instance.dimension - 1 || z == PlayerManager.Instance.dimension - 1)
                        {
                            Vector3 position = new Vector3(x, y, z);
                            {
                                PlayerManager.Instance.cubeMatrix[x, y, z] = Instantiate(cublet, position, Quaternion.identity);
                                PlayerManager.Instance.cubeMatrix[x, y, z].setPositionAndName(position);
                                PlayerManager.Instance.cubeMatrix[x, y, z].setParent(_transform);
                                PlayerManager.Instance.Slices[currentSlice].cublets.Add(new Slice.CubletData(PlayerManager.Instance.cubeMatrix[x, y, z]));

                            }
                        }

                    }
                }
                if(x==0)
                newSlice.Setup(Slice.FaceType.Left);
                else if(x==PlayerManager.Instance.dimension-1)
                    newSlice.Setup(Slice.FaceType.Right);
                else
                    newSlice.Setup(Slice.FaceType.innerFace);
                currentSlice++; ;
            }

            for (int y = 0; y < PlayerManager.Instance.dimension; y++)
            {
                var newSlice = new GameObject($"Slice_{currentSlice}").AddComponent<Slice>();
                newSlice.transform.position = getCenter;
                newSlice.transform.parent = _transform;
                PlayerManager.Instance.Slices.Add(newSlice);
                for (int z = 0; z < PlayerManager.Instance.dimension; z++)
                {
                    for (int x = 0; x < PlayerManager.Instance.dimension; x++)
                    {
                        if (PlayerManager.Instance.cubeMatrix[x, y, z] != null)
                        {
                            newSlice.cublets.Add(new Slice.CubletData(PlayerManager.Instance.cubeMatrix[x, y, z]));

                        }
                    }
                }
                if (y == 0)
                    newSlice.Setup(Slice.FaceType.Down);
                else if (y == PlayerManager.Instance.dimension - 1)
                    newSlice.Setup(Slice.FaceType.Top);
                else
                    newSlice.Setup(Slice.FaceType.innerFace);
               
                currentSlice++; ;
            }
            for (int z = 0; z < PlayerManager.Instance.dimension; z++)
            {
                var newSlice = new GameObject($"Slice_{currentSlice}").AddComponent<Slice>();
                newSlice.transform.position = getCenter;
                newSlice.transform.parent = _transform;
                PlayerManager.Instance.Slices.Add(newSlice);
                for (int y = 0; y < PlayerManager.Instance.dimension; y++)
                {
                    for (int x = 0; x < PlayerManager.Instance.dimension; x++)
                    {
                        if (PlayerManager.Instance.cubeMatrix[x, y, z] != null)
                        {
                            newSlice.cublets.Add(new Slice.CubletData(PlayerManager.Instance.cubeMatrix[x, y, z]));

                        }

                    }
                }
                if (z == 0)
                    newSlice.Setup(Slice.FaceType.Front);
                else if (z == PlayerManager.Instance.dimension - 1)
                    newSlice.Setup(Slice.FaceType.Back);
                else
                    newSlice.Setup(Slice.FaceType.innerFace);
                currentSlice++; ;
            }
            PlayerManager.Instance.CubeGeneratedEvnt?.Invoke();
        }

         
        public void DestroyCube()
        {
            if (!Application.isPlaying || _transform.childCount <= 0) return;
            foreach (var item in PlayerManager.Instance.cubeMatrix)
            {
                if (item)
                    item.DestroyCublet();
            }
            PlayerManager.Instance.cubeMatrix = null;
            for (int i = 0; i < PlayerManager.Instance.Slices.Count; i++)
            {
                PlayerManager.Instance.Slices[i].Destroy();
            }

            PlayerManager.Instance.Slices.Clear();
        }

        // Update is called once per frame
        void Update()
        {

        }
        
    }
}