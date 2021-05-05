namespace ExtremeInsiders.Models
{
    public class CurrenciesRate
    {
        public CurrencyRate Valute { get; set; } // yes, Valute (валюта)
    }

    public class CurrencyRate
    {
        public CurrencyObject USD { get; set; }
        public CurrencyObject EUR { get; set; }

        public class CurrencyObject
        {
            public double Value { get; set; }
            public string CharCode { get; set; }
        }
    }
}