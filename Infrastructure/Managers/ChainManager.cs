using Models.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Managers;

public class ChainManager
{
    private readonly Dictionary<Guid, Chain> _chains = new();
    public event EventHandler<ChainEventArgs> ChainAdded; // Новое событие

    public Guid AddChain(Chain chain)
    {
        if (chain == null)
            throw new ArgumentNullException(nameof(chain));

        // Используем Id, сгенерированный в chain (из конструктора)
        Guid id = chain.Id;
        if (_chains.ContainsKey(id))
        {
            throw new InvalidOperationException($"Chain with ID {id} already exists.");
        }
        _chains[id] = chain;
        OnChainAdded(new ChainEventArgs { ChainId = id, Chain = chain }); // Вызываем событие
        return id;
    }

    public void UpdateChain(Guid id, Chain updatedChain)
    {
        if (!_chains.ContainsKey(id))
        {
            throw new KeyNotFoundException($"Chain with ID {id} not found.");
        }
        _chains[id] = updatedChain;
    }

    public void RemoveChain(Guid id)
    {
        if (!_chains.ContainsKey(id))
        {
            throw new KeyNotFoundException($"Chain with ID {id} not found.");
        }
        _chains.Remove(id);
    }

    public Chain GetChain(Guid id)
    {
        if (!_chains.ContainsKey(id))
        {
            throw new KeyNotFoundException($"Chain with ID {id} not found. Chains: {_chains.Count}");
        }
        return _chains[id];
    }

    public IEnumerable<Chain> GetAllChains()
    {
        return _chains.Values;
    }

    protected virtual void OnChainAdded(ChainEventArgs e)
    {
        ChainAdded?.Invoke(this, e);
    }
}

// Класс аргументов события
public class ChainEventArgs : EventArgs
{
    public Guid ChainId { get; set; }
    public Chain Chain { get; set; }
}