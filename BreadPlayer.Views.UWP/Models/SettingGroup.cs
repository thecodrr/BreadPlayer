using System;

namespace BreadPlayer.Models
{
    public class SettingGroup
    {
        public SettingGroup(string icon, string title, string subTitle, Type page)
        {
            Icon = icon;
            Title = title;
            Subtitle = subTitle;
            Page = page;
        }

        public string Icon { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public Type Page { get; set; }
    }
}