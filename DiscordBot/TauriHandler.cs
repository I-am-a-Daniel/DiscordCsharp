using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DiscordBot
{
    internal class TauriHandler
    {
        public static EmbedBuilder GetAllRealmInfo()
        {
            string url = "https://tauriwow.com/files/serverstatus.php";
            WebClient client = new WebClient();
            string xmlData = client.DownloadString(url);

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlData);

            XmlNodeList serverNodes = xmlDoc.SelectNodes("/root/servers/realm");
            StringBuilder result = new StringBuilder();
            foreach (XmlNode serverNode in serverNodes)
            {
                //string id = serverNode.SelectSingleNode("id").InnerText;
                string name = serverNode.SelectSingleNode("name").InnerText;
                string status = serverNode.SelectSingleNode("status").InnerText;
                string population = status == "Offline" ? "0" : serverNode.SelectSingleNode("online").InnerText;
                if (name != "Reborn WoW Server" && !((name == "TauriBattle" || name == "Burning Blade" || name == "Alaris WoW Server" || name == "Public Test Realm" || name == "Crystalsong" || name == "Crystalsong Alpha" || name == "Mrggllll") && (population == "0" || status == "Offline")))
                {
                    result.AppendLine($"**{name}**: {status}, {population} játékos");
                }
                //Console.WriteLine($"Server: {name}, Status: {status}, Population: {population}");
            }
            EmbedBuilder builder = new EmbedBuilder()
                .WithTitle("Tauri Realm információ")
                .WithDescription(result.ToString());
            return builder;
        }
    }
}
