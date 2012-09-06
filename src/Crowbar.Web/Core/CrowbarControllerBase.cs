using System;
using System.Web.Mvc;
using Raven.Client;

namespace Crowbar.Web.Core
{
    public abstract class CrowbarControllerBase : Controller
    {
        public static IDocumentStore Store;

        protected ActionResult Assert(Func<bool> assertion)
        {
            return assertion() ? HttpOk() : HttpBadRequest();
        }

        protected ActionResult HttpBadRequest()
        {
            return new HttpStatusCodeResult(400);
        }

        protected ActionResult HttpOk()
        {
            return new HttpStatusCodeResult(200);
        }
    }
}