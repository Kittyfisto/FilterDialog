namespace FilterTest
{
	public sealed class MainWindowViewModel
	{
		private readonly BasicFilterViewModel _basic;
		private readonly AdvancedFilterViewModel _advanced;

		public MainWindowViewModel()
		{
			_basic = new BasicFilterViewModel();
			_advanced = new AdvancedFilterViewModel();
		}

		public BasicFilterViewModel Basic => _basic;

		public AdvancedFilterViewModel Advanced => _advanced;
	}
}