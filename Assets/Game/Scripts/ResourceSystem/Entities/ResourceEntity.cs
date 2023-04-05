using Game.Scripts.ResourceSystem.Profiles;
using UnityEngine;

namespace Game.Scripts.ResourceSystem.Entities
{
    public class ResourceEntity : MonoBehaviour
    {
        public ResourceProfile profile = null;

        [Space]
        public bool collectable = false;
    }
}