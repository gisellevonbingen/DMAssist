using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSocketSharp.Server;

namespace DMAssist.Toonat
{
    public class ToonationManager : IDisposable
    {
        private object StateLock { get; } = new object();

        public ToonationState State { get; private set; }
        private HttpServer Server;

        public ToonationManager()
        {
            this.Server = null;
        }

        public void Start()
        {
            lock (this.StateLock)
            {
                this.Stop();

                this.State = ToonationState.Starting;

                var config = Program.Instance.Configuration.Value;
                var server = this.Server = new HttpServer(config.ToonationWidgetPort);
                server.OnGet += this.OnGet;
                server.OnOptions += this.OnGet;
                server.Start();

                this.State = ToonationState.Started;
            }

        }

        public void Stop()
        {
            lock (this.StateLock)
            {
                this.State = ToonationState.Stopping;

                var server = this.Server;

                if (server != null)
                {
                    server.Stop();
                }

                this.State = ToonationState.Stopped;
            }

        }

        public HttpWebResponse RequestToOriginal(string path, string method)
        {
            var uri = new Uri("https://toon.at" + path);
            var wrequest = (HttpWebRequest)WebRequest.Create(uri);
            wrequest.Method = method;
            wrequest.Host = uri.Host;
            wrequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/56.0.2924.87 Safari/537.36";

            return (HttpWebResponse)wrequest.GetResponse();

        }

        private void OnGet(object sender, HttpRequestEventArgs e)
        {
            var request = e.Request;
            var response = e.Response;

            using (var wresponse = RequestToOriginal(request.Url.LocalPath, request.HttpMethod))
            {
                response.StatusCode = (int)HttpStatusCode.OK;
                response.ContentType = "text/json";

                using (var wstream = wresponse.GetResponseStream())
                {
                    using (var sstream = response.OutputStream)
                    {
                        var buffer = new byte[2048];

                        for (var len = 0; (len = wstream.Read(buffer, 0, buffer.Length)) > 0;)
                        {
                            sstream.Write(buffer, 0, len);
                            response.ContentLength64 += len;
                        }

                    }

                }

            }

            response.Headers["Access-Control-Allow-Origin"] = "*";
            response.Headers["Access-Control-Allow-Methods"] = "GET, OPTIONS, POST";
            response.Headers["Access-Control-Allow-Headers"] = "Content-Type";
        }

        ~ToonationManager()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            this.Stop();
        }

    }

}
