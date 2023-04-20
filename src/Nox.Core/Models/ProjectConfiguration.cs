﻿using FluentValidation;
using Nox.Core.Components;
using Nox.Core.Constants;
using Nox.Core.Interfaces;
using Nox.Core.Interfaces.Api;
using Nox.Core.Interfaces.Database;
using Nox.Core.Interfaces.Entity;
using Nox.Core.Interfaces.Etl;
using Nox.Core.Interfaces.Messaging;
using Nox.Core.Interfaces.Secrets;
using Nox.Core.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nox.Core.Models;

public sealed class ProjectConfiguration : MetaBase, IProjectConfiguration
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string EndpointProvider { get; set; } = string.Empty;
    public string KeyVaultUri { get; set; } = KeyVault.DefaultKeyVaultUri;
    public bool AutoMigrations { get; set; } = true;


    IServiceDataSource? IProjectConfiguration.Database
    {
        get => Database;
        set => Database = value as ServiceDatabase;
    }
    public ServiceDatabase? Database { get; set; } = new();
    
    ICollection<IMessagingProvider>? IProjectConfiguration.MessagingProviders
    {
        get => MessagingProviders?.ToList<IMessagingProvider>();
        set => MessagingProviders = value as ICollection<MessagingProvider>;
    }
    
    public ICollection<MessagingProvider>? MessagingProviders { get; set; }

    public void AddMessagingProvider(IMessagingProvider provider)
    {
        MessagingProviders?.Add((MessagingProvider)provider);
    }

    ICollection<IServiceDataSource>? IProjectConfiguration.DataSources
    {
        get => DataSources?.ToList<IServiceDataSource>();
        set => DataSources = value as ICollection<ServiceDatabase>;
    }

    public ICollection<ServiceDatabase>? DataSources { get; set; }

    ICollection<IEntity>? IProjectConfiguration.Entities
    {
        get => Entities?.ToList<IEntity>();
        set => Entities = value as ICollection<Core.Models.Entity>;
    }
    public ICollection<Core.Models.Entity>? Entities { get; set; }

    ICollection<ILoader>? IProjectConfiguration.Loaders
    {
        get => Loaders?.ToList<ILoader>();
        set => Loaders = value as ICollection<Loader>;
    }
    public ICollection<Loader>? Loaders { get; set; }

    ICollection<IApi>? IProjectConfiguration.Apis
    {
        get => Apis?.ToList<IApi>();
        set => Apis = value as ICollection<Api>;
    }
    public ICollection<Api>? Apis { get; set; }

    [NotMapped]
    public ISecret? Secrets { get; set; }

    public void Validate()
    {
        var validator = new MetaServiceValidator();
        validator.ValidateAndThrow(this);
    }
    
    public void Configure()
    {
        Entities = SortEntitiesByDependency();
        Loaders = SortLoadersByEntitySortOrder();
    }

    private ICollection<Loader> SortLoadersByEntitySortOrder()
    {
        var entities = Entities!.ToDictionary(x => x.Name, x => x, StringComparer.OrdinalIgnoreCase);
        return Loaders!.OrderBy(l => entities[l.Target!.Entity].SortOrder).ToList();
    }


    private ICollection<Core.Models.Entity> SortEntitiesByDependency()
    {
        var entities = Entities!.ToList();

        foreach (var entity in entities)
        {
            foreach (var parent in entity.RelatedParents)
            {
                var parentEntity = entities.FirstOrDefault(x => x.Name.Equals(parent, StringComparison.OrdinalIgnoreCase)) ?? 
                    throw new ArgumentException($"Entity parent with name {parent} was not found. Please, check if entity name is correct.", "relatedParents");
 
                parentEntity
                    .RelatedChildren
                    .Add(entity.Name);
            }
        }

        // Rough sort entities by parent count

        entities.Sort((entity1, entity2) =>
            entity1.RelatedParents.Count.CompareTo(entity2.RelatedParents.Count));

        // hierarchy sort to place entities in dependency order

        var i = 0;
        var sortedEntities = new List<Core.Models.Entity>();
        while (entities.Count > 0)
        {
            var count = CountParentsInSortedEntities(entities, sortedEntities, i);

            if (count == entities[i].RelatedParents.Count)
            {
                sortedEntities.Add(entities[i]);
                entities.RemoveAt(i);
                i = 0;
            }
            else
            {
                if (++i >= entities.Count)
                {
                    i = 0;
                }
            }
        }

        i = 1;
        sortedEntities.ForEach(e => e.SortOrder = i++);

        return sortedEntities;

    }

    private static int CountParentsInSortedEntities(
            IList<Core.Models.Entity> unsortedEntities,
            IList<Core.Models.Entity> sortedEntities,
            int iteration)
    {
        var result = 0;

        foreach (string p in unsortedEntities[iteration].RelatedParents)
        {
            result += sortedEntities.Count(x => x.Name.Equals(p));
        }

        return result;
    }

}






