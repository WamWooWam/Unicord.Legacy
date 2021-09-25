using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DiscordLib.Net.Http
{
    public struct HttpProgress
    {
        public long? BytesSent { get; internal set; }
        public long BytesToSend { get; internal set; }
    }

    internal class StreamContentWithProgress : HttpContent
    {
        private const int defaultBufferSize = 4096;

        private Stream content;
        private int bufferSize;
        private bool contentConsumed;
        private IProgress<HttpProgress> progress;

        public StreamContentWithProgress(Stream content, IProgress<HttpProgress> progress) :
            this(content, defaultBufferSize, progress) { }

        public StreamContentWithProgress(Stream content, int bufferSize, IProgress<HttpProgress> progress)
        {
            if (content == null)
            {
                throw new ArgumentNullException("content");
            }
            if (bufferSize <= 0)
            {
                throw new ArgumentOutOfRangeException("bufferSize");
            }

            this.content = content;
            this.bufferSize = bufferSize;
            this.progress = progress;
        }

        protected override async Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            PrepareContent();

            var buffer = new Byte[this.bufferSize];
            var progress = new HttpProgress();
            progress.BytesToSend = content.Length;

            if (this.progress != null)
                this.progress.Report(progress);

            using (content)
            {
                progress.BytesSent = 0;

                int length = 0;
                while ((length = await content.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false)) != 0)
                {
                    await stream.WriteAsync(buffer, 0, length).ConfigureAwait(false);
                    progress.BytesSent += length;

                    if (this.progress != null)
                        this.progress.Report(progress);
                }
            }
        }

        protected override bool TryComputeLength(out long length)
        {
            length = content.Length;
            return true;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                content.Dispose();
            }
            base.Dispose(disposing);
        }


        private void PrepareContent()
        {
            if (contentConsumed)
            {
                // If the content needs to be written to a target stream a 2nd time, then the stream must support
                // seeking (e.g. a FileStream), otherwise the stream can't be copied a second time to a target 
                // stream (e.g. a NetworkStream).
                if (content.CanSeek)
                {
                    content.Position = 0;
                }
                else
                {
                    throw new InvalidOperationException("SR.net_http_content_stream_already_read");
                }
            }

            contentConsumed = true;
        }
    }
}
