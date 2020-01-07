﻿namespace TestOkur.Serializer
{
    using SpanJson;
    using System.Net.Http;
    using System.Net.Mime;
    using System.Text;

    public static class ObjectExtensions
    {
        public static StringContent ToJsonContent<TModel>(this TModel obj)
        {
            var jsonBytes = JsonSerializer.Generic.Utf8.Serialize(obj);
            var json = Encoding.UTF8.GetString(jsonBytes);
            return new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);
        }
    }
}