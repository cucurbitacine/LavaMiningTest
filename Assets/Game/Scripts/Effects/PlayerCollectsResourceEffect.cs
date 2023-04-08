using System.Collections;
using DG.Tweening;
using Game.Scripts.ResourceSystem.Controllers;
using Game.Scripts.ResourceSystem.Entities;
using Game.Scripts.ResourceSystem.Profiles;
using UnityEngine;

namespace Game.Scripts.Effects
{
    public class PlayerCollectsResourceEffect : MonoBehaviour
    {
        public InventoryController inventory = null;
        
        [Space]
        public ResourceEffectProfile effectDefault = null;
        
        public void AnimateCollect(ResourceBehaviour resource)
        {
            StartCoroutine(_Animation(resource));
        }

        private IEnumerator _Animation(ResourceBehaviour resource)
        {
            var effect = resource.profile.effect;
            if (effect == null) effect = effectDefault;
            
            var rTrans = resource.transform;
            
            rTrans.DOComplete();
            rTrans.DOMove(inventory.transform.position, effect.collectDuration);

            yield return new WaitForSeconds(effect.collectDuration);
            
            resource.gameObject.SetActive(false);
        }

        private void Awake()
        {
            if (effectDefault == null) effectDefault = ScriptableObject.CreateInstance<ResourceEffectProfile>();
        }

        private void OnEnable()
        {
            inventory.onPutted.AddListener(AnimateCollect);
        }

        private void OnDisable()
        {
            inventory.onPutted.RemoveListener(AnimateCollect);
        }
    }
}