﻿using Entities.DatasetEntities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
	public class ApplicationUser : IdentityUser
	{
		public string? FirstName { get; set; }
		public string? LastName { get; set; }
		public bool? IsActive { get; set; }

		public void NormalizeUserNameAndEmail()
		{
			if(string.IsNullOrEmpty(this.Email) == false)
				this.NormalizedEmail = this.Email.ToUpper();

            if (string.IsNullOrEmpty(this.UserName) == false)
                this.NormalizedUserName = this.UserName.ToUpper();
        }
    }
}
