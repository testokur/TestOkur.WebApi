﻿namespace TestOkur.WebApi.Data
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Threading.Tasks;
	using Microsoft.EntityFrameworkCore;
	using OfficeOpenXml;
	using TestOkur.Data;
	using TestOkur.Domain.Model.CityModel;

	internal class CitySeeder : ISeeder
	{
		private const string CityExcelFilePath = "cities-districts.xlsx";
		private readonly ApplicationDbContext _dbContext;

		public CitySeeder(ApplicationDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task SeedAsync()
		{
			if (await _dbContext.Cities.AnyAsync())
			{
				return;
			}

			var cityDict = new Dictionary<long, City>();
			var file = new FileInfo(Path.Combine("Data", CityExcelFilePath));

			using (var package = new ExcelPackage(file))
			{
				var workSheet = package.Workbook.Worksheets.First();

				for (var i = 2; i < workSheet.Dimension.Rows + 1; i++)
				{
					var city = ParseCity(workSheet, i);
					var districtId = Convert.ToInt32(workSheet.Cells[i, 3].Value);
					var districtName = workSheet.Cells[i, 4].Value.ToString();
					cityDict.TryAdd(city.Id, city);
					cityDict[city.Id].AddDistrict(districtId, districtName);
				}
			}

			await AddAsync(cityDict.Values);
		}

		private City ParseCity(ExcelWorksheet workSheet, int rowIndex)
		{
			var cityId = Convert.ToInt32(workSheet.Cells[rowIndex, 1].Value);
			var cityName = workSheet.Cells[rowIndex, 2].Value.ToString();

			return new City(cityId, cityName);
		}

		private async Task AddAsync(IEnumerable<City> cities)
		{
			_dbContext.Cities.AddRange(cities.ToList());
			await _dbContext.SaveChangesAsync();
		}
	}
}
