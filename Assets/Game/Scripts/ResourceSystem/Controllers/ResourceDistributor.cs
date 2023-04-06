using System.Collections;
using System.Collections.Generic;
using Game.Scripts.ResourceSystem.Entities;
using Game.Scripts.Tools;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Scripts.ResourceSystem.Controllers
{
    public class ResourceDistributor : MonoBehaviour
    {
        public bool active = true;

        [Space]
        public UnityEvent<ResourceEntity, ResourceSpotEntity> onDistributed = new UnityEvent<ResourceEntity, ResourceSpotEntity>();

        [Space]
        public ResourceMiner miner = null;
        public ResourceInventory inventory = null;
        
        private Coroutine _distributing = null;
        
        private readonly ComponentCache<Collider, ResourceSpotEntity> _cache = new ComponentCache<Collider, ResourceSpotEntity>();
        private readonly Collider[] _overlap = new Collider[CountMaxOverlaps];
        
        private const ushort CountMaxOverlaps = 32;

        private readonly List<ResourceEntity> _hash = new List<ResourceEntity>();

        private IEnumerator _Distributing()
        {
            while (true)
            {
                if (active)
                {
                    var center = transform.position;
                    var count = Physics.OverlapSphereNonAlloc(center, miner.radiusMining, _overlap, miner.sourceLayers, miner.interaction);

                    for (var i = 0; i < count; i++)
                    {
                        if (_cache.TryGetComponent(_overlap[i], out var spot))
                        {
                            if (spot.inventory == null) continue;
                            if (spot.profile.inputResourceProfile == null) continue;

                            if (inventory.TryPick(spot.profile.inputResourceProfile.requiredResources, _hash))
                            {
                                foreach (var entity in _hash)
                                {
                                    yield return new WaitForSeconds(Random.Range(0.1f, 0.2f));
                                    onDistributed.Invoke(entity, spot);
                                }
                            }
                        }
                    }
                }

                yield return new WaitForFixedUpdate();
            }
        }

        private void OnEnable()
        {
            if (_distributing != null) StopCoroutine(_distributing);
            _distributing = StartCoroutine(_Distributing());
        }

        private void OnDisable()
        {
            if (_distributing != null) StopCoroutine(_distributing);
        }
    }
}