namespace Craftmatrix.org.Model
{
    public static class VerificationStatus
    {
        public const string Pending = "pending";
        public const string Approved = "approved";
        public const string Rejected = "rejected";
        public const string UnderReview = "under_review";
        
        public static readonly string[] AllStatuses = { Pending, Approved, Rejected, UnderReview };
        
        public static bool IsValidStatus(string status)
        {
            return AllStatuses.Contains(status?.ToLower());
        }
    }
    
    public static class VerificationType
    {
        public const string DriverLicense = "driver_license";
        public const string NationalId = "national_id";
        public const string Passport = "passport";
        public const string BirthCertificate = "birth_certificate";
        public const string Other = "other";
        
        public static readonly string[] AllTypes = { DriverLicense, NationalId, Passport, BirthCertificate, Other };
        
        public static bool IsValidType(string type)
        {
            return AllTypes.Contains(type?.ToLower());
        }
    }
}