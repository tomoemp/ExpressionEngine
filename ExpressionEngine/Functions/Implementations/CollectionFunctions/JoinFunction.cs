﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExpressionEngine.Functions.Base;

namespace ExpressionEngine.Functions.Implementations.CollectionFunctions
{
    public class JoinFunction : Function
    {
        public JoinFunction() : base("join")
        {
        }

        public override ValueTask<ValueContainer> ExecuteFunction(params ValueContainer[] parameters)
        {
            var collection = parameters[0].GetValue<IEnumerable<ValueContainer>>();
            var delimiter = parameters[1].GetValue<string>();

            return new ValueTask<ValueContainer>(new ValueContainer(collection.Skip(1)
                .Aggregate(collection.First().ToString(), (s, currentVc) => s + delimiter + currentVc)));
        }
    }
}