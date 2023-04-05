using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Tools
{
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