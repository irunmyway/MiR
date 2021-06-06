using System;

namespace Foundation
{
    [Serializable]
    public sealed class AuthenticationResponse
    {
        public string token;
        public string name;
        public bool isnew;
    }
}
