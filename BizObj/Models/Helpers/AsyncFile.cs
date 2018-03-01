using System;
using System.IO;
using System.Threading;
using System.Web;
using BizObj.Data;
using IO.VFS;

namespace BizObj.Models.Helpers
{
    public class AsyncFile: IAsyncResult
    {
        #region [ Fields ]

        private bool _completed;
        private Object _state;
        private AsyncCallback _callback;
        private HttpContext _context;

        #endregion
        
        #region [ Private Properties ]

        private Guid UserId
        {
            get;
            set;
        }
        
        #endregion

        #region [ Object Properties ]
        bool IAsyncResult.IsCompleted
        {
            get
            {
                return _completed;
            }
        }

        WaitHandle IAsyncResult.AsyncWaitHandle
        {
            get
            {
                return null;
            }
        }

        Object IAsyncResult.AsyncState
        {
            get
            {
                return _state;
            }
        }

        bool IAsyncResult.CompletedSynchronously
        {
            get
            {
                return false;
            }
        }
        #endregion

        #region [ Constructors ]

        public AsyncFile(HttpContext context, AsyncCallback callback, Object state, Guid userID)
        {
            _callback = callback;
            _context = context;
            _state = state;
            _completed = false;
            UserId = userID;
        }

        #endregion


        #region [ Methods ]

        public void StartAsyncWork()
        {
            ThreadPool.QueueUserWorkItem(StartAsyncTask, null);
        }

        private void StartAsyncTask(Object workItemState)
        {
            HttpRequest request = _context.Request;

            string fileID = request["id"];
            int id = 0;
            if (!String.IsNullOrEmpty(fileID) && Int32.TryParse(fileID, out id))
            {
                try
                {
                    if (id > 0)
                    {
                        FileVFS fileVFS = new FileVFS(Config.ConnectionString, id);
                        _context.Response.ContentType = fileVFS.Extension.MIMEType.Value;
                        
                        string fileName = String.Format("inline;FileName=\"{0}.{1}\"", fileVFS.Name, fileVFS.Extension);
                        _context.Response.AddHeader("content-disposition", fileName);

                        _context.Response.BinaryWrite(fileVFS.FileContent);
                    }
                    _context.Response.Flush();
                }
                catch (Exception e)
                {
                    _context.Response.Write(e.Message);
                }
            }

            _completed = true;
            _callback(this);
        }

        #endregion
    }
}