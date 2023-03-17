using System;
using UnityEngine;

namespace Acciaio
{
    [Serializable]
    public class TypeReference<T>
    {
        [SerializeField]
        private string _assemblyQualifiedName;

        public Type Type => Type.GetType(_assemblyQualifiedName);

        public TypeReference(Type type)
        {
            if (!typeof(T).IsAssignableFrom(type)) 
                throw new ArgumentException($"Type {typeof(T).Name} is not assignable {type.Name}.");
            _assemblyQualifiedName = type.AssemblyQualifiedName;
        }

        public T BuildInstance()
        {
            var type = Type;
            if (type.IsSubclassOf(typeof(ScriptableObject))) 
                return (T)(object)ScriptableObject.CreateInstance(type);
            if (type.IsSubclassOf(typeof(MonoBehaviour)))
                return (T)(object)new GameObject($"Instance of {type.Name}", type).GetComponent(type);
            return (T)Activator.CreateInstance(type);
        }

        public T BuildInstance(params object[] arguments) => (T)Activator.CreateInstance(Type, arguments);

        public bool Equals(TypeReference<T> reference)
            => reference != null && reference._assemblyQualifiedName.Equals(_assemblyQualifiedName, StringComparison.Ordinal);

        public override bool Equals(object other) => other is TypeReference<T> reference && Equals(reference);

        // ReSharper disable once NonReadonlyMemberInGetHashCode
        public override int GetHashCode() => _assemblyQualifiedName.GetHashCode();

        public override string ToString() => $"Reference<{Type}>";
    }
}
