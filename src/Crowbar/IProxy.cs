﻿using System;
using System.Web;

namespace Crowbar
{
    /// <summary>
    /// Provides common functionality for application proxies.
    /// </summary>
    public interface IProxy
    {
        /// <summary>
        /// Initializes the proxy.
        /// </summary>
        /// <param name="initialize">The initialization code.</param>
        /// <param name="directory">The directory in which the test is run.</param>
        /// <param name="defaults">Default HTTP payload settings, if any.</param>
        void Initialize(SerializableDelegate<Func<HttpApplication>> initialize, string directory, IHttpPayloadDefaults defaults = null);
    }
}