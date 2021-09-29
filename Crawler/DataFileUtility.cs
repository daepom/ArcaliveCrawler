using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Crawler
{
    public static class DataFileUtility
    {
        public static void SerializePosts(List<PostInfo> posts, string filename = "a.dat")
        {
            using (Stream ws = new FileStream(filename, FileMode.Create))
            {
                BinaryFormatter binary = new BinaryFormatter();
                binary.Serialize(ws, posts);
            }
        }

        public static List<PostInfo> DeserializePosts(string filename = "a.dat")
        {
            using (Stream rs = new FileStream(filename, FileMode.Open))
            {
                BinaryFormatter binary = new BinaryFormatter();
                return (List<PostInfo>)binary.Deserialize(rs);
            }
        }
    }
}
