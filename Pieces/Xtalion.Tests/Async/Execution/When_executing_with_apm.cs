using System;

using NUnit.Framework;
using Rhino.Mocks;

namespace Xtalion.Async.Execution
{
	[Context]
	public class When_executing_with_apm : ContextSpecification
	{
		const string parameter = "param";
		const string result = "result";

		AsyncCommand command;
		AsyncQuery<string> query;

		TestInterface instance;
		IAsyncResult ar;

		protected override void Given()
		{
			instance = MockRepository.GenerateStrictMock<TestInterface>();
			ar = MockRepository.GenerateStub<IAsyncResult>();

			instance.Expect(
				x => x.BeginMethod(
						Arg<string>.Is.Equal(parameter), 
						Arg<AsyncCallback>.Is.NotNull,
						Arg<object>.Is.Null))
					.Return(null)
					.WhenCalled(x => ((AsyncCallback)x.Arguments[1])(ar));

			instance.Expect(x => x.EndMethod(ar));

			instance.Expect(
				x => x.BeginFunction(
						Arg<string>.Is.Equal(parameter),
						Arg<AsyncCallback>.Is.NotNull,
						Arg<object>.Is.Null))
					.Return(null)
					.WhenCalled(x => ((AsyncCallback)x.Arguments[1])(ar));

			instance.Expect(x => x.EndFunction(ar)).Return(result);

			command = new AsyncCommand(() => instance.Method(parameter), true);
			query = new AsyncQuery<string>(() => instance.Function(parameter), true);
		}

		protected override void When()
		{
			Execute(command);
			Execute(query);
		}

		[Then]
		public void Should_correctly_call_apm_version()
		{
			instance.VerifyAllExpectations();
		}

		[Then]
		public void Should_handle_result()
		{
			Assert.That(query.Result, Is.EqualTo(result));
		}
	}
}
