using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TSFM.Models
{
    public class Category
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("parent_id")]
        public long? ParentId { get; set; }

        [JsonProperty("expanded")]
        public bool Expanded { get; set; }
    }

    public class TagMetadata
    {
        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("description")]
        public string Description { get; set; } = string.Empty;

        [JsonProperty("preview")]
        public string? Preview { get; set; }

        [JsonProperty("mutually_exclusive")]
        public bool MutuallyExclusive { get; set; }
    }

    public class Mod
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("category_id")]
        public long CategoryId { get; set; }

        [JsonProperty("enabled")]
        public bool Enabled { get; set; } = true;

        [JsonProperty("notes")]
        public string Notes { get; set; } = string.Empty;

        [JsonProperty("preview")]
        public string? Preview { get; set; }

        [JsonProperty("tags")]
        public List<string> Tags { get; set; } = new();
    }

    public class Project
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("description")]
        public string Description { get; set; } = string.Empty;

        [JsonProperty("preview")]
        public string? Preview { get; set; }

        [JsonProperty("projectTypeId")]
        public string ProjectTypeId { get; set; } = "file";

        [JsonProperty("root_folder")]
        public string RootFolder { get; set; } = string.Empty;

        // Property for UI binding (uses Preview value)
        [JsonIgnore]
        public string? ImagePath => Preview;
    }

    public class Database
    {
        [JsonProperty("root_folder")]
        public string RootFolder { get; set; } = string.Empty;

        [JsonProperty("disabled_folder")]
        public string DisabledFolder { get; set; } = "_Disabled";

        [JsonProperty("mod_strategy")]
        public string ModStrategy { get; set; } = string.Empty;

        [JsonProperty("projectTypeId")]
        public string ProjectTypeId { get; set; } = "file";

        [JsonProperty("categories")]
        public List<Category> Categories { get; set; } = new();

        [JsonProperty("mods")]
        public List<Mod> Mods { get; set; } = new();

        [JsonProperty("tags")]
        public List<string> Tags { get; set; } = new();

        [JsonProperty("tag_metadata")]
        public List<TagMetadata> TagMetadata { get; set; } = new();
    }
}
