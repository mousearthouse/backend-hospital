using System.Text.Json.Serialization;
namespace backend_email;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PatientSorting
{
    NameAsc,
    NameDesc,
    CreateAsc,
    CreateDesc,
    InspectionAsc,
    InspectionDesc
}
