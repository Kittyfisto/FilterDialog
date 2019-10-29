namespace FilterTest
{
	public sealed class FilterModeViewModel
	{
		private readonly FilterMode _mode;

		public FilterModeViewModel(FilterMode mode)
		{
			_mode = mode;
		}

		public string Alias
		{
			get { return _mode == FilterMode.And ? "All" : "Any"; }
		}

		public FilterMode Mode
		{
			get { return _mode; }
		}

		#region Overrides of Object

		public override bool Equals(object obj)
		{
			if (!(obj is FilterModeViewModel))
				return false;

			var viewModel = (FilterModeViewModel)obj;
			return _mode == viewModel._mode;
		}

		#endregion
	}
}