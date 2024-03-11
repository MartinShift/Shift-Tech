using Newtonsoft.Json;
using System.Text;
using System.Security.Cryptography;
using Shift_Tech.DbModels;

namespace Shift_Tech.Models.Liqpay;
public class LiqPay
{
    private string _privateKey;
    private string _publicKey;
    private Params _param;
    public LiqPay()
    {
        _publicKey = File.ReadAllText("D:\\SecureFiles\\publicKey.txt");
        _privateKey = File.ReadAllText("D:\\SecureFiles\\privateKey.txt");
    }
    private string Sign(string data)
    {
        return Convert.ToBase64String(SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(data)));
    }
    public Params PayParams(Order order)
    {
        return PayParams(Convert.ToDecimal(Math.Floor((order.TotalPrice() + order.TotalPrice() / 30) * 100) / 100), "Order products", order.Guid.ToString());
    }
    public Params PayParams(decimal amount, string description, string orderId)
    {
        _param = new Params
        {
            Version = 3,
            PublicKey = _publicKey,
            Action = "pay",
            Amount = amount,
            Currency = "USD",
            Description = description,
            OrderId = orderId,
            Language = "uk",
        };
        return _param;
    }
    public string GetData(Params param)
    {
        var json = JsonConvert.SerializeObject(param);
        var data = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
        return data;
    }
    public string GetSignature(Params param)
    {
        var signature = Sign(_privateKey + GetData(param) + _privateKey);
        return signature;
    }
    public bool Verify(Notify notify, Params @params)
    {
        return notify.Data == GetData(@params) && notify.Signature == GetSignature(@params);
    }
}