using Common.Helpers;
using Kavenegar;
using RestSharp;
using System;
using System.Threading.Tasks;

namespace Application.Extensions
{
    public static class SMSProvider
    {
        private static readonly string _apiKey = "3841522F4F2B5637703436712F564B4679745377444F4D465A4C6D736E326E444633516A44332B736C55493D";

        public static async Task<bool> SendOTPCodeAsync(string recipient, string code)
        {
            bool result = false;

            KavenegarApi api = new KavenegarApi(_apiKey);

            var res = await api.VerifyLookup(recipient, code, "kalamato");

            return result;
        }

        public static async Task<bool> SendAsync(string recipient, string token, string template)
        {
            bool result = false;

            KavenegarApi api = new KavenegarApi(_apiKey);

            var res = await api.VerifyLookup(recipient, token, template);

            return !result;
        }

        public static async Task<bool> SendAsync(string recipient, string token, string token2, string template)
        {
            bool result = false;

            KavenegarApi api = new KavenegarApi(_apiKey);

            var res = await api.VerifyLookup(recipient, token, token2, null, template);

            return result;
        }

        public static async Task<bool> SendTextAsync(string recipient, string token, string text, string template)
        {
            bool result = false;

            KavenegarApi api = new KavenegarApi(_apiKey);

            var res = await api.VerifyLookup(recipient, token, "", "", text, template);

            return result;
        }

        //public static bool SendOTPCodeKavenegar(string recipient, string code)
        //{
        //    KavenegarApi api = new KavenegarApi("3841522F4F2B5637703436712F564B4679745377444F4D465A4C6D736E326E444633516A44332B736C55493D");

        //    var result = api.Send("", recipient, "خدمات پیام کوتاه کاوه نگار");
        //}
    }
}
