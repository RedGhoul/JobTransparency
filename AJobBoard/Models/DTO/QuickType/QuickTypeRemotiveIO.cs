using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Jobtransparency.Models.DTO.QuickType.RemotiveIoData
{

    public partial class RemotiveIoData
    {
        [JsonProperty("0-legal-notice", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string The0LegalNotice { get; set; }

        [JsonProperty("job-count", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public long? JobCount { get; set; }

        [JsonProperty("jobs", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public List<Job> Jobs { get; set; }
    }

    public partial class Job
    {
        [JsonProperty("id", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public long? Id { get; set; }

        [JsonProperty("url", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public Uri Url { get; set; }

        [JsonProperty("title", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Title { get; set; }

        [JsonProperty("company_name", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string CompanyName { get; set; }

        [JsonProperty("category", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Category { get; set; }

        [JsonProperty("tags", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public List<object> Tags { get; set; }

        [JsonProperty("job_type", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string JobType { get; set; }

        [JsonProperty("publication_date", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset? PublicationDate { get; set; }

        [JsonProperty("candidate_required_location", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string CandidateRequiredLocation { get; set; }

        [JsonProperty("salary", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Salary { get; set; }

        [JsonProperty("description", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }

        [JsonProperty("company_logo_url", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public Uri CompanyLogoUrl { get; set; }
    }

    public partial class RemotiveIoData
    {
        public static RemotiveIoData FromJson(string json) => JsonConvert.DeserializeObject<RemotiveIoData>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this RemotiveIoData self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}
