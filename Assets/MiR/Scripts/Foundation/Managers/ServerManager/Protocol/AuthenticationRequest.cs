using System;

namespace Foundation
{
    [Serializable]
    public sealed class AuthenticationRequest
    {
        public string UserID;
        public string Secret;
    }
}
