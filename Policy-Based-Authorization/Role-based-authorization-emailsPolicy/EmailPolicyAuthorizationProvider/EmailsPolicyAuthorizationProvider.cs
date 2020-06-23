using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace authBasics.EmailPolicyAuthorizationProvider
{
    public class EmailsRequireHandler : AuthorizationHandler<EmailsRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context, 
            EmailsRequirement requirement)
        {
            List<String> userAllClaims = 
                context.User.Claims.Select(s=>s.Type).ToList();

            //check is if  userAllClaims contains all requirement items.
            bool containsAll = !requirement.emailList.Except<String>(userAllClaims).Any();

            if (containsAll) 
            {
                context.Succeed(requirement);  
            }
            return Task.CompletedTask;
        }
    }
    public class EmailsRequirement 
        : IAuthorizationRequirement {
        public List<String> emailList { get; }

        public EmailsRequirement(params string[] emails) {
            this.emailList = new List<string>(emails);
        }
    }
}
