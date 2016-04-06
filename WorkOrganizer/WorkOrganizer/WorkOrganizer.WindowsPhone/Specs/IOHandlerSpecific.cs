using System;
using System.Threading.Tasks;

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
