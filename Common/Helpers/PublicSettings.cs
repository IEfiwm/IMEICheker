namespace Common.Helpers
{
    public static class PublicSettings
    {
        public const byte OTPExpireDate = 3; // 3 minute

        public const byte OTPCodeLenght = 6; // 5 digit

        public const string OTPTemplate = "Code: #CODE#";
    }
}