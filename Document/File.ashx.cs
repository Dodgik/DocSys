using System;
using System.Web;
using System.Web.Security;
using BizObj.Models.Helpers;

namespace Document
{
    /// <summary>
    /// Сводное описание для File
    /// </summary>
    public class File : IHttpAsyncHandler
    {
        #region Implementation of IHttpHandler

        private bool _isReusable = false;

        /// <summary>
        /// Разрешает обработку веб-запросов НТТР для пользовательского элемента HttpHandler, который реализует интерфейс <see cref="T:System.Web.IHttpHandler"/>.
        /// </summary>
        /// <param name="context">Объект <see cref="T:System.Web.HttpContext"/>, предоставляющий ссылки на внутренние серверные объекты (например, Request, Response, Session и Server), используемые для обслуживания HTTP-запросов. </param>
        public void ProcessRequest(HttpContext context)
        {
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            //context.Response.ContentType = "text/plain";
        }

        /// <summary>
        /// Возвращает значение, позволяющее определить, может ли другой запрос использовать экземпляр класса <see cref="T:System.Web.IHttpHandler"/>.
        /// </summary>
        /// <returns>
        /// Значение true, если экземпляр <see cref="T:System.Web.IHttpHandler"/> доступен для повторного использования; в противном случае — значение false.
        /// </returns>
        public bool IsReusable
        {
            get
            {
                return _isReusable;
            }
        }

        #endregion

        #region Implementation of IHttpAsyncHandler

        /// <summary>
        /// Запускает асинхронный вызов обработчика НТТР.
        /// </summary>
        /// <returns>
        /// Объект <see cref="T:System.IAsyncResult"/>, содержащий сведения о состоянии процесса.
        /// </returns>
        /// <param name="context">Объект <see cref="T:System.Web.HttpContext"/>, предоставляющий ссылки на внутренние серверные объекты (например, Request, Response, Session и Server), которые используются для обслуживания HTTP-запросов. </param><param name="cb">Метод <see cref="T:System.AsyncCallback"/>, который вызывается после завершения асинхронного вызова метода.Если значение параметра <paramref name="cb"/> равно null, делегат не вызывается.</param><param name="extraData">Любые дополнительные данные, необходимые для обработки запроса. </param>
        public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
        {
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            //context.Response.ContentType = "text/plain";

            //context.Response.Write("<p>Begin IsThreadPoolThread is " + Thread.CurrentThread.IsThreadPoolThread + "</p>\r\n");
            AsyncFile uploader = null;

            MembershipUser mu = Membership.GetUser();

            if (mu != null)
            {
                uploader = new AsyncFile(context, cb, extraData, (Guid)mu.ProviderUserKey);
                uploader.StartAsyncWork();
            }
            else
            {
                context.Response.Write("You are not logged!");
            }

            return uploader;
        }

        /// <summary>
        /// Предоставляет метод End асинхронного процесса после завершения процесса.
        /// </summary>
        /// <param name="result">Объект <see cref="T:System.IAsyncResult"/>, содержащий сведения о состоянии процесса. </param>
        public void EndProcessRequest(IAsyncResult result)
        {
            //throw new NotImplementedException();
        }

        #endregion
    }
}