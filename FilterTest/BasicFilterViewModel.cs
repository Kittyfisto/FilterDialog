using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Metrolib;

namespace FilterTest
{
	public sealed class BasicFilterViewModel
		: INotifyPropertyChanged
	{
		private readonly ObservableCollection<FilterViewModel> _filters;
		private FilterMode _mode;
		private string _filterExpression;

		public BasicFilterViewModel()
		{
			_filters = new ObservableCollection<FilterViewModel>();
			AddFilter().FilterText = "Foo";
			AddFilter().FilterText = "Bar";
		}

		public string FilterExpression
		{
			get { return _filterExpression; }
			private set
			{
				if (value == _filterExpression)
					return;

				_filterExpression = value;
				EmitPropertyChanged();
			}
		}

		public IEnumerable<FilterMode> Modes => new[] {FilterMode.And, FilterMode.Or};

		public FilterMode Mode
		{
			get { return _mode; }
			set
			{
				if (value == _mode)
					return;

				_mode = value;
				EmitPropertyChanged();

				Update();
			}
		}

		public IEnumerable<FilterViewModel> Filters => _filters;

		public ICommand AddFilterCommand => new DelegateCommand2(() => AddFilter());

		private FilterViewModel AddFilter()
		{
			var filter = new FilterViewModel();
			filter.Removed += FilterOnRemoved;
			filter.PropertyChanged += FilterOnPropertyChanged;
			_filters.Add(filter);

			Update();

			return filter;
		}

		private void FilterOnRemoved(FilterViewModel filter)
		{
			filter.Removed -= FilterOnRemoved;
			filter.PropertyChanged -= FilterOnPropertyChanged;
			_filters.Remove(filter);
		}

		private void FilterOnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case nameof(FilterViewModel.FilterText):
					UpdateFilterExpression();
					break;
			}
		}

		private void Update()
		{
			UpdateFilterMode();
			UpdateFilterExpression();
		}

		private void UpdateFilterMode()
		{
			if (_filters.Count > 0)
			{
				foreach (var filter in _filters)
				{
					filter.Mode = _mode;
				}

				_filters[_filters.Count - 1].Mode = null;
			}
		}

		private void UpdateFilterExpression()
		{
			var operation = _mode == FilterMode.And ? "AND" : "OR";
			var expression = string.Join(string.Format(" {0} ", operation),
			                             _filters.Where(x => !string.IsNullOrEmpty(x.FilterText)).Select(x => x.FilterText).ToArray());
			FilterExpression = expression;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void EmitPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}