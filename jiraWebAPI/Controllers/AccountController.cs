using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Microsoft.AspNet.Identity;
using jiraWebAPI.Models;
using Atlassian.Jira;
using System.Linq;
using System.Net.Http;
using System.Collections.Generic;
using System.Web.Http.Cors;
using jiraWebAPI.Helpers;
using System;
namespace jiraWebAPI.Controllers
{
    [EnableCors("*","*","*")]
    [RoutePrefix("api/jira")]
    public class AccountController : ApiController
    {
       
        // POST api/Account/RemoveLogin
        [Route("JiraDetails")]
        [HttpGet]
        public HttpResponseMessage JiraDetails()
        {
            IOrderedQueryable<Issue> Strdata;
             
            if (MemoryCacher.GetValue("jiracache") == null)
            {

                var cr = Helpers.Data.GetJiraCredentials();
                var jiralist = "";
                var jira = Jira.CreateRestClient(cr.Item1.ToString(), cr.Item2.ToString(), cr.Item3.ToString());

                var issues = from i in jira.Issues.Queryable
                             where i.FixVersions == "Release 4.0.3" //&& i.Assignee=="s736348"
                             orderby i.Created
                             select i;


                Strdata = issues;
                MemoryCacher.Add("jiracache", Strdata, System.DateTimeOffset.UtcNow.AddMinutes(10));
                
            }
            else
            {
                var result = MemoryCacher.GetValue("jiracache");
                Strdata = (IOrderedQueryable<Issue>)result;
            }



            return Request.CreateResponse(Strdata);
            
        }

        #region Helpers
        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }
        #endregion
    }
}
