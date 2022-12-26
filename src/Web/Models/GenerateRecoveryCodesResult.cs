namespace NoCond.Identity.Web.Models
{
    public class GenerateRecoveryCodesResult
    {
        public string RedirectPath { get; set; }

        public string[] RecoveryCodes { get; set; }

        public string StatusMessage { get; set; }
    }
}
