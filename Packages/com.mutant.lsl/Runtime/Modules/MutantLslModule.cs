using System;

namespace Mutant.LSL
{
	public sealed class MutantLslModule : IDisposable
	{
		public static MutantLslModule Current { get; private set; }

		public IMutantLslService Service { get; }

		public MutantLslModule(IMutantLslService service)
		{
			Service = service ?? throw new MutantLslException("MutantLslModule service is null.");
		}

		public void SetAsCurrent()
		{
			Current = this;
		}

		public static MutantLslModule CreateDefault(bool setAsCurrent = true)
		{
			MutantLslModule module = new MutantLslModule(new MutantLslService());
			if (setAsCurrent)
			{
				module.SetAsCurrent();
			}

			return module;
		}

		public void Dispose()
		{
			if (ReferenceEquals(Current, this))
			{
				Current = null;
			}
		}
	}
}
