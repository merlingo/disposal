using System.Collections.Generic;
using System.IO;
using System;
using System.Text;

namespace MonitoringService
{
    public abstract class Extractor
    {
        //extract etme işlemlerinden sorumlu abstract yapı - extractor sınıflar stream'den metadata'ya uygun olarak verilerin birim chunklar olarak çıkarmasını sağlar. 
        public abstract IEnumerable<Chunk> extract(Stream dataSource, Metadata metadata);
    }

 
}
