#
#
#
#
#



using System;
using System.IO;
using Newtonsoft.Json;

namespace JsonUtility
{
    public static class JsonFileHandler
    {
        /// <summary>
        /// Reads a JSON file and deserializes it into an object of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of object to deserialize to.</typeparam>
        /// <param name="filePath">The path to the JSON file.</param>
        /// <returns>The deserialized object of type T.</returns>
        public static T Read<T>(string filePath) where T : class
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"The file at {filePath} was not found.");

            try
            {
                string jsonContent = File.ReadAllText(filePath);
                T obj = JsonConvert.DeserializeObject<T>(jsonContent);
                return obj;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error reading the JSON file.", ex);
            }
        }

        /// <summary>
        /// Serializes an object of the specified type to a JSON file.
        /// </summary>
        /// <typeparam name="T">The type of object to serialize.</typeparam>
        /// <param name="filePath">The path to save the JSON file.</param>
        /// <param name="data">The object to serialize.</param>
        public static void Write<T>(string filePath, T data) where T : class
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));

            if (data == null)
                throw new ArgumentNullException(nameof(data), "Data cannot be null.");

            try
            {
                string jsonContent = JsonConvert.SerializeObject(data, Formatting.Indented);
                File.WriteAllText(filePath, jsonContent);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error writing the JSON file.", ex);
            }
        }
    }
}
