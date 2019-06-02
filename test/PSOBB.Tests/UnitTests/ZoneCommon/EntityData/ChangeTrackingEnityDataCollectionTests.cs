﻿using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace PSOBB.Tests.Collections
{
	//We inherit from the data entity collection tests because you SHOULD be able to the same
	//operations and expect the same results too.
	public sealed class ChangeTrackingEnityDataCollectionTests : DataEntityCollectionTests
	{
		[Test]
		public void Test_Index_Not_Set_Shows_No_Changes_Set([EntityDataCollectionTestRange] int index)
		{
			//arrange
			ChangeTrackingEntityFieldDataCollectionDecorator<TestFieldType> collection = new ChangeTrackingEntityFieldDataCollectionDecorator<TestFieldType>(base.CreateEntityDataCollection());

			//act
			bool isSet = collection.ChangeTrackingArray.Get(index);

			//assert
			Assert.False(isSet, $"Index: {index} did not change but change tracking was set.");
		}

		[Test]
		public void Test_Index_Set_Causes_Dirty_Bit_Set([EntityDataCollectionTestRange] int index, [Values(1, 2, 3, 4, 5, 6, 7, 8)] int value)
		{
			//arrange
			ChangeTrackingEntityFieldDataCollectionDecorator<TestFieldType> collection = new ChangeTrackingEntityFieldDataCollectionDecorator<TestFieldType>(base.CreateEntityDataCollection());

			//act
			collection.SetFieldValue<int>(index, value);
			bool isSet = collection.ChangeTrackingArray.Get(index);

			//assert
			Assert.True(isSet, $"Set Value: {value} at Index: {index} did not set change tracked bit.");
		}
		
		//TODO: Refactor set tests to be generic
		[Test]
		public void Test_PendingChanges_Not_True_When_EquivalentValues_Set([EntityDataCollectionTestRange] int index, [Values(1, 2, 3, 4, 5, 6, 7, 8)] int value)
		{
			//arrange
			ChangeTrackingEntityFieldDataCollectionDecorator<TestFieldType> collection = new ChangeTrackingEntityFieldDataCollectionDecorator<TestFieldType>(base.CreateEntityDataCollection());

			//act
			collection.SetFieldValue<int>(index, value);
			collection.ClearTrackedChanges(); //clear to remove set track bits
			collection.SetFieldValue<int>(index, value); //re set the same value at the same index
			bool isSet = collection.ChangeTrackingArray.Get(index); //this should be false because it should not have been a new value

			//assert
			Assert.False(isSet, $"Set Value: {value} at Index: {index} should not have a set bit since clearing and re setting same value.");
			Assert.False(collection.HasPendingChanges, $"Set Value: {value} at Index: {index} should cause changes pending after setting same value after clear.");
		}

		//Similar to above but testing float comparision
		[Test]
		public void Test_PendingChanges_Not_True_When_EquivalentValues_Set_Float([EntityDataCollectionTestRange] int index, [Values(1.2f, 2.5f, 3.6335f, 4.673f, 5.22222f, 6.63f, 7.123f, 8.789f)] float value)
		{
			//arrange
			ChangeTrackingEntityFieldDataCollectionDecorator<TestFieldType> collection = new ChangeTrackingEntityFieldDataCollectionDecorator<TestFieldType>(base.CreateEntityDataCollection());

			//act
			collection.SetFieldValue(index, value);
			collection.ClearTrackedChanges(); //clear to remove set track bits
			collection.SetFieldValue(index, value); //re set the same value at the same index
			bool isSet = collection.ChangeTrackingArray.Get(index); //this should be false because it should not have been a new value

			//assert
			Assert.False(isSet, $"Set Value: {value} at Index: {index} should not have a set bit since clearing and re setting same value.");
			Assert.False(collection.HasPendingChanges, $"Set Value: {value} at Index: {index} should cause changes pending after setting same value after clear.");
		}

		[Test]
		public void Test_Indicies_Set_Are_Tracked_And_Unset_Not_Tracked([Values(1, 2, 3, 4, 5, 6, 7, 8)] int value)
		{
			//arrange
			ChangeTrackingEntityFieldDataCollectionDecorator<TestFieldType> collection = new ChangeTrackingEntityFieldDataCollectionDecorator<TestFieldType>(base.CreateEntityDataCollection());

			//act
			collection.SetFieldValue<int>(2, value);
			collection.SetFieldValue<int>(3, value);
			bool isSet2 = collection.ChangeTrackingArray.Get(2);
			bool isSet3 = collection.ChangeTrackingArray.Get(3);
			bool isSet4 = collection.ChangeTrackingArray.Get(4);

			//assert
			Assert.True(isSet2, $"Set Value: {value} at Index: {2} did not set change tracked bit.");
			Assert.True(isSet3, $"Set Value: {value} at Index: {3} did not set change tracked bit.");
			Assert.False(isSet4, $"Set Value: {value} at Index: {4} was not changed but had change bit set.");
		}

		[Test]
		public void Test_Can_Clear_TrackedChanges()
		{
			//arrange
			ChangeTrackingEntityFieldDataCollectionDecorator<TestFieldType> collection = new ChangeTrackingEntityFieldDataCollectionDecorator<TestFieldType>(base.CreateEntityDataCollection());

			//act
			for(int i = 0; i < 4; i++)
				collection.SetFieldValue<int>((TestFieldType)i, 3);

			collection.ClearTrackedChanges();

			//assert
			for(int i = 0; i < 4; i++)
				Assert.False(collection.ChangeTrackingArray.Get(i), $"Change tracking has Index: {i} set. No index should be set.");
		}

		/// <inheritdoc />
		protected override IEntityDataFieldContainer<TestFieldType> CreateEntityDataCollection()
		{
			return new ChangeTrackingEntityFieldDataCollectionDecorator<TestFieldType>(base.CreateEntityDataCollection());
		}
	}
}
