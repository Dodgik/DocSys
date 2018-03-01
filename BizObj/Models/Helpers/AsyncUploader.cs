using System;
using System.Data.SqlClient;
using System.Threading;
using System.Web;
using System.IO;
using BizObj.Data;
using BizObj.Document;
using BizObj.Models.Document;
using IO.VFS;

namespace BizObj.Models.Helpers
{
    public class AsyncUploader: IAsyncResult
    {
        #region [ Fields ]

        private bool _completed;
        private Object _state;
        private AsyncCallback _callback;
        private HttpContext _context;

        #endregion
        
        #region [ Private Properties ]

        private Guid UserId { get; set; }

        private Worker Worker { get; set; }
        
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

        public AsyncUploader(HttpContext context, AsyncCallback callback, Object state, Guid userID) {
            _callback = callback;
            _context = context;
            _state = state;
            _completed = false;
            UserId = userID;
            Worker = new Worker(null, UserId);
        }

        #endregion


        #region [ Methods ]

        public void StartAsyncWork() {
            ThreadPool.QueueUserWorkItem(StartAsyncTask, null);
        }

        private void StartAsyncTask(Object workItemState) {
            HttpRequest request = _context.Request;

            byte[] buffer = new byte[request.ContentLength];
            string fullFileName = Uri.UnescapeDataString(request.Headers["X-File-Name"]);
            string fileName = Path.GetFileNameWithoutExtension(fullFileName);
            string fileExt = Path.GetExtension(fullFileName);
            string documentIdStr = request["documentid"];
            int fileId = 0;
            Stream inputStream;

            if (String.IsNullOrEmpty(fullFileName) && request.Files.Count <= 0)
            {
                _context.Response.Write("{success:false}");
            } else {
                try {
                    if (fullFileName == null) {
                        inputStream = request.Files[0].InputStream;
                        fullFileName = Path.GetFileName(request.Files[0].FileName);
                        fileName = Path.GetFileNameWithoutExtension(fullFileName);
                        fileExt = Path.GetExtension(fullFileName);
                    } else {
                        inputStream = request.InputStream;
                    }

                    using (BinaryReader br = new BinaryReader(inputStream))
                        br.Read(buffer, 0, buffer.Length);

                    long hashCode = FileVFS.ComputeHash(buffer);

                    SqlConnection connection = new SqlConnection(Config.ConnectionString);
                    try {
                        connection.Open();
                        SqlTransaction trans = null;
                        try {
                            trans = connection.BeginTransaction();

                            bool existSameName = FileVFS.Exists(trans, fullFileName, 0);
                            if (existSameName) {
                                fileId = FileVFS.GetFileID(trans, fullFileName, hashCode, 0);
                            }

                            if (fileId <= 0) {
                                if (existSameName) {
                                    fullFileName = fileName + "_" + hashCode + fileExt;
                                }

                                existSameName = FileVFS.Exists(trans, fullFileName, 0);
                                if (existSameName) {
                                    fileId = FileVFS.GetFileID(trans, fullFileName, hashCode, 0);
                                }
                            }

                            if (fileId <= 0) {
                                FileVFS fileVFS = new FileVFS(Config.ConnectionString, fullFileName, 0, buffer);
                                fileVFS.Create(trans);
                                fileId = fileVFS.ID;
                            }

                            if (fileId > 0 && !String.IsNullOrWhiteSpace(documentIdStr)) {
                                 int documentID;
                                 if (int.TryParse(documentIdStr, out documentID) && documentID > 0) {
                                    DocumentFile df = new DocumentFile(Worker);
                                    df.FileID = fileId;
                                    df.DocumentID = documentID;
                                    df.Insert(trans);
                                }
                            }

                            trans.Commit();
                        } catch (Exception) {
                            if (trans != null)
                                trans.Rollback();
                            throw;
                        }
                    } finally {
                        connection.Close();
                    }

                    _context.Response.Write(String.Format("{{ success: true, fileID: {0}, fileName: '{1}'}}", fileId, fullFileName));
                    _context.Response.Flush();
                } catch (Exception e) {
                    _context.Response.Write("{ success: false, error:\"" + e.Message + "\" }");
                }
            }

            _completed = true;
            _callback(this);
        }

        #endregion
    }
}