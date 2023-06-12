using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Serialization;
using F32 = System.Single;

namespace DeathRunner
{
    public class MeshTrail : MonoBehaviour
    {
        [SerializeField] private F32 distanceToSpawnNewMesh = 1.0f;
        //[SerializeField] private F32 meshDestroyDelay = 3f;

        [SerializeField] private Material mat;

        [SerializeField] private SkinnedMeshRenderer[] characterSkinnedMeshes;
        [SerializeField] private MeshFilter[]          characterMeshes;

        private IObjectPool<GameObject> _objectPool;
        //private ListPool<MeshRenderer>  _listPool;
        
        private List<GameObject> _spawnedObjects = new();

        private Vector3 _previousSpawnPoint;

        #if ODIN_INSPECTOR
        private void Reset()
        {
            FindSkinnedMeshRenderers();
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

        private void Start()
        {
            _objectPool = new ObjectPool<GameObject>(createFunc: CreatePooledItem, actionOnGet: null, actionOnRelease: OnReturnedToPool, actionOnDestroy: OnDestroyPoolObject);
            //_listPool   = new ListPool<SkinnedMeshRenderer>();
            _previousSpawnPoint = transform.position;
        }

        private void Update()
        {
            F32 __distanceSinceLastSpawn = Vector3.Distance(a: _previousSpawnPoint, b: transform.position);

            if (__distanceSinceLastSpawn < distanceToSpawnNewMesh) return;
            
            Debug.Log(message: "Spawning new mesh");

            SpawnAfterImage();

            _previousSpawnPoint = transform.position;
        }
        
        [Button]
        private void ReleaseAllSpawnedObjects()
        {
            foreach (GameObject __spawnedObject in _spawnedObjects)
            {
                _objectPool.Release(__spawnedObject);
            }
            _spawnedObjects.Clear();
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
            GameObject __obj = _objectPool.Get();
                    
            __obj.transform.SetPositionAndRotation(position: transform.position, rotation: transform.rotation);
            MeshRenderer __renderer = __obj.GetComponent<MeshRenderer>();
            MeshFilter   __filter   = __obj.GetComponent<MeshFilter>();

            Mesh __mesh = new();
            skinnedMeshRenderer.BakeMesh(mesh: __mesh);

            __filter.mesh = __mesh;
            __renderer.material = mat;
            
            //await UniTask.Delay(TimeSpan.FromSeconds(meshDestroyDelay));
            //Destroy(obj: __obj, t: meshDestroyDelay);
            //_objectPool.Release(__obj);
            
            _spawnedObjects.Add(__obj);
        }

        private void CreateCopyOfMesh(MeshFilter meshFilter)
        {
            GameObject __obj = _objectPool.Get();
            
            __obj.transform.SetPositionAndRotation(position: transform.position, rotation: transform.rotation);
            MeshRenderer __renderer = __obj.GetComponent<MeshRenderer>();
            MeshFilter   __filter   = __obj.GetComponent<MeshFilter>();
            
            __filter.mesh = meshFilter.mesh;
            __renderer.material = mat;
            
            _spawnedObjects.Add(__obj);
        }

        private static GameObject CreatePooledItem()
        {
            GameObject __gameObject = new(name: "AfterImage");
            
            __gameObject.AddComponent<MeshRenderer>();
            __gameObject.AddComponent<MeshFilter>();
            
            return __gameObject;
        }

        private static void OnReturnedToPool(GameObject item)
        {
            item.SetActive(value: false);
        }

        private static void OnDestroyPoolObject(GameObject item)
        {
            Destroy(obj: item);
        }

        private void OnDestroy()
        {
            _objectPool?.Clear();
            //_listPool?.Clear();
        }
    }
}
