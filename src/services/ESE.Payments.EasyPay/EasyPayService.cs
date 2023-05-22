namespace ESE.Payments.EasyPay
{
    public class EasyPayService
    {
        public readonly string ApiKey;
        public readonly string EncryptionKey;

        public EasyPayService(string apiKey, string encryptionKey)
        {
            ApiKey = apiKey;
            EncryptionKey = encryptionKey;
        }
    }
}