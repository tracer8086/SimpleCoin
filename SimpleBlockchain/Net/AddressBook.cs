﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.Collections;
using SimpleBlockchain.Crypto.Hash;

namespace SimpleBlockchain.Net
{
    public class AddressBook : IAddressBook
    {
        private string bookPath;
        private Dictionary<PeerAddress, string> addressBook;
        private ByteConverter converter;

        public string this[PeerAddress address]
        {
            get => addressBook[address];
            set => addressBook[address] = value;
        }

        public AddressBook(string path)
        {
            bookPath = path;
            converter = new ByteConverter();

            Dictionary<string, string> dict;
            JsonSerializer jsonSerializer = new JsonSerializer();

            using (Stream jsonFile = File.Open(bookPath, FileMode.Open, FileAccess.Read, FileShare.None))
            using (StreamReader reader = new StreamReader(jsonFile))
            using (JsonReader jsonReader = new JsonTextReader(reader))
                dict = jsonSerializer.Deserialize<Dictionary<string, string>>(jsonReader) ?? new Dictionary<string, string>();

            addressBook = dict.ToDictionary(item => new PeerAddress(converter.ConvertToByteArray(item.Key)), item => item.Value);
        }

        public void Add(PeerAddress address, string url) => addressBook.Add(address, url);

        public void Remove(PeerAddress address) => addressBook.Remove(address);

        public void SaveOnDrive()
        {
            JsonSerializer jsonSerializer = new JsonSerializer();
            Dictionary<string, string> dict = addressBook.ToDictionary(item => converter.ConvertToString(item.Key.PublicKey), item => item.Value);

            using (Stream jsonFile = File.Open(bookPath, FileMode.Create, FileAccess.Write, FileShare.None))
            using (StreamWriter writer = new StreamWriter(jsonFile))
            using (JsonWriter jsonWriter = new JsonTextWriter(writer))
                jsonSerializer.Serialize(jsonWriter, dict);
        }

        public IEnumerator<KeyValuePair<PeerAddress, string>> GetEnumerator() => addressBook.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
