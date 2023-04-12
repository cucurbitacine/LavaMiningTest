using System.Collections;
using Game.Scripts.Player;
using Game.Scripts.ResourceSystem.Entities;
using Game.Scripts.Tools;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Scripts.ResourceSystem.Controllers
{
    /// <summary>
    /// Controller of mining. Mining of resources from Sources
    /// </summary>
    public class MinerController : MonoBehaviour
    {
        public bool active = true;
        public bool mining = false;
        
        [Space]
        [Min(0f)]
        public float radiusMining = 2f;
        public LayerMask sourceLayers = 1;
        public QueryTriggerInteraction interaction = QueryTriggerInteraction.UseGlobal;
        
        [Space]
        public UnityEvent<SourceBehaviour> onSourceMined = new UnityEvent<SourceBehaviour>();

        [Space]
        public PlayerController player = null;
        
        private Coroutine _mining = null;
        
        /// <summary>
        /// Source cache. Used to reduce the number of calls "GetComponent"
        /// </summary>
        private readonly ComponentCache<Collider, SourceBehaviour> _sourceCache = new ComponentCache<Collider, SourceBehaviour>();
        private readonly Collider[] _overlap = new Collider[CountMaxOverlaps];
        
        private const ushort CountMaxOverlaps = 32;

        private IEnumerator _Mining()
        {
            // waiting player
            while (player == null)
            {
                yield return null;
            }
            
            while (true)
            {
                mining = false;
                
                // mining if active and player is not moving
                if (active && !player.moving)
                {
                    // make overlap
                    var center = transform.position;
                    var count = Physics.OverlapSphereNonAlloc(center, radiusMining, _overlap, sourceLayers, interaction);

                    for (var i = 0; i < count; i++)
                    {
                        // try get source by its collider
                        if (_sourceCache.TryGetComponent(_overlap[i], out var source))
                        {
                            // skip if source is recovering resources
                            if (source.recovering) continue;
                            
                            mining = true;
                            
                            // rotating player to direction to source 
                            player.View(source.transform.position - player.transform.position);
                            
                            // MINE!
                            if (source.Mine())
                            {
                                // if mining is success - invoke and break, because miner can mining only one source
                                onSourceMined.Invoke(source);
                                break;
                            }
                        }
                    }
                }

                yield return new WaitForFixedUpdate();
            }
        }
        
        private void OnEnable()
        {
            if (_mining != null) StopCoroutine(_mining);
            _mining = StartCoroutine(_Mining());
        }

        private void OnDisable()
        {
            if (_mining != null) StopCoroutine(_mining);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, radiusMining);
        }
    }
}