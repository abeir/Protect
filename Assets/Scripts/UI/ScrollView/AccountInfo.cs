using System;

namespace UI.ScrollView
{

    [Serializable]
    public struct AccountInfo
    {
        public string name;
        public string url;
        public string user;
        public string password;

        public string id;
        public long savedAt;
        public long modifiedAt;
    }
}