using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class Repositories
{
    private static ResourcePoolRepository resourcePoolRepository;
    public ResourcePoolRepository getResourcePoolRepository()
    {
        if (resourcePoolRepository == null)
            resourcePoolRepository = new ResourcePoolRepository();
        return resourcePoolRepository;
    }

    private static BankRepository bankRepository;
    public BankRepository getBankRepository()
    {
        if (bankRepository == null)
            bankRepository = new BankRepository();
        return bankRepository;
    }
}

public class ResourcePoolRepository
{
    private List<ResourcePool> resourcePools = new List<ResourcePool>();
    public void Add(ResourcePool pool)
    {
        resourcePools.Add(pool);
    }
    public List<ResourcePool> GetAll()
    {
        return resourcePools;
    }
    public void Remove(ResourcePool pool)
    {
        resourcePools = resourcePools.Where(x => x.id != pool.id).ToList();
    }
}

public class BankRepository
{
    private List<Bank> banks = new List<Bank>();
    public void Add(Bank pool)
    {
        banks.Add(pool);
    }
    public List<Bank> GetAll()
    {
        return banks;
    }
    public void Remove(Bank pool)
    {
        banks = banks.Where(x => x.id != pool.id).ToList();
    }
}
