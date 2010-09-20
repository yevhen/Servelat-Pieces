using System;
using System.Runtime.Serialization;

namespace Sample
{
	[DataContract]
	public class Task
	{
		[DataMember]
		public Guid Id
		{
			get; set;
		}

		[DataMember]
		public string Description
		{
			get; set;
		}

		[DataMember]
		public bool IsCompleted
		{
			get; set;
		}

		[DataMember]
		public DateTime Created
		{
			get; set;
		}

		[DataMember]
		public DateTime Completed
		{
			get; set;
		}
	}
}