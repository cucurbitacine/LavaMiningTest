using System.Collections;
using System.Collections.Generic;
using Game.Scripts.Player;
using Game.Scripts.ResourceSystem.Entities;
using Game.Scripts.ResourceSystem.Profiles;
using Game.Scripts.Tools;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Scripts.ResourceSystem.Controllers
{
    public class DistributorController : MonoBehaviour
    {
        public bool active = true;

        [Space]
        public UnityEvent<ResourceBehaviour, SpotBehaviour> onDistributed = new UnityEvent<ResourceBehaviour, SpotBehaviour>();

        [Space]
        public PlayerController player = null;
        
        private Coroutine _distributing = null;
        
        private readonly ComponentCache<Collider, SpotBehaviour> _spotCache = new ComponentCache<Collider, SpotBehaviour>();
        private readonly Collider[] _overlap = new Collider[CountMaxOverlaps];
        
        private const ushort CountMaxOverlaps = 32;

        private List<ResourceStack> _cacheRequired = new List<ResourceStack>();
        private List<ResourceBehaviour> _cacheResources = new List<ResourceBehaviour>();

        public MinerController miner => player.miner;
        public InventoryController inventory => player.inventory;
        
        private IEnumerator _Distributing()
        {
            while (miner == null || inventory == null)
            {
                yield return null;
            }
            
            while (true)
            {
                if (active && !miner.player.moving)
                {
                    var center = transform.position;
                    var count = Physics.OverlapSphereNonAlloc(center, miner.radiusMining, _overlap, miner.sourceLayers, miner.interaction);

                    for (var i = 0; i < count; i++)
                    {
                        if (_spotCache.TryGetComponent(_overlap[i], out var spot))
                        {
                            if (spot.producting) continue;
                            
                            spot.FillRequired(ref _cacheRequired);

                            foreach (var required in _cacheRequired)
                            {
                                for (var j = 0; j < required.amount; j++)
                                {
                                    if (inventory.TryPick(required.profile, out var resource))
                                    {
                                        spot.inventory.Put(resource);
                                        
                                        onDistributed.Invoke(resource, spot);
                                        
                                        yield return new WaitForSeconds(Random.Range(0.1f, 0.2f));
                                    }
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