using System.ComponentModel;

namespace UriShell.Samples.TabApp
{
	public abstract class ObservableObject : INotifyPropertyChanged
	{
		private event PropertyChangedEventHandler _propertyChanged;

		protected void OnPropertyChanged(string propertyName)
		{
			var propertyChanged = this._propertyChanged;
			if (propertyChanged != null)
			{
				propertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
		{
			add
			{
				this._propertyChanged += value;
			}
			remove
			{
				this._propertyChanged -= value;
			}
		}
	}
}