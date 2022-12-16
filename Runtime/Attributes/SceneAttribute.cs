using System;
using System.Diagnostics;
using UnityEngine;

namespace Acciaio
{
	[Conditional("UNITY_EDITOR")]
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public sealed class SceneAttribute : PropertyAttribute
	{
		private const string DEFAULT_NONE_LABEL = "(None)";
        
		public bool ShowNoneOption { get; }
		public string NoneOptionLabel { get; }

		public SceneAttribute() : this(true, DEFAULT_NONE_LABEL) { }
		public SceneAttribute(bool showNoneOption) : this(showNoneOption, DEFAULT_NONE_LABEL) { }
		public SceneAttribute(string noneOptionLabel) : this(true, noneOptionLabel) { }
		private SceneAttribute(bool showNoneOption, string noneOptionLabel)
		{
			ShowNoneOption = showNoneOption;
			NoneOptionLabel = noneOptionLabel;
		}
	}
}