using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Pool;
using F32 = System.Single;

namespace DeathRunner
{
    public class PawnAfterImages : MonoBehaviour
    {
        [SerializeField] private F32 distanceToSpawnNewMesh = 1.0f;

        [SerializeField] private Material mat;

        [SerializeField] private SkinnedMeshRenderer[] characterSkinnedMeshes;
        [SerializeField] private MeshFilter[]          characterMeshes;

        private IObjectPool<GameObject> _objectPool;

        private List<GameObject> _spawnedObjects = new();

        private Vector3 _previousSpawnPoint;

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

        private void Start()
        {
            _objectPool = new ObjectPool<GameObject>(createFunc: CreatePooledItem, actionOnGet: null, actionOnRelease: OnReturnedToPool, actionOnDestroy: OnDestroyPoolObject);
            _previousSpawnPoint = transform.position;
        }

        private void Update()
        {
            F32 __distanceSinceLastSpawn = Vector3.Distance(a: _previousSpawnPoint, b: transform.position);

            if (__distanceSinceLastSpawn < distanceToSpawnNewMesh) return;

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
            GameObject __obj = _objectPool.Get();
            __obj.transform.SetPositionAndRotation(position: transform.position, rotation: transform.rotation);
            MeshRenderer __renderer = __obj.GetComponent<MeshRenderer>();
            MeshFilter   __filter   = __obj.GetComponent<MeshFilter>();
            __obj.name = skinnedMeshRenderer.name + " AfterImage";

            Mesh __mesh = new();
            skinnedMeshRenderer.BakeMesh(mesh: __mesh);

            __filter.mesh = __mesh;
            __renderer.material = mat;

            _spawnedObjects.Add(__obj);
        }

        private void CreateCopyOfMesh(MeshFilter meshFilter)
        {
            GameObject __obj = _objectPool.Get();
            Transform __meshFilterTransform = meshFilter.transform;
            __obj.transform.SetPositionAndRotation(position: __meshFilterTransform.position, rotation: __meshFilterTransform.rotation);
            MeshRenderer __renderer = __obj.GetComponent<MeshRenderer>();
            MeshFilter   __filter   = __obj.GetComponent<MeshFilter>();
            __obj.name = meshFilter.name + " AfterImage";
            
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
        
        [Button]
        private void ReleaseAllSpawnedObjects()
        {
            foreach (GameObject __spawnedObject in _spawnedObjects)
            {
                _objectPool.Release(__spawnedObject);
            }
            _spawnedObjects.Clear();
        }

        private void OnDestroy()
        {
            _objectPool?.Clear();
            //_listPool?.Clear();
        }
    }
}
