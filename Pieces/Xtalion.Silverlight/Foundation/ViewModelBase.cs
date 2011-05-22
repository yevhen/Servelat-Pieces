#region Author

//// Rob Eisenberg, Blue Spire Consulting, Inc 
//// Yevhen Bobrov, http://blog.xtalion.com 

#endregion

using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Threading;

namespace Xtalion.Silverlight
{
	public class ViewModelBase : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged = delegate { };

		readonly SynchronizationContext dispatcher = SynchronizationContext.Current;

		void NotifyOfPropertyChange(string propertyName)
		{
			if (dispatcher != null)
			{
				dispatcher.Post(x => RaisePropertyChanged(propertyName), null);
				return;
			}

			RaisePropertyChanged(propertyName);
		}

		protected void NotifyOfPropertyChange<TProperty>(Expression<Func<TProperty>> property)
		{
			var lambda = (LambdaExpression)property;

			MemberExpression memberExpression;
			if (lambda.Body is UnaryExpression)
			{
				var unaryExpression = (UnaryExpression)lambda.Body;
				memberExpression = (MemberExpression)unaryExpression.Operand;
			}
			else
			{
				memberExpression = (MemberExpression)lambda.Body;
			}

			NotifyOfPropertyChange(memberExpression.Member.Name);
		}

		void RaisePropertyChanged(string propertyName)
		{
			PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}