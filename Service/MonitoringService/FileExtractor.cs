using System.Collections.Generic;
using System.IO;
using System;
using System.Text;

namespace MonitoringService
{
    public class FileExtractor : Extractor
    {
        String chunkSep;
        string startBuf, endBuf;
        public FileExtractor(String sep)
        {
            chunkSep = sep;
            endBuf = ""; startBuf = "";
        }
        public FileExtractor(String sb, String eb)
        {
            chunkSep = "";
            startBuf = sb;
            endBuf = eb;
        }
        public override IEnumerable<Chunk> extract(Stream dataSource, Metadata metadata)
        {
            StreamReader reader = new StreamReader(dataSource);
            reader.DiscardBufferedData();
            reader.BaseStream.Seek(0, System.IO.SeekOrigin.Begin);
            var sb = new StringBuilder();
            Chunk chunk = new Chunk();
            chunk.Metadata = metadata;

            var sepbuffer = new Queue<char>(chunkSep.Length);
            var sepArray = chunkSep.ToCharArray();
            int i = 0;
            if (chunkSep != "")
            {
                while (reader.Peek() >= 0)
                {
                    var nextChar = (char)reader.Read();
                    if (nextChar == chunkSep[sepbuffer.Count])
                    {
                        sepbuffer.Enqueue(nextChar);
                        if (sepbuffer.Count == chunkSep.Length)
                        {
                            chunk = new Chunk();
                            chunk.Metadata = metadata;
                            chunk.rawString = sb.ToString();
                            chunk.rawData = Encoding.ASCII.GetBytes(chunk.rawString);
                            chunk.index = i;
                            i++;
                            yield return chunk;
                            sb.Length = 0;
                            sepbuffer.Clear();
                        }
                    }
                    else
                    {
                        sepbuffer.Enqueue(nextChar);
                        while (sepbuffer.Count > 0)
                        {
                            sb.Append(sepbuffer.Dequeue());
                        }
                    }
                }
                chunk = new Chunk();
                chunk.Metadata = metadata;
                chunk.rawString = sb.ToString();
                chunk.rawData = Encoding.ASCII.GetBytes(chunk.rawString);
                yield return chunk;

            }
            else
            {
                //NOT: START VE END STRINGLERI AYNI OLURSA HATA VERIR.
                if (startBuf == endBuf)
                {
                    throw new Exception("FileExtractor.extract: Start ve End bufferlari ayni olamaz!!");
                }
                char[] startb = startBuf.ToCharArray();
                char[] endb = endBuf.ToCharArray();
                bool icerde = false;
                int si = 0, ei = 0;
                while (reader.Peek() >= 0)
                {
                    Char nextChar = (char)reader.Read();

                    if (icerde || nextChar == startb[si])
                    {
                        //chunk olarak dondurulecek yer basladi mi? bu start string'inin tamamı bulundugunda icerde degerinin true yapilmasi ile gerceklesir.
                        if (nextChar == startb[si])
                        {
                            if (si == startb.Length - 1)
                            {
                                icerde = true;
                                si = 0; //start indeksi basa alalim.
                            }
                            else
                                si++;
                        }
                        sb.Append(nextChar);
                        //icerde ve endb basladiginda. chunkin sonu yakalandi. ya FP yani yakalandi ama devam etmedi. Ya da TP yakaladi ve bitirdi. 
                        if (icerde && nextChar == endb[ei])//&& (icerde)  - icerde olmasa bu calisamaz.
                        {
                            if (ei == endb.Length - 1)
                            {
                                icerde = false;
                                ei = 0;
                                chunk = new Chunk();
                                chunk.Metadata = metadata;
                                chunk.rawString = sb.ToString();
                                chunk.rawData = Encoding.ASCII.GetBytes(chunk.rawString);
                                chunk.index = i;
                                yield return chunk;
                                sb.Clear();
                            }
                            else
                                ei++;
                        }
                        else if (ei > 0)
                        {
                            //FP yani yakalandi ama devam etmedi. saymaya bastan baslasin
                            ei = 0;
                        }
                    }
                    else
                    {
                        //icerde degil ve nextChar bir sonraki si degerine esit degil. eger sb doluysa startb degeri yarim kalmistir. si=0 ve sb=0 yapilmali.
                        if (si > 0)
                        {
                            si = 0;
                            sb.Clear();
                        }
                    }
                    i++;
                }

            }
        }
        public static IEnumerable<byte[]> ReadChunks(string path)
        {
            var lengthBytes = new byte[sizeof(int)];

            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                int n = fs.Read(lengthBytes, 0, sizeof(int));  // Read block size.

                if (n == 0)      // End of file.
                    yield break;

                if (n != sizeof(int))
                    throw new InvalidOperationException("Invalid header");

                int blockLength = BitConverter.ToInt32(lengthBytes, 0);
                var buffer = new byte[blockLength];
                n = fs.Read(buffer, 0, blockLength);

                if (n != blockLength)
                    throw new InvalidOperationException("Missing data");

                yield return buffer;
            }
        }
    }
}
