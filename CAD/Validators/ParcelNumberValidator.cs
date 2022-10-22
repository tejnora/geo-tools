using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace CAD.Validators
{
    class ParcelNumberValidator : ValidationRule
    {
        public override ValidationResult Validate(object value,
         System.Globalization.CultureInfo cultureInfo)
        {
            string str = value.ToString();
            if(str.Length==0)
                return new ValidationResult(false,"Musí být zadáno číslo.");
            if (str.Contains("/"))
            {
                string[] numbers = str.Split('/');
                if (numbers.Length != 2)
                    return new ValidationResult(false, "Chybi hodnota pred, nebo po lomitku.");
                for (Int32 i = 0; i < numbers[0].Length; i++)
                {
                    if (!char.IsDigit((numbers[0][i])))
                        return new ValidationResult(false, "Před lomítkem smějí být pouze čísla.");
                }
                for (Int32 i = 0; i < numbers[1].Length; i++)
                {
                    if (!char.IsDigit((numbers[1][i])))
                        return new ValidationResult(false, "Za lomítkem smějí být pouze čísla.");
                }
                return ValidationResult.ValidResult;
            }
            foreach (var s in str)
            {
                if(!char.IsDigit(s))
                {
                    return new ValidationResult(false,"Mužou být pouze čísla.");
                }
            }
            return ValidationResult.ValidResult;
        }

    }
}
