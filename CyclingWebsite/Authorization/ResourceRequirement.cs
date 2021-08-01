using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CyclingWebsite.Authorization
{
    public enum OperationOnResource
    {
        Create,
        Delete,
        Edit,
        Other
    }

    public class ResourceRequirement:IAuthorizationRequirement
    {
        public OperationOnResource Operation {get;set;}

        public ResourceRequirement(OperationOnResource operation)
        {
            Operation = operation;
        }

    }
}
