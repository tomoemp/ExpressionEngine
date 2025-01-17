﻿using ExpressionEngine.Functions.Base;
using ExpressionEngine.Functions.Implementations.CollectionFunctions;
using ExpressionEngine.Functions.Implementations.ConversionFunctions;
using ExpressionEngine.Functions.Implementations.LogicalComparisonFunctions;
using ExpressionEngine.Functions.Implementations.StringFunctions;
using ExpressionEngine.Functions.Math;
using Microsoft.Extensions.DependencyInjection;

namespace ExpressionEngine
{
    public static class FlowRunnerDependencyExtension
    {
        public static void AddExpressionEngine(this IServiceCollection services)
        {
            services.AddScoped<IExpressionEngine, ExpressionEngine>();
            services.AddScoped<ExpressionGrammar>();

            AddStringFunctions(services);
            AddCollectionFunction(services);
            AddConversionFunction(services);
            AddLogicalComparisonFunctions(services);
            AddMathFunctions(services);

            services.AddTransient<IFunction, LengthFunction>();
            services.AddTransient<IFunction, GreaterFunction>();
        }

        private static void AddStringFunctions(IServiceCollection services)
        {
            services.AddTransient<IFunction, ConcatFunction>();
            services.AddTransient<IFunction, EndsWithFunction>();
            services.AddTransient<IFunction, FormatNumberFunction>();
            services.AddTransient<IFunction, GuidFunction>();
            services.AddTransient<IFunction, IndexOfFunction>();
            services.AddTransient<IFunction, LastIndexOfFunction>();
            services.AddTransient<IFunction, LengthFunction>();
            services.AddTransient<IFunction, ReplaceFunction>();
            services.AddTransient<IFunction, SplitFunction>();
            services.AddTransient<IFunction, StartsWithFunction>();
            services.AddTransient<IFunction, SubstringFunction>();
            services.AddTransient<IFunction, ToLowerFunction>();
            services.AddTransient<IFunction, ToUpperFunction>();
            services.AddTransient<IFunction, TrimFunction>();
        }

        private static void AddCollectionFunction(IServiceCollection services)
        {
            services.AddTransient<IFunction, ContainsFunction>();
            services.AddTransient<IFunction, EmptyFunction>();
            services.AddTransient<IFunction, FirstFunction>();
            services.AddTransient<IFunction, InterSectionFunction>();
            services.AddTransient<IFunction, JoinFunction>();
            services.AddTransient<IFunction, LastFunction>();
            services.AddTransient<IFunction, LengthFunction>();
            services.AddTransient<IFunction, SkipFunction>();
            services.AddTransient<IFunction, TakeFunction>();
            services.AddTransient<IFunction, UnionFunction>();
        }

        private static void AddConversionFunction(IServiceCollection services)
        {
            services.AddTransient<IFunction, ArrayFunction>();
            services.AddTransient<IFunction, Base64Function>();
            services.AddTransient<IFunction, Base64ToBinaryFunction>();
            services.AddTransient<IFunction, Base64ToStringFunction>();
            services.AddTransient<IFunction, BinaryFunction>();
            services.AddTransient<IFunction, BoolFunction>();
            services.AddTransient<IFunction, CreateArrayFunction>();
            services.AddTransient<IFunction, DataUriFunction>();
            services.AddTransient<IFunction, DataUriToBinaryFunction>();
        }

        private static void AddLogicalComparisonFunctions(IServiceCollection services)
        {
            services.AddTransient<IFunction, AndFunction>();
            services.AddTransient<IFunction, EqualFunction>();
            services.AddTransient<IFunction, GreaterFunction>();
            services.AddTransient<IFunction, GreaterOrEqualsFunction>();
            services.AddTransient<IFunction, IfFunction>();
            services.AddTransient<IFunction, LessFunction>();
            services.AddTransient<IFunction, LessOrEqualsFunction>();
            services.AddTransient<IFunction, NotFunction>();
            services.AddTransient<IFunction, OrFunction>();
        }

        private static void AddMathFunctions(IServiceCollection services)
        {
            services.AddTransient<IFunction, AndFunction>();
            services.AddTransient<IFunction, DivFunction>();
            services.AddTransient<IFunction, MaxFunction>();
            services.AddTransient<IFunction, MinFunction>();
            services.AddTransient<IFunction, ModFunction>();
            services.AddTransient<IFunction, MulFunction>();
            services.AddTransient<IFunction, RandFunction>();
            services.AddTransient<IFunction, RangeFunction>();
            services.AddTransient<IFunction, SubFunction>();
        }
    }
}