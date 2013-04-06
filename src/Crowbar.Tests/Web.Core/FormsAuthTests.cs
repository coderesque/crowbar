﻿using Crowbar.Mvc.Common;
using NUnit.Framework;

namespace Crowbar.Tests.Web.Core
{
    public class FormsAuthTests : TestBase
    {
        [TestCase("DELETE")]
        [TestCase("GET")]
        [TestCase("POST")]
        [TestCase("PUT")]
        public void Should_be_able_to_perform_a_request_using_forms_authentication(string method)
        {
            Application.Execute(client =>
            {
                var response = client.PerformRequest(method, CrowbarRoute.FormsAuth, x => x.FormsAuth("crowbar"));
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            });
        }
    }
}