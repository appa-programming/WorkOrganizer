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
                    Stream.Flush();
                    Stream.Position = 0;
                    Serializer.WriteObject(Stream, events);
                    Stream.Dispose();
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
                    Stream.Flush();
                    Stream.Position = 0;
                    Serializer.WriteObject(Stream, list);
                    Stream.Dispose();
                }
                return true;
            }
            catch (Exception e)
            {
                throw (e);
            }
        }

        public static async Task<string> ReadXMLAsync(string fileName)
        {
            try
            {
                string Content = String.Empty;
                var MyStream = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(fileName);
                MyStream.Flush();
                MyStream.Position = 0;
                using (StreamReader Reader = new StreamReader(MyStream))
                {
                    Content = await Reader.ReadToEndAsync();
                    Reader.Dispose();
                }
                return Content;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public static async Task<IEnumerable<T>> ReadJsonAsync<T>(string fileName)
        {
            Stream MyStream = null;
            try
            {
                var JsonSerializer = new DataContractJsonSerializer(typeof(IEnumerable<T>));
                MyStream = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(fileName);
                MyStream.Flush();
                MyStream.Position = 0;
                return (IEnumerable<T>)JsonSerializer.ReadObject(MyStream);
            }
            catch (Exception e)
            {
                throw (e);
            }
            finally
            {
                if (MyStream != null)
                {
                    MyStream.Flush();
                    MyStream.Dispose();
                }
            }
        }

        public static async Task<bool> ExistsFile(string fileName)
        {
            return await IOHandlerSpecific.ExistsFile(fileName);
        }
    }
}
