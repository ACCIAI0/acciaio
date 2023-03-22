using System;
using UnityEngine;

namespace Acciaio
{
    [Serializable]
    public sealed class SceneReference
    {
#if UNITY_EDITOR
        [SerializeField]
        private string _assetGuid;
#endif
        
        [SerializeField]
        private string _path;
        
#if USE_ADDRESSABLES
        [field: SerializeField]
        public bool IsAddressable { get; private set; }
#endif
        
#if UNITY_EDITOR
        [field: SerializeField]
        public bool IsEditorOverrideable { get; private set; }
#endif

        public string Path
        {
            get
            {
#if UNITY_EDITOR
                EditorReferenceConsistencyCheck();
#endif
                return _path;
            }
        }

        public SceneReference(string path) => _path = path;
        
#if USE_ADDRESSABLES
        public SceneReference(string path, bool isAddressable) : this(path) => IsAddressable = isAddressable;
#endif

#if UNITY_EDITOR
        private void EditorReferenceConsistencyCheck()
        {
            if (string.IsNullOrEmpty(_assetGuid)) return;

            var assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(_assetGuid);

            if (assetPath.Equals(_path, StringComparison.Ordinal)) return;

            _path = assetPath;
            
            Debug.LogWarning($"A scene reference changed but was not explicitly updated: {assetPath}");
        }
#endif

        public override bool Equals(object other)
        {
            if (ReferenceEquals(this, other)) return true;

            if (other is not SceneReference otherRef) return false;
            
            var result = otherRef.Path.Equals(Path, StringComparison.Ordinal);
#if USE_ADDRESSABLES
            result &= IsAddressable == otherRef.IsAddressable;
#endif
            return result;
        }

        public override int GetHashCode()
        {
            // ReSharper disable NonReadonlyMemberInGetHashCode
#if USE_ADDRESSABLES
            return HashCode.Combine(Path.GetHashCode(), IsAddressable.GetHashCode());
#else
            return Path.GetHashCode();
#endif
            // ReSharper restore NonReadonlyMemberInGetHashCode
        }
    }
}
