using System;
using System.Collections.Generic;
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
                //System.IO.File.Exists(string.Format(@"{0}\{1}", ApplicationData.Current.LocalFolder.Path, fileName);
                string Aux = await IOHandler.ReadXMLAsync(fileName);
                return (Aux != null);
            }
            catch (Exception e)
            {
                throw (e);
            }
        }
    }
}
