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
        public DropperBehaviour dropper = null;
        
        public void AnimateSend(ResourceBehaviour resource)
        {
            StartCoroutine(_Animation(Instantiate(resource)));
        }

        private IEnumerator _Animation(ResourceBehaviour resource)
        {
            dropper.transform.DOComplete();
            dropper.transform.DOShakeScale(duration, new Vector3(1f, 0.2f, 1f));
                
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

        private void Awake()
        {
            if (dropper == null) dropper = GetComponent<DropperBehaviour>();
        }

        private void OnEnable()
        {
            dropper.onResourceDropped.AddListener(AnimateSend);
        }
        
        private void OnDisable()
        {
            dropper.onResourceDropped.RemoveListener(AnimateSend);
        }
    }
}