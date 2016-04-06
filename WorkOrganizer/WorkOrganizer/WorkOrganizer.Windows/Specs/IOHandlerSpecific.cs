using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace WorkOrganizer.Specs
{
    class IOHandlerSpecific
    {
        internal static async Task<bool> ExistsFile(string fileName)
        {
            try
            {
                var Stream = (await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(fileName));
                bool Resp = Stream != null;
                Stream.Flush();
                Stream.Dispose();
                return Resp;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
