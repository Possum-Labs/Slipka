using PossumLabs.Specflow.Core;
using PossumLabs.Specflow.Core.Validations;
using PossumLabs.Specflow.Core.Variables;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace PossumLabs.Specflow.Selenium
{
    public class WebValidationFactory:ValidationFactory
    {
        public WebValidationFactory(Interpeter interpeter) : base(interpeter)
        {
        }

        new public WebValidation Create(string constructor, string field = null)
            => new WebValidation(o =>
            {
                if (field != null)
                    o = o.Resolve(field);
                if (MakePredicate(constructor).Invoke(o) != true)
                    return $" the value was '{((Element)o).Values.LogFormat()}' wich was not '{constructor}'";
                return null;
            }, constructor);

        public TableValidation Create(List<Dictionary<string,WebValidation>> validation)
            => new TableValidation(validation);

        public override Predicate<object> MakePredicate(string predicate)
        {
            if (Parser.IsElement.IsMatch(predicate))
                return BuildPredicate(predicate,(e)=>e.Tag == Parser.IsElement.Match(predicate).Groups[1].Value);
            if (Parser.IsClass.IsMatch(predicate))
                return BuildPredicate(predicate, (e) => e.Classes.Contains(Parser.IsClass.Match(predicate).Groups[1].Value));
            if (Parser.IsId.IsMatch(predicate))
                return BuildPredicate(predicate, (e) => e.Id == Parser.IsId.Match(predicate).Groups[1].Value);
            return BuildPredicate(predicate, (e) => e.Values.Select(value => base.MakePredicate(predicate).Invoke(value)).Any());
        }

        public Predicate<object> BuildPredicate(string predicate, Func<Element,bool> test)
        {
            return v =>
            {
                if (v is Element)
                {
                    var e = v as Element;
                    return test(e);
                }
                else
                    throw new GherkinException($"the predicate {predicate} only works on Elements");
            };
        }
    }
}
