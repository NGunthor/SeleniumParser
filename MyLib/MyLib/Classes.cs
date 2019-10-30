using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MyLib
{
    [DataContract]
    public abstract class Post
    {
        [DataMember]
        public string Id { get; set; }
        public Post(string id)
        {
            Id = id;
        }
    }
    [DataContract]
    public class Link : Post
    {
        [DataMember]
        public IEnumerable<string> ContentLinks { get; set; }

        public Link(string id, List<string> links) : base(id) => ContentLinks = links;

    }
    [DataContract]
    public class Text : Post
    {
        [DataMember]
        public string ContentText { get; set; }
        public Text(string id, string text) : base(id) => ContentText = text;

    }
}