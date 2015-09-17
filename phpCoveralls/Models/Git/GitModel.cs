using System.Runtime.Serialization;

namespace phpCoveralls.Models.Git
{
    [DataContract]
    public class GitModel
    {
        public GitModel()
        {
            Head = new GitHeadModel();
        }

        [DataMember(Name = "head")]
        public GitHeadModel Head { get; set; }

        [DataMember(Name = "branch")]
        public string Branch { get; set; }
    }
}