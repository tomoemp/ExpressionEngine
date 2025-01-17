﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExpressionEngine.Functions.Base;

namespace ExpressionEngine.Functions.Implementations.CollectionFunctions
{
    public class FirstFunction : Function
    {
        public FirstFunction() : base("first")
        {
        }

        public override ValueTask<ValueContainer> ExecuteFunction(params ValueContainer[] parameters)
        {
            var value = parameters[0];

            return value.Type() switch
            {
                ValueType.String => new ValueTask<ValueContainer>(new ValueContainer(
                    value.GetValue<string>().Substring(0, 1))),
                ValueType.Array => new ValueTask<ValueContainer>(new ValueContainer(
                    value.GetValue<IEnumerable<ValueContainer>>().First())),
                _ => throw new ExpressionEngineException(
                    $"Empty expression can only operate on String or Array types, not {value.Type()}.")
            };
        }
    }
}