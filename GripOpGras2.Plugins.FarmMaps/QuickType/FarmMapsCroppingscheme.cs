using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Globalization;

namespace GripOpGras2.Plugins.FarmMaps.QuickType
{
	public partial class FarmMapsCroppingscheme
	{
		[JsonProperty("parentCode", NullValueHandling = NullValueHandling.Ignore)]
		public string ParentCode { get; set; }

		[JsonProperty("geometry", NullValueHandling = NullValueHandling.Ignore)]
		public Geometry Geometry { get; set; }

		[JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
		public Data Data { get; set; }

		[JsonProperty("tags", NullValueHandling = NullValueHandling.Ignore)]
		public object[] Tags { get; set; }

		[JsonProperty("isEditable", NullValueHandling = NullValueHandling.Ignore)]
		public bool? IsEditable { get; set; }

		[JsonProperty("url", NullValueHandling = NullValueHandling.Ignore)]
		public string Url { get; set; }

		[JsonProperty("code", NullValueHandling = NullValueHandling.Ignore)]
		public string Code { get; set; }

		[JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
		public string Name { get; set; }

		[JsonProperty("created", NullValueHandling = NullValueHandling.Ignore)]
		public DateTimeOffset? Created { get; set; }

		[JsonProperty("updated", NullValueHandling = NullValueHandling.Ignore)]
		public DateTimeOffset? Updated { get; set; }

		[JsonProperty("dataDate", NullValueHandling = NullValueHandling.Ignore)]
		public DateTimeOffset? DataDate { get; set; }

		[JsonProperty("dataEndDate")]
		public object DataEndDate { get; set; }

		[JsonProperty("itemType", NullValueHandling = NullValueHandling.Ignore)]
		public string ItemType { get; set; }

		[JsonProperty("sourceTask")]
		public object SourceTask { get; set; }

		[JsonProperty("size", NullValueHandling = NullValueHandling.Ignore)]
		public long? Size { get; set; }

		[JsonProperty("state", NullValueHandling = NullValueHandling.Ignore)]
		public long? State { get; set; }

		[JsonProperty("thumbnail", NullValueHandling = NullValueHandling.Ignore)]
		public bool? Thumbnail { get; set; }
	}

	public class Data
	{
		[JsonProperty("naw", NullValueHandling = NullValueHandling.Ignore)]
		public Naw Naw { get; set; }
	}

	public class Naw
	{
		[JsonProperty("city", NullValueHandling = NullValueHandling.Ignore)]
		public string City { get; set; }

		[JsonProperty("phone", NullValueHandling = NullValueHandling.Ignore)]
		public string Phone { get; set; }

		[JsonProperty("mobile", NullValueHandling = NullValueHandling.Ignore)]
		public string Mobile { get; set; }

		[JsonProperty("address", NullValueHandling = NullValueHandling.Ignore)]
		public string Address { get; set; }

		[JsonProperty("country", NullValueHandling = NullValueHandling.Ignore)]
		public string Country { get; set; }

		[JsonProperty("postalCode", NullValueHandling = NullValueHandling.Ignore)]
		public string PostalCode { get; set; }
	}

	public class Geometry
	{
		[JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
		public string Type { get; set; }

		[JsonProperty("coordinates", NullValueHandling = NullValueHandling.Ignore)]
		public double[] Coordinates { get; set; }
	}

	public partial class FarmMapsCroppingscheme
	{
		public static FarmMapsCroppingscheme[] FromJson(string json) => JsonConvert.DeserializeObject<FarmMapsCroppingscheme[]>(json, Converter.Settings);
	}

	public static class Serialize
	{
		public static string ToJson(this FarmMapsCroppingscheme[] self) => JsonConvert.SerializeObject(self, Converter.Settings);
	}

	internal static class Converter
	{
		public static readonly JsonSerializerSettings Settings = new()
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