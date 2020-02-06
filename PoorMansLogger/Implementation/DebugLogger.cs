using System;
using System.Linq;
using System.Text;
using System.Diagnostics;
using PoorMansLogger.Base;
using PoorMansLogger.Interface;

namespace PoorMansLogger
{

    /// <summary>
    /// A quick way to time and log information about blocks of code in DEBUG mode with minimal boilerplate code.
    /// Implements ILogger for VS Output window logging. Preprocessor Directives are included so code is not executed 
    /// when the consuming application is NOT in Debug mode. Initially this class was to implement ILogger, however
    /// due to the use of [Conditional("DEBUG")] attributes, this could not be done.
    /// </summary>
    public class DebugLogger : LoggerBase
    {

        // NOTE: We are not going to implement the ILogger interface due to having methods with the [Conditional("DEBUG")] attribute

        public long LastStartID { get; set; } = 0;

        [Conditional("DEBUG")]
        public void StartDebug(string codeBlockName, params object[] parameterValuesToBLogged)
        {
            if (codeBlockName == null)
            {
                StackTrace stackTrace = new StackTrace();
                codeBlockName = stackTrace.GetFrame(1).GetMethod().Name.Replace("()", "");
            }

            LastStartID = Start(codeBlockName, parameterValuesToBLogged);
        }

        public long Start(string codeBlockName, params object[] parameterValuesToBLogged)
        {
            long retObj = 0;

            // We cannot apply a [Conditional("DEBUG")] to a method that returns a value, so we will use the precompiler directive to prevent this from running outside of DEBUG mode
#if DEBUG

            retObj = Stopwatch.GetTimestamp();
            StringBuilder sb = new StringBuilder();

            if (codeBlockName == null)
            {
                StackTrace stackTrace = new StackTrace();
                codeBlockName = stackTrace.GetFrame(1).GetMethod().Name.Replace("()", "");
            }

            sb.Append("(");

            if (parameterValuesToBLogged != null)
                sb.Append(ParamValuesToString(parameterValuesToBLogged));

            sb.Append(")");

            if (codeBlockName != "")
            {
                string indent = "";
                for (int x = 0; x < IndentNumber; x++)
                {
                    indent += "\t";
                }

                Elements.Add(new LoggerElement(retObj, Prefix == "" ? codeBlockName : Prefix + " -> " + codeBlockName, sb.ToString(), indent, DisplayStartMessages));
            }
#endif

            return retObj;
        }

        [Conditional("DEBUG")]
        public void StopDebug(long ID)
        {
            Stop(ID);
        }

        [Conditional("DEBUG")]
        public void StopDebug(string codeBlockName)
        {
            if (codeBlockName == null)
            {
                StackTrace stackTrace = new StackTrace();
                codeBlockName = stackTrace.GetFrame(1).GetMethod().Name.Replace("()", "");
            }

            Stop(Prefix == "" ? codeBlockName : Prefix + " -> " + codeBlockName);
        }

        public double Stop(long ID)
        {
            double ret = 0;

#if DEBUG
            ret = LogAndRemove(Elements.Where(x => x.ID == ID).LastOrDefault());
#endif
            return ret;
        }

        public double Stop(string codeBlockName)
        {
            double ret = 0;
#if DEBUG
            ret = LogAndRemove(Elements.Where(x => x.CodeBlockName == codeBlockName).LastOrDefault());
#endif
            return ret;
        }

        [Conditional("DEBUG")]
        public void WriteParamValuesToString(params object[] parameterValues)
        {
            WriteMessage(ParamValuesToString(parameterValues));
        }

        [Conditional("DEBUG")]
        public void WriteMessage(string Message)
        {
            if (Message != null & Message != "")
                Debug.WriteLine("******** Message  : " + Message);
        }

        private double LogAndRemove(LoggerElement element)
        {
            double ret = 0;

            if (element != null)
            {
                ret = element.Stop();
                Debug.WriteLine("******** Completed: " + element.Indent + element.CodeBlockName + "() took [" + ret + "ms] to execute.");

                Elements.Remove(element);
            }
            else
            {
                Debug.WriteLine("******** Oops     : Element was not found in the stop watch stack.");
            }

            return ret;
        }
               
               
    }
}