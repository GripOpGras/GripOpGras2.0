using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Globalization;

namespace GripOpGras2.Plugins.FarmMaps.QuickType.Cropfield
{
	public partial class FarmMapsCropfield
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

		[JsonProperty("dataEndDate", NullValueHandling = NullValueHandling.Ignore)]
		public DateTimeOffset? DataEndDate { get; set; }

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
		[JsonProperty("area", NullValueHandling = NullValueHandling.Ignore)]
		public double? Area { get; set; }

		[JsonProperty("final", NullValueHandling = NullValueHandling.Ignore)]
		public bool? Final { get; set; }

		[JsonProperty("soilCode", NullValueHandling = NullValueHandling.Ignore)]
		[JsonConverter(typeof(ParseStringConverter))]
		public long? SoilCode { get; set; }

		[JsonProperty("soilName", NullValueHandling = NullValueHandling.Ignore)]
		public string SoilName { get; set; }

		[JsonProperty("cropTypeCode", NullValueHandling = NullValueHandling.Ignore)]
		[JsonConverter(typeof(ParseStringConverter))]
		public long? CropTypeCode { get; set; }

		[JsonProperty("cropTypeName", NullValueHandling = NullValueHandling.Ignore)]
		public string CropTypeName { get; set; }

		[JsonProperty("rootDepthMax")]
		public object RootDepthMax { get; set; }

		[JsonProperty("emergenceDate")]
		public object EmergenceDate { get; set; }
	}

	public class Geometry
	{
		[JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
		public string Type { get; set; }

		[JsonProperty("coordinates", NullValueHandling = NullValueHandling.Ignore)]
		public double[][][] Coordinates { get; set; }
	}

	public partial class FarmMapsCropfield
	{
		public static FarmMapsCropfield[] FromJson(string json) => JsonConvert.DeserializeObject<FarmMapsCropfield[]>(json, Cropfield.Converter.Settings);
	}

	public static class Serialize
	{
		public static string ToJson(this FarmMapsCropfield[] self) => JsonConvert.SerializeObject(self, Cropfield.Converter.Settings);
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

	internal class ParseStringConverter : JsonConverter
	{
		public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

		public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.Null) return null;
			string? value = serializer.Deserialize<string>(reader);
			long l;
			if (long.TryParse(value, out l))
			{
				return l;
			}
			throw new Exception("Cannot unmarshal type long");
		}

		public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
		{
			if (untypedValue == null)
			{
				serializer.Serialize(writer, null);
				return;
			}
			long value = (long)untypedValue;
			serializer.Serialize(writer, value.ToString());
			return;
		}

		public static readonly ParseStringConverter Singleton = new();
	}
}