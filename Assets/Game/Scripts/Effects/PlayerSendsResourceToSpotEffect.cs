using System.Collections;
using DG.Tweening;
using Game.Scripts.ResourceSystem.Controllers;
using Game.Scripts.ResourceSystem.Entities;
using Game.Scripts.ResourceSystem.Profiles;
using UnityEngine;

namespace Game.Scripts.Effects
{
    public class PlayerSendsResourceToSpotEffect : MonoBehaviour
    {
        [Space]
        public DistributorController distributor = null;
        
        [Space]
        public ResourceEffectProfile effectDefault = null;
        
        public void AnimationSend(ResourceBehaviour resource, SpotBehaviour spot)
        {
            if (resource == null || spot == null) return;
            
            StartCoroutine(_Animation(resource, spot));
        }

        private IEnumerator _Animation(ResourceBehaviour resource, SpotBehaviour spot)
        {
            var effect = resource.profile.effect;
            if (effect == null) effect = effectDefault;
            
            resource.gameObject.SetActive(true);
            
            // Resource dropping

            var initPosition = distributor.transform.position;
            
            var rTrans = resource.transform;

            var startDrop = effect.GetDropStart(initPosition);
            var targetDrop = effect.GetDropTarget(initPosition);

            rTrans.position = startDrop;
            rTrans.DOMove(targetDrop, effect.dropDuration);
            rTrans.DOShakeScale(effect.dropDuration, effect.dropShakeScalePower);

            yield return new WaitForSeconds(effect.dropDuration);
            
            // Resource flying

            var targetDropZone = spot.transform.position;

            rTrans.DOMove(targetDropZone, effect.dropFlyDuration);
            
            yield return new WaitForSeconds(effect.dropFlyDuration);
            
            resource.gameObject.SetActive(false);
        }

        private void Awake()
        {
            if (effectDefault == null) effectDefault = ScriptableObject.CreateInstance<ResourceEffectProfile>();
        }

        private void OnEnable()
        {
            if (distributor != null) distributor.onDistributed.AddListener(AnimationSend);
        }
        
        private void OnDisable()
        {
            if (distributor != null) distributor.onDistributed.RemoveListener(AnimationSend);
        }
    }
}