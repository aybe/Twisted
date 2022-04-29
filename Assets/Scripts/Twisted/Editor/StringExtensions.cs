using UnityEngine;

namespace Twisted.Editor
{
    public static class StringExtensions
    {
        public static string ToStringMarkup(this object? value, Color? color = null, bool? bold = null, bool? italic = null)
        {
            var text = value?.ToString() ?? string.Empty;

            if (color.HasValue)
            {
                text = $"<color=#{ColorUtility.ToHtmlStringRGBA(color.Value)}>{text}</color>";
            }

            if (bold is true)
            {
                text = $"<b>{text}</b>";
            }

            if (italic is true)
            {
                text = $"<i>{text}</i>";
            }

            return text;
        }
    }
}