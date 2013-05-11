using System.Collections.Generic;
using System.IO;

namespace HelperLib
{
    public static class IO
    {
        /// <summary>
        /// Reads text file and returns it one line per item.
        /// </summary>
        /// <param name="p">Path to file</param>
        /// <returns></returns>
        public static IEnumerable<string> ReadFileIntoList(string p)
        {
            using (var r = new StreamReader(p))
            {
                string line;
                while ((line = r.ReadLine()) != null)
                {
                    yield return line;
                }
            }
        }

        /// <summary>
        /// Reads text file and returns its content as string
        /// </summary>
        /// <param name="p">Path to file</param>
        /// <returns></returns>
        public static string ReadToString(string p)
        {
            using (var r = new StreamReader(p))
            {
                return r.ReadToEnd();
            }
        }

        /// <summary>
        /// Writes string to text file
        /// </summary>
        /// <param name="path">Path to file</param>
        /// <param name="data">String to write</param>
        public static void WriteStringToFile(string path, string data)
        {
            using (var w = new StreamWriter(path))
            {
                w.Write(data);
                w.Close();
            }
        }
    }
}
