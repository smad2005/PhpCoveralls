using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using phpCoveralls.Models.Git;

namespace phpCoveralls.Models
{
    [DataContract]
    internal class CoverallsModelcs
    {
        public CoverallsModelcs()
        {
            SourceFiles = new List<SourceFileModel>();
            Git = new GitModel();
            ServiceName = "auto";
            ServiceBranch = "master";
        }

        [DataMember(Name = "repo_token")]
        public string RepoToken { get; set; }

        [DataMember(Name = "service_branch")]
        public string ServiceBranch { get; set; }

        [DataMember(Name = "source_files")]
        public List<SourceFileModel> SourceFiles { get; set; }

        [DataMember(Name = "service_name")]
        public string ServiceName { get; set; }

        [DataMember(Name = "git")]
        public GitModel Git { get; set; }

        public byte[] ToJson()
        {
            var json = new DataContractJsonSerializer(typeof (CoverallsModelcs));
            using (var ms = new MemoryStream())
            {
                json.WriteObject(ms, this);
                return ms.ToArray();
            }
        }
    }
}