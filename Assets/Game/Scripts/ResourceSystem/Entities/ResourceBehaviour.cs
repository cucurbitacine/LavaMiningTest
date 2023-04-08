using Game.Scripts.ResourceSystem.Profiles;
using UnityEngine;

namespace Game.Scripts.ResourceSystem.Entities
{
    public class ResourceBehaviour : MonoBehaviour
    {
        public ResourceProfile profile = null;

        [Space]
        public bool collectable = false;
    }
}