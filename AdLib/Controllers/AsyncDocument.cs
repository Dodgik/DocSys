using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Security;
using System.Xml;
using AdLib.Data;
using AdLib.Models.Helpers;
using AdLib.Models.JqGrid;
using AdLib.Models.Pager;
using Newtonsoft.Json;

namespace AdLib.Controllers
{
    public class AsyncDocument : IAsyncResult
    {

        #region [ Fields ]

        private AsyncCallback _callback;
        private HttpContext _context;

        private string UserName { get; set; }
        private Guid UserId { get; set; }

        #endregion

        #region Implementation of IAsyncResult

        private bool _isCompleted;

        private WaitHandle _asyncWaitHandle = null;

        private object _asyncState;

        private bool _completedSynchronously = false;

        /// <summary>
        /// Возвращает значение, показывающее, выполнена ли асинхронная операция.
        /// </summary>
        /// <returns>
        /// Значение true, если операция завершена, в противном случае — значение false.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public bool IsCompleted
        {
            get
            {
                return _isCompleted;
            }
        }

        /// <summary>
        /// Возвращает дескриптор <see cref="T:System.Threading.WaitHandle"/>, используемый для режима ожидания завершения асинхронной операции.
        /// </summary>
        /// <returns>
        /// Объект <see cref="T:System.Threading.WaitHandle"/>, используемый для режима ожидания завершения асинхронной операции.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public WaitHandle AsyncWaitHandle
        {
            get
            {
                return _asyncWaitHandle;
            }
        }

        /// <summary>
        /// Возвращает определенный пользователем объект, который определяет или содержит в себе сведения об асинхронной операции.
        /// </summary>
        /// <returns>
        /// Определенный пользователем объект, который определяет или содержит в себе сведения об асинхронной операции.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public object AsyncState
        {
            get
            {
                return _asyncState;
            }
        }

        /// <summary>
        /// Возвращает значение, показывающее, синхронно ли закончилась асинхронная операция.
        /// </summary>
        /// <returns>
        /// Значение true, если асинхронная операция завершилась синхронно, в противном случае — значение false.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public bool CompletedSynchronously
        {
            get
            {
                return _completedSynchronously;
            }
        }

        #endregion

        
        #region [ Constructors ]

        public AsyncDocument(HttpContext context, AsyncCallback cb, object extraData, string userName, Guid userId)
        {
            _callback = cb;
            _context = context;
            _asyncState = extraData;
            UserName = userName;
            UserId = userId;
            _isCompleted = false;
        }

        #endregion

        #region [ Methods ]

        public void RunProcessRequestAsync()
        {
            ThreadPool.QueueUserWorkItem(ProcessRequestAsync, null);
        }

        public void ProcessRequestAsync(Object workItemState)
        {
            HttpRequest request = _context.Request;
            HttpResponse response = _context.Response;
            try
            {
                
                string obj = request["obj"];
                string template = request["t"];

                if (!String.IsNullOrWhiteSpace(obj))
                {
                    switch (obj)
                    {
                        case "users":
                            new UserScope(_context, UserId).ParceRequest();
                            break;
                    }
                }
                else if (!String.IsNullOrWhiteSpace(template))
                {
                    
                }
            }
            catch (Exception e)
            {
                response.Write(e.Message);
                response.StatusCode = (int) HttpStatusCode.UnsupportedMediaType;
            }
            finally
            {
                response.Flush();
                
                _isCompleted = true;
                _callback(this);
            }
        }


        #region [ Users ]
        
        #endregion
        
        #endregion
    }

    [Serializable]
    public class ProcessingResult
    {
        public bool Success { get; set; }

        private string _message = String.Empty;
        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
            }
        }

        private string _data = String.Empty;
        public string Data
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value;
            }
        }

        public ProcessingResult()
        {
            
        }

        public ProcessingResult(bool success, string message)
        {
            Success = success;
            Message = message;
        }
    }
}