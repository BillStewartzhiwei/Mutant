using System;

namespace Mutant.LSL
{
	public sealed class MutantLslException : Exception
	{
		public MutantLslException(string message) : base(message)
		{
		}

		public MutantLslException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
