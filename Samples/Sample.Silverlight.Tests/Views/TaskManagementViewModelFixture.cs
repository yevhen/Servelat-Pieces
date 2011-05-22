using System;
using System.Collections.Generic;

using NUnit.Framework;
using Rhino.Mocks;

using Xtalion.Coroutines;

namespace Sample.Silverlight.Views
{
	[TestFixture]
	public class TaskManagementViewModelFixture
	{
		ITaskService mock;
		TaskManagementViewModel model;

		[SetUp]
		public void SetUp()
		{
			mock = MockRepository.GenerateMock<ITaskService>();
			model = new TaskManagementViewModel(mock);
		}

		[Test]
		public void On_activate()
		{
			// arrange
			var tasks = new[] { CreateTask("Task1"), CreateTask("Task2") };
			mock.Expect(service => service.GetAll()).Return(tasks);

			// act
			Execute(model.Activate());

			// assert
			Assert.AreEqual(tasks.Length, model.Tasks.Count);
			Assert.AreSame(model.Tasks[0], model.Selected);
		}

		[Test]
		public void On_create()
		{
			// arrange
			model.Description = "test";
			var newTask = CreateTask(model.Description);

			// new Task instance will be returned from service
			mock.Expect(service => service.Create(model.Description))
				.Return(newTask);

			// act
			Execute(model.Create());

			// assert
			Assert.AreSame(model.Tasks[0], newTask);
			Assert.AreSame(model.Tasks[0], model.Selected);
		}

		[Test]
		public void On_mark_complete()
		{
			// arrange
			var task = CreateTask("Task");
			model.Tasks.Add(task);
			model.Selected = task;

			var updatedTask = CreateTask(task.Id, "Task");
			updatedTask.IsCompleted = true;

			// should pass id of currently selected task
			mock.Expect(service => service.MarkComplete(task.Id))
				.Return(updatedTask);
				
			// act
			Execute(model.MarkComplete());

			// assert
			Assert.AreSame(model.Tasks[0], updatedTask, "Should replace task for updated one");
			Assert.AreSame(model.Selected, updatedTask, "Should set updated one as selected");
		}

		[Test]
		public void On_delete()
		{
			// arrange
			var task1 = CreateTask("Task1");
			var task2 = CreateTask("Task2");

			model.Tasks.Add(task1);
			model.Tasks.Add(task2);

			model.Selected = task2;

			// should pass id of currently selected task
			mock.Expect(service => service.Delete(task2.Id));

			// act
			Execute(model.Delete());

			// assert
			Assert.AreEqual(model.Tasks.Count, 1);
			Assert.AreSame(model.Tasks[0], model.Selected);
		}

		static Task CreateTask(string description)
		{
			return CreateTask(Guid.NewGuid(), description);
		}

		static Task CreateTask(Guid id, string description)
		{
			return new Task { Id = id, Description = description };
		}

		static void Execute(IEnumerable<IAction> routine)
		{
			foreach (var action in routine)
			{
				action.Execute();
			}
		}
	}
}
