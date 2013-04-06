using System;
using System.Linq;
using System.Web;

namespace Crowbar
{
    /// <summary>
    /// A common base class for any MVC application proxy.
    /// <typeparam name="THttpApplication">The HTTP application type.</typeparam>
    /// <typeparam name="TDelegate">The script delegate type.</typeparam>
    /// </summary>
    public abstract class ProxyBase<THttpApplication, TDelegate> : MarshalByRefObject, IProxy
        where THttpApplication : HttpApplication
        where TDelegate : class
    {
        private THttpApplication httpApplication;
        private Client client;
        private string testBaseDirectory;

        /// <summary>
        /// Initializes the proxy.
        /// </summary>
        /// <param name="initialize">The initialization code.</param>
        /// <param name="directory">The directory in which the test is run.</param>
        /// <param name="defaults"></param>
        public void Initialize(SerializableDelegate<Func<HttpApplication>> initialize, string directory, SerializableDelegate<Action<HttpPayload>> defaults = null)
        {
            var action = (defaults != null) ? defaults.Delegate : null;
            client = Ensure.NotNull(CreateClient(GetMvcMajorVersion(), action), "client");

            testBaseDirectory = directory;

            httpApplication = initialize.Delegate() as THttpApplication;
            if (httpApplication == null)
            {
                string message = string.Format("The HTTP application is not of type '{0}'.", typeof(THttpApplication));
                throw new InvalidOperationException(message);
            }

            OnApplicationStart(httpApplication);
        }

        /// <inheritdoc />
        public override object InitializeLifetimeService()
        {
            return null;
        }

        /// <summary>
        /// Creates a new client object.
        /// </summary>
        /// <param name="mvcMajorVersion">The major version of the MVC framework</param>
        /// <param name="defaults">The default client context settings.</param>
        /// <returns>A Client.</returns>
        protected virtual Client CreateClient(int mvcMajorVersion, Action<HttpPayload> defaults)
        {
            return new Client(mvcMajorVersion, defaults);
        }

        /// <summary>
        /// Handles exceptions thrown by the proxy.
        /// </summary>
        /// <param name="exception">The exception that should be handled.</param>
        protected void HandleException(Exception exception)
        {
            if (exception.IsSerializable())
            {
                throw new CrowbarException("An exception was thrown during the execution of the test.", exception);
            }

            throw new Exception(string.Format("An exception was thrown during the execution of the test: {0}", exception.Message));
        }

        /// <summary>
        /// Provides the opportunity of customizing the application post application start.
        /// </summary>
        /// <param name="application">The HTTP application.</param>
        protected virtual void OnApplicationStart(THttpApplication application) { }

        /// <summary>
        /// Runs the test script against the MVC application proxy.
        /// </summary>
        /// <param name="script">The test script to be run.</param>
        public void Process(SerializableDelegate<TDelegate> script)
        {
            ProcessCore(script, httpApplication, client, testBaseDirectory);
        }

        /// <summary>
        /// Runs the test script against the MVC application proxy.
        /// </summary>
        /// <param name="script">The test script to be run.</param>
        /// <param name="application">The HTTP application.</param>
        /// <param name="client">The client object.</param>
        /// <param name="testBaseDirectory">The directory in which the test is run.</param>
        protected abstract void ProcessCore(SerializableDelegate<TDelegate> script, THttpApplication application, Client client, string testBaseDirectory);

        private static int GetMvcMajorVersion()
        {
            var mvc = AppDomain.CurrentDomain.GetAssemblies()
                .Select(x => x.GetName())
                .FirstOrDefault(x => x.Name == "System.Web.Mvc");

            if (mvc != null)
            {
                return mvc.Version.Major;
            }

            // Uh-huh... no MVC assembly... should not work.
            return 0;
        }
    }
}