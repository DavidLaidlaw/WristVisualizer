using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;

namespace libWrist.ExceptionHandling
{
    public class HandledExceptionManager
    {
        private bool _blnHaveException = false;
        private bool _blnEmailError = true;
        private string _strEmailBody;
        private string _strExceptionType;
        private const string _strDefaultMore = "No further information is available. If the problem persists, contact (contact).";

        public enum UserErrorDefaultButton
        {
            Default = 0,
            Button1 = 1,
            Button2 = 2,
            Button3 = 3
        }

        /// <summary>
        /// replace generic constants in strings with specific values
        /// </summary>
        private static string ReplaceStringVals(string strOutput)
        {
            string strTemp;
            if (strOutput == null)
                strTemp = "";
            else
                strTemp = strOutput;

            strTemp = strTemp.Replace("(app)", Application.ProductName);
            strTemp = strTemp.Replace("(contact)", Application.CompanyName);
            return strTemp;
        }

        /// <summary>
        /// make sure "More" text is populated with something useful
        /// </summary>        
        private string GetDefaultMore(string strMoreDetails)
        {
            if (strMoreDetails == "")
            {
                StringBuilder objStringBuilder = new StringBuilder();
                objStringBuilder.Append(_strDefaultMore);
                objStringBuilder.AppendLine();
                objStringBuilder.AppendLine();
                objStringBuilder.AppendLine("Basic technical information follows: ");
                objStringBuilder.AppendLine("---");
                objStringBuilder.Append(SysInfoToString(true));
                return objStringBuilder.ToString();
            }
            else
                return strMoreDetails;
        }

        /// <summary>
        /// converts exception to a formatted "more" string
        /// </summary>
        /// <param name="objException"></param>
        /// <returns></returns>
        private static string ExceptionToMore(Exception objException)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Detailed technical information follows: ");
            sb.AppendLine("---");
            sb.Append(HandledExceptionManager.ExceptionToString(objException));
            return sb.ToString();
        }

        /// <summary>
        /// perform our string replacements for (app) and (contact), etc etc
        /// also make sure More has default values if it is blank.
        /// </summary>
        private static void ProcessStrings(ref string strWhatHappened, ref string strHowUserAffected, ref string strWhatUserCanDo, ref string strMoreDetails)
        {
            strWhatHappened = ReplaceStringVals(strWhatHappened);
            strHowUserAffected = ReplaceStringVals(strHowUserAffected);
            strWhatUserCanDo = ReplaceStringVals(strWhatUserCanDo);
            strMoreDetails = ReplaceStringVals(strMoreDetails);
        }

        /// <summary>
        /// simplest possible error dialog
        /// </summary>
        public static DialogResult ShowDialog(string strWhatHappened, string strHowUserAffected, string strWhatUserCanDo)
        {
            return ShowDialogInternal(strWhatHappened, strHowUserAffected, strWhatUserCanDo, "", MessageBoxButtons.OK, MessageBoxIcon.Warning, UserErrorDefaultButton.Default);
        }

        /// <summary>
        /// advanced error dialog with Exception object
        /// </summary>
        public static DialogResult ShowDialog(string strWhatHappened, string strHowUserAffected, string strWhatUserCanDo, Exception objException, MessageBoxButtons Buttons, MessageBoxIcon Icon, UserErrorDefaultButton DefaultButton)
        {
            return ShowDialogInternal(strWhatHappened, strHowUserAffected, strWhatUserCanDo, ExceptionToMore(objException), Buttons, Icon, DefaultButton);
        }
        /// <summary>
        /// advanced error dialog with Exception object
        /// </summary>
        public static DialogResult ShowDialog(string strWhatHappened, string strHowUserAffected, string strWhatUserCanDo, Exception objException, MessageBoxButtons Buttons, MessageBoxIcon Icon)
        {
            return ShowDialog(strWhatHappened, strHowUserAffected, strWhatUserCanDo, objException, Buttons, Icon, UserErrorDefaultButton.Default);
        }
        /// <summary>
        /// advanced error dialog with Exception object
        /// </summary>
        public static DialogResult ShowDialog(string strWhatHappened, string strHowUserAffected, string strWhatUserCanDo, Exception objException, MessageBoxButtons Buttons)
        {
            return ShowDialog(strWhatHappened, strHowUserAffected, strWhatUserCanDo, objException, Buttons, MessageBoxIcon.Warning, UserErrorDefaultButton.Default);
        }
        /// <summary>
        /// advanced error dialog with Exception object
        /// </summary>
        public static DialogResult ShowDialog(string strWhatHappened, string strHowUserAffected, string strWhatUserCanDo, Exception objException)
        {
            return ShowDialog(strWhatHappened, strHowUserAffected, strWhatUserCanDo, objException, MessageBoxButtons.OK, MessageBoxIcon.Warning, UserErrorDefaultButton.Default);
        }

        /// <summary>
        /// advanced error dialog with More string
        /// leave "more" string blank to get the default
        /// </summary>
        public static DialogResult ShowDialog(string strWhatHappened, string strHowUserAffected, string strWhatUserCanDo, string strMoreDetails, MessageBoxButtons Buttons, MessageBoxIcon Icon, UserErrorDefaultButton DefaultButton)
        {
            return ShowDialogInternal(strWhatHappened, strHowUserAffected, strWhatUserCanDo, strMoreDetails, Buttons, Icon, DefaultButton);
        }
        /// <summary>
        /// advanced error dialog with More string
        /// leave "more" string blank to get the default
        /// </summary>
        public static DialogResult ShowDialog(string strWhatHappened, string strHowUserAffected, string strWhatUserCanDo, string strMoreDetails, MessageBoxButtons Buttons, MessageBoxIcon Icon)
        {
            return ShowDialogInternal(strWhatHappened, strHowUserAffected, strWhatUserCanDo, strMoreDetails, Buttons, Icon, UserErrorDefaultButton.Default);
        }
        /// <summary>
        /// advanced error dialog with More string
        /// leave "more" string blank to get the default
        /// </summary>
        public static DialogResult ShowDialog(string strWhatHappened, string strHowUserAffected, string strWhatUserCanDo, string strMoreDetails, MessageBoxButtons Buttons)
        {
            return ShowDialogInternal(strWhatHappened, strHowUserAffected, strWhatUserCanDo, strMoreDetails, Buttons, MessageBoxIcon.Warning, UserErrorDefaultButton.Default);
        }
        /// <summary>
        /// advanced error dialog with More string
        /// leave "more" string blank to get the default
        /// </summary>
        public static DialogResult ShowDialog(string strWhatHappened, string strHowUserAffected, string strWhatUserCanDo, string strMoreDetails)
        {
            return ShowDialogInternal(strWhatHappened, strHowUserAffected, strWhatUserCanDo, strMoreDetails, MessageBoxButtons.OK, MessageBoxIcon.Warning, UserErrorDefaultButton.Default);
        }

        /// <summary>
        /// internal method to show error dialog
        /// </summary>
        private static DialogResult ShowDialogInternal(string strWhatHappened, string strHowUserAffected, string strWhatUserCanDo, string strMoreDetails, MessageBoxButtons Buttons, MessageBoxIcon Icon, UserErrorDefaultButton DefaultButton)
        {
            //set default values, etc
            ProcessStrings(ref strWhatHappened, ref strHowUserAffected, ref strWhatUserCanDo, ref strMoreDetails);
            ExceptionDialog objForm = new ExceptionDialog();
            objForm.Text = ReplaceStringVals(objForm.Text);
            objForm.ErrorBox.Text = strWhatHappened;
            objForm.ScopeBox.Text = strHowUserAffected;
            objForm.ActionBox.Text = strWhatUserCanDo;
            objForm.txtMore.Text = strMoreDetails;

            //determine what button text, visibility, and defaults are
            switch (Buttons)
            {
                case MessageBoxButtons.AbortRetryIgnore:
                    objForm.btn1.Text = "&Abort";
                    objForm.btn2.Text = "&Retry";
                    objForm.btn3.Text = "&Ignore";
                    objForm.AcceptButton = objForm.btn2;
                    objForm.CancelButton = objForm.btn3;
                    break;
                case MessageBoxButtons.OK:
                    objForm.btn3.Text = "OK";
                    objForm.btn1.Visible = false;
                    objForm.btn2.Visible = false;
                    objForm.AcceptButton = objForm.btn3;
                    break;
                case MessageBoxButtons.OKCancel:
                    objForm.btn2.Text = "OK";
                    objForm.btn3.Text = "Cancel";
                    objForm.btn1.Visible = false;
                    objForm.AcceptButton = objForm.btn2;
                    objForm.CancelButton = objForm.btn3;
                    break;
                case MessageBoxButtons.RetryCancel:
                    objForm.btn2.Text = "&Retry";
                    objForm.btn3.Text = "Cancel";
                    objForm.btn1.Visible = false;
                    objForm.AcceptButton = objForm.btn2;
                    objForm.CancelButton = objForm.btn3;
                    break;
                case MessageBoxButtons.YesNo:
                    objForm.btn2.Text = "&Yes";
                    objForm.btn3.Text = "&No";
                    objForm.btn1.Visible = false;
                    break;
                case MessageBoxButtons.YesNoCancel:
                    objForm.btn1.Text = "&Yes";
                    objForm.btn2.Text = "&No";
                    objForm.btn3.Text = "Cancel";
                    objForm.CancelButton = objForm.btn3;
                    break;
            }

            //set the proper dialog icon
            switch (Icon)
            {
                case MessageBoxIcon.Exclamation:
                    objForm.PictureBox1.Image = System.Drawing.SystemIcons.Exclamation.ToBitmap();
                    break;
                case MessageBoxIcon.Information:
                    objForm.PictureBox1.Image = System.Drawing.SystemIcons.Information.ToBitmap();
                    break;
                case MessageBoxIcon.Question:
                    objForm.PictureBox1.Image = System.Drawing.SystemIcons.Question.ToBitmap();
                    break;
                case MessageBoxIcon.Error:
                default:
                    objForm.PictureBox1.Image = System.Drawing.SystemIcons.Error.ToBitmap();
                    break;
            }

            // override the default button
            switch (DefaultButton)
            {
                case UserErrorDefaultButton.Button1:
                    objForm.AcceptButton = objForm.btn1;
                    objForm.btn1.TabIndex = 0;
                    break;
                case UserErrorDefaultButton.Button2:
                    objForm.AcceptButton = objForm.btn2;
                    objForm.btn2.TabIndex = 0;
                    break;
                case UserErrorDefaultButton.Button3:
                    objForm.AcceptButton = objForm.btn3;
                    objForm.btn3.TabIndex = 0;
                    break;
            }

            // show the user our error dialog
            return objForm.ShowDialog();
        }


        /// <summary>
        /// exception-safe WindowsIdentity.GetCurrent retrieval returns "domain\username"
        /// per MS, this sometimes randomly fails with "Access Denied" particularly on NT4
        /// </summary>
        private static string CurrentWindowsIdentity()
        {
            try
            {
                return System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            }
            catch (Exception)
            {
                return "";
            }
        }

        /// <summary>
        /// exception-safe "domain\username" retrieval from Environment
        /// </summary>
        private static string CurrentEnvironmentIdentity()
        {
            try
            {
                return Environment.UserDomainName + "\\" + System.Environment.UserName;
            }
            catch (Exception)
            {
                return "";
            }
        }

        /// <summary>
        /// retrieve identity with fallback on error to safer method
        /// </summary>
        private static string UserIdentity()
        {
            string strTemp;
            strTemp = CurrentWindowsIdentity();
            if (strTemp == "")
                strTemp = CurrentEnvironmentIdentity();
            return strTemp;
        }

        /// <summary>
        /// get IP address of this machine
        /// not an ideal method for a number of reasons (guess why!)
        /// but the alternatives are very ugly
        /// </summary>
        /// <returns></returns>
        private static string GetCurrentIP()
        {
            try
            {
                return System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList[0].ToString();
            }
            catch (Exception)
            {
                return "127.0.0.1";
            }
        }

        private static System.Reflection.Assembly ParentAssembly()
        {
            if (System.Reflection.Assembly.GetEntryAssembly() == null)
                return System.Reflection.Assembly.GetCallingAssembly();
            else
                return System.Reflection.Assembly.GetEntryAssembly();
        }

        /// <summary>
        /// exception-safe file attrib retrieval; we don't care if this fails
        /// </summary>
        private static DateTime AssemblyFileTime(System.Reflection.Assembly objAssembly)
        {
            try
            {
                return System.IO.File.GetLastWriteTime(objAssembly.Location);
            }
            catch (Exception)
            {
                return DateTime.MaxValue;
            }
        }

        private static DateTime AssemblyBuildDate(System.Reflection.Assembly objAssembly)
        {
            System.Version objVersion = objAssembly.GetName().Version;
            DateTime dtBuild;

            dtBuild = new DateTime(2000, 1, 1).AddDays(objVersion.Build).AddSeconds(objVersion.Revision * 2);
            if (TimeZone.IsDaylightSavingTime(DateTime.Now, TimeZone.CurrentTimeZone.GetDaylightChanges(DateTime.Now.Year)))
                dtBuild = dtBuild.AddHours(1);
            if (dtBuild > DateTime.Now || objVersion.Build < 730 || objVersion.Revision == 0)
                dtBuild = AssemblyFileTime(objAssembly);

            return dtBuild;
        }


        /// <summary>
        /// enhanced stack trace generator
        /// </summary>
        private static string EnhancedStackTrace(StackTrace objStackTrace)
        {
            return EnhancedStackTrace(objStackTrace, "");
        }

        /// <summary>
        /// enhanced stack trace generator
        /// </summary>
        private static string EnhancedStackTrace(StackTrace objStackTrace, string strSkipClassName)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine("---- Stack Trace ----");

            for (int intFrame = 0; intFrame < objStackTrace.FrameCount; intFrame++)
            {
                StackFrame sf = objStackTrace.GetFrame(intFrame);
                System.Reflection.MemberInfo mi = sf.GetMethod();
                if (strSkipClassName == "" || !mi.DeclaringType.Name.Contains(strSkipClassName))
                    sb.Append(StackFrameToString(sf));
            }
            sb.AppendLine();
            return sb.ToString();
        }


        /// <summary>
        /// enhanced stack trace generator (exception)
        /// </summary>
        private static string EnhancedStackTrace(Exception objException)
        {
            return EnhancedStackTrace(new StackTrace(objException, true));
        }

        /// <summary>
        /// enhanced stack trace generator (no params)
        /// </summary>
        private static string EnhancedStackTrace()
        {
            return EnhancedStackTrace(new StackTrace(true), "ExceptionManager");
        }


        private static string StackFrameToString(StackFrame sf)
        {
            StringBuilder sb = new StringBuilder();
            System.Reflection.MemberInfo mi = sf.GetMethod();

            //build method name
            sb.Append("   ");
            sb.Append(mi.DeclaringType.Namespace);
            sb.Append(".");
            sb.Append(mi.DeclaringType.Name);
            sb.Append(".");
            sb.Append(mi.Name);

            // build method params

            System.Reflection.ParameterInfo[] objParameters = sf.GetMethod().GetParameters();
            sb.Append("(");
            int intParam = 0;
            foreach (System.Reflection.ParameterInfo objParameter in objParameters)
            {
                intParam += 1;
                if (intParam > 1)
                    sb.Append(", ");
                sb.Append(objParameter.Name);
                sb.Append(" As ");
                sb.Append(objParameter.ParameterType.Name);
            }
            sb.AppendLine(")");

            // if source code is available, append location info
            sb.Append("       ");
            if (sf.GetFileName() == null || sf.GetFileName().Length == 0)
            {
                sb.Append(System.IO.Path.GetFileName(ParentAssembly().CodeBase));
                // native code offset is always available
                sb.Append(": N ");
                sb.Append(String.Format("{0:#00000}", sf.GetNativeOffset()));
            }
            else
            {
                sb.Append(System.IO.Path.GetFileName(sf.GetFileName()));
                sb.Append(": line ");
                sb.Append(String.Format("{0:#0000}", sf.GetFileLineNumber()));
                sb.Append(", col ");
                sb.Append(String.Format("{0:#00}", sf.GetFileColumnNumber()));
                // if IL is available, append IL location info
                if (sf.GetILOffset() != StackFrame.OFFSET_UNKNOWN)
                {
                    sb.Append(", IL ");
                    sb.Append(String.Format("{0:#0000}", sf.GetILOffset()));
                }
            }
            sb.AppendLine();
            return sb.ToString();
        }

        /// <summary>
        /// gather some system information that is helpful to diagnosing exception
        /// </summary>
        private static string SysInfoToString(bool blnIncludeStackTrace)
        {
            StringBuilder objStringBuilder = new StringBuilder();

            objStringBuilder.Append("Date and Time:         ");
            objStringBuilder.AppendLine(DateTime.Now.ToString());

            objStringBuilder.Append("Machine Name:          ");
            try
            {
                objStringBuilder.Append(Environment.MachineName);
            }
            catch (Exception e)
            {
                objStringBuilder.Append(e.Message);
            }
            objStringBuilder.AppendLine();

            objStringBuilder.Append("IP Address:            ");
            objStringBuilder.AppendLine(GetCurrentIP());

            objStringBuilder.Append("Current User:          ");
            objStringBuilder.AppendLine(UserIdentity());
            objStringBuilder.AppendLine();

            objStringBuilder.Append("Application Domain:    ");
            try
            {
                objStringBuilder.AppendLine(System.AppDomain.CurrentDomain.FriendlyName);
            }
            catch (Exception e)
            {
                objStringBuilder.AppendLine(e.Message);
            }

            objStringBuilder.Append("Assembly Codebase:     ");
            try
            {
                objStringBuilder.AppendLine(ParentAssembly().CodeBase);
            }
            catch (Exception e)
            {
                objStringBuilder.AppendLine(e.Message);
            }

            objStringBuilder.Append("Assembly Full Name:    ");
            try
            {
                objStringBuilder.AppendLine(ParentAssembly().FullName);
            }
            catch (Exception e)
            {
                objStringBuilder.AppendLine(e.Message);
            }

            objStringBuilder.Append("Assembly Version:      ");
            try
            {
                objStringBuilder.AppendLine(ParentAssembly().GetName().Version.ToString());
            }
            catch (Exception e)
            {
                objStringBuilder.AppendLine(e.Message);
            }

            objStringBuilder.Append("Assembly Build Date:   ");
            try
            {
                objStringBuilder.AppendLine(AssemblyBuildDate(ParentAssembly()).ToString());
            }
            catch (Exception e)
            {
                objStringBuilder.AppendLine(e.Message);
            }
            objStringBuilder.AppendLine();

            if (blnIncludeStackTrace)
                objStringBuilder.Append(EnhancedStackTrace());

            return objStringBuilder.ToString();
        }


        private static string ExceptionToString(Exception objException)
        {
            StringBuilder sb = new StringBuilder();
            if (objException.InnerException != null)
            {
                // sometimes the original exception is wrapped in a more relevant outer exception
                // the detail exception is the "inner" exception
                // see http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dnbda/html/exceptdotnet.asp
                sb.AppendLine("(Inner Exception)");
                sb.AppendLine(ExceptionToString(objException.InnerException));
                sb.AppendLine("(Outer Exception)");
            }

            // get general system and app information
            sb.Append(SysInfoToString(true));

            //  get exception-specific information
            sb.Append("Exception Source:      ");
            try
            {
                sb.AppendLine(objException.Source);
            }
            catch (Exception e)
            {
                sb.AppendLine(e.Message);
            }

            sb.Append("Exception Type:        ");
            try
            {
                sb.AppendLine(objException.GetType().FullName);
            }
            catch (Exception e)
            {
                sb.AppendLine(e.Message);
            }

            sb.Append("Exception Message:     ");
            try
            {
                sb.AppendLine(objException.Message);
            }
            catch (Exception e)
            {
                sb.AppendLine(e.Message);
            }

            sb.Append("Exception Target Site: ");
            try
            {
                sb.AppendLine(objException.TargetSite.Name);
            }
            catch (Exception e)
            {
                sb.AppendLine(e.Message);
            }

            try
            {
                sb.AppendLine(EnhancedStackTrace(objException));
            }
            catch (Exception e)
            {
                sb.AppendLine(e.Message);
            }

            return sb.ToString();
        }
    }
}
