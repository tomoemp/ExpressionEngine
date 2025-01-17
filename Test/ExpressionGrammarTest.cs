﻿using System.Collections.Generic;
using System.Threading.Tasks;
using ExpressionEngine;
using ExpressionEngine.Functions.Base;
using ExpressionEngine.Functions.CustomException;
using NUnit.Framework;

namespace Test
{
    [TestFixture]
    public class ExpressionGrammarTest
    {
        private ExpressionGrammar _expressionGrammar;
        private DummyFunction _dummyFunction;

        [SetUp]
        public void SetUp()
        {
            _dummyFunction = new DummyFunction();

            var functions = new List<IFunction> {_dummyFunction};

            _expressionGrammar = new ExpressionGrammar(functions);
        }

        [Test]
        public async Task IndicesTest()
        {
            _dummyFunction.ValueContainer = new ValueContainer(new Dictionary<string, ValueContainer>
            {
                {
                    "prop1", new ValueContainer(new Dictionary<string, ValueContainer>
                    {
                        {
                            "prop2", new ValueContainer(new Dictionary<string, ValueContainer>
                            {
                                {"prop3", new ValueContainer("value")}
                            })
                        }
                    })
                }
            });
            const string expressionString = "@dummyFunction()['prop1'].prop2['prop3']";

            var result = await _expressionGrammar.EvaluateToString(expressionString);

            Assert.AreEqual("value", result);
        }

        [Test]
        public async Task BooleanTest()
        {
            const string expressionString = "@dummyFunction(true, false)";

            await _expressionGrammar.EvaluateToValueContainer(expressionString);

            Assert.NotNull(_dummyFunction.Parameters);
            Assert.AreEqual(2, _dummyFunction.Parameters.Length);
            var param1 = _dummyFunction.Parameters[0];
            var param2 = _dummyFunction.Parameters[1];
            Assert.NotNull(param1);
            Assert.NotNull(param2);
            Assert.AreEqual(ValueType.Boolean, param1.Type());
            Assert.AreEqual(ValueType.Boolean, param2.Type());
            Assert.AreEqual(true, param1.GetValue<bool>());
            Assert.AreEqual(false, param2.GetValue<bool>());
        }

        [Test]
        public async Task NullConditional()
        {
            _dummyFunction.ValueContainer = new ValueContainer(new Dictionary<string, ValueContainer>());

            const string expressionString = "@dummyFunction()?.name1";

            var result = await _expressionGrammar.EvaluateToValueContainer(expressionString);

            Assert.NotNull(result);
            Assert.AreEqual(ValueType.Null, result.Type());
        }
        
        [Test]
        public async Task IndexOnNonObject()
        {
            _dummyFunction.ValueContainer = new ValueContainer("");

            const string expressionString = "@dummyFunction()?.name1";

            var exception = Assert.ThrowsAsync<InvalidTemplateException>(async () => await _expressionGrammar.EvaluateToValueContainer(expressionString));
            
            Assert.AreEqual("Unable to process template language expressions in action 'Compose' inputs " +
                            "at line 'x' and column 'y': 'The template language expression 'dummyFunction()?.name1' cannot be " +
                            "evaluated because property 'name1' cannot be selected. Property selection is not supported on values " +
                            "of type 'String'.", exception.Message);
        }
    }

    public class DummyFunction : Function
    {
        internal ValueContainer ValueContainer;
        internal ValueContainer[] Parameters;

        public DummyFunction() : base("dummyFunction")
        {
        }

        public override ValueTask<ValueContainer> ExecuteFunction(params ValueContainer[] parameters)
        {
            Parameters = parameters;
            return new ValueTask<ValueContainer>(ValueContainer);
        }
    }
}