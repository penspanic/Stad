using System;
using Utf8Json;

namespace Stad
{
    [JsonFormatter(typeof(ModelKeyJsonFormatter))]
    public readonly struct ModelKey
    {
        public ModelKey(int intValue)
        {
            IntValue = intValue;
            StringValue = null;
        }

        public ModelKey(string stringValue)
        {
            StringValue = stringValue;
            IntValue = 0;
        }

        public readonly int IntValue;
        public readonly string StringValue;

        public bool IsIntKey => StringValue == null;
    }

    public class ModelKeyJsonFormatter : IJsonFormatter<ModelKey>, IObjectPropertyNameFormatter<ModelKey>
    {
        public void Serialize(ref JsonWriter writer, ModelKey value, IJsonFormatterResolver formatterResolver)
        {
            if (value.IsIntKey)
            {
                formatterResolver.GetFormatterWithVerify<Int32>().Serialize(ref writer, value.IntValue, formatterResolver);
            }
            else
            {
                formatterResolver.GetFormatterWithVerify<String>().Serialize(ref writer, value.StringValue, formatterResolver);
            }
        }

        public ModelKey Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull()) return default;
            
            // if target type is primitive, you can also use reader.Read***.
            string name =  reader.ReadPropertyName();
            if (name == "IntKey")
            {
                return new ModelKey(reader.ReadInt32());
            }
            else if (name == "StrKey")
            {
                return new ModelKey(reader.ReadString());
            }

            throw new Exception($"Not model key! {reader}");
        }

        public void SerializeToPropertyName(
            ref JsonWriter writer,
            ModelKey value,
            IJsonFormatterResolver formatterResolver)
        {
            writer.WriteQuotation();
            if (value.IsIntKey)
                writer.WriteInt32(value.IntValue);
            else
                writer.WriteString(value.StringValue);
            writer.WriteQuotation();
        }

        public ModelKey DeserializeFromPropertyName(
            ref JsonReader reader,
            IJsonFormatterResolver formatterResolver)
        {
            if (reader.GetCurrentJsonToken() == JsonToken.Number)
            {
                return new ModelKey(reader.ReadInt32()); 
            }
            if (reader.GetCurrentJsonToken() == JsonToken.String)
            {
                return new ModelKey(reader.ReadString());
            }

            throw new Exception($"Not model key! {reader}");
        }
    }
}