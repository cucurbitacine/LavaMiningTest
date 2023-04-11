using System.Collections;
using Game.Scripts.ResourceSystem.Profiles;
using UnityEngine;

namespace Game.Scripts.ResourceSystem.Entities
{
    public class ResourceBehaviour : MonoBehaviour
    {
        public ResourceProfile profile = null;

        [Space]
        public bool collectable = false;

        private Coroutine _collectable = null;
        
        public void Collectable(bool value, float delayInSeconds = 0f)
        {
            if (_collectable != null) StopCoroutine(_collectable);
            _collectable = StartCoroutine(_Collectable(value, delayInSeconds));
        }

        private IEnumerator _Collectable(bool value, float delayInSeconds)
        {
            yield return new WaitForSeconds(delayInSeconds);
            collectable = value;
        }
    }
}