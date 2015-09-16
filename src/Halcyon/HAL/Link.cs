﻿using Halcyon.Templates;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Halcyon.HAL {
    public class Link {
        public const string RelForSelf = "self";

        private static readonly Regex isTemplatedRegex = new Regex(@"{.+}", RegexOptions.Compiled);

        public Link(string rel, string href, string title = null, string method = null) {
            this.Rel = rel;
            this.Href = href;
            this.Title = title;
            this.Method = method;
        }

        [JsonIgnore]
        public string Rel { get; private set; }
        public string Href { get; private set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? Templated {
            get {
                return !string.IsNullOrEmpty(Href) && isTemplatedRegex.IsMatch(Href) ? (bool?)true : null;
            }
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Method { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Deprecation { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Profile { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Title { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string HrefLang { get; set; }
        
        public Link CreateLink(string newRel, IDictionary<string, object> parameters) {
            var clone = Clone();

            clone.Rel = newRel;
            clone.Href = CreateUri(parameters).ToString();

            return clone;
        }

        internal Link CreateLink(IDictionary<string, object> parameters) {
            return CreateLink(Rel, parameters);
        }

        internal Uri CreateUri(IDictionary<string, object> parameters) {
            var href = this.Href.SubstituteParams(parameters);
            return GetHrefUri(href);
        }

        internal Link RebaseLink(string baseUriString) {
            var clone = Clone();

            var hrefUri = GetHrefUri(clone.Href);
            if (!hrefUri.IsAbsoluteUri) {
                var baseUri = new Uri(baseUriString, UriKind.RelativeOrAbsolute);
                var rebasedUri = new Uri(baseUri, hrefUri);

                clone.Href = rebasedUri.ToString();
            }

            return clone;
        }

        public Link Clone() {
            return (Link)MemberwiseClone();
        }

        private static Uri GetHrefUri(string href) {
            return new Uri(href, UriKind.RelativeOrAbsolute);
        }
    }
}