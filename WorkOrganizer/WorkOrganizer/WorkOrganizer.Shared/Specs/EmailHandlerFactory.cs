using System;
using System.Collections.Generic;
using System.Text;

namespace WorkOrganizer.Specs
{
    static class EmailHandlerFactory
    {
        internal static EmailHandler CreateNewHandler(string type)
        {
            if (type == "Type1")
                return new Type1EmailHandler();
            else if (type == "Type2")
                return new Type2EmailHandler();
            else if (type == "Type3")
                return new Type3EmailHandler();
            else
                return null;
        }
    }
}
