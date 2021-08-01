using CyclingWebsite.Entities;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CyclingWebsite.Authorization
{
    public class ResourceRequirementHandler : AuthorizationHandler<ResourceRequirement, Tour>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ResourceRequirement requirement,
            Tour tour)
        {
            if(requirement.Operation == OperationOnResource.Create || requirement.Operation == OperationOnResource.Other)
            {
                context.Succeed(requirement);
            }
            if( requirement.Operation == OperationOnResource.Delete || 
                requirement.Operation == OperationOnResource.Edit)
            {
               var id = int.Parse(context.User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value);
                if (id == tour.UserId)
                    context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
