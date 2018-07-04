using PossumLabs.Specflow.Core;
using PossumLabs.Specflow.Core.Validations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PossumLabs.Specflow.Selenium
{
    public class TableValidation : Validation
    {
        public TableValidation(List<Dictionary<string, WebValidation>> validations) : base((o)=>AggregatePredicate(o,validations), "")
        {
        }

        private static string AggregatePredicate(object o, List<Dictionary<string, WebValidation>> validations)
        {
            if (!(o is TableElement))
                return "This validation can only work on Tables";

            var table = o as TableElement;

            foreach(var rowValidation in validations)
            {
                if (table.Rows.Where(r => ValidateRow(rowValidation, r)).None())
                    return $"Unable to find row {rowValidation.LogFormat()}";
            }

            return null;
        }

        private static bool ValidateRow(Dictionary<string, WebValidation> validations, Dictionary<string, Element> row)
        {
            if (validations.Keys.Except(row.Keys).Any())
                return false;

            if (validations.Keys
                .Select(column => validations[column].Predicate(row[column]))
                .Any(msg => !string.IsNullOrEmpty(msg)))
                return false;
            return true;
        }
    }
}
