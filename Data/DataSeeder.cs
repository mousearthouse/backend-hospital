using backend_email.Data.Models;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace backend_email.Data;

public class DataSeeder
{
    private readonly AppDbContext _dbContext;

    public DataSeeder(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public async Task SeedDataAsync()
    {
        if (!await _dbContext.Icd10Records.AnyAsync())
        {
            Console.WriteLine("Starting ICD-10 data load");
            await LoadIcd10DataAsync();
        }

        if (!await _dbContext.Specialities.AnyAsync())
        {
            Console.WriteLine("Starting doctor specializations load");
            await LoadDoctorSpecializationsAsync();
        }
    }
    

    private async Task LoadIcd10DataAsync()
    {
        try
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "icd10.json");
            if (!File.Exists(filePath))
            {
                Console.WriteLine("ICD-10 file not found");
                return;
            }

            var jsonData = await File.ReadAllTextAsync(filePath);
            Console.WriteLine($"Loaded JSON data: {jsonData.Substring(0, Math.Min(200, jsonData.Length))}");
            var records = JsonSerializer.Deserialize<Icd10JsonStructure>(jsonData);

            if (records?.Records != null && records.Records.Count > 0)
            {
                Console.WriteLine($"Loaded {records.Records.Count} ICD-10 records from JSON");

                var icd10Records = records.Records.Select(record => new Icd10Record
                {
                    Id = record.ID.ToString(),
                    Code = record.MKB_CODE,
                    Name = record.MKB_NAME,
                    IdParent = string.IsNullOrWhiteSpace(record.ID_PARENT) ? null : record.ID_PARENT
                }).ToList();

                foreach (var record in icd10Records)
                {
                    Console.WriteLine($"Loaded record: {record.Code} - {record.Name}, Parent ID: {record.IdParent}");
                }
                _dbContext.Icd10Records.AddRange(icd10Records);
                await _dbContext.SaveChangesAsync();

                Console.WriteLine($"Successfully added {icd10Records.Count} ICD-10 records to the database");
            }
            else
            {
                Console.WriteLine("No records found in JSON or deserialization failed");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while loading ICD-10 data: {ex.Message}");
        }
    }


    private async Task LoadDoctorSpecializationsAsync()
    {
        var specializations = new List<Speciality>
        {
            new() { Name = "Cardiologist" },
            new() { Name = "Neurologist" },
            new() { Name = "Pediatrician" }
        };

        _dbContext.Specialities.AddRange(specializations);
        await _dbContext.SaveChangesAsync();
        Console.WriteLine("Specialities added successfully.");
    }


    private class Icd10JsonStructure
    {
        [JsonPropertyName("records")]
        public List<Icd10RecordJson> Records { get; set; } = new List<Icd10RecordJson>();
    }


    private class Icd10RecordJson
    {
        [JsonPropertyName("ID")]
        public int ID { get; set; }

        [JsonPropertyName("REC_CODE")]
        public string REC_CODE { get; set; }

        [JsonPropertyName("MKB_CODE")]
        public string MKB_CODE { get; set; }

        [JsonPropertyName("MKB_NAME")]
        public string MKB_NAME { get; set; }

        [JsonPropertyName("ID_PARENT")]
        public string ID_PARENT { get; set; }
        public int ACTUAL { get; set; }
    }
}
