
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;


   



namespace Critical_Events_Finder_Api.Utilities
{

    public static class JsonSchemaValidator
    {
        private static readonly JSchema JsonSchema = JSchema.Parse(@"
        {
            'type': 'array',
            'items': {
                'type': 'object',
                'properties': {
                    'id': {'type': 'string'},
                    'events': {
                        'type': 'array',
                        'items': {
                            'type': 'object',
                            'properties': {
                                'intersection': {'type': 'string'},
                                'event': {'type': 'string'}
                            },
                            'required': ['intersection', 'event']
                        }
                    }
                },
                'required': ['id', 'events']
            }
        }");

        public static bool ValidateJson(string json)
        {
            var jsonObject = JArray.Parse(json);

            // ✅ Explicitly define the type of the output parameter
            IList<string> errorMessages = new List<string>();

            bool isValid = jsonObject.IsValid(JsonSchema, out errorMessages);

            if (!isValid)
            {
                foreach (var error in errorMessages)
                {
                    Console.WriteLine($"Schema Validation Error: {error}");
                }
            }

            return isValid;
        }
    }

}
