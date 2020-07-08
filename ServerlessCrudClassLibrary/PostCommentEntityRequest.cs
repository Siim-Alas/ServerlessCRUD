using Microsoft.Azure.Documents;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerlessCrudClassLibrary
{
    public class PostCommentEntityRequest
    {
        public PostCommentEntityRequest()
        {

        }
        public PostCommentEntityRequest(CommentEntity comment, IdentityProviders identityProvider, string userId, string accessToken)
        {
            Comment = comment;
            IdentityProvider = identityProvider;
            UserID = userId;
            AccessToken = accessToken;
        }

        public enum IdentityProviders
        {
            Facebook
        }

        public CommentEntity Comment { get; set; }
        public IdentityProviders IdentityProvider { get; set; }
        public string UserID { get; set; }
        public string AccessToken { get; set; }
        public string Text { get; set; }
    }
}
