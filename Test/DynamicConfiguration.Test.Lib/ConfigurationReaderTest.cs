using DynamicConfiguration.Application.Dtos.ConfigurationSettingDtos;
using DynamicConfiguration.Lib;
using DynamicConfiguration.Persistance.Services;
using System.Diagnostics;

namespace DynamicConfiguration.Test.Lib
{
	public class ConfigurationReaderTest
	{
		[Fact]
		public async Task GetValueGenericTest()
		{
			var mongoCstr = "mongodb://root:root@localhost:27017";
			var mongoDb = "configurationSettingsTestDb";

			var service = new ConfigurationSettingService(mongoCstr, mongoDb);

			var config = await service.GetByName(new ConfigurationSettingGetByNameRequestDto("App-Test", "Test-int"), default);

			if (config == null)
			{
				await service.Create(new ConfigurationSettingCreateRequestDto(
						"Test-int",
						"Int32",
						"10",
						true,
						"App-Test"
					), default);

				await service.Create(new ConfigurationSettingCreateRequestDto(
						"Test-string",
						"String",
						"Ankara",
						true,
						"App-Test"
					), default);


				await service.Create(new ConfigurationSettingCreateRequestDto(
						"Test-bool",
						"Boolean",
						"true",
						true,
						"App-Test"
					), default);


				await service.Create(new ConfigurationSettingCreateRequestDto(
						"Test-decimal",
						"Decimal",
						"10.5",
						true,
						"App-Test"
					), default);
			}

			var reader = new ConfigurationReader("App-Test", mongoCstr, mongoDb, 0);

			var intValue = await reader.GetValue<int>("Test-int");
			var stringValue = await reader.GetValue<string>("Test-string");
			var boolValue = await reader.GetValue<bool>("Test-bool");
			var decimalValue = await reader.GetValue<decimal>("Test-decimal");

			Debug.Assert(intValue == 10);
			Debug.Assert(stringValue == "Ankara");
			Debug.Assert(boolValue == true);
			Debug.Assert(decimalValue == 10.5m);
		}

		[Fact]
		public async Task GetValueTest()
		{
			var mongoCstr = "mongodb://root:root@localhost:27017";
			var mongoDb = "configurationSettingsTestDb";

			var service = new ConfigurationSettingService(mongoCstr, mongoDb);

			var config = await service.GetByName(new ConfigurationSettingGetByNameRequestDto("App-Test", "Test-int"), default);

			if (config == null)
			{
				await service.Create(new ConfigurationSettingCreateRequestDto(
						"Test-int",
						"Int32",
						"10",
						true,
						"App-Test"
					), default);

				await service.Create(new ConfigurationSettingCreateRequestDto(
						"Test-string",
						"String",
						"Ankara",
						true,
						"App-Test"
					), default);


				await service.Create(new ConfigurationSettingCreateRequestDto(
						"Test-bool",
						"Boolean",
						"true",
						true,
						"App-Test"
					), default);


				await service.Create(new ConfigurationSettingCreateRequestDto(
						"Test-decimal",
						"Decimal",
						"10.5",
						true,
						"App-Test"
					), default);
			}

			var reader = new ConfigurationReader("App-Test", mongoCstr, mongoDb, 0);

			var intValue = await reader.GetValue("Test-int");
			var stringValue = await reader.GetValue("Test-string");
			var boolValue = await reader.GetValue("Test-bool");
			var decimalValue = await reader.GetValue("Test-decimal");
			
			Debug.Assert(intValue.GetType() == 10.GetType());
			Debug.Assert(stringValue.GetType() == "Ankara".GetType());
			Debug.Assert(boolValue.GetType() == true.GetType());
			Debug.Assert(decimalValue.GetType() == 10.5m.GetType());
		}
	}
}