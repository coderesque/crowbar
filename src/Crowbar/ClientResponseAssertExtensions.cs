﻿using System;
using System.Linq;
using System.Net;
using System.Web;
using System.Xml.Linq;
using CsQuery;

namespace Crowbar
{
    /// <summary>
    /// Defines extensions for the <see cref="ClientResponse"/> type.
    /// </summary>
    public static class ClientResponseAssertExtensions
    {
        /// <summary>
        /// Asserts that the response content type is of the MIME-type 'text/html'.
        /// </summary>
        /// <param name="response">The <see cref="ClientResponse"/> that the assert should be made on.</param>
        /// <param name="assertions">Additional assertions on the CQ object.</param>
        /// <returns>The CQ object.</returns>
        public static CQ ShouldBeHtml(this ClientResponse response, Action<CQ> assertions = null)
        {
            response.ShouldHaveContentType("text/html");

            CQ document;

            try
            {
                document = response.AsCsQuery();
            }
            catch (Exception exception)
            {
                throw AssertException.Create(response, "Failed to convert response body into a CQ object.", exception);
            }

            assertions.TryInvoke(document);

            return document;
        }

        /// <summary>
        /// Asserts that the response content type is of the MIME-type 'application/json' or the specified override.
        /// </summary>
        /// <param name="response">The <see cref="ClientResponse"/> that the assert should be made on.</param>
        /// <param name="assertions">Additional assertions on the JSON object.</param>
        /// <param name="contentType">The expected content type.</param>
        /// <returns>The JSON object.</returns>
        public static dynamic ShouldBeJson(this ClientResponse response, Action<dynamic> assertions = null, string contentType = "application/json")
        {
            response.ShouldHaveContentType(contentType);

            dynamic json;

            try
            {
                json = response.AsJson();
            }
            catch (Exception exception)
            {
                throw AssertException.Create(response, "Failed to convert response body into JSON.", exception);
            }

            if (assertions != null)
            {
                assertions(json);
            }

            return json;
        }

        /// <summary>
        /// Asserts that the response content type is of the MIME-type 'application/xml' or the specified override.
        /// </summary>
        /// <param name="response">The <see cref="ClientResponse"/> that the assert should be made on.</param>
        /// <param name="assertions">Additional assertions on the XML object.</param>
        /// <param name="contentType">The expected content type.</param>
        /// <returns>An XElement.</returns>
        public static XElement ShouldBeXml(this ClientResponse response, Action<XElement> assertions = null, string contentType = "application/xml")
        {
            response.ShouldHaveContentType(contentType);

            XElement xml;

            try
            {
                xml = response.AsXml();
            }
            catch (Exception exception)
            {
                throw AssertException.Create(response, "Failed to convert response body into XML.", exception);
            }

            assertions.TryInvoke(xml);

            return xml;
        }

        /// <summary>
        /// Asserts that the response has the specified content type.
        /// </summary>
        /// <param name="response">The <see cref="ClientResponse"/> that the assert should be made on.</param>
        /// <param name="expectedContentType">The expected content type.</param>
        public static void ShouldHaveContentType(this ClientResponse response, string expectedContentType)
        {
            string actualContentType = response.ContentType ?? string.Empty;
            string[] values = actualContentType.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

            if (!values.Contains(expectedContentType))
            {
                throw AssertException.Create(response, "The content type should have been '{0}' but was '{1}'.", expectedContentType, response.ContentType);
            }
        }

        /// <summary>
        /// Asserts that the response has a cookie with the specified name.
        /// </summary>
        /// <param name="response">The <see cref="ClientResponse"/> that the assert should be made on.</param>
        /// <param name="name">The name of the cookie.</param>
        /// <returns>The cookie.</returns>
        public static HttpCookie ShouldHaveCookie(this ClientResponse response, string name)
        {
            Ensure.NotNullOrEmpty(name, "name");

            var cookie = response.ParseCookieOrDefault(name);
            if (cookie == null)
            {
                throw AssertException.Create(response, "Missing cookie '{0}'.", name);
            }

            return cookie;
        }

        /// <summary>
        /// Asserts that the response has a cookie with the specified name and value.
        /// </summary>
        /// <param name="response">The <see cref="ClientResponse"/> that the assert should be made on.</param>
        /// <param name="name">The name of the cookie.</param>
        /// <param name="expectedValue">The value of the cookie.</param>
        /// <returns>The cookie.</returns>
        public static HttpCookie ShouldHaveCookie(this ClientResponse response, string name, string expectedValue)
        {
            var cookie = response.ShouldHaveCookie(name);
            if (!string.Equals(cookie.Value, expectedValue, StringComparison.Ordinal))
            {
                throw AssertException.Create(response, "The value of cookie '{0}' should have been '{1}' but was '{2}'.", name, expectedValue, cookie.Value);
            }

            return cookie;
        }

        /// <summary>
        /// Asserts that the response does not have a cookie with the specified name.
        /// </summary>
        /// <param name="response">The <see cref="ClientResponse"/> that the assert should be made on.</param>
        /// <param name="name">The name of the cookie.</param>
        public static void ShouldNotHaveCookie(this ClientResponse response, string name)
        {
            Ensure.NotNullOrEmpty(name, "name");

            var cookie = response.ParseCookieOrDefault(name);
            if (cookie != null)
            {
                throw AssertException.Create(response, "Unexpected cookie '{0}'.", name);
            }
        }

        /// <summary>
        /// Asserts that the response has the specified header.
        /// </summary>
        /// <param name="response">The <see cref="ClientResponse"/> that the assert should be made on.</param>
        /// <param name="name">The name of the header.</param>
        public static void ShouldHaveHeader(this ClientResponse response, string name)
        {
            Ensure.NotNullOrEmpty(name, "name");

            string actualValue = response.Headers[name];
            if (actualValue == null)
            {
                throw AssertException.Create(response, "Missing header '{0}'.", name);
            }
        }

        /// <summary>
        /// Asserts that the response has the specified header.
        /// </summary>
        /// <param name="response">The <see cref="ClientResponse"/> that the assert should be made on.</param>
        /// <param name="name">The name of the header.</param>
        /// <param name="expectedValue">The value of the header.</param>
        public static void ShouldHaveHeader(this ClientResponse response, string name, string expectedValue)
        {
            Ensure.NotNullOrEmpty(name, "name");

            string actualValue = response.Headers[name];
            if (!string.Equals(actualValue, expectedValue, StringComparison.Ordinal))
            {
                throw AssertException.Create(response, "The value of header '{0}' should have been '{1}' but was '{2}'.", name, expectedValue, actualValue);
            }
        }

        /// <summary>
        /// Asserts that the response does not have the specified header.
        /// </summary>
        /// <param name="response">The <see cref="ClientResponse"/> that the assert should be made on.</param>
        /// <param name="name">The name of the header.</param>
        public static void ShouldNotHaveHeader(this ClientResponse response, string name)
        {
            Ensure.NotNullOrEmpty(name, "name");

            string actualValue = response.Headers[name];
            if (actualValue != null)
            {
                throw AssertException.Create(response, "Unexcepted header '{0}'.", name);
            }
        }

        /// <summary>
        /// Asserts that a permanent redirect to a specified location took place.
        /// </summary>
        /// <param name="response">The <see cref="ClientResponse"/> that the assert should be made on.</param>
        /// <param name="location">The location that should have been redirected to.</param>
        public static void ShouldHavePermanentlyRedirectTo(this ClientResponse response, string location)
        {
            response.ShouldHaveRedirectedTo(location, HttpStatusCode.MovedPermanently);
        }

        /// <summary>
        /// Asserts that a temporarily redirect to a specified location took place.
        /// </summary>
        /// <param name="response">The <see cref="ClientResponse"/> that the assert should be made on.</param>
        /// <param name="location">The location that should have been redirected to.</param>
        public static void ShouldHaveTemporarilyRedirectTo(this ClientResponse response, string location)
        {
            response.ShouldHaveRedirectedTo(location, HttpStatusCode.Found);
        }

        /// <summary>
        /// Asserts that the response has the specified status code.
        /// </summary>
        /// <param name="response">The <see cref="ClientResponse"/> that the assert should be made on.</param>
        /// <param name="expectedStatusCode">The expected status code.</param>
        public static void ShouldHaveStatusCode(this ClientResponse response, HttpStatusCode expectedStatusCode)
        {
            if (response.StatusCode != expectedStatusCode)
            {
                throw AssertException.Create(response, "HTTP status code should have been '{0}' but was '{1}'.", expectedStatusCode, response.StatusCode);
            }
        }

        private static void ShouldHaveRedirectedTo(this ClientResponse response, string location, HttpStatusCode expectedStatusCode)
        {
            response.ShouldHaveStatusCode(expectedStatusCode);

            if (response.Headers["Location"] != location)
            {
                throw AssertException.Create(response, "Location should have been '{0}' but was '{1}'.", location, response.Headers["Location"]);
            }
        }

        private static HttpCookie ParseCookieOrDefault(this ClientResponse response, string name)
        {
            string header = response.Headers["Set-Cookie"];
            if (string.IsNullOrWhiteSpace(header))
            {
                return null;
            }

            var container = new CookieContainer();

            var uri = new Uri("http://localhost");

            // parse 'Set-Cookie' header, is there a better way other than taking a dependency on HttpResponse?
            container.SetCookies(uri, header);
            var cookies = container.GetCookies(uri);

            var cookie = cookies[name];
            if (cookie == null)
            {
                return null;
            }

            return new HttpCookie(cookie.Name, cookie.Value)
            {
                Domain = cookie.Domain,
                Expires = cookie.Expires,
                HttpOnly = cookie.HttpOnly,
                Path = cookie.Path,
                Secure = cookie.Secure
            };
        }
    }
}