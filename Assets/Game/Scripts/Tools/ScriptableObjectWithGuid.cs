using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game.Scripts.Tools
{
    public abstract class ScriptableObjectWithGuid : ScriptableObject
    {
        [StringReadOnly]
        [SerializeField] private string _guid = string.Empty;

        public Guid Guid
        {
            get => Guid.TryParse(_guid, out var guid) ? guid : (Guid = Guid.NewGuid());
            private set
            {
                _guid = value.ToString();
#if UNITY_EDITOR
                EditorUtility.SetDirty(this);
#endif
            }
        }

        [ContextMenu("Update Guid")]
        private void UpdateGuid()
        {
            Guid = Guid.NewGuid();
        }
        
        private void Validate()
        {
            if (Guid == Guid.Empty) UpdateGuid();
        }

        protected virtual void Awake()
        {
            Validate();
        }

        protected virtual void Reset()
        {
            Validate();
        }

        protected virtual void OnValidate()
        {
            Validate();
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class StringReadOnlyAttribute : PropertyAttribute
    {
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(StringReadOnlyAttribute))]
    public class StringReadOnlyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            string labelStr = $"{label.text} (read only)";

            switch (prop.propertyType)
            {
                case SerializedPropertyType.String:
                    EditorGUI.TextField(position, labelStr, prop.stringValue);
                    break;
                default:
                    EditorGUI.ObjectField(position, prop);
                    break;
            }
        }
    }
#endif
}