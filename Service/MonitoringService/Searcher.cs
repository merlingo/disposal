using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringService
{
    public class FileSearcher : Filter
    {
        //belirtilen attr'lerin value'leri icinde regex veya string olarak belirtilen keyword'lerin aranmasi ve tespit edilmesi - varsa ve yoksa fonksiyonlarının calistirilmasi
        public override Chunk filter(Chunk datasource, EventBus eb)
        {
            throw new NotImplementedException();
        }
    }
}
