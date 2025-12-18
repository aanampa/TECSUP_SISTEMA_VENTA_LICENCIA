namespace Application.DTOs.Niubiz
{
    public class Antifraud
    {
        public string clientIp { get; set; }
        public MerchantDefineData merchantDefineData { get; set; }
    }

    public class MerchantDefineData
    {
        public string MDD4 { get; set; }
        public int MDD21 { get; set; }
        public string MDD32 { get; set; }
        public string MDD75 { get; set; }
        public int MDD77 { get; set; }

    }
    public class RootPagoWeb
    {
        public string channel { get; set; }
        public double amount { get; set; }
        public Antifraud antifraud { get; set; }
    }

    public class RootSessionToken
    {
        public string sessionKey { get; set; }
        public long expirationTime { get; set; }
    }

    //-------------
    public class AutorizacionPagoWeb
    {
        public string channel { get; set; }
        public string captureType { get; set; }
        public bool countable { get; set; }
        public Order order { get; set; }
    }

    //-------------
    //
    //-------------
    public class AutorizationResponse
    {
        public int errorCode { get; set; }
        public string errorMessage { get; set; }
        public Header header { get; set; }
        public Fulfillment fulfillment { get; set; }
        public OrderResponse order { get; set; }
        public DataMap dataMap { get; set; }
        public Data data { get; set; }
    }

    public class Header
    {
        public string ecoreTransactionUUID { get; set; }
        public long ecoreTransactionDate { get; set; }
        public int millis { get; set; }
    }

    public class Fulfillment
    {
        public string channel { get; set; }
        public string merchantId { get; set; }
        public string terminalId { get; set; }
        public string captureType { get; set; }
        public bool countable { get; set; }
        public bool fastPayment { get; set; }
        public string signature { get; set; }
    }

    public class OrderResponse
    {
        public string tokenId { get; set; }
        public string purchaseNumber { get; set; }
        public double amount { get; set; }
        public int installment { get; set; }
        public string currency { get; set; }
        public double authorizedAmount { get; set; }
        public string authorizationCode { get; set; }
        public string actionCode { get; set; }
        public string traceNumber { get; set; }
        public string transactionDate { get; set; }
        public string transactionId { get; set; }
    }

    public class DataMap
    {
        public string TERMINAL { get; set; }
        public string BRAND_ACTION_CODE { get; set; }
        public string BRAND_HOST_DATE_TIME { get; set; }
        public string TRACE_NUMBER { get; set; }
        public string CARD_TYPE { get; set; }
        public string ECI_DESCRIPTION { get; set; }
        public string SIGNATURE { get; set; }
        public string CARD { get; set; }
        public string MERCHANT { get; set; }
        public string STATUS { get; set; }
        public string ACTION_DESCRIPTION { get; set; }
        public string ID_UNICO { get; set; }
        public string AMOUNT { get; set; }
        public string AUTHORIZATION_CODE { get; set; }
        public string CURRENCY { get; set; }
        public string TRANSACTION_DATE { get; set; }
        public string ACTION_CODE { get; set; }
        public string CVV2_VALIDATION_RESULT { get; set; }
        public string ECI { get; set; }
        public string ID_RESOLUTOR { get; set; }
        public string BRAND { get; set; }
        public string ADQUIRENTE { get; set; }
        public string BRAND_NAME { get; set; }
        public string PROCESS_CODE { get; set; }
        public string TRANSACTION_ID { get; set; }
    }

    public class Data
    {
        public string TERMINAL { get; set; }
        public string BRAND_ACTION_CODE { get; set; }
        public string BRAND_HOST_DATE_TIME { get; set; }
        public string TRACE_NUMBER { get; set; }
        public string CARD_TYPE { get; set; }
        public string ECI_DESCRIPTION { get; set; }
        public string SIGNATURE { get; set; }
        public string CARD { get; set; }
        public string MERCHANT { get; set; }
        public string STATUS { get; set; }
        public string ACTION_DESCRIPTION { get; set; }
        public string ID_UNICO { get; set; }
        public string AMOUNT { get; set; }
        public string AUTHORIZATION_CODE { get; set; }
        public string CURRENCY { get; set; }
        public string TRANSACTION_DATE { get; set; }
        public string ACTION_CODE { get; set; }
        public string CVV2_VALIDATION_RESULT { get; set; }
        public string ECI { get; set; }
        public string ID_RESOLUTOR { get; set; }
        public string BRAND { get; set; }
        public string ADQUIRENTE { get; set; }
        public string BRAND_NAME { get; set; }
        public string PROCESS_CODE { get; set; }
        public string TRANSACTION_ID { get; set; }
    }
}
