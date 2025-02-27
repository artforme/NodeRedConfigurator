// Presentation/Services/ChainService.cs
using Infrastructure.Managers;
using Models.Domain.Entities;
using Models.Domain.Entities.RgbStrip;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Infrastructure.Logging;
using Presentation.ViewsModels;

namespace Presentation.Services;

public class ChainService
{
    public ChainManager ChainManager { get; }
    private readonly ILogger _logger;

    public ChainService(ChainManager chainManager, ILogger logger)
    {
        ChainManager = chainManager ?? throw new ArgumentNullException(nameof(chainManager));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public ObservableCollection<string> GetChainTypes()
    {
        _logger.Info("Retrieving chain types.");
        return new ObservableCollection<string> { "RgbStrip", "Type1", "Type2", "Type3" };
    }

    public ObservableCollection<ChainParameterViewModel> GetParametersForChainType(string chainType)
    {
        _logger.Info($"Loading parameters for chain type: {chainType}");
        var parameters = new ObservableCollection<ChainParameterViewModel>();
        switch (chainType)
        {
            case "RgbStrip":
                parameters.Add(new ChainParameterViewModel("Name", "Text"));
                parameters.Add(new ChainParameterViewModel("Device", "Text"));
                parameters.Add(new ChainParameterViewModel("DevAddress", "Text"));
                parameters.Add(new ChainParameterViewModel("DevIndex", "Number"));
                parameters.Add(new ChainParameterViewModel("Room", "Text"));
                break;
            case "Type1":
                parameters.Add(new ChainParameterViewModel("Name", "Text"));
                parameters.Add(new ChainParameterViewModel("Count", "Number"));
                parameters.Add(new ChainParameterViewModel("Active", "Boolean"));
                break;
            default:
                _logger.Error($"Unsupported chain type: {chainType}");
                throw new NotSupportedException($"Chain type {chainType} is not supported.");
        }
        return parameters;
    }

    public ObservableCollection<ChainParameterViewModel> GetParametersFromChain(Chain chain)
    {
        _logger.Info($"Extracting parameters from chain: {chain.Type.Value}");
        var parameters = new ObservableCollection<ChainParameterViewModel>();
        foreach (var prop in chain.Properties)
        {
            parameters.Add(new ChainParameterViewModel(prop.Key, GetParameterType(prop.Key))
            {
                Value = prop.Value
            });
        }
        return parameters;
    }

    public Chain CreateChain(string chainType, ObservableCollection<ChainParameterViewModel> parameters)
    {
        _logger.Info($"Creating chain of type: {chainType}");
        if (string.IsNullOrEmpty(chainType))
            throw new ArgumentNullException(nameof(chainType));
        if (parameters == null || !parameters.Any())
            throw new ArgumentException("Parameters cannot be null or empty", nameof(parameters));

        var properties = parameters.ToDictionary(
            param => param.Name,
            param => param.Value ?? string.Empty
        );

        return chainType switch
        {
            "RgbStrip" => new RgbStripChain(properties),
            _ => throw new NotSupportedException($"Chain type {chainType} is not supported.")
        };
    }

    public void SaveChain(Chain chain)
    {
        _logger.Info($"Attempting to save chain: {chain.Type.Value}");
        var validationResult = ValidateChain(chain);
        if (!validationResult.IsValid)
        {
            _logger.Warning($"Chain validation failed: {validationResult.ErrorMessage}");
            throw new InvalidOperationException(validationResult.ErrorMessage);
        }

        ChainManager.AddChain(chain);
        _logger.Info($"Chain saved successfully: {chain.Id}");
    }

    public void UpdateChain(Guid chainId, Chain chain)
    {
        _logger.Info($"Attempting to update chain with ID: {chainId}");
        var oldChain = ChainManager.GetChain(chainId); // Получаем старый объект
        _logger.Info($"Chain properties before update: {string.Join(", ", oldChain.Properties.Select(kvp => $"{kvp.Key}: {kvp.Value}"))}");
        var validationResult = ValidateChain(chain);
        if (!validationResult.IsValid)
        {
            _logger.Warning($"Chain validation failed: {validationResult.ErrorMessage}");
            throw new InvalidOperationException(validationResult.ErrorMessage);
        }
        chain.Id = chainId;
        ChainManager.UpdateChain(chainId, chain);
        _logger.Info($"Chain updated successfully: {chainId}");
        var updatedChain = ChainManager.GetChain(chainId);
        _logger.Info($"Chain properties after update: {string.Join(", ", updatedChain.Properties.Select(kvp => $"{kvp.Key}: {kvp.Value}"))}");
    }

    private ValidationResult ValidateChain(Chain chain)
    {
        if (chain == null)
            return new ValidationResult(false, "Chain cannot be null.");

        if (!chain.Properties.ContainsKey("Name") || string.IsNullOrEmpty(chain.Properties["Name"]?.ToString()))
            return new ValidationResult(false, "Chain must have a non-empty Name.");

        if (chain.Type.Value == "RgbStrip")
        {
            if (!chain.Properties.ContainsKey("Device") || string.IsNullOrEmpty(chain.Properties["Device"]?.ToString()))
                return new ValidationResult(false, "RgbStrip chain must have a non-empty Device.");
            if (!chain.Properties.ContainsKey("DevIndex") || !int.TryParse(chain.Properties["DevIndex"]?.ToString(), out _))
                return new ValidationResult(false, "RgbStrip chain must have a valid numeric DevIndex.");
        }
        // else if (chain.Type.Value == "Type1")
        // {
        //     if (!chain.Properties.ContainsKey("Count") || !int.TryParse(chain.Properties["Count"]?.ToString(), out _))
        //         return new ValidationResult(false, "Type1 chain must have a valid numeric Count.");
        // }

        return new ValidationResult(true);
    }

    private string GetParameterType(string key)
    {
        return key.Contains("Count") || key.Contains("Index") ? "Number" : "Text";
    }
}

public class ValidationResult
{
    public bool IsValid { get; }
    public string ErrorMessage { get; }

    public ValidationResult(bool isValid, string errorMessage = null)
    {
        IsValid = isValid;
        ErrorMessage = errorMessage;
    }
}