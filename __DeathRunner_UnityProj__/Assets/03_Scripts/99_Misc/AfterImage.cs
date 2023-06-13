using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GenericScriptableArchitecture;
using UnityEngine;
using UnityEngine.Pool;

using Sirenix.OdinInspector;
using Sirenix.Utilities;

using F32 = System.Single;

namespace DeathRunner
{
    public sealed class AfterImage : MonoBehaviour
    {
        [SerializeField] private F32 distanceToSpawnNewMesh = 1.0f;

        [SerializeField] private Material mat;

		[SerializeField] private Gradient colorGradient;

        [SerializeField] private SkinnedMeshRenderer[] characterSkinnedMeshes;
        [SerializeField] private MeshFilter[]          characterMeshes;

        [SerializeField] private ScriptableEvent start;
		[SerializeField] private ScriptableEvent stop;
        
        private Boolean _doEffect = false;

        //private IObjectPool<GameObject> _objectPool;

        private readonly List<GameObject>   _spawnedObjects = new();
        private readonly List<MeshRenderer> _spawnedSkinnedMeshes = new();
        private readonly List<MeshRenderer> _spawnedMeshes = new();

        private Vector3 _previousSpawnPoint;
        private static readonly Int32 color = Shader.PropertyToID(name: "_BaseColor");

        #if ODIN_INSPECTOR
        private void Reset()
        {
            FindSkinnedMeshRenderers();
            
            FindMeshRenderers();
        }

        private void OnValidate()
        {
            if (characterSkinnedMeshes.IsNullOrEmpty())
            {
                FindSkinnedMeshRenderers();
            }
            
            if (characterMeshes.IsNullOrEmpty())
            {
                FindMeshRenderers();
            }
        }
        
        [ContextMenu("Find Skinned Mesh Renderers")]
        private void FindSkinnedMeshRenderers()
        {
            characterSkinnedMeshes = GetComponentsInChildren<SkinnedMeshRenderer>();
        }
        
        [ContextMenu("Find Mesh Renderers")]
        private void FindMeshRenderers()
        {
            characterMeshes = GetComponentsInChildren<MeshFilter>();
        }
        #endif

        private void OnEnable()
        {
            start += EffectOn;
            stop  += EffectOff;
        }
        
        private void OnDisable()
        {
            start -= EffectOn;
            stop  -= EffectOff;
            
            ReleaseAllSpawnedObjects();
        }

        private void EffectOn()
        {
            _doEffect = true;
        }

        private void EffectOff()
        {
            _doEffect = false;
            ReleaseAllSpawnedObjects();
        }

        private void Start()
        {
            //_objectPool = new ObjectPool<GameObject>(createFunc: CreatePooledItem, actionOnGet: null, actionOnRelease: OnReturnedToPool, actionOnDestroy: OnDestroyPoolObject);
            _previousSpawnPoint = transform.position;
        }

        private void Update()
        {
            if (!_doEffect) return;
            
            RefreshAfterImagesGradient();

            F32 __distanceSinceLastSpawn = Vector3.Distance(a: _previousSpawnPoint, b: transform.position);
            if (__distanceSinceLastSpawn < distanceToSpawnNewMesh) return;
            
            //Debug.Log(message: "Spawning new mesh");

            SpawnAfterImage();

            _previousSpawnPoint = transform.position;
        }

        private void SpawnAfterImage()
        {
            foreach (SkinnedMeshRenderer __skinnedMeshRenderer in characterSkinnedMeshes)
            {
                CreateCopyOfSkinnedMesh(skinnedMeshRenderer: __skinnedMeshRenderer);
            }

            foreach (MeshFilter __meshFilter in characterMeshes)
            {
                CreateCopyOfMesh(meshFilter: __meshFilter);
            }
        }

        private void CreateCopyOfSkinnedMesh(SkinnedMeshRenderer skinnedMeshRenderer)
        {
            //GameObject __obj = _objectPool.Get();
            GameObject __obj = new GameObject();
            __obj.transform.SetPositionAndRotation(position: transform.position, rotation: transform.rotation);
            //MeshRenderer __renderer = __obj.GetComponent<MeshRenderer>();
            //MeshFilter   __filter   = __obj.GetComponent<MeshFilter>();
            MeshRenderer __renderer = __obj.AddComponent<MeshRenderer>();
            __renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            MeshFilter   __filter   = __obj.AddComponent<MeshFilter>();
            __obj.name = skinnedMeshRenderer.name + " AfterImage";

            Mesh __mesh = new();
            skinnedMeshRenderer.BakeMesh(mesh: __mesh);

            __filter.mesh = __mesh;
            __renderer.material = mat;

            _spawnedObjects.Add(__obj);
            _spawnedSkinnedMeshes.Add(__renderer);
        }

        private void CreateCopyOfMesh(MeshFilter meshFilter)
        {
            //GameObject __obj = _objectPool.Get();
            GameObject __obj = new GameObject();
            Transform __meshFilterTransform = meshFilter.transform;
            __obj.transform.SetPositionAndRotation(position: __meshFilterTransform.position, rotation: __meshFilterTransform.rotation);
            //MeshRenderer __renderer = __obj.GetComponent<MeshRenderer>();
            //MeshFilter   __filter   = __obj.GetComponent<MeshFilter>();
            MeshRenderer __renderer = __obj.AddComponent<MeshRenderer>();
            __renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            MeshFilter   __filter   = __obj.AddComponent<MeshFilter>();
            __obj.name = meshFilter.name + " AfterImage";
            
            __filter.mesh = meshFilter.mesh;
            __renderer.material = mat;
            
            _spawnedObjects.Add(__obj);
            _spawnedMeshes.Add(__renderer);
        }

        private void RefreshAfterImagesGradient()
        {
            for (Int32 __index = 0; __index < _spawnedSkinnedMeshes.Count; __index++)
            {
                F32 __primantissa = Mathf.Clamp01((F32)__index / _spawnedSkinnedMeshes.Count);
                _spawnedSkinnedMeshes[__index].material.SetColor(nameID: color, value: colorGradient.Evaluate(time: __primantissa));
            }
            
            for (Int32 __index = 0; __index < _spawnedMeshes.Count; __index++)
            {
                F32 __primantissa = Mathf.Clamp01((F32)__index / _spawnedSkinnedMeshes.Count);
                _spawnedSkinnedMeshes[__index].material.SetColor(nameID: color, value: colorGradient.Evaluate(time: __primantissa));
            }
        }

        // private static GameObject CreatePooledItem()
        // {
        //     GameObject __gameObject = new(name: "AfterImage");
        //     
        //     __gameObject.AddComponent<MeshRenderer>();
        //     __gameObject.AddComponent<MeshFilter>();
        //     
        //     return __gameObject;
        // }

        // private static void OnReturnedToPool(GameObject item)
        // {
        //     item.SetActive(value: false);
        // }

        // private static void OnDestroyPoolObject(GameObject item)
        // {
        //     Destroy(obj: item);
        // }

        // private void OnDestroy()
        // {
        //     _objectPool?.Clear();
        // }

        [Button]
        private void ReleaseAllSpawnedObjects()
        {
            foreach (GameObject __spawnedObject in _spawnedObjects)
            {
                //_objectPool.Release(__spawnedObject);
                Destroy(obj: __spawnedObject);
            }
            _spawnedObjects.Clear();
            _spawnedMeshes.Clear();
            _spawnedSkinnedMeshes.Clear();
        }
    }
}
