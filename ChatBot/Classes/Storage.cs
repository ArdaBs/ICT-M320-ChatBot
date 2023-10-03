using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ChatBot.Classes
{
    public class Storage
    {
        [Serializable]
        public class Conversation
        {
            public string User { get; set; }
            public string Assistant { get; set; }
            public bool IsUserMessage { get; set; }
        }

        [XmlElement("Conversation")]
        public List<Conversation> List { get; set; }

        private string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\", "Conversations.xml");

        /// <summary>
        /// Saving (override) current conversation to
        /// conversation XML file
        /// </summary>
        public void Save()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Storage));
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                serializer.Serialize(writer, this);
            }
        }

        /// <summary>
        /// Loading words from current conversation to storage list
        /// </summary>
        public void Load()
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Storage));
                if (!File.Exists(filePath))
                {
                    // Erstelle eine leere Liste, wenn die Datei nicht existiert
                    List = new List<Conversation>();
                    Save(); // Speichern Sie die leere Liste, um die Datei zu erstellen
                }
                else
                {
                    using (StreamReader reader = new StreamReader(filePath))
                    {
                        var loadedData = (Storage)serializer.Deserialize(reader);
                        List = loadedData.List;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fehler beim Laden der Konversationen: " + ex.Message);
            }
        }

        /// <summary>
        /// Deleting whole conversation and save empty conversation
        /// </summary>
        public void Clear()
        {
            List.Clear();
            Save();
        }
    }
}
