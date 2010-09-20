using System;
using System.ServiceModel;

namespace Sample
{
	[ServiceContract]
	public interface ITaskService
	{
		[OperationContract]
		Task[] GetAll();

		[OperationContract]
		Task Create(string description);

		[OperationContract]
		Task MarkComplete(Guid id);

		[OperationContract]
		void Delete(Guid id);
	}
}