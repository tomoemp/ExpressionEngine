﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExpressionEngine.Functions.Base;
using ExpressionEngine.Functions.CustomException;

namespace ExpressionEngine.Functions.Implementations.StringFunctions
{
    public class LengthFunction : Function
    {
        public LengthFunction() : base("length")
        {
        }

        public override ValueTask<ValueContainer> ExecuteFunction(params ValueContainer[] parameters)
        {
            if (parameters.Length != 1)
            {
                throw new ArgumentError(parameters.Length > 1 ? "Too many arguments" : "Too few arguments");
            }

            var item = parameters[0];

            return item.Type() switch
            {
                ValueType.Array =>
                    new ValueTask<ValueContainer>(new ValueContainer(parameters[0].GetValue<IEnumerable<ValueContainer>>().Count())),
                ValueType.String => new ValueTask<ValueContainer>(new ValueContainer(parameters[0].GetValue<string>().Length)),
                _ => throw new Exception(
                    "The template language function 'length' expects its parameter to be an array or a string. " +
                    $"The provided value is of type '{item.Type()}'. " +
                    "Please see https://aka.ms/logicexpressions#length for usage details.")
            };
        }
    }
}