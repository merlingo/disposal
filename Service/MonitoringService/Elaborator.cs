using System;
using System.IO;
using System.Collections.Generic;
using System.Net;


namespace MonitoringService
{
    public struct Metadata
    {
        //Degisik kaynaklardan çekilen vulnerability verisi için, farklı kaynakların tanımlanması için kullanılan veri yapısı
        public String fileType; //veri kaynagının tipi: XML,JSON,CSV
        public String ObjectName; //
        public string isFieldAttr;
        public AttributesDataModel attributes;
    }
    public class Chunk
    {
        //Stream'den alinan parca ve pipe boyunca filterdan filter'a iletilen veri yapisi
        public byte[] rawData;
        public String rawString;
        public Metadata Metadata { get; set; }
        public DataModel dm { get; set; }
        public int index;

    }
    public class Elaborator
    {
        //buyuk verinin alinmasi ve veritabanina parcalanmis olarak iletilmesinden gorevli connector, pipe and filter mimarisi. Tek seferlik büyük verinin ya da api ile çekilen internet verisinin
        //repository'e eklenmesinden sorumlu
        //taşınan: Stream - Stream yapısına uygun her türlü veri olabilir. Stream  Filterlar içinde StreamReader ya da StreamWriter ile işlenir.
        //pipe: Elaborator: alici ve verici vardır. Alicidan alinan ile vericiye verilen "data" nin uyumlu olmasini saglar. en bastaki filter sabitir:transformer - buyuk dosya tipinden parcalanmis veri gruplarini olusturur
        //filter: veri ya da veri listesi alir, veri ya da veri listesi gonderir. saklama, tespit, parse, kayit ve kullanim modulleri vardir.
        Queue<Filter> filters;
        public Extractor root;
        public FileDetector fileDetector;
        //veri kaynagi root'a verilir, her dataStream pipe tarafindan yieldlenerek filterler'e sokulur
        string host;
        string ip;
        string filename;
        string dataType;
        EventBus eb;
        public Elaborator()
        {
            filters = new Queue<Filter>();
        }


        public Elaborator(Extractor ex, FileDetector fd, string fn, List<Filter> fs, string dataType)
        {
            filters = new Queue<Filter>();
            this.host = Dns.GetHostName();
            this.ip = EventBus.GetLocalIPAddress();
            root = ex;
            fileDetector = fd;
            filename = fn;
            this.dataType = dataType;

            foreach (Filter filter in fs)
                filters.Enqueue(filter);
        }
        public void setEvenBus(EventBus eb)
        {
            this.eb = eb;
        }

        public void pipe()
        {
            // dataStream yapısı System.IO kütüphanesindeki Stream abstract classından olusacaktir. Filter'daki filter fonksiyonlari Stream için StreamReader ya da StreamWriter ile isleyceklerdir.
            Stream dataSource = null;
            if (dataType.Trim().Equals("File"))
                dataSource = new FileStream(filename, FileMode.Open);
            else if (dataType.Trim().Equals("Network"))
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(filename);
                request.Method = "GET";
                WebResponse response = request.GetResponse();
                dataSource = response.GetResponseStream();
            }
            else
                throw new Exception("Config file exception!!! DATATYPE should be file or Network");
            //önce file detector'e verilmeli - cünkü bir kere attributesler tespit edilse yeter. sonra obje oluşturulup değerlendirilir. Eğer obje db'de yoksa attributes çikarilir kayit edilir. varsa oradan çekilir.
            Metadata metadata = new Metadata();
            if (fileDetector != null)
                metadata = fileDetector.detect(dataSource, eb);

            IEnumerable<Chunk> list = root.extract(dataSource, metadata);
            IEnumerator<Chunk> it = list.GetEnumerator();
            while (it.MoveNext())
            {
                Chunk chunk = it.Current;
                foreach (Filter f in filters)
                {
                    if (chunk == null)//herhangi bir filterda ya da oncesinde bir problem oldugunda chunk null yapılır. boylece pipe kırılır ve bir sonraki veriye geçer it.next
                        break;
                    if (f == null)//herhangi bir filterda ya da oncesinde bir problem oldugunda chunk null yapılır. boylece pipe kırılır ve bir sonraki veriye geçer it.next
                        continue;
                    chunk = f.filter(chunk, eb);
                }
            }
        }
    }
}
