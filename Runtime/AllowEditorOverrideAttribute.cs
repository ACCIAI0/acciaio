using System;
using System.Diagnostics;
using UnityEngine;

namespace Acciaio
{
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Field)]
    public class AllowEditorOverrideAttribute : PropertyAttribute { }
}
