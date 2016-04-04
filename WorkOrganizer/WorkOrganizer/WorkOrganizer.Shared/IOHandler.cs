using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using Windows.Storage;
using WorkOrganizer.Specs;

namespace WorkOrganizer
{
    public static class IOHandler
    {
        public static async Task<bool> WriteXMLAsync(string fileName, IEnumerable<WorkEvent> events)
        {
            try
            {
                var Serializer = new DataContractSerializer(typeof(IEnumerable<WorkEvent>));
                using (var Stream = await ApplicationData.Current.LocalFolder.OpenStreamForWriteAsync(
                    fileName,
                    CreationCollisionOption.ReplaceExisting))
                {
                    Serializer.WriteObject(Stream, events);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static async Task<bool> WriteJsonAsync<T>(string fileName, IEnumerable<T> list)
        {
            try
            {
                var Serializer = new DataContractJsonSerializer(typeof(IEnumerable<T>));
                using (var Stream = await ApplicationData.Current.LocalFolder.OpenStreamForWriteAsync(
                    fileName,
                    CreationCollisionOption.ReplaceExisting))
                {
                    Serializer.WriteObject(Stream, list);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static async Task<string> ReadXMLAsync(string fileName)
        {
            try
            {
                string Content = String.Empty;
                var MyStream = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(fileName);
                using (StreamReader Reader = new StreamReader(MyStream))
                {
                    Content = await Reader.ReadToEndAsync();
                }
                return Content;
            }
            catch (Exception)
            {
                return "";
            }
        }
        public static async Task<IEnumerable<T>> ReadJsonAsync<T>(string fileName)
        {
            try
            {
                var JsonSerializer = new DataContractJsonSerializer(typeof(IEnumerable<WorkEvent>));
                var MyStream = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(fileName);

                return (IEnumerable<T>)JsonSerializer.ReadObject(MyStream);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
