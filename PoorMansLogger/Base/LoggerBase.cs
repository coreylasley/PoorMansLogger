
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace PoorMansLogger.Base
{
    public abstract class LoggerBase
    {
        protected List<LoggerElement> Elements { get; set; } = new List<LoggerElement>();

        public string Prefix { get; set; }
        public int IndentNumber { get; set; }
        public int MaxElementsIfNonNumericList { get; set; } = 0;
        public int MaxElementsIfNumericList { get; set; } = 0;
        public int MaxStringLength { get; set; } = 0;
        public bool DisplayStartMessages { get; set; } = true;

        /// <summary>
        /// Converts an unlimited number of parameters to a comma delimited string. NOTE: Adding parameterValues introduces what is known as boxing which can degrade performance. 
        /// </summary>
        /// <param name="parameterValues"></param>
        /// <returns></returns>
        public string ParamValuesToString(params object[] parameterValues)
        {
            StringBuilder sb = new StringBuilder();
            if (parameterValues != null && parameterValues.Count() > 0)
            {
                string v;
                int c = 0;
                foreach (object o in parameterValues)
                {
                    c++;
                    if (c > 1) sb.Append(", ");

                    if (o != null)
                    {

                        v = MaxString(o.ToString(), MaxStringLength);

                        Type t = o.GetType();

                        if (t.Equals(typeof(int)) 
                            || t.Equals(typeof(long)) 
                            || t.Equals(typeof(decimal)) 
                            || t.Equals(typeof(double)) 
                            || t.Equals(typeof(bool))
                            || t.Equals(typeof(byte))
                            || t.Equals(typeof(float))
                            || t.Equals(typeof(uint))
                            || t.Equals(typeof(ulong))
                            || t.Equals(typeof(short))
                            || t.Equals(typeof(ushort))
                            || t.Equals(typeof(sbyte)))
                            sb.Append(v);

                        else if (t.Equals(typeof(DateTime)) || t.Equals(typeof(TimeSpan)) || t.Equals(typeof(string)) || t.Equals(typeof(char)))
                            sb.Append("\"" + v + "\"");

                        // As for the following, I havent figured out a way to cast an object to a generic list to avoid all the repetative else ifs 

                        else if (t.Equals(typeof(List<int>)))
                            sb.Append(GetList((IList<int>)o, false));

                        else if (t.Equals(typeof(List<string>)))
                            sb.Append(GetList((IList<string>)o, true));

                        else if (t.Equals(typeof(List<char>)))
                            sb.Append(GetList((IList<char>)o, false));

                        else if (t.Equals(typeof(List<long>)))
                            sb.Append(GetList((IList<long>)o, false));

                        else if (t.Equals(typeof(List<decimal>)))
                            sb.Append(GetList((IList<decimal>)o, false));

                        else if (t.Equals(typeof(List<double>)))
                            sb.Append(GetList((IList<double>)o, false));

                        else if (t.Equals(typeof(List<float>)))
                            sb.Append(GetList((IList<float>)o, false));

                        else if (t.Equals(typeof(List<short>)))
                            sb.Append(GetList((IList<short>)o, false));

                        else if (t.Equals(typeof(List<uint>)))
                            sb.Append(GetList((IList<uint>)o, false));

                        else if (t.Equals(typeof(List<ulong>)))
                            sb.Append(GetList((IList<ulong>)o, false));

                        else if (t.Equals(typeof(List<bool>)))
                            sb.Append(GetList((IList<bool>)o, false));

                        else if (t.Equals(typeof(List<byte>)))
                            sb.Append(GetList((IList<byte>)o, false));

                        else if (t.Equals(typeof(List<sbyte>)))
                            sb.Append(GetList((IList<sbyte>)o, false));

                        else if (t.Equals(typeof(List<DateTime>)))
                            sb.Append(GetList((IList<DateTime>)o, true));

                        else if (t.Equals(typeof(List<TimeSpan>)))
                            sb.Append(GetList((IList<TimeSpan>)o, true));
                    }
                    else
                    {
                        sb.Append("null");
                    }
                }
            }

            return sb.ToString();
        }
        /// <summary>
        /// Trims a string down to maxStringLengh IF maxStringLength is greater than 0
        /// </summary>
        /// <param name="str"></param>
        /// <param name="maxStringLength"></param>
        /// <returns></returns>
        private string MaxString(string str, int maxStringLength)
        {
            return str.Length > maxStringLength && maxStringLength > 0 ? str.Substring(0, maxStringLength) + " ..." : str;
        }

        /// <summary>
        /// Converts a generic List of objects into a comma delimited string of strings
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The generic List to be converted</param>
        /// <param name="treatAsString">If TRUE, each element will be wrapped in double-quotes</param>
        /// <param name="maxElements">The maximum number of elements to convert and append to the string</param>
        /// <param name="maxStringLength">Values will be trimmed to a max string length of X, or use 0 for no trimming</param>
        /// <returns>A string that represents the values within a generic List</returns>
        private string GetList<T>(IList<T> list, bool treatAsString, int? maxElements = null, int? maxStringLength = null)
        {
            StringBuilder sb = new StringBuilder();
            string wrap = treatAsString ? "\"" : "";

            maxElements = maxElements == null ? (treatAsString ? MaxElementsIfNonNumericList : MaxElementsIfNumericList) : maxElements;
            maxStringLength = maxStringLength == null ? MaxStringLength : maxStringLength;

            sb.Append("[");
            int c = 0;

            foreach (var i in list)
            {
                c++;
                if (c > 1) sb.Append(", ");

                sb.Append(wrap + MaxString(i.ToString(), (int)maxStringLength) + wrap);

                if (c == maxElements && maxElements != 0)
                {
                    if (c < list.Count()) sb.Append(", ...");
                    break;
                }
            }
            sb.Append("]");

            return sb.ToString();
        }


        protected class LoggerElement
        {
            public Stopwatch Watch { get; set; } = new Stopwatch();
            public string MethodName { get; set; }
            public string Indent { get; set; }
            public string ShortName { get; set; }

            public LoggerElement(string methodName, string indent, bool displayMessage)
            {
                Indent = indent;
                MethodName = methodName;

                int beforeParen = methodName.IndexOf("(");
                if (beforeParen > 0)
                {
                    ShortName = methodName.Substring(0, beforeParen);
                }

                Watch.Start();
                if (displayMessage) Debug.WriteLine("******** Started  : " + indent + MethodName + (MethodName.Contains("(") ? "" : "()") + " ...");
            }

            public double Stop()
            {
                Watch.Stop();
                return Watch.Elapsed.TotalMilliseconds;
            }
        }

    }
}
