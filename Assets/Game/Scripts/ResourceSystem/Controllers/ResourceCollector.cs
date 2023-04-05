using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Game.Scripts.ResourceSystem.Entities;
using Game.Scripts.Tools;
using UnityEngine;

namespace Game.Scripts.ResourceSystem.Controllers
{
    [RequireComponent(typeof(SphereCollider))]
    public class ResourceCollector : MonoBehaviour
    {
        [Min(0f)]
        public float radiusCollecting = 3f;

        [Space]
        public ResourceInventory inventory = null;
        public List<ResourceEntity> resources = new List<ResourceEntity>();

        private  SphereCollider _sphereCollect = null;
        
        private readonly ComponentCache<Collider, ResourceEntity> _cache = new ComponentCache<Collider, ResourceEntity>();
        
        public void Collect(ResourceEntity resource)
        {
            // Collect
            resource.collectable = false;
            StartCoroutine(_Collecting(resource));
        }

        public float duration = 3f;
        
        private IEnumerator _Collecting(ResourceEntity resource)
        {
            yield return null;
            
            resources.Remove(resource);

            var delay = duration;

            var trg = resource.transform;

            trg.DOJump(trg.position, 1, 2, delay);
            trg.DOShakeScale(delay);
            trg.DOShakeRotation(delay);
            
            yield return new WaitForSeconds(delay);
            
            inventory.Put(resource);
        }
        
        private void Awake()
        {
            if (inventory == null) inventory = GetComponent<ResourceInventory>();
            
            _sphereCollect = GetComponent<SphereCollider>();
        }

        private void Update()
        {
            foreach (var resource in resources)
            {
                if (resource.collectable)
                {
                    Collect(resource);
                }
            }
        }

        private void OnValidate()
        {
            if (_sphereCollect == null) _sphereCollect = GetComponent<SphereCollider>();
            if (_sphereCollect != null) _sphereCollect.radius = radiusCollecting;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (_cache.TryGetComponent(other, out var resource))
            {
                resources.Add(resource);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (_cache.TryGetComponent(other, out var resource))
            {
                resources.Remove(resource);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, radiusCollecting);
        }
    }
}