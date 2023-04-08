using System;
using UnityEngine;

namespace Game.Scripts.Tools
{
    public abstract class ScriptableObjectWithGuid : ScriptableObject
    {
        public Guid Guid
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_guid) || !Guid.TryParse(_guid, out var guid))
                {
                    guid = Guid.NewGuid();
                    Guid = guid;
                }

                return guid;
            }
            private set => _guid = value.ToString();
        }
        
        private string _guid = string.Empty;
        [SerializeField] private string displayGuid = string.Empty;

        [ContextMenu("Update Guid")]
        protected void UpdateGuid()
        {
            Guid = Guid.NewGuid();
            
            DisplayGuid();
        }

        private void DisplayGuid()
        {
            displayGuid = Guid.ToString();
        }

        protected virtual void Awake()
        {
            DisplayGuid();
        }
        
        protected virtual void OnValidate()
        {
            DisplayGuid();
        }
    }
}