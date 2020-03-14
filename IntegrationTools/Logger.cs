using System;
using System.Text;
using System.Timers;
using IntegrationTools.FileTools;
using System.IO;
using System.Threading;

namespace IntegrationTools
{
    public class Logger : IDisposable
    {
        StringBuilder _logsb = new StringBuilder();
        StringBuilder _exlogsb = new StringBuilder();
        IOHelper iohelper = new IOHelper();
        string _logsDirPath = string.Empty;
        string _logsExPath = string.Empty;
        bool _createFile = false;
        const int _maxbuffer = 3072000;
        bool _stopstamp = false;
        const int stamptime = 5000;

        System.Timers.Timer _logtimer = null;
        bool isbusy = false;

        public GetCurrentLogCallBack GetCurrentLog;
        public GetCurrentErrorLogCallBack GetCurrentErrorLog;
        public delegate void GetCurrentLogCallBack(string fixedMessage);
        public delegate void GetCurrentErrorLogCallBack(string fixedMessage);

        public void Stop()
        {
            Stamp("**StopStamp**");
            _stopstamp = true;
            Thread.Sleep(stamptime + 1000);
        }

        public Logger()
        {

        }

        public void LogTick(object sender, ElapsedEventArgs e)
        {
            if (!isbusy)
            {
                try
                {
                    string message = null;

                    lock (_logsb)
                    {
                        message = _logsb.ToString();
                        _logsb.Clear();
                    }

                    if (!string.IsNullOrEmpty(message))
                        CreateMainLog(message, false);

                    string exmessage = null;

                    lock (_exlogsb)
                    {
                        exmessage = _exlogsb.ToString();
                        _exlogsb.Clear();
                    }

                    if (!string.IsNullOrEmpty(exmessage))
                        CreateLogEx(exmessage);

                    if (_stopstamp)
                    {
                        _logtimer.Stop();
                        _logtimer.Enabled = false;
                    }
                    else
                    {
                        if (!_logtimer.Enabled)
                            _logtimer.Enabled = true;
                    }
                }
                catch
                {

                }

                isbusy = false;
            }
        }

        public Logger(string logsDirPath)
        {
            _createFile = true;
            _logsDirPath = logsDirPath;
            DirectoryInfo path = new DirectoryInfo(_logsDirPath);
            if (!path.Exists)
                path.Create();
            _logsExPath = path.Parent.FullName + "\\" + path.Name + "_err";
            DirectoryInfo expath = new DirectoryInfo(_logsExPath);
            if (!expath.Exists)
                expath.Create();


            _logtimer = new System.Timers.Timer(stamptime);
            _logtimer.Elapsed += new ElapsedEventHandler(LogTick);
            _logtimer.Enabled = true;
            _logtimer.Start();
        }

        public void Stamp(string message)
        {
            if (!_stopstamp)
            {
                try
                {
                    lock (_logsb)
                    {
                        _logsb.AppendLine(string.Format("{0} {1}", DateTime.Now.ToString("HH:mm:ss"), message.TrimEnd('\r', '\n')));
                    }
                }
                catch
                {


                }
            }
        }

        public void StampEx(Exception e, string msg)
        {
            StringBuilder sb = new StringBuilder();
            BuildErrorLog(sb, e);
            Stamp(sb.ToString());
            try
            {
                lock (_exlogsb)
                {
                    if (msg == null)
                        _exlogsb.AppendLine(string.Format("{0},{1}", DateTime.Now.ToString("HH:mm:ss"), e.Message));
                    else
                        _exlogsb.AppendLine(string.Format("{0},{1} MSG:{2}", DateTime.Now.ToString("HH:mm:ss"), e.Message, msg));
                }
            }
            catch
            {

            }
        }

        public void StampEx(Exception e)
        {
            StampEx(e, null);
        }

        public void BuildErrorLog(StringBuilder sb, Exception e)
        {
            sb.Append(e.Message);
            sb.Append(e.StackTrace);
            if (e.InnerException != null)
                sb.Append(e.InnerException);
        }

        public StringBuilder Stamp(StringBuilder sb, string message)
        {
            return sb.AppendLine(string.Format("{0} {1}", DateTime.Now.ToString("HH:mm:ss"), message));
        }

        public StringBuilder StampSub(StringBuilder sb, string message)
        {
            return sb.AppendLine(string.Format("   {0}", message));
        }

        public string StampSub(string message)
        {
            return CreateMainLog(message, true);
        }

        public StringBuilder StampBuilder(StringBuilder sb)
        {
            CreateMainLog(sb.ToString(), false);
            return sb;
        }

        private void CreateLogEx(string logRecord)
        {
            string filePath = null;

            if (_createFile)
                filePath = iohelper.CreateFile(_logsExPath, _maxbuffer);

            if (_createFile)
            {
                iohelper.AppendToFile(filePath, logRecord.Trim());
            }
            if (GetCurrentErrorLog != null)
                GetCurrentErrorLog(logRecord);
        }

        private string CreateMainLog(string logRecord, bool isSub)
        {
            string filePath = null;
            string fixMessage = string.Empty;

            if (_createFile)
                filePath = iohelper.CreateFile(_logsDirPath, _maxbuffer);

            if (filePath == null)
                return "Err Creating File";

            fixMessage = isSub ? string.Format("   {0}", logRecord.Trim()) : logRecord.Trim();

            if (_createFile)
            {
                iohelper.AppendToFile(filePath, fixMessage.TrimEnd('\r', '\n'));
            }

            return fixMessage;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            if (disposing && _logtimer != null)
            {
                _logtimer.Close();
                _logtimer.Dispose();
            }
        }
    }
}
