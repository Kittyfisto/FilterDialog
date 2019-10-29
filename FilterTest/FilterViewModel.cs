using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Metrolib;

namespace FilterTest
{
	public sealed class FilterViewModel
		: INotifyPropertyChanged
	{
		private string _filterText;
		private FilterMode? _mode;
		private bool _isActive;

		public bool IsActive
		{
			get { return _isActive; }
			set
			{
				if (value == _isActive)
					return;

				_isActive = value;
				EmitPropertyChanged();
			}
		}

		public string FilterText
		{
			get { return _filterText; }
			set
			{
				if (value == _filterText)
					return;

				_filterText = value;
				EmitPropertyChanged();
			}
		}

		public FilterMode? Mode
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

		public ICommand RemoveCommand
		{
			get { return new DelegateCommand2(EmitRemoved); }
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public event Action<FilterViewModel> Removed;

		private void EmitRemoved()
		{
			Removed?.Invoke(this);
		}

		private void EmitPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}