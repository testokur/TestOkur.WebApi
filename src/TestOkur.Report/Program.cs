﻿namespace TestOkur.Report
{
	using System;
	using System.Net;
	using App.Metrics.AspNetCore;
	using Microsoft.AspNetCore;
	using Microsoft.AspNetCore.Hosting;
	using Microsoft.Extensions.Logging;

	public static class Program
	{
		public static void Main(string[] args)
		{
			BuildWebHost(args).Run();
		}

		public static IWebHost BuildWebHost(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.UseStartup<Startup>()
				.UseMetrics()
				.UseMetricsWebTracking()
				.UseSentry(options =>
				{
					options.Release = "qa";
					options.MaxBreadcrumbs = 200;
					options.HttpProxy = null;
					options.DecompressionMethods = DecompressionMethods.None;
					options.MaxQueueItems = 100;
					options.ShutdownTimeout = TimeSpan.FromSeconds(5);
				})
				.ConfigureLogging((hostingContext, logging) =>
				{
					logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
					logging.AddConsole();
					logging.AddDebug();
				}).Build();
	}
}
