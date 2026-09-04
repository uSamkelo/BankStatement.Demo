using BankStatement.Demo.Models;
using BankStatement.Demo.Models.BankStatement;
using BankStatement.Demo.Models.BankStatement.WIP;
using System.Text.Json;

namespace BankStatement.Demo.Data.Seeded
{
    public class SeededData
    {
        private static readonly JsonSerializerOptions _options = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private static T DeserializeFile<T>(string fileName)
        {
            //string filePath = Path.Combine("c:"fileName);
            string filePath = Path.Combine("C:\\Users\\Admin\\source\\repos\\BankStatement.Demo\\Data\\Seeded\\" , fileName);

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Seed data file not found: {filePath}");
            }

            string jsonString = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<T>(jsonString, _options);
        }

        // Method for File 1
        public static List<BankStatement1> GetBank1()
        {
            var statement = DeserializeFile<BankStatement1>("statement1.json");
            return statement != null ? new List<BankStatement1> { statement } : new List<BankStatement1>();
        }

        // Method for File 2
        public static List<BankStatement2> GetBank2()
        {
            return DeserializeFile<List<BankStatement2>>("statement2.json") ?? new List<BankStatement2>();
        }
    }
}
