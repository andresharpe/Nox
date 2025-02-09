﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Nox.Core.Components;
using Nox.Core.Interfaces.Etl;

namespace Nox.Core.Models
{
    public sealed class LoaderLoadStrategy : MetaBase, ILoaderLoadStrategy
    {
        public string Type { get; set; } = string.Empty;
        
        [NotMapped]
        public string[] Columns { get; set; } = Array.Empty<string>();

        [MaxLength(512)]
        public string ColumnsJson { get => string.Join('|', Columns.ToArray()); set => Columns = value.Split('|'); }
    }
}
