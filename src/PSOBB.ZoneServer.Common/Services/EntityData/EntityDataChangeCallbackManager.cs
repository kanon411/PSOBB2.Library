using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;

namespace PSOBB
{
	//TODO: We need some handling for callback cleanup, especially when an entity disappears.
	public sealed class EntityDataChangeCallbackManager : IEntityDataChangeCallbackRegisterable, IEntityDataChangeCallbackService
	{
		private Dictionary<NetworkEntityGuid, Dictionary<EntityDataFieldType, Action<IEntityDataFieldContainer>>> CallbackMap { get; }

		public EntityDataChangeCallbackManager()
		{
			CallbackMap = new Dictionary<NetworkEntityGuid, Dictionary<EntityDataFieldType, Action<IEntityDataFieldContainer>>>();
		}

		/// <inheritdoc />
		public void RegisterCallback<TCallbackValueCastType>(NetworkEntityGuid entity, EntityDataFieldType dataField, Action<NetworkEntityGuid, EntityDataChangedArgs<TCallbackValueCastType>> callback) 
			where TCallbackValueCastType : struct
		{
			//TODO: Anyway we can avoid this for registering callbacks, wasted cycles kinda
			if(!CallbackMap.ContainsKey(entity))
				CallbackMap.Add(entity, new Dictionary<EntityDataFieldType, Action<IEntityDataFieldContainer>>());

			//TODO: This isn't thread safe, this whole thinjg isn't. That could be problematic
			Action<IEntityDataFieldContainer> dataChangeEvent = dataContainer =>
			{
				//TODO: If we ever support original value we should change this
				//So, the callback needs to send the entity guid and the entity data change args which contain the original (not working yet) and new value.
				callback(entity, new EntityDataChangedArgs<TCallbackValueCastType>(default(TCallbackValueCastType), dataContainer.GetFieldValue<TCallbackValueCastType>((int)dataField)));
			};

			//We need to add a null action here or it will throw when we try to add the action. But if one exists we need to Delegate.Combine
			if(!CallbackMap[entity].ContainsKey(dataField))
				CallbackMap[entity].Add(dataField, dataChangeEvent);
			else
				CallbackMap[entity][dataField] += dataChangeEvent;
		}

		/// <inheritdoc />
		public void InvokeChangeEvents(NetworkEntityGuid entity, EntityDataFieldType field, IEntityDataFieldContainer dataContainer)
		{
			//We aren't watching ANY data changes for this particular entity.
			if(!CallbackMap.ContainsKey(entity))
				return;

			//If we have any registered callbacks for this entity's data change we should dispatch it (they will all be called)
			if(CallbackMap[entity].ContainsKey(field))
				CallbackMap[entity][field]?.Invoke(dataContainer);
		}
	}
}
