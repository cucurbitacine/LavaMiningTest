using Game.Scripts.ResourceSystem.Entities;
using Game.Scripts.Tools;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Scripts.ResourceSystem.Controllers
{
    public class ResourceCollector : MonoBehaviour
    {
        public bool active = true;
        
        [Space]
        [Min(0f)]
        public float radiusCollecting = 3f;
        public LayerMask resourceLayers = 1;
        public QueryTriggerInteraction interaction = QueryTriggerInteraction.UseGlobal;
        [Space]
        public UnityEvent<ResourceEntity> onResourceCollected = new UnityEvent<ResourceEntity>();
        
        private readonly ComponentCache<Collider, ResourceEntity> _cache = new ComponentCache<Collider, ResourceEntity>();
        private readonly Collider[] _overlap = new Collider[CountMaxOverlaps];

        private const ushort CountMaxOverlaps = 32;
        
        public void Collect(ResourceEntity resource)
        {
            resource.collectable = false;
            
            onResourceCollected.Invoke(resource);
        }
        
        private void Collecting()
        {
            var center = transform.position;
            var count = Physics.OverlapSphereNonAlloc(center, radiusCollecting, _overlap, resourceLayers, interaction);

            for (var i = 0; i < count; i++)
            {
                if (_cache.TryGetComponent(_overlap[i], out var res))
                {
                    if (res.collectable)
                    {
                        Collect(res);
                    }
                }
            }
        }
        
        private void FixedUpdate()
        {
            if (active) Collecting();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, radiusCollecting);
        }
    }
}