using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using JetBrains.Annotations;

namespace Exploratorium.Utility
{
    public static class HtmlExtensions
    {
        public static readonly HashSet<string> RitchTextTags = new HashSet<string>
        {
            "i", "em", "size", "b", "strong", "color", "qt", "h1", "h2", "h3", "p", "center", "blockquote", "ul",
            "ol", "li", "pre", "a", "u", "big", "small", "code", "tt", "font", "img", "hr", "br", "nobr", "tr",
            "td", "th", "author", "dl", "dt", "dd"
        };

        public static readonly HashSet<string> TextMeshProTags = new HashSet<string>
        {
            "align", //	Text alignment.
            "alpha", "color", // Color and opacity.
            "b", "i", // Bold and italic style.
            "cspace", // Character spacing.
            "font", // Font and material selection.
            "indent", // Indentation.
            "line-height", // Line height.
            "line-indent", // Line indentation.
            "link", // Text metadata.
            "lowercase", "uppercase", "smallcaps", // Capitalization.
            "margin", // Text margins.
            "mark", // Marking text.
            "mspace", // Monospacing.
            "noparse", // Prevent parsing.
            "nobr", // Non-breaking spaces.
            "page", // Page break.
            "pos", // Horizontal caret position.
            "size", // Font size.
            "space", // Horizontal space.
            "sprite", // Insert sprites.
            "s", "u", // Strikethrough and underline.
            "style", // Custom styles.
            "sub", "sup", // Subscript and superscript.
            "voffset", // Baseline offset.
            "width"
        };

        public static readonly Dictionary<string, string> SkbgTagsToTextMeshProTags = new Dictionary<string, string>
        {
            {"em", "i"}, {"strong", "b"}
        };

        public static readonly HashSet<string> SkbgTags = new HashSet<string>
        {
            "em", "i"
        };

        internal static HtmlDocument RemoveTags(this HtmlDocument document, in HashSet<string> whitelist)
        {
            if (document == null)
                throw new ArgumentException();
            if (whitelist == null)
                return document;

            Queue<HtmlNode> nodes = new Queue<HtmlNode>(document.DocumentNode.SelectNodes("./*|./text()"));
            while (nodes.Count > 0)
            {
                HtmlNode node = nodes.Dequeue();
                HtmlNode parentNode = node.ParentNode;

                if (!whitelist.Contains(node.Name) && node.Name != "#text")
                {
                    HtmlNodeCollection childNodes = node.SelectNodes("./*|./text()");
                    if (childNodes != null)
                    {
                        foreach (var child in childNodes)
                        {
                            nodes.Enqueue(child);
                            parentNode.InsertBefore(child, node);
                        }
                    }

                    parentNode.RemoveChild(node);
                }
            }

            return document;
        }

        internal static HtmlDocument ReplaceTag(
            [NotNull] this HtmlDocument document,
            [NotNull] string xpath,
            [NotNull] string replacement)
        {
            if (document == null)
                throw new ArgumentException();
            if (string.IsNullOrEmpty(replacement))
                return document;
            if (string.IsNullOrEmpty(xpath))
                return document;

            HtmlNodeCollection nodes = document.DocumentNode.SelectNodes(xpath);
            if (nodes == null)
                return document;
            foreach (HtmlNode node in nodes) 
                node.Name = replacement;
            return document;
        }

        internal static HtmlDocument ReplaceTag(
            [NotNull] this HtmlDocument document,
            [NotNull] in Dictionary<string, string> replacements)
        {
            
            if (document == null)
                throw new ArgumentException();
            if (replacements == null)
                throw new ArgumentException();
            if (!replacements.Any())
                return document;

            foreach (KeyValuePair<string, string> pair in replacements)
                document.ReplaceTag(pair.Key, pair.Value);
            return document;
        }

        internal static HtmlDocument ParseHtml(this string source)
        {
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(source);
            return document;
        }
    }
}