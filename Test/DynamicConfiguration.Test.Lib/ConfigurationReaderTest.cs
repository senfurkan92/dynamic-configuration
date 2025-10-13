using DynamicConfiguration.Application.Dtos.ConfigurationSettingDtos;
using DynamicConfiguration.Lib;
using DynamicConfiguration.Persistance.Services;
using System.Diagnostics;

namespace DynamicConfiguration.Test.Lib
{
    public class ConfigurationReaderTest
    {
        const string MongoCstr = "mongodb://root:root@localhost:27017";

        [Fact]
        public async Task GetValueGenericTest()
        {
            var mongoDb = "configurationSettingsTestDb";

            var service = new ConfigurationSettingService(MongoCstr, mongoDb);

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

            var reader = new ConfigurationReader("App-Test", MongoCstr, mongoDb, 0);

            var intValue = reader.GetValue<int>("Test-int");
            var stringValue = reader.GetValue<string>("Test-string");
            var boolValue = reader.GetValue<bool>("Test-bool");
            var decimalValue = reader.GetValue<decimal>("Test-decimal");

            Debug.Assert(intValue == 10);
            Debug.Assert(stringValue == "Ankara");
            Debug.Assert(boolValue == true);
            Debug.Assert(decimalValue == 10.5m);
        }

        [Fact]
        public async Task GetValueTest()
        {
            var mongoDb = "configurationSettingsTestDb";

            var service = new ConfigurationSettingService(MongoCstr, mongoDb);

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

            var reader = new ConfigurationReader("App-Test", MongoCstr, mongoDb, 0);

            var intValue = reader.GetValue("Test-int");
            var stringValue = reader.GetValue("Test-string");
            var boolValue = reader.GetValue("Test-bool");
            var decimalValue = reader.GetValue("Test-decimal");

            Debug.Assert(intValue.GetType() == 10.GetType());
            Debug.Assert(stringValue.GetType() == "Ankara".GetType());
            Debug.Assert(boolValue.GetType() == true.GetType());
            Debug.Assert(decimalValue.GetType() == 10.5m.GetType());
        }
    }
}