﻿using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using SD.Helpers;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace MainApp.Filters
{
    public class UserIsInsadminResourceFilter : Attribute, IResourceFilter
    {

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            if (!context.HttpContext.User.HasCustomClaim("SpecialAuthClaim", "insadmin"))
            {
                context.Result = new NotFoundResult();   
            }
        }

        public void OnResourceExecuted(ResourceExecutedContext context)
        {
        }
    }
}
