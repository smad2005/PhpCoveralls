using System.Runtime.Serialization;

namespace phpCoveralls.Models
{
    [DataContract]
    public class SourceFileModel
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "source_digest")]
        public string SourceDigest { get; set; }

        [DataMember(Name = "coverage")]
        public PhpList Coverage { get; set; }
    }
}