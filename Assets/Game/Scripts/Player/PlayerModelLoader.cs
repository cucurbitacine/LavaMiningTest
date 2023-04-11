using UnityEngine;

namespace Game.Scripts.Player
{
    [RequireComponent(typeof(PlayerController))]
    public class PlayerModelLoader : MonoBehaviour
    {
        public GameObject currentModel = null;

        [Space]
        public bool loadOnEnable = true;
        public int numberModelDefault = 0;
        public GameObject[] modelPrefabs = null;

        [Space]
        public PlayerController player = null;

        public void Load(int number)
        {
            if (number < 0 || modelPrefabs.Length <= number) number = numberModelDefault;
            
            if (currentModel != null) Destroy(currentModel);

            currentModel = Instantiate(modelPrefabs[number], player.transform, false);
        }
        
        private void Awake()
        {
            player = GetComponent<PlayerController>();
        }

        private void OnEnable()
        {
            if (loadOnEnable) Load(numberModelDefault);
        }

        private void OnValidate()
        {
            numberModelDefault = Mathf.Clamp(numberModelDefault, 0, modelPrefabs.Length - 1);
        }
    }
}