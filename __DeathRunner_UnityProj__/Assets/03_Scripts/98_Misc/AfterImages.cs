using UnityEngine;

using UnityEngine.Pool;

using Sirenix.OdinInspector;

using Bool = System.Boolean;

using F32 = System.Single;

using I32 = System.Int32;

namespace Game
{
    public sealed class AfterImages : MonoBehaviour
    {
        private const I32 MAX_AFTER_IMAGES = 10;
        
        #if ODIN_INSPECTOR
        [SuffixLabel(label: "seconds")]
        #endif
        [SerializeField] private F32 afterImageEvery = 0.1f;
        
        [SerializeField] private F32 afterImageFadeOutDuration = 0.5f;

        [SerializeField] private SkinnedMeshRenderer[] originalMeshes;
        
        [SerializeField] private Transform originalTransform;

        //private bool isActivated = false;
        //private Mesh[] afterImages;

        private ObjectPool<AfterImage> afterImagePool;

        private void OnEnable()
        {
            afterImagePool = new ObjectPool<AfterImage>
            (
                createFunc: () =>
                {
                    //Instantiate copies of original meshes
                    AfterImage __afterImage = new()
                    {
                        gameObjects = new GameObject[originalMeshes.Length],
                    };

                    //Generate a new gameObject, meshRenderer and meshFilter for each original mesh.
                    for (I32 __meshIndex = 0; __meshIndex < originalMeshes.Length; __meshIndex += 1)
                    {
                        GameObject __afterImageGameObject = new();
                        MeshRenderer __afterImageMeshRenderer = __afterImageGameObject.AddComponent<MeshRenderer>();
                        MeshFilter __afterImageMeshFilter = __afterImageGameObject.AddComponent<MeshFilter>();

                        Mesh __afterImageMesh = new();
                        originalMeshes[__meshIndex].BakeMesh(__afterImageMesh);
                        __afterImageMeshFilter.mesh = __afterImageMesh;

                        __afterImageMeshRenderer.material = originalMeshes[__meshIndex].material;

                        __afterImage.gameObjects[__meshIndex] = __afterImageGameObject;
                    }

                    return __afterImage;
                },
                actionOnGet: afterImage =>
                {
                    foreach (GameObject __gameObject in afterImage.gameObjects)
                    {
                        __gameObject.SetActive(value: true);
                    }
                },
                actionOnRelease: afterImage =>
                {
                    foreach (GameObject __gameObject in afterImage.gameObjects)
                    {
                        __gameObject.SetActive(value: false);
                    }
                },
                actionOnDestroy: afterImage =>
                {
                    foreach (GameObject __gameObject in afterImage.gameObjects)
                    {
                        Destroy(__gameObject);
                    }
                }, 
                collectionCheck: true,
                defaultCapacity: MAX_AFTER_IMAGES,
                maxSize: MAX_AFTER_IMAGES
            );
        }
        
        private void OnDisable()
        {
            afterImagePool.Dispose();
        }


        // private IEnumerator ActivateTrail()
        // {
        //
        //     if (afterImages == null)
        //     {
        //         afterImages = GetComponentsInChildren<SkinnedMeshRenderer>();
        //     }
        //     
        //     foreach (Mesh afterImage in afterImages)
        //     {
        //         afterImage.enabled = true;
        //     }
        // }

        // private void SpawnAfterImage()
        // {
        //     for (I32 __meshIndex = 0; __meshIndex < originalMeshes.Length; __meshIndex += 1)
        //     {
        //         SkinnedMeshRenderer[] afterImage = Instantiate(originalMeshes, originalTransform.position, originalTransform.rotation);
        //         afterImage.transform.parent = transform;
        //         afterImage.enabled = false;
        //     }
        // }
        
        private class AfterImage
        {
            //public Mesh[] meshes;
            //public MeshFilter[] meshFilters;

            public GameObject[] gameObjects;
        }
    }
    
}
