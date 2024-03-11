using Newtonsoft.Json;
using System.Text.Json.Serialization;
namespace Shift_Tech.Models.Liqpay;
public class Params
{
    [JsonPropertyName("version")]
    [JsonProperty("version")]
    public int Version { get; set; }
    [JsonPropertyName("public_key")]
    [JsonProperty("public_key")]
    public string PublicKey { get; set; }
    [JsonPropertyName("action")]
    [JsonProperty("action")]
    public string Action { get; set; }
    [JsonPropertyName("amount")]
    [JsonProperty("amount")]
    public decimal Amount { get; set; }
    [JsonPropertyName("currency")]
    [JsonProperty("currency")]
    public string Currency { get; set; }
    [JsonPropertyName("description")]
    [JsonProperty("description")]
    public string Description { get; set; }
    [JsonPropertyName("order_id")]
    [JsonProperty("order_id")]
    public string OrderId { get; set; }
    [JsonPropertyName("language")]
    [JsonProperty("language")]
    public string Language { get; set; }
}