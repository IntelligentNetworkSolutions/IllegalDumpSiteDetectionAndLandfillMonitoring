﻿using Dal.ApplicationStorage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dal.Extensions
{
	public static class ServiceExtensions
	{
		public static void ConfigureApplicationDbContext(this IServiceCollection services, IConfiguration config)
		{
			//var connectionString = config["mysqlconnection:connectionString"];
			//todo: da go izvadam samo connection string-ot od parametar a ne cel config posto i taka od tenants bazata treba da dojde konekcijata
			services.AddDbContext<ApplicationDbContext>(options =>
						options.UseNpgsql(config.GetConnectionString("DefaultConnection"),
							assembly => assembly.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)
						));
		}
	}
}
