namespace ChatBotConsole
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Serialization;

    public class Storage
    {
        [Serializable]
        public class Conversation
        {
            public string User { get; set; }
            public string ChatBot { get; set; }
        }

        [XmlElement("Conversation")]
        public List<Conversation> List { get; set; }

        private string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\", "Conversation.xml");

        public void Save()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Storage));
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                serializer.Serialize(writer, this);
            }
        }

        public void Load()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Storage));
            if (File.Exists(filePath))
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    List = ((Storage)serializer.Deserialize(reader)).List;
                }
            }
            
        }
        public void Clear()
        {
            List.Clear();
            Save();
        }


    }

}
