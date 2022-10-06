﻿namespace Nox.Dynamic.MetaData
{
    public sealed class LoaderSource : MetaBase, IServiceDatabase
    {
        public string Name { get; set; } = string.Empty;
        public string Provider { get; set; } = "SqlServer";
        public string Server { get; set; } = "localhost";
        public string User { get; set; } = "user";
        public string Password { get; set; } = "password";
        public string Options { get; set; } = "";
        public string? ConnectionString { get; set; }
        public string? ConnectionVariable { get; set; }
        public string Query { get; set; } = string.Empty;
        public int MinimumExpectedRecords { get; set; } = 0;
    }
}
