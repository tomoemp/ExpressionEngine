﻿using System;
using System.Threading.Tasks;
using ExpressionEngine;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using ValueType = ExpressionEngine.ValueType;

namespace Test
{
    [TestFixture]
    public class ValueContainerJTokenConstructorTest
    {
        [TestCaseSource(nameof(_valueContainerConstructorInput))]
        public async Task ConstructorTest(JToken jToken, ValueType expectedValueType,
            ValueContainer expectedValue)
        {
            var valueContainer = await ValueContainerExtension.CreateValueContainerFromJToken(jToken);
            
            Assert.AreEqual(expectedValueType, valueContainer.Type());
            Assert.AreEqual(expectedValue, valueContainer);
        }

        private static object[] _valueContainerConstructorInput =
        {
            new object[]
            {
                new JValue("Some random string"),
                ValueType.String,
                new ValueContainer("Some random string")
            },
            new object[]
            {
                new JValue(23),
                ValueType.Integer,
                new ValueContainer(23)
            },
            new object[]
            {
                new JValue(25.6),
                ValueType.Float,
                new ValueContainer(25.6)
            },
            new object[]
            {
                new JValue(true),
                ValueType.Boolean,
                new ValueContainer(true)
            },
            new object[]
            {
                new JValue(new Guid("b4a9b9ee-96c3-49c4-871c-bc74870a134a")),
                ValueType.String,
                new ValueContainer("b4a9b9ee-96c3-49c4-871c-bc74870a134a")
            },
            new object[]
            {
                new JValue((object) null),
                ValueType.Null,
                new ValueContainer()
            },
            new object[]
            {
                new JValue(true),
                ValueType.Boolean,
                new ValueContainer(true)
            },
        };
    }
}