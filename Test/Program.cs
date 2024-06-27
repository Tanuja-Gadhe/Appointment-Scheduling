using System;
using Google.Cloud.SecretManager.V1;
using Newtonsoft.Json;


namespace Test
{
    public class AccessSecretVersionSample
    {
        public static void ParseJSONString(string jsonObj="")
        {
            List<string> keys = new List<string> { "Key1", "Key2", "Key3", "Key4" };
            Dictionary<string, string> dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonObj);

   
            foreach(var key in keys)
            {
                Console.WriteLine(dictionary[key]);
            }
        }

        static void Main()
        {
            ParseJSONString("{\"Key1\":\"Value1\",\"Key2\":\"Value2\"}");
        }
    }
}



