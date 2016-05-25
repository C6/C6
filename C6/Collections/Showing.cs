// This file is part of the C6 Generic Collection Library for C# and CLI
// See https://github.com/C6/C6/blob/master/LICENSE.md for licensing details.

using System;
using System.Diagnostics.Contracts;
using System.Text;

using static C6.Speed;

using SCG = System.Collections.Generic;


namespace C6.Collections
{
    public static class Showing
    {
        public static string Ellipses => "...";

        public static bool Show(object obj, StringBuilder stringbuilder, ref int rest, IFormatProvider formatProvider)
        {
            if (rest <= 0) {
                return false;
            }

            var showable = obj as IShowable;
            if (showable != null) {
                return showable.Show(stringbuilder, ref rest, formatProvider);
            }

            var oldLength = stringbuilder.Length;
            stringbuilder.AppendFormat(formatProvider, "{0}", obj);
            rest -= stringbuilder.Length - oldLength;
            return true;
        }

        //TODO: do not test here at run time, but select code at compile time perhaps by delivering the print type to this method
        public static bool Show<T>(ICollectionValue<T> collectionValue, StringBuilder stringbuilder, ref int rest, IFormatProvider formatProvider)
        {
            string startDelimiter = "{ ", endDelimiter = " }";
            var showIndices = false;
            var showMultiplicities = false;

            var list = collectionValue as IList<T>;
            var collection = collectionValue as ICollection<T>;

            // Select format based on collection type
            if (list != null) {
                startDelimiter = "[ ";
                endDelimiter = " ]";
                showIndices = list.IndexingSpeed == Constant;
            }
            else if (collection != null && collection.AllowsDuplicates) {
                startDelimiter = "{{ ";
                endDelimiter = " }}";
                showMultiplicities = collection.DuplicatesByCounting;
            }

            stringbuilder.Append(startDelimiter);
            rest -= startDelimiter.Length + endDelimiter.Length;

            var first = true;
            var complete = true;

            if (showMultiplicities) {
                foreach (var p in collection.ItemMultiplicities()) {
                    complete = false;

                    if (rest <= 0) {
                        break;
                    }

                    // Only append ", " in-between items
                    if (first) {
                        first = false;
                    }
                    else {
                        stringbuilder.Append(", ", ref rest);
                    }

                    complete = Show(p.Key, stringbuilder, ref rest, formatProvider);
                    if (complete) {
                        stringbuilder.Append($"(*{p.Value})", ref rest);
                    }
                }
            }
            else {
                var index = 0;
                foreach (var x in collectionValue) {
                    complete = false;

                    if (rest <= 0) {
                        break;
                    }

                    // Only append ", " in-between items
                    if (first) {
                        first = false;
                    }
                    else {
                        stringbuilder.Append(", ", ref rest);
                    }

                    if (showIndices) {
                        stringbuilder.Append($"{index++}:", ref rest);
                    }

                    complete = Show(x, stringbuilder, ref rest, formatProvider);
                }
            }

            if (!complete) {
                stringbuilder.Append(Ellipses, ref rest);
            }

            stringbuilder.Append(endDelimiter);

            return complete;
        }

        public static bool Show<TKey, TValue>(SCG.IDictionary<TKey, TValue> dictionary, StringBuilder stringbuilder, ref int rest, IFormatProvider formatProvider)
        {
            throw new NotImplementedException();
            /*
            var sorted = dictionary is ISortedDictionary<K, V>;
            stringbuilder.Append(sorted ? "[ " : "{ ");
            rest -= 4; // Account for "( " and " )"
            var first = true;
            var complete = true;

            foreach (var p in dictionary) {
                complete = false;
                if (rest <= 0)
                    break;
                if (first)
                    first = false;
                else {
                    stringbuilder.Append(", ");
                    rest -= 2;
                }
                complete = Show(p, stringbuilder, ref rest, formatProvider);
            }

            if (!complete) {
                stringbuilder.Append("...");
                rest -= 3;
            }

            stringbuilder.Append(sorted ? " ]" : " }");
            return complete;
            */
        }

        public static string ShowString(IShowable showable, string format, IFormatProvider formatProvider)
        {
            var rest = maxLength(format);
            var stringBuilder = new StringBuilder();
            showable.Show(stringBuilder, ref rest, formatProvider);
            return stringBuilder.ToString();
        }

        #region Private Methods

        /// <summary>
        ///     Append the specified string to the string builder and subtract its length from <paramref name="rest"/>.
        /// </summary>
        /// <param name="stringBuilder">
        ///     The <see cref="StringBuilder"/> to which the string should be appended.
        /// </param>
        /// <param name="string">
        ///     The string to append to the string builder.
        /// </param>
        /// <param name="rest">
        ///     The number of remaining characters from which the length of the string should be subtracted.
        /// </param>
        private static void Append(this StringBuilder stringBuilder, string @string, ref int rest)
        {
            stringBuilder.Append(@string);
            rest -= @string.Length;
        }

        private static int maxLength(string format)
        {
            Contract.Ensures(format != null || Contract.Result<int>() == 80);

            //TODO: validate format string
            return format == null ? 80 : (format.Length > 1 && format.StartsWith("L") ? int.Parse(format.Substring(1)) : int.MaxValue);
        }

        #endregion
    }
}