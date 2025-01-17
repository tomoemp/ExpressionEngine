﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExpressionEngine.Functions.Base;
using ExpressionEngine.Rules;
using Sprache;

namespace ExpressionEngine
{
    public class ExpressionGrammar
    {
        private readonly Parser<IRule> _method;
        private readonly Parser<Task<ValueContainer>> _input;

        public ExpressionGrammar(IEnumerable<IFunction> functions)
        {
            var functionCollection = functions ?? throw new ArgumentNullException(nameof(functions));

            #region BasicAuxParsers

            Parser<IRule> boolean = Parse.String("true").Select(b => new ConstantRule(new ValueContainer(true)))
                .Or(Parse.String("false").Select(b => new ConstantRule(new ValueContainer(false))));

            Parser<IRule> integer =
                Parse.Digit.AtLeastOnce().Text()
                    .Select(
                        constString => new ConstantRule(new ValueContainer(constString, true))
                    );

            Parser<string> simpleString =
                Parse.AnyChar.Except(Parse.Char('@')).AtLeastOnce().Text();

            Parser<char> escapedCharacters =
                from c in
                    Parse.String("''").Select(n => '\'')
                        .Or(Parse.String("''").Select(n => '\''))
                select c;

            Parser<IRule> stringLiteral =
                from content in Parse.CharExcept('\'').Or(escapedCharacters).Many().Text()
                    .Contained(Parse.Char('\''), Parse.Char('\''))
                select new StringLiteralRule(new ValueContainer(content));

            Parser<string> allowedCharacters =
                Parse.String("@@").Select(_ => '@')
                    .Or(Parse.AnyChar)
                    .Except(Parse.String("@{"))
                    .Select(c => c.ToString());

            #endregion

            var lBracket = Parse.Char('[');
            var rBracket = Parse.Char(']');
            var lParenthesis = Parse.Char('(');
            var rParenthesis = Parse.Char(')');

            Parser<bool> nullConditional = Parse.Char('?').Optional().Select(nC => !nC.IsEmpty);

            Parser<IRule> bracketIndices =
                from nll in nullConditional
                from index in _method.Or(stringLiteral).Or(integer).Contained(lBracket, rBracket)
                select new IndexRule(index, nll);

            Parser<IRule> dotIndices =
                from nll in nullConditional
                from dot in Parse.Char('.')
                from index in Parse.AnyChar.Except(
                    Parse.Chars('[', ']', '{', '}', '(', ')', '@', ',', '.', '?')
                ).Many().Text()
                select new IndexRule(new StringLiteralRule(new ValueContainer(index)), nll);

            Parser<IRule> argument =
                from arg in Parse.Ref(() => _method.Or(stringLiteral).Or(integer).Or(boolean))
                select arg;

            Parser<IOption<IEnumerable<IRule>>> arguments =
                from args in argument.Token().DelimitedBy(Parse.Char(',')).Optional()
                select args;

            Parser<IRule> function =
                from mandatoryLetter in Parse.Letter
                from rest in Parse.LetterOrDigit.Many().Text()
                from args in arguments.Contained(lParenthesis, rParenthesis)
                select new ExpressionRule(functionCollection, mandatoryLetter + rest,
                    args.IsEmpty
                        ? null
                        : args.Get());

            _method =
                Parse.Ref(() =>
                    from func in function
                    from indexes in bracketIndices.Or(dotIndices).Many()
                    select indexes.Aggregate(func, (acc, next) => new AccessValueRule(acc, next)));

            Parser<ValueTask<ValueContainer>> enclosedExpression =
                _method.Contained(
                        Parse.String("@{"),
                        Parse.Char('}'))
                    .Select(x => x.Evaluate());

            Parser<Task<ValueContainer>> expression =
                Parse.Char('@').SelectMany(at => _method, async (at, method) => await method.Evaluate());

            Parser<string> allowedString =
                from t in simpleString.Or(allowedCharacters).Many()
                select string.Concat(t);

            Parser<Task<ValueContainer>> joinedString =
                from e in (
                        from preFix in allowedString
                        from exp in enclosedExpression.Optional()
                        select exp.IsEmpty ? preFix : preFix + exp.Get())
                    .Many()
                select Task.FromResult(new ValueContainer(string.Concat(e)));

            _input = expression.Or(joinedString);
        }

        public async ValueTask<string> EvaluateToString(string input)
        {
            var output = await _input.Parse(input);

            return output.GetValue<string>();
        }

        public async ValueTask<ValueContainer> EvaluateToValueContainer(string input)
        {
            return await _input.Parse(input);
        }
    }
}