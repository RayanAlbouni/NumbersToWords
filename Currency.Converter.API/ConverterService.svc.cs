using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace Currency.Converter.API
{
    public class ConverterService : IConverterService
    {
        public string Converter(string value, string lang)
        {
            // parsing and discarding invalid inputs
            // discard empty input
            if (string.IsNullOrWhiteSpace(value))
                return string.Format("Result will be shown here ..");
            // trim spaces from thr input
            value = value.Replace(" ", string.Empty);
            //discard input with '.' separator
            if (value.Contains('.'))
                return string.Format("Error: Invalid separator (Use ',' for cents separator): {0}", value);
            // discard input with more than one separators
            if (value.Split(',').Count() > 2)
                return string.Format("Error: Invalid value, only 1 separator allowed: {0}", value);
            // discard input with cents more than 2 digits
            if (value.Split(',').Count() == 2 && value.Split(',')[1].Length > 2)
                return string.Format("Error: Invalid value, The Minimum number of cents is 01 and the maximum is 99");
            /* parse the string input to double, first we have to replace the ',' with '.' as this is the natural separator
             then we will try to parse the input and set the output to val */
            value = value.Replace(',', '.');
            double val = 0;
            if (!double.TryParse(value, out val))
                return string.Format("Error: Invalid value (not numeric): {0}", value);
            // discard inputs larger than maximum
            if (val > 999999999.99)
                return string.Format("Error: Too long value: {0}, Max value is : 999 999 999,99", value);
            // discard inputs lower than minimum
            if (val < 0)
                return string.Format("Error: Too short value: {0}, Min value is : 0", value);

            /* if we reach this, then the input is valid, now we are going to split the given value 
               for the integer part we will get the floor of the given value converted to int32
               ,and for the decimal part we will subtract the calculated floor value from the given value and since
               the decimal part is 2 digits, we multiply the value by 100 to get the the integer of decimal part */
            int _dollars = Convert.ToInt32(Math.Floor(val));
            int _cents = Convert.ToInt32((val - _dollars) * 100);

            // the word "dollar" or "dollars" if dollars not equal to 1
            // the word "cent" or "cents" if cents not equal to 1
            string txt_dollars = _dollars == 1 ? "dollar" : "dollars";
            string txt_cents = _cents == 1 ? "cent" : "cents";

            // Get the word match for each part and combine the results together
            string dollars = string.Format("{0} {1}", GetWords(_dollars, lang), txt_dollars);
            string cents = _cents > 0 ? string.Format(" and {0} {1}", GetWords(_cents, lang), txt_cents) : "";
            return string.Format("{0}{1}", dollars, cents).Replace("  ", " ");
        }

        private string GetWords(int val,string lang)
        {
            // strings of zero, hundred, thousand and million
            string zero = "zero",hundred = "hundred" , thousand = "thousand", million = "million";
            /* array of single digits from 1 to 9, the first index is empty because array index starts at 0 position
               so when we call the index of a number, we get the correct word of that number */
            string[] singles = new[] { "", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };
            /* array of teen digits, from 10 to 19, this will be called when the length of the number equals to 2 digits 
               and less than 20, get the respective index by getting the division remainder by 10 */
            string[] teen = new[] { "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
            // array of tens, when the length of the number equals to 2 digits and the division remainder by 10 equals to 0.
            string[] tens = new[] { "", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };
            
            // check if the given value equals to 0.
            // [handled values: (0)]
            if (val == 0)
                return zero;
            else
            {
                // get the length of the given number
                var length = Math.Floor(Math.Log10(val) + 1);
                // if the length equals to one, get the single word of the given value
                // [handled values: (1,2,3,4,5,6,7,8,9)]
                if (length == 1)
                    return singles[val];
                // if the length equals to 2 and the given value less than 20, get the teen number of the given value
                // [handled values: (10,11,12,13,14,15,16,17,18,19)]
                else if (length == 2 && val < 20)
                    return teen[val % 10];
                // if the length equals to 2 and the given value less than 20 and division remainder of the given value by 10 equals to 0,
                // that means the given value is a multiple of 10, so get the ten word of the given value from the tens array.
                // [handled values: (20,30,40,50,60,70,80,90)]
                else if (length == 2 && val % 10 == 0)
                    return tens[val / 10];
                // rest of numbers with length of 2, split the value to tens and singls and get the word for each.
                // [handled values: from (21 to 99) except the ones already handled (20,30,40,50,60,70,80,90)]
                else if (length == 2)
                {
                    int _tens = (val / 10) * 10;
                    int _singles = val % 10;
                    return string.Format("{0}-{1}", GetWords(_tens, lang), GetWords(_singles, lang));
                }
                // rest of numbers
                // [handled values: from (100 to 999,999,999)]
                else// if (length > 2)
                {
                    // split the given value to "millions" and "Thousands" and "Hundreds" and the "Rest" of the given value
                    int _millions = val / 1000000;
                    int _thousands = _millions > 0 ? (val - (_millions * 1000000)) / 1000 : val / 1000;
                    int _hundreds = _millions > 0 ? (val - (_millions * 1000000) - (_thousands * 1000)) / 100 : _thousands > 0 ? (val - (_thousands * 1000)) / 100 : val / 100;
                    int _rest = val - (_millions * 1000000) - (_thousands * 1000) - (_hundreds * 100);

                    // if "millions" part, greater than 0, get the word of this part, else set it to empty
                    string millions = _millions > 0 ? string.Format("{0} {1} ", GetWords(_millions, lang), million) : "";
                    // if "thousands" part, greater than 0, get the word of this part, else set it to empty
                    string thousands = _thousands > 0 ? string.Format("{0} {1} ", GetWords(_thousands, lang), thousand) : "";
                    // if "hundreds" part, greater than 0, get the word of this part, else set it to empty
                    string hundreds = _hundreds > 0 ? string.Format("{0} {1} ", GetWords(_hundreds, lang), hundred) : "";
                    // if "rest" part, greater than 0, get the word of this part, else set it to empty
                    string rest = _rest > 0 ? string.Format("{0} ", GetWords(_rest, lang)) : "";
                    // handle UK language by linking the parts with "and"
                    if (lang == "UK")
                    {
                        millions = _millions > 0 ? string.Format("{0} and ", millions): millions;
                        thousands = _thousands > 0 ? string.Format("{0} and ", thousands) : thousands;
                        hundreds = _hundreds > 0 ? string.Format("{0} and ", hundreds) : hundreds;
                        
                        return string.Format("{0}{1}{2}{3}", millions, thousands, hundreds, rest);
                    }
                    // return the results by copining all the parts together.
                    return string.Format("{0}{1}{2}{3}", millions, thousands, hundreds, rest);
                }
            }
        }
    }
}
