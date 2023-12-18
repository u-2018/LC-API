using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LC_API
{
    public static class Utils
    {
        // Thanks Elias https://stackoverflow.com/a/11105164
        /// <summary>
        /// Replaces a string and attempts to keep its casing.
        /// </summary>
        /// <param name="input">The input <see cref="string"/>.</param>
        /// <param name="toReplace">The <see cref="string"/> to replace.</param>
        /// <param name="replacement">The replacement <see cref="string"/>.</param>
        /// <returns>The <see cref="string"/> with all instances of toReplace replaced with replacement.</returns>
        public static string ReplaceWithCase(this string input, string toReplace, string replacement)
        {
            Dictionary<string, string> map = new Dictionary<string, string>
            {
                { toReplace, replacement }
            };

            return input.ReplaceWithCase(map);
        }

        // Thanks Elias https://stackoverflow.com/a/11105164
        /// <summary>
        /// Replaces a string and attempts to keep its casing.
        /// </summary>
        /// <param name="input">The input <see cref="string"/>.</param>
        /// <param name="map">A <see cref="Dictionary{TKey, TValue}"/> of strings to replace and their replacement.</param>
        /// <returns>The completed <see cref="string"/>.</returns>
        public static string ReplaceWithCase(this string input, Dictionary<string, string> map)
        {
            string temp = input;
            foreach (var entry in map)
            {
                string key = entry.Key;
                string value = entry.Value;
                temp = Regex.Replace(temp, key, match =>
                {
                    bool isFirstUpper, isEachUpper, isAllUpper;

                    string sentence = match.Value;
                    char[] sentenceArray = sentence.ToCharArray();

                    string[] words = sentence.Split(' ');

                    isFirstUpper = char.IsUpper(sentenceArray[0]);

                    isEachUpper = words.All(w => char.IsUpper(w[0]) || !char.IsLetter(w[0]));

                    isAllUpper = sentenceArray.All(c => char.IsUpper(c) || !char.IsLetter(c));

                    if (isAllUpper)
                        return value.ToUpper();

                    if (isEachUpper)
                    {
                        string capitalized = Regex.Replace(value, @"\b\w", charMatch => charMatch.Value.ToUpper());
                        return capitalized;
                    }


                    char[] result = value.ToCharArray();
                    result[0] = isFirstUpper
                        ? char.ToUpper(result[0])
                        : char.ToLower(result[0]);
                    return new string(result);
                }, RegexOptions.IgnoreCase);
            }

            return temp;
        }
    }
}
