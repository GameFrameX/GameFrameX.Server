using GameFrameX.ProtoBuf.Net;
using ProtoBuf;
using Xunit;
using Xunit.Abstractions;
using System.Collections.Generic;
using GameFrameX.NetWork.Messages;
using GameFrameX.NetWork.Abstractions;
using System;
using GameFrameX.NetWork;

namespace GameFrameX.Tests.ProtoBuff
{
    /// <summary>
    /// 对象池场景测试类 / Object pool scenario test class
    /// </summary>
    /// <remarks>
    /// 测试 ProtoBufSerializerHelper 在对象池场景下的正确性，
    /// 确保对象能够被完全重用而不会产生数据污染。
    /// 
    /// Tests the correctness of ProtoBufSerializerHelper in object pool scenarios,
    /// ensuring objects can be completely reused without data contamination.
    /// </remarks>
    public class ObjectPoolTest
    {
        private readonly ITestOutputHelper _output;

        /// <summary>
        /// 构造函数 / Constructor
        /// </summary>
        /// <param name="output">测试输出助手 / Test output helper</param>
        public ObjectPoolTest(ITestOutputHelper output)
        {
            _output = output;
        }

        /// <summary>
        /// 嵌套对象测试类 / Nested object test class
        /// </summary>
        [ProtoContract]
        public class NestedTestData : MessageObject
        {
            /// <summary>
            /// 嵌套ID / Nested ID
            /// </summary>
            [ProtoMember(1)]
            public int NestedId { get; set; }

            /// <summary>
            /// 嵌套名称 / Nested name
            /// </summary>
            [ProtoMember(2)]
            public string NestedName { get; set; } = string.Empty;

            /// <summary>
            /// 嵌套列表 / Nested list
            /// </summary>
            [ProtoMember(3)]
            public List<string> NestedItems { get; set; } = new List<string>();

            /// <summary>
            /// 清除嵌套对象状态 / Clear nested object state
            /// </summary>
            public override void Clear()
            {
                NestedId = default;
                NestedName = default;
                NestedItems.Clear();
            }
        }

        /// <summary>
        /// 对象池测试数据类 / Object pool test data class
        /// </summary>
        [ProtoContract]
        public class PoolTestData : MessageObject
        {
            /// <summary>
            /// ID 属性 / ID property
            /// </summary>
            [ProtoMember(1)]
            public int Id { get; set; }

            /// <summary>
            /// 名称属性 / Name property
            /// </summary>
            [ProtoMember(2)]
            public string Name { get; set; } = string.Empty;

            /// <summary>
            /// 是否激活属性 / IsActive property
            /// </summary>
            [ProtoMember(3)]
            public bool IsActive { get; set; }

            /// <summary>
            /// 字符串列表 / String list
            /// </summary>
            [ProtoMember(4)]
            public List<string> Items { get; set; } = new List<string>();

            /// <summary>
            /// 数值列表 / Number list
            /// </summary>
            [ProtoMember(5)]
            public List<int> Numbers { get; set; } = new List<int>();

            /// <summary>
            /// 分数属性 / Score property
            /// </summary>
            [ProtoMember(6)]
            public float Score { get; set; }

            /// <summary>
            /// 嵌套对象 / Nested object
            /// </summary>
            [ProtoMember(7)]
            public NestedTestData NestedData { get; set; }

            /// <summary>
            /// 嵌套对象列表 / Nested object list
            /// </summary>
            [ProtoMember(8)]
            public List<NestedTestData> NestedList { get; set; } = new List<NestedTestData>();

            /// <summary>
            /// 清除对象状态 / Clear object state
            /// </summary>
            public override void Clear()
            {
                Id = default;
                Name = default;
                IsActive = default;
                Items.Clear();
                Numbers.Clear();
                Score = default;
                NestedData?.Clear();
                NestedData = null;
                foreach (var nested in NestedList)
                {
                    nested.Clear();
                }
                NestedList.Clear();
            }
        }

        /// <summary>
        /// 测试对象池场景下的完全覆盖 / Test complete overwrite in object pool scenario
        /// </summary>
        /// <remarks>
        /// 验证从对象池中取出的对象能够被完全覆盖，不会保留之前的数据。
        /// 
        /// Verifies that objects taken from the object pool can be completely overwritten without retaining previous data.
        /// </remarks>
        [Fact]
        public void TestObjectPool_CompleteOverwrite()
        {
            // 创建第一个对象并序列化 / Create first object and serialize
            var firstData = new PoolTestData
            {
                Id = 1,
                Name = "First",
                IsActive = true,
                Items = new List<string> { "item1", "item2", "item3" },
                Numbers = new List<int> { 10, 20, 30 },
                Score = 95.5f
            };
            var firstBytes = ProtoBufSerializerHelper.Serialize(firstData);

            // 创建第二个对象并序列化 / Create second object and serialize
            var secondData = new PoolTestData
            {
                Id = 2,
                Name = "Second",
                IsActive = false,
                Items = new List<string> { "newItem" },
                Numbers = new List<int> { 100 },
                Score = 75.0f
            };
            var secondBytes = ProtoBufSerializerHelper.Serialize(secondData);

            // 模拟对象池场景：重用同一个对象实例 / Simulate object pool scenario: reuse the same object instance
            var pooledObject = new PoolTestData();

            // 第一次使用：反序列化第一个对象的数据 / First use: deserialize first object's data
            ProtoBufSerializerHelper.Deserialize(firstBytes, pooledObject);
            
            Assert.Equal(1, pooledObject.Id);
            Assert.Equal("First", pooledObject.Name);
            Assert.True(pooledObject.IsActive);
            Assert.Equal(3, pooledObject.Items.Count);
            Assert.Contains("item1", pooledObject.Items);
            Assert.Contains("item2", pooledObject.Items);
            Assert.Contains("item3", pooledObject.Items);
            Assert.Equal(3, pooledObject.Numbers.Count);
            Assert.Contains(10, pooledObject.Numbers);
            Assert.Contains(20, pooledObject.Numbers);
            Assert.Contains(30, pooledObject.Numbers);
            Assert.Equal(95.5f, pooledObject.Score);

            // 模拟对象池返回时调用Clear / Simulate object pool calling Clear when returning object
            pooledObject.Clear();

            // 第二次使用：反序列化第二个对象的数据，应该完全覆盖 / Second use: deserialize second object's data, should completely overwrite
            ProtoBufSerializerHelper.Deserialize(secondBytes, pooledObject);
            
            Assert.Equal(2, pooledObject.Id);
            Assert.Equal("Second", pooledObject.Name);
            Assert.False(pooledObject.IsActive); // 重要：布尔值应该被正确覆盖 / Important: boolean should be correctly overwritten
            Assert.Single(pooledObject.Items); // 重要：列表应该被完全替换，不是追加 / Important: list should be completely replaced, not appended
            Assert.Contains("newItem", pooledObject.Items);
            Assert.Single(pooledObject.Numbers);
            Assert.Contains(100, pooledObject.Numbers);
            Assert.Equal(75.0f, pooledObject.Score);
        }

        /// <summary>
        /// 测试空数据覆盖 / Test empty data overwrite
        /// </summary>
        /// <remarks>
        /// 验证空数据能够正确覆盖已有数据，确保对象被完全重置。
        /// 
        /// Verifies that empty data can correctly overwrite existing data, ensuring the object is completely reset.
        /// </remarks>
        [Fact]
        public void TestObjectPool_EmptyDataOverwrite()
        {
            // 创建包含数据的对象 / Create object with data
            var dataWithContent = new PoolTestData
            {
                Id = 999,
                Name = "HasContent",
                IsActive = true,
                Items = new List<string> { "content1", "content2" },
                Numbers = new List<int> { 1, 2, 3, 4, 5 },
                Score = 88.8f
            };
            var contentBytes = ProtoBufSerializerHelper.Serialize(dataWithContent);

            // 创建空数据对象 / Create empty data object
            var emptyData = new PoolTestData(); // 所有字段都是默认值 / All fields are default values
            var emptyBytes = ProtoBufSerializerHelper.Serialize(emptyData);

            // 模拟对象池场景 / Simulate object pool scenario
            var pooledObject = new PoolTestData();

            // 先填充数据 / First fill with data
            ProtoBufSerializerHelper.Deserialize(contentBytes, pooledObject);
            Assert.Equal(999, pooledObject.Id);
            Assert.Equal("HasContent", pooledObject.Name);
            Assert.True(pooledObject.IsActive);
            Assert.Equal(2, pooledObject.Items.Count);
            Assert.Equal(5, pooledObject.Numbers.Count);
            Assert.Equal(88.8f, pooledObject.Score);

            // 模拟对象池返回时调用Clear / Simulate object pool calling Clear when returning object
            pooledObject.Clear();

            // 然后用空数据覆盖 / Then overwrite with empty data
            ProtoBufSerializerHelper.Deserialize(emptyBytes, pooledObject);
            Assert.Equal(0, pooledObject.Id); // 默认值 / Default value
            Assert.Equal(string.Empty, pooledObject.Name); // 默认值 / Default value
            Assert.False(pooledObject.IsActive); // 默认值 / Default value
            Assert.Empty(pooledObject.Items); // 应该被清空 / Should be cleared
            Assert.Empty(pooledObject.Numbers); // 应该被清空 / Should be cleared
            Assert.Equal(0f, pooledObject.Score); // 默认值 / Default value
        }

        /// <summary>
        /// 测试多次重用对象池对象 / Test multiple reuses of object pool object
        /// </summary>
        /// <remarks>
        /// 验证对象池中的对象能够被多次重用，每次都能正确覆盖数据。
        /// 
        /// Verifies that objects in the object pool can be reused multiple times, with data correctly overwritten each time.
        /// </remarks>
        [Fact]
        public void TestObjectPool_MultipleReuses()
        {
            var pooledObject = new PoolTestData();

            // 第一次使用 / First use
            var data1 = new PoolTestData
            {
                Id = 100,
                Name = "Test1",
                IsActive = true,
                Items = new List<string> { "a", "b" },
                Numbers = new List<int> { 1, 2 },
                Score = 10.0f
            };
            var bytes1 = ProtoBufSerializerHelper.Serialize(data1);
            ProtoBufSerializerHelper.Deserialize(bytes1, pooledObject);
            
            Assert.Equal(100, pooledObject.Id);
            Assert.Equal("Test1", pooledObject.Name);
            Assert.True(pooledObject.IsActive);
            Assert.Equal(2, pooledObject.Items.Count);
            Assert.Equal(2, pooledObject.Numbers.Count);

            // 模拟对象池返回时调用Clear / Simulate object pool calling Clear when returning object
            pooledObject.Clear();

            // 第二次使用 / Second use
            var data2 = new PoolTestData
            {
                Id = 200,
                Name = "Test2",
                IsActive = false,
                Items = new List<string> { "x", "y", "z" },
                Numbers = new List<int> { 10 },
                Score = 20.0f
            };
            var bytes2 = ProtoBufSerializerHelper.Serialize(data2);
            ProtoBufSerializerHelper.Deserialize(bytes2, pooledObject);
            
            Assert.Equal(200, pooledObject.Id);
            Assert.Equal("Test2", pooledObject.Name);
            Assert.False(pooledObject.IsActive);
            Assert.Equal(3, pooledObject.Items.Count); // 应该是新的数据，不是追加 / Should be new data, not appended
            Assert.Single(pooledObject.Numbers);

            // 模拟对象池返回时调用Clear / Simulate object pool calling Clear when returning object
            pooledObject.Clear();

            // 第三次使用 / Third use
            var data3 = new PoolTestData
            {
                Id = 300,
                Name = "Test3",
                IsActive = true,
                Items = new List<string>(), // 空列表 / Empty list
                Numbers = new List<int> { 100, 200, 300, 400 },
                Score = 30.0f
            };
            var bytes3 = ProtoBufSerializerHelper.Serialize(data3);
            ProtoBufSerializerHelper.Deserialize(bytes3, pooledObject);
            
            Assert.Equal(300, pooledObject.Id);
            Assert.Equal("Test3", pooledObject.Name);
            Assert.True(pooledObject.IsActive);
            Assert.Empty(pooledObject.Items); // 应该是空的 / Should be empty
            Assert.Equal(4, pooledObject.Numbers.Count);
        }

        /// <summary>
        /// 测试对象引用保持不变 / Test object reference remains unchanged
        /// </summary>
        /// <remarks>
        /// 验证反序列化过程中对象引用保持不变，这是对象池的核心要求。
        /// 
        /// Verifies that object references remain unchanged during deserialization, which is a core requirement for object pools.
        /// </remarks>
        [Fact]
        public void TestObjectPool_ReferenceStability()
        {
            var pooledObject = new PoolTestData();
            var originalReference = pooledObject;
            var originalItemsReference = pooledObject.Items;
            var originalNumbersReference = pooledObject.Numbers;

            var testData = new PoolTestData
            {
                Id = 42,
                Name = "ReferenceTest",
                IsActive = true,
                Items = new List<string> { "ref1", "ref2" },
                Numbers = new List<int> { 42, 84 },
                Score = 42.0f
            };
            var bytes = ProtoBufSerializerHelper.Serialize(testData);

            // 反序列化后对象引用应该保持不变 / Object reference should remain unchanged after deserialization
            var result = ProtoBufSerializerHelper.Deserialize(bytes, pooledObject);
            
            Assert.Same(originalReference, result); // 返回的应该是同一个对象 / Should return the same object
            Assert.Same(originalReference, pooledObject); // 原始引用应该保持不变 / Original reference should remain unchanged
            Assert.Same(originalItemsReference, pooledObject.Items); // 列表引用应该保持不变 / List reference should remain unchanged
            Assert.Same(originalNumbersReference, pooledObject.Numbers); // 列表引用应该保持不变 / List reference should remain unchanged
            
            // 但内容应该被正确更新 / But content should be correctly updated
            Assert.Equal(42, pooledObject.Id);
            Assert.Equal("ReferenceTest", pooledObject.Name);
            Assert.True(pooledObject.IsActive);
            Assert.Equal(2, pooledObject.Items.Count);
            Assert.Equal(2, pooledObject.Numbers.Count);
        }
        /// <summary>
        /// 测试对象池与嵌套数据的序列化和反序列化 / Test object pool serialization and deserialization with nested data
        /// </summary>
        [Fact]
        public void TestObjectPoolWithNestedData()
        {
            // 创建原始数据 / Create original data
            var originalData = new PoolTestData
            {
                Id = 100,
                Name = "NestedTest",
                IsActive = true,
                Items = new List<string> { "item1", "item2" },
                Numbers = new List<int> { 10, 20 },
                Score = 95.5f,
                NestedData = new NestedTestData
                {
                    NestedId = 200,
                    NestedName = "Nested1",
                    NestedItems = new List<string> { "nested1", "nested2" }
                },
                NestedList = new List<NestedTestData>
                {
                    new NestedTestData { NestedId = 300, NestedName = "List1", NestedItems = new List<string> { "list1" } },
                    new NestedTestData { NestedId = 400, NestedName = "List2", NestedItems = new List<string> { "list2", "list3" } }
                }
            };

            // 序列化 / Serialize
            var serializedData = ProtoBufSerializerHelper.Serialize(originalData);
            Assert.NotNull(serializedData);
            Assert.True(serializedData.Length > 0);

            // 创建目标对象并清除 / Create target object and clear
            var targetData = new PoolTestData();
            targetData.Clear();

            // 反序列化 / Deserialize
            var deserializedData = ProtoBufSerializerHelper.Deserialize(serializedData, targetData);

            // 验证基本属性 / Verify basic properties
            Assert.Same(targetData, deserializedData);
            Assert.Equal(100, deserializedData.Id);
            Assert.Equal("NestedTest", deserializedData.Name);
            Assert.True(deserializedData.IsActive);
            Assert.Equal(2, deserializedData.Items.Count);
            Assert.Equal("item1", deserializedData.Items[0]);
            Assert.Equal("item2", deserializedData.Items[1]);
            Assert.Equal(2, deserializedData.Numbers.Count);
            Assert.Equal(10, deserializedData.Numbers[0]);
            Assert.Equal(20, deserializedData.Numbers[1]);
            Assert.Equal(95.5f, deserializedData.Score);

            // 验证嵌套对象 / Verify nested object
            Assert.NotNull(deserializedData.NestedData);
            Assert.Equal(200, deserializedData.NestedData.NestedId);
            Assert.Equal("Nested1", deserializedData.NestedData.NestedName);
            Assert.Equal(2, deserializedData.NestedData.NestedItems.Count);
            Assert.Equal("nested1", deserializedData.NestedData.NestedItems[0]);
            Assert.Equal("nested2", deserializedData.NestedData.NestedItems[1]);

            // 验证嵌套对象列表 / Verify nested object list
            Assert.Equal(2, deserializedData.NestedList.Count);
            Assert.Equal(300, deserializedData.NestedList[0].NestedId);
            Assert.Equal("List1", deserializedData.NestedList[0].NestedName);
            Assert.Single(deserializedData.NestedList[0].NestedItems);
            Assert.Equal("list1", deserializedData.NestedList[0].NestedItems[0]);
            Assert.Equal(400, deserializedData.NestedList[1].NestedId);
            Assert.Equal("List2", deserializedData.NestedList[1].NestedName);
            Assert.Equal(2, deserializedData.NestedList[1].NestedItems.Count);
            Assert.Equal("list2", deserializedData.NestedList[1].NestedItems[0]);
            Assert.Equal("list3", deserializedData.NestedList[1].NestedItems[1]);

            _output.WriteLine("嵌套数据测试通过 / Nested data test passed");
        }

        /// <summary>
        /// 测试嵌套数据的清除功能 / Test nested data clearing functionality
        /// </summary>
        [Fact]
        public void TestNestedDataClear()
        {
            // 创建包含嵌套数据的对象 / Create object with nested data
            var testData = new PoolTestData
            {
                Id = 500,
                Name = "ClearTest",
                NestedData = new NestedTestData
                {
                    NestedId = 600,
                    NestedName = "ToClear",
                    NestedItems = new List<string> { "clear1", "clear2" }
                },
                NestedList = new List<NestedTestData>
                {
                    new NestedTestData { NestedId = 700, NestedName = "ClearList", NestedItems = new List<string> { "clearlist1" } }
                }
            };

            // 验证数据存在 / Verify data exists
            Assert.Equal(500, testData.Id);
            Assert.Equal("ClearTest", testData.Name);
            Assert.NotNull(testData.NestedData);
            Assert.Equal(600, testData.NestedData.NestedId);
            Assert.Single(testData.NestedList);

            // 清除数据 / Clear data
            testData.Clear();

            // 验证所有数据已清除 / Verify all data is cleared
            Assert.Equal(0, testData.Id);
            Assert.Null(testData.Name);
            Assert.Null(testData.NestedData);
            Assert.Empty(testData.NestedList);
            Assert.Empty(testData.Items);
            Assert.Empty(testData.Numbers);
            Assert.Equal(0f, testData.Score);

            _output.WriteLine("嵌套数据清除测试通过 / Nested data clear test passed");
        }
        /// <summary>
        /// 动物接口 / Animal interface
        /// </summary>
        public interface IAnimal : IMessageObject
        {
            /// <summary>
            /// 动物名称 / Animal name
            /// </summary>
            string Name { get; set; }
            
            /// <summary>
            /// 发出声音 / Make sound
            /// </summary>
            /// <returns>声音内容 / Sound content</returns>
            string MakeSound();
        }

        /// <summary>
        /// 狗类 / Dog class
        /// </summary>
        [ProtoContract]
        public class Dog : IAnimal
        {
            /// <summary>
            /// 动物名称 / Animal name
            /// </summary>
            [ProtoMember(1)]
            public string Name { get; set; }

            /// <summary>
            /// 品种 / Breed
            /// </summary>
            [ProtoMember(2)]
            public string Breed { get; set; }

            /// <summary>
            /// 发出声音 / Make sound
            /// </summary>
            /// <returns>汪汪声 / Barking sound</returns>
            public string MakeSound() => "Woof!";

            /// <summary>
            /// 清理对象状态 / Clear object state
            /// </summary>
            public void Clear()
            {
                Name = null;
                Breed = null;
            }
        }

        /// <summary>
        /// 猫类 / Cat class
        /// </summary>
        [ProtoContract]
        public class Cat : IAnimal
        {
            /// <summary>
            /// 动物名称 / Animal name
            /// </summary>
            [ProtoMember(1)]
            public string Name { get; set; }

            /// <summary>
            /// 颜色 / Color
            /// </summary>
            [ProtoMember(2)]
            public string Color { get; set; }

            /// <summary>
            /// 发出声音 / Make sound
            /// </summary>
            /// <returns>喵喵声 / Meowing sound</returns>
            public string MakeSound() => "Meow!";

            /// <summary>
            /// 清理对象状态 / Clear object state
            /// </summary>
            public void Clear()
            {
                Name = null;
                Color = null;
            }
        }

        /// <summary>
        /// 动物容器类 / Animal container class
        /// </summary>
        [ProtoContract]
        public class AnimalContainer : IMessageObject
        {
            /// <summary>
            /// 动物列表 / Animal list
            /// </summary>
            [ProtoMember(1)]
            public List<IAnimal> Animals { get; set; } = new List<IAnimal>();

            /// <summary>
            /// 容器名称 / Container name
            /// </summary>
            [ProtoMember(2)]
            public string ContainerName { get; set; }

            /// <summary>
            /// 清理对象状态 / Clear object state
            /// </summary>
            public void Clear()
            {
                Animals?.Clear();
                ContainerName = null;
            }
        }

        /// <summary>
        /// 测试对象池与继承关系的兼容性 / Test object pool compatibility with inheritance
        /// </summary>
        [Fact]
        public void TestObjectPoolWithInheritance()
        {
            // 从对象池获取对象 / Get object from pool
            var dog = MessageObjectPoolHelper.Get<Dog>();
            dog.Name = "Rex";
            dog.Breed = "German Shepherd";
            
            // 序列化 / Serialize
            var serializedData = ProtoBufSerializerHelper.Serialize(dog);
            
            // 创建新的对象用于反序列化 / Create new object for deserialization
            var targetDog = MessageObjectPoolHelper.Get<Dog>();
            
            // 使用合并模式反序列化 / Deserialize using merge mode
            var result = ProtoBufSerializerHelper.Deserialize(serializedData, targetDog);
            
            Assert.Equal("Rex", result.Name);
            Assert.Equal("German Shepherd", result.Breed);
            Assert.Same(targetDog, result); // 确保是同一个对象 / Ensure it's the same object
            
            // 归还对象到池中 / Return objects to pool
            MessageObjectPoolHelper.Return(dog);
            MessageObjectPoolHelper.Return(result);
            
            _output.WriteLine("Object pool with inheritance test completed successfully");
        }
    }
}