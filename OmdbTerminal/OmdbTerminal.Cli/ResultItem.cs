using OmdbTerminal.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace OmdbTerminal.Cli
{
    internal class ResultItem
    {
        public string ImdbId { get; set; } = "";

        public string DisplayText { get; set; } = "";

        public MovieDetails Details { get; set; } = new();
    }
}
