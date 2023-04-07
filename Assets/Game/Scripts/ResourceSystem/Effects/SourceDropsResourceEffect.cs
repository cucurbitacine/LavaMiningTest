using System.Collections;
using DG.Tweening;
using Game.Scripts.ResourceSystem.Entities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Scripts.ResourceSystem.Effects
{
    public class SourceDropsResourceEffect : MonoBehaviour
    {
        public float heightDrop = 2f;
        public float radiusDrop = 2f;

        [Space]
        public ResourceSourceEntity source = null;
        
        public void AnimateDrop(ResourceSourceEntity source)
        {
            for (int i = 0; i < source.profile.outputResourceAmount; i++)
            {
                var resource = Instantiate(source.profile.outputResourcePrefab);
                StartCoroutine(_Animation(resource));
            }
        }

        private IEnumerator _Animation(ResourceEntity resource)
        {
            var startPoint = transform.position + Vector3.up * heightDrop;
            var dropPoint = transform.position +
                            radiusDrop * Vector3.ProjectOnPlane(Random.onUnitSphere, Vector3.up).normalized;
            var targetScale = resource.transform.localScale;
            
            resource.collectable = false;
            resource.transform.position = startPoint;
            resource.transform.localScale = Vector3.zero;
            
            var duration = 0.5f;
            resource.transform.DOJump(dropPoint, 1, 3, duration);
            resource.transform.DOShakeScale(duration);
            resource.transform.DOScale(targetScale, duration);
            
            yield return new WaitForSeconds(duration);
            
            resource.collectable = true;
        }

        private void OnEnable()
        {
            source.onWasMined.AddListener(AnimateDrop);
        }
        
        private void OnDisable()
        {
            source.onWasMined.RemoveListener(AnimateDrop);
        }
    }
}