using DynamicConfiguration.Application.Dtos.ConfigurationSettingDtos;
using DynamicConfiguration.Lib;
using DynamicConfiguration.Persistance.Services;
using DynamicConfiguration.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DynamicConfiguration.WebUI.Controllers;

public class HomeController : Controller
{
    private ConfigurationReader? _configurationReader;
    private string? _mongoCstr;
    private string? _mongoDatabaseName;

    public IActionResult Index()
    {
        return View();
    }

    public async Task<List<ConfigurationSettingListApplicationResponseDto>> ListApplications([FromBody] ConfigurationSettingListApplicationsRequest request, CancellationToken cancellationToken)
    {
        var service = new ConfigurationSettingService(
                request.MongoCstr,
                request.MongoDatabaseName
            );

        return await service.ListApplications(cancellationToken);
    }

    public async Task<List<ConfigurationSettingListResponseDto>> ListConfigurationSettings([FromBody] ConfigurationSettingListConfigurationSettingsRequest request, CancellationToken cancellationToken)
    {
        var service = new ConfigurationSettingService(
                request.MongoCstr,
                request.MongoDatabaseName
            );

        return await service.List(cancellationToken);
    }

    public async Task<ConfigurationSettingGetResponseDto?> GetConfiGurationSetting([FromBody] ConfigurationSettingGetRequest request, CancellationToken cancellationToken)
    {
        var service = new ConfigurationSettingService(
                request.MongoCstr,
                request.MongoDatabaseName
            );

        return await service.Get(new ConfigurationSettingGetRequestDto(
                request.Id
            ), cancellationToken);
    }

    public async Task<ConfigurationSettingCreateResponseDto> CreateConfiGurationSetting([FromBody] ConfigurationSettingCreateRequest request, CancellationToken cancellationToken)
    {
        var service = new ConfigurationSettingService(
                request.MongoCstr,
                request.MongoDatabaseName
            );

        return await service.Create(new ConfigurationSettingCreateRequestDto(
                request.Name,
                request.Type,
                request.Value,
                request.IsActive,
                request.ApplicationName
            ), cancellationToken);
    }

    public async Task<ConfigurationSettingUpdateResponseDto> UpdateConfiGurationSetting([FromBody] ConfigurationSettingUpdateRequest request, CancellationToken cancellationToken)
    {
        var service = new ConfigurationSettingService(
                request.MongoCstr,
                request.MongoDatabaseName
            );

        return await service.Update(new ConfigurationSettingUpdateRequestDto(
                request.Id,
                request.Name,
                request.Type,
                request.Value,
                request.IsActive,
                request.TimeStamp
            ), cancellationToken);
    }

    public async Task<ConfigurationSettingDeleteResponseDto> DeleteConfiGurationSetting([FromBody] ConfigurationSettingDeleteRequest request, CancellationToken cancellationToken)
    {
        var service = new ConfigurationSettingService(
                request.MongoCstr,
                request.MongoDatabaseName
            );

        return await service.Delete(new ConfigurationSettingDeleteRequestDto(
                request.Id
            ), cancellationToken);
    }

    public object? GetValue([FromBody] ConfigurationSettingGetValueRequest request, CancellationToken cancellationToken)
    {
        if (_configurationReader == null || _mongoCstr != request.MongoCstr || _mongoDatabaseName != request.MongoDatabaseName)
        {
            _configurationReader = new ConfigurationReader(request.ApplicationName, request.MongoCstr, request.MongoDatabaseName, 5000);
        }

        _mongoCstr = request.MongoCstr;
        _mongoDatabaseName = request.MongoDatabaseName;

        var data = _configurationReader.GetValue(request.Name);

        return data;
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
