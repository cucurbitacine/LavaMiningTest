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
    /// <summary>
    /// Controller of distributing resources from player's inventory to spot's inventory 
    /// </summary>
    public class DistributorController : MonoBehaviour
    {
        public bool active = true;

        [Space]
        [Min(0f)]
        public float timeoutDistributing = 0.15f;
        
        [Space]
        public UnityEvent<ResourceBehaviour, SpotBehaviour> onDistributed = new UnityEvent<ResourceBehaviour, SpotBehaviour>();

        [Space]
        public PlayerController player = null;
        
        private Coroutine _distributing = null;
        
        /// <summary>
        /// Spot cache. Used to reduce the number of calls "GetComponent"
        /// </summary>
        private readonly ComponentCache<Collider, SpotBehaviour> _spotCache = new ComponentCache<Collider, SpotBehaviour>();
        private readonly Collider[] _overlap = new Collider[CountMaxOverlaps];
        
        private const ushort CountMaxOverlaps = 32;

        private List<ResourceStack> _cacheRequired = new List<ResourceStack>();

        public MinerController playerMiner => player?.miner;
        public InventoryController playerInventory => player?.inventory;
        
        private IEnumerator _Distributing()
        {
            // waiting miner and inventory
            while (player == null || playerMiner == null || playerInventory == null)
            {
                yield return null;
            }
            
            while (true)
            {
                // work only if active and player is not moving
                if (active && !player.moving)
                {
                    // make overlap
                    var center = transform.position;
                    var count = Physics.OverlapSphereNonAlloc(center, playerMiner.radiusMining, _overlap, playerMiner.sourceLayers, playerMiner.interaction);

                    for (var i = 0; i < count; i++)
                    {
                        // try get spot by its collider
                        if (_spotCache.TryGetComponent(_overlap[i], out var spot))
                        {
                            // if spot is busy - skip
                            if (spot.producing) continue;
                            
                            // get its required resource
                            spot.FillRequired(ref _cacheRequired);

                            // looking through its required
                            foreach (var required in _cacheRequired)
                            {
                                for (var j = 0; j < required.amount; j++)
                                {
                                    // try pick resources from player's inventory 
                                    if (playerInventory.TryPick(required.profile, out var resource))
                                    {
                                        // put picked resource to spot's inventory 
                                        spot.inventory.Put(resource);
                                        
                                        onDistributed.Invoke(resource, spot);
                                        
                                        yield return new WaitForSeconds(timeoutDistributing);
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