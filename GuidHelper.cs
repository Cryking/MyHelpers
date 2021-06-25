using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YFPos.Utils
{
    public class GuidHelper
    {   /// <summary>
        /// 新建GUID
        /// </summary>
        public static string NewGuid()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }
    }
}
