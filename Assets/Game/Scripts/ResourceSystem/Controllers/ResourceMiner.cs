﻿using System.Collections;
using Game.Scripts.ResourceSystem.Entities;
using Game.Scripts.Tools;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Scripts.ResourceSystem.Controllers
{
    public class ResourceMiner : MonoBehaviour
    {
        public bool active = true;

        [Space]
        [Min(0f)]
        public float radiusMining = 2f;
        public LayerMask sourceLayers = 1;
        public QueryTriggerInteraction interaction = QueryTriggerInteraction.UseGlobal;
        
        [Space]
        public UnityEvent<ResourceSourceEntity> onSourceMined = new UnityEvent<ResourceSourceEntity>();

        private Coroutine _mining = null;
        
        private readonly ComponentCache<Collider, ResourceSourceEntity> _cache = new ComponentCache<Collider, ResourceSourceEntity>();
        private readonly Collider[] _overlap = new Collider[CountMaxOverlaps];
        
        private const ushort CountMaxOverlaps = 32;

        private IEnumerator _Mining()
        {
            while (true)
            {
                if (active)
                {
                    var center = transform.position;
                    var count = Physics.OverlapSphereNonAlloc(center, radiusMining, _overlap, sourceLayers, interaction);

                    for (var i = 0; i < count; i++)
                    {
                        if (_cache.TryGetComponent(_overlap[i], out var source))
                        {
                            yield return new WaitForSeconds(1f / source.profile.frequencyMining);
                            
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