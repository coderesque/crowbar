﻿using Crowbar.Tests.Mvc.Common;
using NUnit.Framework;

namespace Crowbar.Tests.Web.Core
{
    public class JsonBodyTests : TestBase
    {
        [TestCase("DELETE")]
        [TestCase("POST")]
        [TestCase("PUT")]
        public void Should_be_able_to_send_json(string method)
        {
            Execute(client =>
            {
                var response = client.PerformRequest(method, CrowbarRoute.JsonBody, x => x.JsonBody(new { payload = "text" }));
                response.ShouldHaveStatusCode(HttpStatusCode.OK);
            });
        }
    }
}