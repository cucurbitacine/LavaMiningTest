using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Tools
{

    /*
     * Component cache.
     *
     * Useful when you need to very often search for some component by another component.
     * It decreases calls of "GetComponent<...>()", because first it will search component in the cache
     * If it is not contains in the cache, will be called "GetComponent<...>()" and will be saved to the cache
     * 
     */
    
    public class ComponentCache<TWhere, TWho>
        where TWhere : Component
        where TWho : Component
    {
        private readonly Dictionary<TWhere, TWho> _cache = new Dictionary<TWhere, TWho>();

        public bool TryGetComponent(TWhere where, out TWho who)
        {
            if (!_cache.TryGetValue(where, out who))
            {
                if (!where.TryGetComponent<TWho>(out who))
                {
                    who = null;
                }

                _cache.Add(where, who);
            }

            return who != null;
        }

        public bool TryAdd(TWhere where, TWho who)
        {
            return _cache.TryAdd(where, who);
        }
        
        public bool Remove(TWhere where)
        {
            return _cache.Remove(where);
        }
    }
}