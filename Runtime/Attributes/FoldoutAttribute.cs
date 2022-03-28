using System;
using UnityEngine;

namespace Acciaio
{
    /// <summary>
    /// Begins an Inspector Foldout starting with the field the attribute is assigned to.
    /// The user can specify how many fields to include whether to box it or not and whether the fields are read only or not.
    /// </summary>
#if USE_NAUGHTY_ATTRIBUTES
    [Obsolete("Naughty Attributes detected. Use NaughtyAttributes.Foldout instead")]
#endif
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class FoldoutAttribute : PropertyAttribute
    {
        public readonly string Name;
        public readonly int Count;
        public readonly bool IsReadOnly;
        public readonly bool IsBoxed;

        /// <summary>
        /// Creates a Foldout that includes all the following fields up to the end or to the next Foldout.
        /// </summary>
        public FoldoutAttribute(string name) : this(name, false) { }

        /// <summary>
        /// Creates a Foldout that includes all the following fields up to the end or to the next Foldout.
        /// Setting it as read-only disables all fields in the Foldout.
        /// </summary>
        public FoldoutAttribute(string name, bool isReadonly) : this(name, isReadonly, false) { }

        /// <summary>
        /// Creates a Foldout that includes all the following fields up to the end or to the next Foldout.
        /// Setting it as read-only disables all fields in the Foldout. If isBoxed is true the fields in the
        /// Foldout will be enclosed in a GroupBox.
        /// </summary>
        public FoldoutAttribute(string name, bool isReadonly, bool isBoxed) : this (name, 0, isReadonly, isBoxed) => Count = -1;

        /// <summary>
        /// Creates a Foldout that includes the following count fields or up to the end or to the next Foldout.
        /// </summary>
        public FoldoutAttribute(string name, ushort count) : this(name, count, false) { }

        /// <summary>
        /// Creates a Foldout that includes the following count fields or up to the end or to the next Foldout.
        /// Setting it as read-only disables all fields in the Foldout.
        /// </summary>
        public FoldoutAttribute(string name, ushort count, bool isReadonly) : this(name, count, isReadonly, false) { }

        /// <summary>
        /// Creates a Foldout that includes the following count fields or up to the end or to the next Foldout.
        /// Setting it as read-only disables all fields in the Foldout. If isBoxed is true the fields in the
        /// Foldout will be enclosed in a GroupBox.
        /// </summary>
        public FoldoutAttribute(string name, ushort count, bool isReadonly, bool isBoxed)
        {
            Name = name;
            Count = count;
            IsReadOnly = isReadonly;
            IsBoxed = isBoxed;
        }
    }
}
