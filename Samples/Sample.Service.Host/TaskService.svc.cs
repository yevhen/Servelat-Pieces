using System;
using System.Collections.Generic;

namespace Sample.Service
{
	public class TaskService : ITaskService
	{
		static readonly List<Task> tasks = new List<Task>
		{
			new Task { Id = Guid.NewGuid(), Description = "Task 1"}, 
			new Task { Id = Guid.NewGuid(), Description = "Task 2", IsCompleted = true}
		};

		public Task[] GetAll()
		{
			return tasks.ToArray();
		}

		public Task Create(string description)
		{
			var task = new Task
			{
				Id = Guid.NewGuid(),
				Description = description,
				Created = DateTime.Now,
				Completed = DateTime.Now
			};

			tasks.Add(task);

			return task;
		}

		public Task MarkComplete(Guid id)
		{
			Task task = tasks.Find(x => x.Id == id);

			task.IsCompleted = true;
			task.Completed = DateTime.Now;

			return task;
		}

		public void Delete(Guid id)
		{
			Task existent = tasks.Find(x => x.Id == id);
			tasks.Remove(existent);
		}
	}
}