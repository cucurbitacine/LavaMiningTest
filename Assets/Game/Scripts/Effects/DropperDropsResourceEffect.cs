using System.Collections;
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
        
        public void AnimateDrop(ResourceBehaviour resource)
        {
            StartCoroutine(_Animation(Instantiate(resource)));
        }

        private Vector3 GetDropZonePoint()
        {
            var dir = Random.insideUnitCircle;
            var radius = Random.value * (outerRadiusDropZone - innerRadiusDropZone) + innerRadiusDropZone;
            var scatter = new Vector3(dir.x, 0f, dir.y) * radius;
            
            return dropper.transform.TransformPoint(localPointDropZone) + scatter; 
        }
        
        private IEnumerator _Animation(ResourceBehaviour resource)
        {
            var effect = resource.profile.effect;
            if (effect == null) effect = effectDefault;

            // Dropper shaking
            
            var dTrans = dropper.transform;
            
            dTrans.DOComplete();
            dTrans.DOShakeScale(shakeDuration, shakeScalePower);
            
            // Resource dropping
            
            var rTrans = resource.transform;

            var startDrop = effect.GetDropStart(dTrans.position);
            var targetDrop = effect.GetDropTarget(dTrans.position);

            rTrans.position = startDrop;
            rTrans.DOMove(targetDrop, effect.dropDuration);
            rTrans.DOShakeScale(effect.dropDuration, effect.dropShakeScalePower);

            yield return new WaitForSeconds(effect.dropDuration);
            
            // Resource flying

            var targetDropZone = GetDropZonePoint();

            rTrans.DOMove(targetDropZone, effect.dropFlyDuration);
            
            yield return new WaitForSeconds(effect.dropFlyDuration);
            
            yield return new WaitForSeconds(effect.dropIdleTimeout);
            
            resource.collectable = true;
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