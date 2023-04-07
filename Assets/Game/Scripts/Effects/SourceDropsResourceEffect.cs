using System.Collections;
using DG.Tweening;
using Game.Scripts.ResourceSystem.Entities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Scripts.Effects
{
    public class SourceDropsResourceEffect : MonoBehaviour
    {
        public float heightDrop = 2f;
        public float radiusDrop = 2f;
        public float duration = 0.5f;

        [Space]
        public ResourceSourceEntity source = null;
        
        public void AnimateSend(ResourceSourceEntity source)
        {
            for (var i = 0; i < source.profile.outputResourceAmount; i++)
            {
                var resource = Instantiate(source.profile.outputResourcePrefab);
                StartCoroutine(_Animation(resource));
            }
        }

        private IEnumerator _Animation(ResourceEntity resource)
        {
            source.transform.DOComplete();
            source.transform.DOShakeScale(duration, new Vector3(1f, 0.2f, 1f));
                
            var startPoint = transform.position + Vector3.up * heightDrop;
            var dropPoint = transform.position +
                            radiusDrop * Vector3.ProjectOnPlane(Random.onUnitSphere, Vector3.up).normalized;
            var targetScale = resource.transform.localScale;
            
            resource.collectable = false;
            resource.transform.position = startPoint;
            resource.transform.localScale = Vector3.one * 0.1f;
            
            
            resource.transform.DOJump(dropPoint, 1, 3, duration);
            resource.transform.DOShakeScale(duration);
            resource.transform.DOScale(targetScale, duration);
            
            yield return new WaitForSeconds(duration);
            
            resource.collectable = true;
        }

        private void OnEnable()
        {
            source.onResourceDropped.AddListener(AnimateSend);
        }
        
        private void OnDisable()
        {
            source.onResourceDropped.RemoveListener(AnimateSend);
        }
    }
}