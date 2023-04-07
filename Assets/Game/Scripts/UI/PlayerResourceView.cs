using Game.Scripts.ResourceSystem.Profiles;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.Scripts.UI
{
    public class PlayerResourceView : MonoBehaviour
    {
        [Min(0)]
        public int amount = 0;
        public ResourceProfile profile = null;

        [Space]
        public UnityEvent<int> onAmountChanged = new UnityEvent<int>();

        [Space]
        public Image iconImage = null;
        public TextMeshProUGUI amountText = null;

        public void Initiate(ResourceProfile initProfile, int initAmount = 0)
        {
            profile = initProfile;
            
            if (iconImage != null && profile.icon != null) iconImage.sprite = initProfile.icon;

            amount = initAmount;
            
            onAmountChanged.Invoke(amount);
        }
        
        public int Increase()
        {
            amount++;

            onAmountChanged.Invoke(amount);
            
            return amount;
        }
        
        public int Decrease()
        {
            if (amount == 0) return 0;

            amount--;
            
            onAmountChanged.Invoke(amount);
            
            return amount;
        }

        public void UpdateView(int value)
        {
            if (amountText != null) amountText.text = value.ToString("D");
        }
        
        private void OnEnable()
        {
            onAmountChanged.AddListener(UpdateView);
        }
        
        private void OnDisable()
        {
            onAmountChanged.RemoveListener(UpdateView);
        }
    }
}