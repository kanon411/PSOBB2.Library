using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Guardians
{
	[TestFixture]
	public sealed class EntityDataChangeCallbackTests
	{
		[Test]
		public void Test_Can_Create_Ctor_Without_Exception()
		{
			Assert.DoesNotThrow(() => new EntityDataChangeCallbackManager());
		}

		[Test]
		[TestCase(5, EntityDataFieldType.EntityMaxHealth)]
		[TestCase(5, EntityDataFieldType.EntityCurrentHealth)]
		[TestCase(555, EntityDataFieldType.EntityCurrentHealth)]
		public void Test_Can_Register_Callback_Without_Throw(long guid, EntityDataFieldType fieldType)
		{
			//arrange
			EntityDataChangeCallbackManager callbackManager = new EntityDataChangeCallbackManager();

			Assert.DoesNotThrow(() => callbackManager.RegisterCallback<float>(new NetworkEntityGuid((ulong)guid), fieldType, (eg, args) => { }));
		}

		[Test]
		[TestCase(5, EntityDataFieldType.EntityMaxHealth)]
		[TestCase(5, EntityDataFieldType.EntityCurrentHealth)]
		[TestCase(555, EntityDataFieldType.EntityCurrentHealth)]
		public void Test_Can_Register_Multple_Callback_Without_Throw(long guid, EntityDataFieldType fieldType)
		{
			//arrange
			EntityDataChangeCallbackManager callbackManager = new EntityDataChangeCallbackManager();

			Assert.DoesNotThrow(() => callbackManager.RegisterCallback<float>(new NetworkEntityGuid((ulong)guid), fieldType, (eg, args) => { }));
			Assert.DoesNotThrow(() => callbackManager.RegisterCallback<float>(new NetworkEntityGuid((ulong)guid), fieldType, (eg, args) => { }));
		}

		[Test]
		public void Test_Can_ServiceCallbacks_For_Empty_CallbackRegister()
		{
			//arrange
			EntityDataChangeCallbackManager callbackManager = new EntityDataChangeCallbackManager();

			//assert
			Assert.DoesNotThrow(() => callbackManager.InvokeChangeEvents(NetworkEntityGuid.Empty, EntityDataFieldType.EntityCurrentHealth, new EntityFieldDataCollection<EntityDataFieldType>()));
		}
	}
}
