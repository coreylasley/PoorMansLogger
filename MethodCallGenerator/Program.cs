using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Linq;


namespace MethodCallGenerator
{
    class Program
    {
        /* This is part of a concept I came up with that would facilitate the generation of a string
         * representation of a method call with parameter values. This could be used for logging method calls in an application.
         * The Main() method is the test of this functionality.
         * 
         * The test runs about 1ms per GenerateMethodCallString() + Debug.WriteLine() call.
         * Test machine: Intel(R) Core(TM) i5-2400 CPU @ 3.10Ghz / Windows 10 x64
         * 
         * NOTES: Using StringBuilder is dramatically faster than standard string concatenation (no kidding, right?)
         */

        static void Main(string[] args)
        {
            // Lets create some Lists and objects to use in our Method Call generator
            List<string> list = new List<string>();
            list.Add("This");
            list.Add("Is");
            list.Add("A");
            list.Add("Test");

            Dude c = new Dude();

            List<DateTime> dts = new List<DateTime>();
            dts.Add(DateTime.Now);
            dts.Add(DateTime.Now.AddDays(150));

            List<int> ints = new List<int>();
            for (int i = 0; i < 10000; i++)
            {
                ints.Add(i);
            }

            // Lets run the speed test 25 times
            for (int y = 0; y < 25; y++)
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();

                // For this speed test, lets generate method calls and write them to the Debug window in VS, 
                // while writing the elapsed time to the Console, and we will do this 1000 times per test.
                for (int x = 0; x < 1000; x++)
                {
                    // Lets generate a method call string with various parameters
                    Debug.WriteLine(GenerateMethodCallString("TestMethod", 3, 25, 45, "hello", DateTime.Now, list, c, 234.5678, ints, null, "this is a big test to see what happens if we do something like this", dts));
                }

                sw.Stop();
                Console.WriteLine(sw.ElapsedMilliseconds + "ms");
            }
        }


        /// <summary>
        /// Generates a string representing a method call with parameter values including a visual of Lists of common types
        /// </summary>
        /// <param name="methodName">The name of the method to be represented</param>
        /// <param name="maxElementsIfNonNumericList">If a parameter object is a List of non-numeric type, only list up to X number of elements, or use 0 for unlimited</param>
        /// <param name="maxElementsIfNumericList">If a parameter object is a List of a common numeric type, only list up to X number of elements, or use 0 for unlimited</param>
        /// <param name="maxStringLength">Any parameters that are strings will be trimmed to a max length of X, or use 0 for no trimming</param>
        /// <param name="MethodParameterValues">Unlimited parameter values of any type that should be represented in the method call string </param>
        /// <returns>A string that can be used for logging</returns>
        public static string GenerateMethodCallString(string methodName, int maxElementsIfNonNumericList, int maxElementsIfNumericList, int maxStringLength, params object[] MethodParameterValues)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(methodName + "(");

            string v;
            int c = 0;
            foreach (object o in MethodParameterValues)
            {
                c++;
                if (c > 1) sb.Append(", ");

                if (o != null)
                {
                    v = MaxString(o.ToString(), maxStringLength);

                    Type t = o.GetType();
                    //Type i = t.GetInterface("ilist", true); // Note: this slows down the method

                    if (t.Equals(typeof(int)) || t.Equals(typeof(long)) || t.Equals(typeof(decimal)) || t.Equals(typeof(double)))
                        sb.Append(v);

                    else if (t.Equals(typeof(DateTime)) || t.Equals(typeof(TimeSpan)) || t.Equals(typeof(string)))
                        sb.Append("\"" + v + "\"");

                    else if (t.Equals(typeof(List<int>)))
                        sb.Append(GetList((IList<int>)o, false, maxElementsIfNumericList, maxStringLength));

                    else if (t.Equals(typeof(List<string>)))
                        sb.Append(GetList((IList<string>)o, true, maxElementsIfNonNumericList, maxStringLength));

                    else if (t.Equals(typeof(List<long>)))
                        sb.Append(GetList((IList<long>)o, false, maxElementsIfNumericList, maxStringLength));

                    else if (t.Equals(typeof(List<decimal>)))
                        sb.Append(GetList((IList<decimal>)o, false, maxElementsIfNumericList, maxStringLength));

                    else if (t.Equals(typeof(List<double>)))
                        sb.Append(GetList((IList<double>)o, false, maxElementsIfNumericList, maxStringLength));

                    else if (t.Equals(typeof(List<DateTime>)))
                        sb.Append(GetList((IList<DateTime>)o, true, maxElementsIfNonNumericList, maxStringLength));

                    else if (t.Equals(typeof(List<TimeSpan>)))
                        sb.Append(GetList((IList<TimeSpan>)o, true, maxElementsIfNonNumericList, maxStringLength));

                    else
                        sb.Append("[" + t.ToString() + "]");
                }
                else
                {
                    sb.Append("null");
                }
            }

            sb.Append(")");

            return sb.ToString();
        }

        /// <summary>
        /// Trims a string down to maxStringLengh IF maxStringLength is greater than 0
        /// </summary>
        /// <param name="str"></param>
        /// <param name="maxStringLength"></param>
        /// <returns></returns>
        private static string MaxString(string str, int maxStringLength)
        {
            return str.Length > maxStringLength && maxStringLength > 0 ? str.Substring(0, maxStringLength) + " ..." : str;
        }

        /// <summary>
        /// Converts a generic List of objects into a comma delimited string of string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The generic List to be converted</param>
        /// <param name="treatAsString">If TRUE, each element will be wrapped in double-quotes</param>
        /// <param name="maxElements">The maximum number of elements to convert and append to the string</param>
        /// <param name="maxStringLength">Values will be trimmed to a max string length of X, or use 0 for no trimming</param>
        /// <returns>A string that represents the values within a generic List</returns>
        private static string GetList<T>(IList<T> list, bool treatAsString, int maxElements = 0, int maxStringLength = 0)
        {
            StringBuilder sb = new StringBuilder();
            string wrap = treatAsString ? "\"" : "";

            sb.Append("[");
            int c = 0;
            string s = "";
            foreach (var i in list)
            {
                c++;
                if (c > 1) sb.Append(", ");

                sb.Append(wrap + MaxString(i.ToString(), maxStringLength) + wrap);

                if (c == maxElements && maxElements != 0)
                {
                    if (c < list.Count()) sb.Append(", ...");
                    break;
                }
            }
            sb.Append("]");

            return sb.ToString();
        }
    }

    public class Dude
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
