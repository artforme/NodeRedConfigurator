using Models.Domain.Entities;

namespace Infrastructure.Managers;

public class ChainManager
{
    private readonly Dictionary<Guid, Chain> _chains = new();

    public Guid AddChain(Chain chain)
    {
        var id = Guid.NewGuid();
        _chains[id] = chain;
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
            throw new KeyNotFoundException($"Chain with ID {id} not found.");
        }
        return _chains[id];
    }

    public IEnumerable<Chain> GetAllChains()
    {
        return _chains.Values;
    }
}