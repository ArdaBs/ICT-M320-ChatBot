using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ChatBot.Classes
{
    public class Message
    {
        public string Keyword { get; set; }
        public string Answer { get; set; }
    }

    [XmlRoot("Messages")]
    public class Messages
    {
        [XmlElement("Message")]
        public List<Message> List { get; set; }

        private string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\", "Trigger.xml");

        /// <summary>
        /// Reading the trigger XML file
        /// </summary>
        public void Load()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Messages));
            using (StreamReader reader = new StreamReader(filePath))
            {
                List = ((Messages)serializer.Deserialize(reader)).List;
            }
        }

    }
}
