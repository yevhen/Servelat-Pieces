using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

using Xtalion.Coroutines;
using Xtalion.Silverlight;

namespace Sample.Silverlight.Views
{
	public class TaskManagementViewModel : ViewModelServiceBase<ITaskService>
	{
		const string address = "http://localhost:2508/TaskService.svc";

		Task selected;
		string description;

		public TaskManagementViewModel() : base(address)
		{
			InitializeProperties();
			InitializeCommands();
		}
		
		void InitializeProperties()
		{
			Tasks = new ObservableCollection<Task>();
		}

		void InitializeCommands()
		{
			var cmd = new ViewModelCommandBuilder(this);

			CreateCommand		= cmd.For(()=> Create());
			DeleteCommand		= cmd.For(()=> Delete());
			MarkCompleteCommand = cmd.For(()=> MarkComplete());
		}

		public ICommand CreateCommand
		{
			get; private set;
		}

		public ICommand MarkCompleteCommand
		{
			get; private set;
		}

		public ICommand DeleteCommand
		{
			get; private set;
		}

		public ObservableCollection<Task> Tasks
		{
			get; private set;
		}

		public Task Selected
		{
			get { return selected; }
			set
			{
				selected = value;

				NotifyOfPropertyChange(() => Selected);
				NotifyOfPropertyChange(() => CanDelete);
				NotifyOfPropertyChange(() => CanMarkComplete);
			}
		}

		public string Description
		{
			get { return description; }
			set
			{
				description = value;

				NotifyOfPropertyChange(() => Description);
				NotifyOfPropertyChange(() => CanCreate);
			}
		}

		public IEnumerable<IAction> Activate()
		{
			var action = Query(service => service.GetAll());
			yield return action;

			foreach (Task each in action.Result)
			{
				Tasks.Add(each);
			}

			Selected = Tasks[0];
		}

		public IEnumerable<IAction> Create()
		{
			var action = Query(service => service.Create(Description));
			yield return action;

			Task newTask = action.Result;

			Tasks.Insert(0, newTask);
			Selected = newTask;

			Description = "";
		}

		public bool CanCreate
		{
			get { return !string.IsNullOrEmpty(Description); }
		}

		public IEnumerable<IAction> MarkComplete()
		{
			Task task = Selected;

			var command = Query(service => service.MarkComplete(task.Id));
			yield return command;

			Task updatedTask = command.Result;
			Replace(task, updatedTask);			
		}

		public bool CanMarkComplete
		{
			get { return Selected != null && !Selected.IsCompleted; }
		}

		void Replace(Task prevTask, Task newTask)
		{
			var position = Tasks.IndexOf(prevTask);
			
			Tasks.RemoveAt(position);
			Tasks.Insert(position, newTask);
		}

		public IEnumerable<IAction> Delete()
		{
			Task task = Selected;

			yield return Command(service => service.Delete(task.Id));
			Tasks.Remove(task);

			if (Tasks.Count > 0)
				Selected = Tasks[0];
		}

		public bool CanDelete
		{
			get { return Selected != null; }
		}
	}
}
