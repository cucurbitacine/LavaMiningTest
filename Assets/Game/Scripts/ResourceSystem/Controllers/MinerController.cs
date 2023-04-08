using System.Collections;
using Game.Scripts.Player;
using Game.Scripts.ResourceSystem.Entities;
using Game.Scripts.Tools;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Scripts.ResourceSystem.Controllers
{
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
        
        private readonly ComponentCache<Collider, SourceBehaviour> _sourceCache = new ComponentCache<Collider, SourceBehaviour>();
        private readonly Collider[] _overlap = new Collider[CountMaxOverlaps];
        
        private const ushort CountMaxOverlaps = 32;

        private IEnumerator _Mining()
        {
            while (true)
            {
                mining = false;
                
                if (active && !player.moving)
                {
                    var center = transform.position;
                    var count = Physics.OverlapSphereNonAlloc(center, radiusMining, _overlap, sourceLayers, interaction);

                    for (var i = 0; i < count; i++)
                    {
                        if (_sourceCache.TryGetComponent(_overlap[i], out var source))
                        {
                            if (source.recovering) continue;
                            
                            mining = true;
                            
                            player.View(source.transform.position - player.transform.position);
                            
                            if (source.waiting) continue;

                            if (source.Mine())
                            {
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