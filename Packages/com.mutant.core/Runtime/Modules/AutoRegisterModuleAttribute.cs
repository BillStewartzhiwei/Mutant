using System;

namespace Mutant.Core.Modules
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public sealed class AutoRegisterModuleAttribute : Attribute
	{
	}
}
