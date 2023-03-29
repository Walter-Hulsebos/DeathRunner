using System;
using System.Collections;

using UnityEngine;
//using UnityEngine.Rendering;

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
        [SerializeField] private F32 placeAfterImageEvery = 0.1f;

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
                        meshes      = new Mesh[originalMeshes.Length],
                        meshFilters = new MeshFilter[originalMeshes.Length]
                    };

                    for (I32 __meshIndex = 0; __meshIndex < originalMeshes.Length; __meshIndex += 1)
                    {
                        //Instantiate new Gameobject, add MeshFilter and MeshRenderer
                        
                        __afterImage.meshes[__meshIndex] = new Mesh();
                        __afterImage.meshFilters[__meshIndex] = new MeshFilter();
                        originalMeshes[__meshIndex].BakeMesh(__afterImage.meshes[__meshIndex]);
                        
                    }

                    return __afterImage;
                },
                actionOnGet: afterImage =>
                {
                    afterImage
                } 
                
            )
        }
        
        private void OnDisable()
        {
            
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

        private void SpawnAfterImage()
        {
            for (I32 __meshIndex = 0; __meshIndex < originalMeshes.Length; __meshIndex += 1)
            {
                SkinnedMeshRenderer[] afterImage = Instantiate(originalMeshes, originalTransform.position, originalTransform.rotation);
                afterImage.transform.parent = transform;
                afterImage.enabled = false;
            }
        }
        
        private class AfterImage
        {
            public Mesh[] meshes;
            public MeshFilter[] meshFilters;
        }
    }
    
}
