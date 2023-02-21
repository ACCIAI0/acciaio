using System;
using UnityEditor;
using UnityEngine;

namespace Acciaio.Editor.Extensions
{
    public static class SerializedObjectExtensions
    {
        public static SerializedId FindId(this SerializedObject sObject, string path)
        {
            var property = sObject.FindProperty(path);

            if (property is null) return null;
            if (property.type != nameof(Id) && property.type != nameof(AutoId))
            {
                Debug.LogError($"Property of type {property.type} is being extracted as IdProperty.");
                return null;
            }

            return new(property);
        }
        
        public static SerializedReferenceId FindReferenceId(this SerializedObject sObject, string path)
        {
            var refProperty = sObject.FindProperty(path);

            if (refProperty is null) return null;
            if (refProperty.type != nameof(Id) && refProperty.type != typeof(ReferenceId<>).Name)
            {
                Debug.LogError($"Property of type {refProperty.type} is being extracted as ReferenceIdProperty.");
                return null;
            }

            return new(refProperty);
        }
    }
}