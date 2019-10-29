using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Metrolib;

namespace FilterTest
{
	public sealed class AdvancedFilterViewModel
		: INotifyPropertyChanged
	{
		private readonly ObservableCollection<FilterGroupViewModel> _filterGroups;
		private FilterMode _mode;
		private string _filterExpression;

		public AdvancedFilterViewModel()
		{
			_filterGroups = new ObservableCollection<FilterGroupViewModel>();
			AddGroup();
			AddGroup();
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

		public FilterMode Mode
		{
			get { return _mode; }
			set
			{
				if (value == _mode)
					return;

				_mode = value;
				EmitPropertyChanged();
			}
		}

		public IEnumerable<FilterMode> Modes
		{
			get
			{
				return new[]
				{
					FilterMode.And,
					FilterMode.Or
				};
			}
		}

		public IEnumerable<FilterGroupViewModel> FilterGroups
		{
			get { return _filterGroups; }
		}

		public ICommand AddGroupCommand
		{
			get { return new DelegateCommand2(AddGroup); }
		}

		private void AddGroup()
		{
			var group = new FilterGroupViewModel();
			_filterGroups.Add(group);
			group.Removed += OnGroupRemoved;
			group.PropertyChanged += GroupOnPropertyChanged;

			UpdateFilterExpression();
		}

		private void GroupOnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case nameof(FilterGroupViewModel.FilterExpression):
					UpdateFilterExpression();
					break;
			}
		}

		private void OnGroupRemoved(FilterGroupViewModel group)
		{
			_filterGroups.Remove(group);

			group.Removed -= OnGroupRemoved;
			group.PropertyChanged -= GroupOnPropertyChanged;

			UpdateFilterExpression();
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void EmitPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		private void UpdateFilterExpression()
		{
			var @operator = _mode == FilterMode.And ? "And" : "Or";
			var expression = string.Join($" {@operator} ",
			                             _filterGroups.Where(x => !string.IsNullOrEmpty(x.FilterExpression)).Select(x => $"({x.FilterExpression})"));
			FilterExpression = expression;
		}
	}
}