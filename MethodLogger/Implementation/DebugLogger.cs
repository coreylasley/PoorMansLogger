using System;
using System.Linq;
using System.Text;
using System.Diagnostics;
using PoorMansLogger.Base;
using PoorMansLogger.Interface;

namespace PoorMansLogger.Implementation
{

    /// <summary>
    /// A quick way to time and log information about blocks of code in DEBUG mode with minimal boilerplate code.
    /// Implements ILogger for VS Output window logging.
    /// Preprocessor Directives are included so code is not executed when the consuming application is NOT in Debug mode
    /// </summary>
    public class DebugLogger : LoggerBase, ILogger
    {

        public string Start(string codeBlockName, params object[] parameterValuesToBLogged)
        {

#if DEBUG
            StringBuilder sb = new StringBuilder();

            if (codeBlockName == null)
            {
                StackTrace stackTrace = new StackTrace();
                codeBlockName = stackTrace.GetFrame(1).GetMethod().Name.Replace("()", "");
            }

            sb.Append(codeBlockName + "(");

            sb.Append(ParamValuesToString(parameterValuesToBLogged));

            sb.Append(")");

            codeBlockName = sb.ToString();

            if (codeBlockName.Trim() != "")
            {
                string indent = "";
                for (int x = 0; x < IndentNumber; x++)
                {
                    indent += "\t";
                }
                Elements.Add(new LoggerElement(Prefix == "" ? codeBlockName : Prefix + " -> " + codeBlockName, indent, DisplayStartMessages));
            }

#endif
            return codeBlockName;
        }


        public void Stop(string codeBlockName)
        {
            long ret = 0;
#if DEBUG
            if (codeBlockName == null)
            {
                StackTrace stackTrace = new StackTrace();
                codeBlockName = stackTrace.GetFrame(1).GetMethod().Name.Replace("()", "");
            }

            if (codeBlockName.Trim() != "")
            {
                codeBlockName = Prefix == "" ? codeBlockName : Prefix + " -> " + codeBlockName;

                LoggerElement swe = Elements.Where(x => x.MethodName == codeBlockName).FirstOrDefault();
                if (swe != null)
                {
                    ret = swe.Stop();
                    Debug.WriteLine("******** Completed: " + swe.Indent + swe.ShortName + "() took [" + String.Format("{0:n0}", ret) + "ms] to execute.");
                    Elements.Remove(swe);
                }
                else
                {
                    Debug.WriteLine("******** Oops     : " + codeBlockName + "() was not found in the stop watch stack.");
                }
            }
#endif
        }
            

        public void WriteParamValuesToString(params object[] parameterValues)
        {
            WriteMessage(ParamValuesToString(parameterValues));
        }

        public void WriteMessage(string Message)
        {
#if DEBUG
            if (Message != null & Message.Trim() != "")
                Debug.WriteLine("******** Message  : " + Message);
#endif
        }

        /// <summary>
        /// Returns the type of a generic enumerator (currently not used)
        /// </summary>
        /// <param name="enumeratorType"></param>
        /// <returns></returns>
        private Type GetTypeOfEnumerator(Type enumeratorType)
        {
            Type ret = null;

            Type[] tas = enumeratorType.GetGenericArguments();
            foreach (Type ta in tas)
            {
                ret = ta;
            }

            return ret;
        }

    }
}