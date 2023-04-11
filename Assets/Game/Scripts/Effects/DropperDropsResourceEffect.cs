using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Game.Scripts.ResourceSystem.Entities;
using Game.Scripts.ResourceSystem.Profiles;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Scripts.Effects
{
    public class DropperDropsResourceEffect : MonoBehaviour
    {
        public float shakeDuration = 0.5f;
        public Vector3 shakeScalePower = new Vector3(1f, 0.2f, 1f);

        [Space]
        public Vector3 localPointDropZone = Vector3.zero;
        [Min(0f)]
        public float innerRadiusDropZone = 0f;
        [Min(0f)]
        public float outerRadiusDropZone = 1f;
        
        [Space]
        public ResourceEffectProfile effectDefault = null;
        
        [Space]
        public DropperBehaviour dropper = null;
        
        public void AnimateDrop(List<ResourceBehaviour> resources)
        {
            StartCoroutine(_Animation(resources));
        }

        private Vector3 GetDropZonePoint()
        {
            var dir = Random.insideUnitCircle;
            var radius = Random.value * (outerRadiusDropZone - innerRadiusDropZone) + innerRadiusDropZone;
            var scatter = new Vector3(dir.x, 0f, dir.y).normalized * radius;

            return dropper.transform.position + localPointDropZone + scatter;
        }
        
        private IEnumerator _Animation(List<ResourceBehaviour> resources)
        {
            // Dropper shaking
            
            var dTrans = dropper.transform;
            
            dTrans.DOComplete();
            dTrans.DOShakeScale(shakeDuration, shakeScalePower);

            resources.ForEach(r => r.gameObject.SetActive(false));
            
            foreach (var resource in resources)
            {
                resource.gameObject.SetActive(true);
                
                // Resource dropping

                var effect = resource.profile.effect;
                if (effect == null) effect = effectDefault;
            
                var rTrans = resource.transform;

                var startDrop = effect.GetDropStart(dTrans.position);
                var targetDrop = effect.GetDropTarget(dTrans.position);

                rTrans.position = startDrop;
                rTrans.DOShakeScale(effect.dropDuration, effect.dropShakeScalePower);
                rTrans.DOMove(targetDrop, effect.dropDuration).OnComplete(() =>
                {
                    var targetDropZone = GetDropZonePoint();
                    rTrans.DOMove(targetDropZone, effect.dropFlyDuration).OnComplete(() =>
                    {
                        resource.Collectable(true, effect.dropCollectableTimeout);
                    });
                });

                yield return new WaitForSeconds(dropper.GetProfile().timeoutDropping);
            }
        }

        private void Awake()
        {
            if (dropper == null) dropper = GetComponent<DropperBehaviour>();

            if (effectDefault == null) effectDefault = ScriptableObject.CreateInstance<ResourceEffectProfile>();
        }

        private void OnEnable()
        {
            dropper.onResourceDropped.AddListener(AnimateDrop);
        }
        
        private void OnDisable()
        {
            dropper.onResourceDropped.RemoveListener(AnimateDrop);
        }

        private void OnDrawGizmos()
        {
            if (dropper != null)
            {
                var pointDropZone = dropper.transform.TransformPoint(localPointDropZone);
                Gizmos.DrawWireSphere(pointDropZone, innerRadiusDropZone);
                Gizmos.DrawWireSphere(pointDropZone, outerRadiusDropZone);
            }
            
            
        }
    }
}