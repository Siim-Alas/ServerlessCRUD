using ServerlessCrudClassLibrary.TableEntities;

namespace ServerlessCrudClassLibrary.HttpRequestModels
{
    public class PostCommentEntityRequest
    {
        public PostCommentEntityRequest()
        {

        }
        public PostCommentEntityRequest(CommentEntity comment, IdentityProviders identityProvider, string userId, string token)
        {
            Comment = comment;
            IdentityProvider = identityProvider;
            UserID = userId;
            Token = token;
        }

        public enum IdentityProviders
        {
            Facebook, 
            Google
        }

        public CommentEntity Comment { get; set; }
        public IdentityProviders IdentityProvider { get; set; }
        public string UserID { get; set; }
        public string Token { get; set; }
        public string Text { get; set; }
    }
}
