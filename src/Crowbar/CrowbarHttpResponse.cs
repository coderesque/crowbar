﻿using System;
using System.Collections.Specialized;
using System.IO;

namespace Crowbar
{
    /// <summary>
    /// Represents the response of the HTTP worker.
    /// </summary>
    public class CrowbarHttpResponse
    {
        private readonly NameValueCollection headers;

        /// <summary>
        /// Creates an instance of <see cref="CrowbarHttpResponse"/>.
        /// </summary>
        public CrowbarHttpResponse()
        {
            headers = new NameValueCollection();
            Output = new StringWriter();
        }

        internal StringWriter Output { get; set; }

        /// <summary>
        /// Gets the HTTP status code.
        /// </summary>
        public HttpStatusCode StatusCode { get; internal set; }

        /// <summary>
        /// Gets the HTTP status description.
        /// </summary>
        public string StatusDescription { get; internal set; }

        /// <summary>
        /// Adds an HTTP headers to the response.
        /// </summary>
        /// <param name="name">The name of the header.</param>
        /// <param name="value">The value of the header.</param>
        public void AddHeader(string name, string value)
        {
            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            headers.Add(name, value);
        }

        /// <summary>
        /// Returns the HTTP headers for the response.
        /// </summary>
        /// <returns>The HTTP headers.</returns>
        public NameValueCollection GetHeaders()
        {
            return headers;
        }

        /// <summary>
        /// Returns the response body.
        /// </summary>
        /// <returns>The response body.</returns>
        public string GetResponseBody()
        {
            return Output.ToString();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            using (var writer = new StringWriter())
            {
                writer.WriteLine("HTTP/1.0 {0} {1}", (int)StatusCode, StatusDescription);
                foreach (string name in headers)
                {
                    string value = headers[name];
                    writer.WriteLine("{0}: {1}", name, value);
                }

                writer.WriteLine(Environment.NewLine + Output);

                return writer.ToString();
            }
        }
    }
}