using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ONITwitchCore;

// Mostly copied from https://stackoverflow.com/questions/11561597/deserialize-json-recursively-to-idictionarystring-object/31250524#31250524
internal class NestedDictionaryReader : JsonConverter
{
	public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
	{
		throw new NotImplementedException();
	}

	public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
	{
		return ReadValue(reader);
	}

	private object ReadValue(JsonReader reader)
	{
		while (reader.TokenType == JsonToken.Comment)
		{
			if (!reader.Read())
				throw new JsonSerializationException("Unexpected Token when converting IDictionary<string, object>");
		}

		switch (reader.TokenType)
		{
			case JsonToken.StartObject:
				return ReadObject(reader);
			case JsonToken.StartArray:
				return this.ReadArray(reader);
			case JsonToken.Integer:
			case JsonToken.Float:
			case JsonToken.String:
			case JsonToken.Boolean:
			case JsonToken.Undefined:
			case JsonToken.Null:
			case JsonToken.Date:
			case JsonToken.Bytes:
				return reader.Value;
			default:
				throw new JsonSerializationException(
					$"Unexpected token when converting IDictionary<string, object>: {reader.TokenType}"
				);
		}
	}

	private object ReadArray(JsonReader reader)
	{
		IList<object> list = new List<object>();

		while (reader.Read())
		{
			switch (reader.TokenType)
			{
				case JsonToken.Comment:
					break;
				default:
					var v = ReadValue(reader);

					list.Add(v);
					break;
				case JsonToken.EndArray:
					return list;
			}
		}

		throw new JsonSerializationException("Unexpected end when reading IDictionary<string, object>");
	}

	private object ReadObject(JsonReader reader)
	{
		var obj = new Dictionary<string, object>();

		while (reader.Read())
		{
			switch (reader.TokenType)
			{
				case JsonToken.PropertyName:
					var propertyName = reader.Value.ToString();

					if (!reader.Read())
					{
						throw new JsonSerializationException("Unexpected end when reading IDictionary<string, object>");
					}

					var v = ReadValue(reader);

					obj[propertyName] = v;
					break;
				case JsonToken.Comment:
					break;
				case JsonToken.EndObject:
					return obj;
			}
		}

		throw new JsonSerializationException("Unexpected end when reading IDictionary<string, object>");
	}

	public override bool CanConvert(Type objectType)
	{
		return typeof(IDictionary<string, object>).IsAssignableFrom(objectType);
	}

	public override bool CanWrite => false;
}
