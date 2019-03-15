using SkinnyJson;

namespace ADSD
{
    /// <summary>Dictionary extensions for serializations</summary>
    public static class JsonExtensions
    {
        /// <summary>Serializes an object to JSON.</summary>
        /// <param name="value">The object to serialize</param>
        /// <returns>the object as JSON.</returns>
        public static string SerializeToJson(object value)
        {
            return Json.Freeze(value);
        }

        /// <summary>Deserialzes JSON into an instance of type T.</summary>
        /// <typeparam name="T">the object type.</typeparam>
        /// <param name="jsonString">the JSON to deserialze.</param>
        /// <returns>a new instance of type T.</returns>
        public static T DeserializeFromJson<T>(string jsonString) where T : class
        {
            return Json.Defrost<T>(jsonString);
        }

        /// <summary>
        /// Deserialzes JSON into an instance of <see cref="T:System.IdentityModel.Tokens.JwtHeader" />.
        /// </summary>
        /// <param name="jsonString">the JSON to deserialze.</param>
        /// <returns>a new instance <see cref="T:System.IdentityModel.Tokens.JwtHeader" />.</returns>
        public static JwtHeader DeserializeJwtHeader(string jsonString)
        {
            return Json.Defrost<JwtHeader>(jsonString);
        }

        /// <summary>
        /// Deserialzes JSON into an instance of <see cref="T:System.IdentityModel.Tokens.JwtPayload" />.
        /// </summary>
        /// <param name="jsonString">the JSON to deserialze.</param>
        /// <returns>a new instance <see cref="T:System.IdentityModel.Tokens.JwtPayload" />.</returns>
        public static JwtPayload DeserializeJwtPayload(string jsonString)
        {
            return Json.Defrost<JwtPayload>(jsonString);
        }
    }
}