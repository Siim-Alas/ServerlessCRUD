
namespace ServerlessCrudClassLibrary.HttpResponseModels
{
    public class GoogleIdTokenResponse
    {
        public string Iss { get; set; }
        public string Aud { get; set; }
        public string Sub { get; set; }
        public long Exp { get; set; }

        public string Name { get; set; }
    }
}
