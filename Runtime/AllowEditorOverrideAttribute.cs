using System;
using System.Diagnostics;
using Codice.CM.Common;
using UnityEngine;

namespace Acciaio
{
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Field)]
    public class AllowEditorOverrideAttribute : PropertyAttribute { }
}
