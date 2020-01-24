
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MethodLogger
{

    /// <summary>
    /// A quick way to time blocks of code in DEBUG mode with minimal code
    /// </summary>
    public class DebugLogger : ILogger
    {
        private List<LoggerElement> Elements { get; set; } = new List<LoggerElement>();

        public string Prefix { get; set; }

        public int IndentNumber { get; set; }

        public int MaxElementsIfNonNumericList { get; set; } = 0;
        public int MaxElementsIfNumericList { get; set; } = 0;
        public int MaxStringLength { get; set; } = 0;

        public bool DisplayStartMessages { get; set; } = true;
               


        public string Start(string methodName, params object[] MethodParameterValues)
        {

#if DEBUG
            StringBuilder sb = new StringBuilder();

            if (methodName == null)
            {
                StackTrace stackTrace = new StackTrace();
                methodName = stackTrace.GetFrame(1).GetMethod().Name.Replace("()", "");
            }            

            sb.Append(methodName + "(");

            if (MethodParameterValues != null && MethodParameterValues.Count() > 0)
            {
                string v;
                int c = 0;
                foreach (object o in MethodParameterValues)
                {
                    c++;
                    if (c > 1) sb.Append(", ");

                    if (o != null)
                    {
                        v = MaxString(o.ToString(), MaxStringLength);

                        Type t = o.GetType();
                        //Type i = t.GetInterface("ilist", true); // Note: this slows down the method

                        if (t.Equals(typeof(int)) || t.Equals(typeof(long)) || t.Equals(typeof(decimal)) || t.Equals(typeof(double)))
                            sb.Append(v);

                        else if (t.Equals(typeof(DateTime)) || t.Equals(typeof(TimeSpan)) || t.Equals(typeof(string)))
                            sb.Append("\"" + v + "\"");

                        else if (t.Equals(typeof(List<int>)))
                            sb.Append(GetList((IList<int>)o, false, MaxElementsIfNumericList, MaxStringLength));

                        else if (t.Equals(typeof(List<string>)))
                            sb.Append(GetList((IList<string>)o, true, MaxElementsIfNonNumericList, MaxStringLength));

                        else if (t.Equals(typeof(List<long>)))
                            sb.Append(GetList((IList<long>)o, false, MaxElementsIfNumericList, MaxStringLength));

                        else if (t.Equals(typeof(List<decimal>)))
                            sb.Append(GetList((IList<decimal>)o, false, MaxElementsIfNumericList, MaxStringLength));

                        else if (t.Equals(typeof(List<double>)))
                            sb.Append(GetList((IList<double>)o, false, MaxElementsIfNumericList, MaxStringLength));

                        else if (t.Equals(typeof(List<DateTime>)))
                            sb.Append(GetList((IList<DateTime>)o, true, MaxElementsIfNonNumericList, MaxStringLength));

                        else if (t.Equals(typeof(List<TimeSpan>)))
                            sb.Append(GetList((IList<TimeSpan>)o, true, MaxElementsIfNonNumericList, MaxStringLength));

                        else
                            sb.Append("[" + t.ToString() + "]");
                    }
                    else
                    {
                        sb.Append("null");
                    }
                }
            }

            sb.Append(")");

            methodName = sb.ToString();

            if (methodName.Trim() != "")
            {
                string indent = "";
                for (int x = 0; x < IndentNumber; x++)
                {
                    indent += "\t";
                }
                Elements.Add(new LoggerElement(Prefix == "" ? methodName : Prefix + " -> " + methodName, indent, DisplayStartMessages));
            }

#endif
            return methodName;
        }


        public void Stop(string MethodName)
        {
            long ret = 0;
#if DEBUG
            if (MethodName == null)
            {
                StackTrace stackTrace = new StackTrace();
                MethodName = stackTrace.GetFrame(1).GetMethod().Name.Replace("()", "");
            }

            if (MethodName.Trim() != "")
            {
                MethodName = Prefix == "" ? MethodName : Prefix + " -> " + MethodName;

                LoggerElement swe = Elements.Where(x => x.MethodName == MethodName).FirstOrDefault();
                if (swe != null)
                {
                    ret = swe.Stop();
                    Debug.WriteLine("******** Completed: " + swe.Indent + swe.ShortName + "() took [" + String.Format("{0:n0}", ret) + "ms] to execute.");
                    Elements.Remove(swe);
                }
                else
                {
                    Debug.WriteLine("******** Oops     : " + MethodName + "() was not found in the stop watch stack.");
                }
            }
#endif
        }


        public void WriteMessage(string Message)
        {
#if DEBUG
            if (Message != null & Message.Trim() != "")
                Debug.WriteLine("******** Message  : " + Message);
#endif
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
        /// Converts a generic List of objects into a comma delimited string of string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The generic List to be converted</param>
        /// <param name="treatAsString">If TRUE, each element will be wrapped in double-quotes</param>
        /// <param name="maxElements">The maximum number of elements to convert and append to the string</param>
        /// <param name="maxStringLength">Values will be trimmed to a max string length of X, or use 0 for no trimming</param>
        /// <returns>A string that represents the values within a generic List</returns>
        private string GetList<T>(IList<T> list, bool treatAsString, int maxElements = 0, int maxStringLength = 0)
        {
            StringBuilder sb = new StringBuilder();
            string wrap = treatAsString ? "\"" : "";

            sb.Append("[");
            int c = 0;
            
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


        private class LoggerElement
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

            public long Stop()
            {
                Watch.Stop();
                return Watch.ElapsedMilliseconds;
            }
        }

    }
}
