using System.Runtime.Serialization;

namespace phpCoveralls.Models.Git
{
    [DataContract]
    public class GitHeadModel
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "author_name")]
        public string AuthorName { set; get; }

        [DataMember(Name = "committer_name")]
        public string CommitterName { get; set; }

        [DataMember(Name = "message")]
        public string Message { get; set; }
    }
}