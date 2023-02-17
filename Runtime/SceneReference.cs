using System;
using UnityEngine;

namespace Acciaio
{
    [Serializable]
    public sealed class SceneReference
    {
#if USE_ADDRESSABLES
        [field: SerializeField]
        public bool IsAddressable { get; private set; }
#endif
        
#if UNITY_EDITOR
        [field: SerializeField]
        public bool IsEditorOverrideable { get; private set; }
#endif

        [field: SerializeField]
        public string Path { get; private set; }

        public SceneReference(string path) => Path = path;
        
#if USE_ADDRESSABLES
        public SceneReference(string path, bool isAddressable) : this(path) => IsAddressable = isAddressable;
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
            return Name.GetHashCode();
#endif
            // ReSharper restore NonReadonlyMemberInGetHashCode
        }
    }
}
