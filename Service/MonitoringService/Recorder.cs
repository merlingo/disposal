using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringService
{
    public class FileRecorder : Filter
    {
        //verinin kayıt edilmesi - DataStream olarak olabilir ya da Parse edilmiş obje olarak olabilir. İlgili veritabanında ilgili vermodeline objenin kayit edilmesi
        public override Chunk filter(Chunk datasource, EventBus eb)
        {
            throw new NotImplementedException();
        }
    }
}
