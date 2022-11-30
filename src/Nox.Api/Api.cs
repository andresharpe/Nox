﻿using System.Collections.ObjectModel;
using Nox.Core.Components;
using Nox.Core.Interfaces;

namespace Nox.Api;

public sealed class Api : MetaBase, IApi
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    ICollection<IApiRoute>? IApi.Routes
    {
        get => (ICollection<IApiRoute>?)Routes;
        set => Routes = value as ICollection<ApiRoute>;
    }
    
    public ICollection<ApiRoute>? Routes { get; set; } = new Collection<ApiRoute>();
}



