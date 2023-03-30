using System.IO;

namespace OSS.Common
{
    public class Tools
    {
        /// <summary>
        /// 系统留痕
        /// </summary>
        /// <param name="ip">IP</param>
        /// <param name="conten">内容</param>
        public static void Writerlog(string ip, string path, string fileName)
        {
            var logPath = Directory.GetCurrentDirectory() + "/Content/";
            using (StreamWriter stream = new StreamWriter(logPath + "log.txt", true))
            {
                stream.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}\t\t{ip}\t\t{path}\t\t{fileName}");
                stream.Close();
            }
        }
    }
}
