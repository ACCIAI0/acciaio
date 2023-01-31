using System;
using System.Diagnostics;
using UnityEngine;

namespace Acciaio
{
	[Conditional("UNITY_EDITOR")]
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public sealed class SceneAttribute : PropertyAttribute
	{
		private const string DefaultNoneLabel = "(None)";
        
		public bool ShowNoneOption { get; }
		public string NoneOptionLabel { get; }
        
		private SceneAttribute(bool showNoneOption, string noneOptionLabel)
		{
			ShowNoneOption = showNoneOption;
			NoneOptionLabel = noneOptionLabel;
		}

		public SceneAttribute() : this(true, DefaultNoneLabel) { }

		public SceneAttribute(bool showNoneOption) : this(showNoneOption, DefaultNoneLabel) { }

		public SceneAttribute(string noneOptionLabel) : this(true, noneOptionLabel) { }
	}
}