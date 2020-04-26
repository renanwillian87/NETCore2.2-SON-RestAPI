using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudyRestAPI.Hateoas
{
    public class Hateoas
    {
        public Hateoas(string url)
        {
            Url = url;
        }
        public Hateoas(string url, string protocol)
        {
            Url = url;
            Protocol = protocol;
        }

        private string Url;
        private string Protocol = "https://";
        public List<Link> Actions = new List<Link>();

        public void AddAction(string rel, string method)
        {
            Actions.Add(new Link(Protocol + Url, rel, method));
        }

        public Link[] GetActions(string sufix)
        {
            Link[] tempLinks = new Link[Actions.Count];

            for (int i = 0; i < tempLinks.Length; i++)
            {
                tempLinks[i] = new Link(Actions[i].Href, Actions[i].Rel, Actions[i].Method);
            }

            foreach (var link in tempLinks)
            {
                link.Href = link.Href + "/" + sufix;
            }
            return tempLinks;
        }
    }
}
