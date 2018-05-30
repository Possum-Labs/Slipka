using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using System.Reflection;

namespace PossumLabs.Specflow.Core
{
    public class ValidationFactory
    {
        public ValidationFactory(Interpeter interpeter)
        {
            Interpeter = interpeter;
        }

        //TODO: refactor to utlility class
        IEnumerable<string> splitter(string name) => name.Split(new[] { '.', ' ' });

        //TODO: refactor to utlility class
        private object resolve<T>(T source, string name)
        {
            object item=source;
            var path = splitter(name);
            foreach(var piece in path)
            {
                if (item == null)
                    throw new ValidationException($"the property '{piece}' of '{name}' does not exist on null");
                var properties = item.GetType().GetProperties();
                var property = properties.FirstOrDefault(p => p.Name.ToUpper() == piece.ToUpper());
                if (property == null)
                    throw new GherkinException($"The path '{name}' cannot be resolved, the piece '{piece}' does not match any of these options {properties.LogFormat(p=>p.Name)}");
                item = property.GetValue(item);
            }
            return item;
        }

        public Validation Create(string constructor, string field = null)
            => new Validation(o =>
            {
                if (field != null)
                    o = resolve(o,field);
                if (MakePredicate(constructor).Invoke(o) != true)
                    return $" the value was '{o}' wich was not '{constructor}'";
                return null;
             });

        private Interpeter Interpeter;

        #region regular expressions
        // "== 1"   "~= 2"   "!= 1"    ">= 1"    "> 2"   "< 1"   "<= 2"
        private Regex isTest = new Regex(@"^([=~!><]=|[><]) ?([0-9,\.\-]+) ?%?$", RegexOptions.Compiled);

        // /regex/
        private Regex isRegex = new Regex(@"^/(.*)/$", RegexOptions.Compiled);

        // 'litteral'
        // "litteral"
        private Regex isLitteral = new Regex(@"^['""](.*)['""]$", RegexOptions.Compiled);
        private Regex findLitterals = new Regex(@"{[\w\.]+}", RegexOptions.Compiled);

        // `stuff${variable}stuff`
        private Regex isSubstituted = new Regex(@"^`(.*)`$", RegexOptions.Compiled);

        // 1
        // 0.1
        // 1,000,000.1
        // 0.100
        private Regex isNumber = new Regex(@"^([0-9\.,])+$", RegexOptions.Compiled);

        // 1%
        // 1.00%
        private Regex isPercentage = new Regex(@"^([0-9\.,]+) ?%$", RegexOptions.Compiled);

        // $.01
        // -$.01
        // $(1.10)
        // ($1.10)
        // $ .01
        // -$ .01
        // $ (1.10)
        // ($ 1.10)
        private Regex isMoney = new Regex(@"^[\p{Sc}\(\- ]*(\-?[0-9\.,]+)\)?$", RegexOptions.Compiled);
        // \Sc - currency
        #endregion 

        public Predicate<object> MakePredicate(string predicate)
        {
            if (isTest.IsMatch(predicate))
                return ProcessTest(predicate);
            else if (isRegex.IsMatch(predicate))
                return ProcessRegex(predicate);
            else if (isLitteral.IsMatch(predicate))
                return ProcessLitteral(predicate);
            else if (isSubstituted.IsMatch(predicate))
                return ProcessSubstitution(predicate);
            else if (isNumber.IsMatch(predicate))
                return ProcessNumber(predicate);
            else if (isPercentage.IsMatch(predicate))
                return ProcessPercentage(predicate);
            else if (isMoney.IsMatch(predicate))
                return ProcessMoney(predicate);
            else
                return v => Interpeter.Convert<string>(v) == predicate;
        }

        public Predicate<object> ProcessSubstitution(string predicate)
        {
            var match = isSubstituted.Match(predicate).Groups[1].Value;
            foreach (var token in findLitterals.Matches(predicate).Cast<Match>().Select(x => x.Groups[1].Value))
                match = match.Replace("{" + token + "}", Interpeter.Get<string>(token));
            return v => Interpeter.Convert<string>(v) == match;
        }

        public Predicate<object> ProcessMoney(string predicate)
        {
            var number = decimal.Parse(isMoney.Match(predicate).Groups[1].Value);
            if (predicate.Contains("(") || predicate.Contains("-"))
                number *= -1;
            return v => Interpeter.Convert<decimal>(v) == number;
        }

        //TODO: not ideal
        public Predicate<object> ProcessPercentage(string predicate)
        {
            var number = decimal.Parse(isPercentage.Match(predicate).Groups[1].Value);
            return v => Interpeter.Convert<decimal>(v) == number ||
                        Interpeter.Convert<decimal>(v) == (number / 100);
        }

        public Predicate<object> ProcessNumber(string predicate)
            => v => Interpeter.Convert<decimal>(v) == decimal.Parse(predicate);

        public Predicate<object> ProcessLitteral(string predicate)
            => v => isLitteral.Match(predicate).Groups[1].Value == Interpeter.Convert<string>(v);

        public Predicate<object> ProcessRegex(string predicate)
            => v => new Regex(isRegex.Match(predicate).Groups[1].Value).IsMatch(Interpeter.Convert<string>(v));

        public Predicate<object> ProcessTest(string predicate)
        {
            var comparer = isTest.Match(predicate).Groups[1].Value;
            var number = isTest.Match(predicate).Groups[2].Value;
            var target = Decimal.Parse(number);
            var low = (!number.Contains(".")) ?
                (target - new decimal(.5)) :
                (target - new decimal(5 * Math.Pow(10, (number.LastIndexOf('.') - number.Length))));
            var high = (!number.Contains(".")) ?
                (target + new decimal(.5)) :
                (target + new decimal(5 * Math.Pow(10, (number.LastIndexOf('.') - number.Length))));
            switch (comparer)
            {
                case "==":
                    return v => Interpeter.Convert<decimal>(v) == target;
                case "~=":
                    return v => Interpeter.Convert<decimal>(v) >= low && Interpeter.Convert<decimal>(v) < high;
                case "!=":
                    return v => Interpeter.Convert<decimal>(v) != target;
                case ">=":
                    return v => Interpeter.Convert<decimal>(v) >= target;
                case ">":
                    return v => Interpeter.Convert<decimal>(v) > target;
                case "<=":
                    return v => Interpeter.Convert<decimal>(v) <= target;
                case "<":
                    return v => Interpeter.Convert<decimal>(v) < target;
                default:
                    throw new GherkinException($"the comparer '{comparer}' is unknown, only ==, ~=, !=, >=, >, <, <= are supported");
            }
        }
    }
}
