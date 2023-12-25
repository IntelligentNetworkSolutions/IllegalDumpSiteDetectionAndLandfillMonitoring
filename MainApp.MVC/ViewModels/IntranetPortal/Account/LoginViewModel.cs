﻿using System.ComponentModel.DataAnnotations;

namespace MainApp.MVC.ViewModels.IntranetPortal.Account
{
    public class LoginViewModel
	{
		public string Username { get; set; }
		[DataType(DataType.Password)]
		public string Password { get; set; }
		public bool RememberMe { get; set; }
	}
}
