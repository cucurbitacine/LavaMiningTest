﻿using System.Collections;
using DG.Tweening;
using Game.Scripts.ResourceSystem.Controllers;
using Game.Scripts.ResourceSystem.Entities;
using UnityEngine;

namespace Game.Scripts.ResourceSystem.Effects
{
    public class PlayerPutsResourceToInventoryEffect : MonoBehaviour
    {
        public ResourceInventory inventory = null;

        public void AnimatePickUp(ResourceEntity resource)
        {
            StartCoroutine(_Animation(resource));
        }

        private IEnumerator _Animation(ResourceEntity resource)
        {
            resource.collectable = false;

            var startScale = resource.transform.localScale;

            var duration = 1f;

            resource.transform.DOJump(transform.position, 1, 3, duration);
            resource.transform.DOShakeScale(duration);
            resource.transform.DOScale(Vector3.zero, duration);

            yield return new WaitForSeconds(duration);

            resource.transform.localScale = startScale;
            resource.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            inventory.onPutted.AddListener(AnimatePickUp);
        }

        private void OnDisable()
        {
            inventory.onPutted.RemoveListener(AnimatePickUp);
        }
    }
}