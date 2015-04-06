using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UriShell.Shell;
using UriShell.Shell.Connectors;

namespace UriShell.Samples.TabApp.UriShellPrerequisites
{
	public class FakeConnectedDragDrop : IConnectedDragDrop
	{
		public bool IsActive
		{
			get
			{
				return false;
			}
		}

		public void Drag(object connected)
		{
		}

		public void Drop(IUriPlacementConnector target)
		{
		}

		public bool IsDragging(object resolved)
		{
			return false;
		}

		public void SetData<TFormat>(ConnectedDragDropKey<TFormat> key, TFormat data)
		{
		}

		public TFormat GetData<TFormat>(ConnectedDragDropKey<TFormat> key)
		{
			return default(TFormat);
		}

		public bool GetDataPresent<TFormat>(ConnectedDragDropKey<TFormat> key)
		{
			return false;
		}

		public event EventHandler DraggedClosed;
	}
}
