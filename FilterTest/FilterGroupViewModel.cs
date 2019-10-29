using Metrolib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace FilterTest
{
	public sealed class FilterGroupViewModel
		: INotifyPropertyChanged
	{
		private readonly ObservableCollection<FilterViewModel> _filters;
		private FilterModeViewModel _mode;

		public FilterGroupViewModel()
		{
			_filters = new ObservableCollection<FilterViewModel>();
			_mode = new FilterModeViewModel(FilterMode.And);
			AddFilter();
		}

		private string _filterExpression;

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

		public FilterModeViewModel Mode
		{
			get { return _mode; }
			set
			{
				if (Equals(value, _mode))
					return;

				_mode = value;
				EmitPropertyChanged();

				Update();
			}
		}

		public IEnumerable<FilterModeViewModel> Modes
		{
			get
			{
				return new[]
				{
					new FilterModeViewModel(FilterMode.And),
					new FilterModeViewModel(FilterMode.Or)
				};
			}
		}

		public IEnumerable<FilterViewModel> Filters
		{
			get { return _filters; }
		}

		public ICommand AddFilterCommand
		{
			get { return new DelegateCommand2(AddFilter); }
		}

		public ICommand RemoveCommand
		{
			get { return new DelegateCommand2(EmitRemoved); }
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void EmitRemoved()
		{
			Removed?.Invoke(this);
		}

		public event Action<FilterGroupViewModel> Removed;

		private void AddFilter()
		{
			var filter = new FilterViewModel();
			_filters.Add(filter);
			filter.Removed += OnFilterRemoved;
			filter.PropertyChanged += OnFilterPropertyChanged;

			Update();
		}

		private void OnFilterPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case nameof(FilterViewModel.FilterText):
					UpdateFilterExpression();
					break;
			}
		}

		private void OnFilterRemoved(FilterViewModel filter)
		{
			_filters.Remove(filter);
			filter.Removed -= OnFilterRemoved;
			filter.PropertyChanged -= OnFilterPropertyChanged;
		}

		private void EmitPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
					filter.Mode = _mode.Mode;
				}

				_filters[_filters.Count - 1].Mode = null;
			}
		}

		private void UpdateFilterExpression()
		{
			var operation = _mode.Mode == FilterMode.And ? "AND" : "OR";
			var expression = string.Join(string.Format(" {0} ", operation),
			                             _filters.Where(x => !string.IsNullOrEmpty(x.FilterText)).Select(x => x.FilterText).ToArray());
			FilterExpression = expression;
		}
	}
}