using Game.Scripts.ResourceSystem.Entities;
using Game.Scripts.Tools;
using UnityEngine;

namespace Game.Scripts.ResourceSystem.Controllers
{
    [RequireComponent(typeof(SphereCollider))]
    public class ResourceMiner : MonoBehaviour
    {
        [Min(0f)]
        public float radiusMining = 2f;
        
        private  SphereCollider _sphereMining = null;
        
        private readonly ComponentCache<Collider, ResourceSourceEntity> _cache = new ComponentCache<Collider, ResourceSourceEntity>();

        public ResourceSourceEntity currentSource = null;

        public void StartMining(ResourceSourceEntity source)
        {
            currentSource = source;
        }
        
        public void StopMining()
        {
            currentSource = null;
        }
        
        private void Awake()
        {
            _sphereMining = GetComponent<SphereCollider>();
        }

        private void OnValidate()
        {
            if (_sphereMining == null) _sphereMining = GetComponent<SphereCollider>();
            if (_sphereMining != null) _sphereMining.radius = radiusMining;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (currentSource == null && _cache.TryGetComponent(other, out var source))
            {
                StartMining(source);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (_cache.TryGetComponent(other, out var source))
            {
                if (source == currentSource) StopMining();
            }
        }
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, radiusMining);
        }
    }
}