using Newtonsoft.Json;
using System.Text;

namespace JJ_API.Service
{
    public abstract class PropertiesSingletonBase
    {
        public static PropertiesSingleton Properties { get; set; } = new PropertiesSingleton();

        public readonly static string path = string.Format("C:\\Users\\Kamil\\source\\repos\\Testy2023_2024\\API\\properties.xml");
        public readonly static string pathBackup = string.Format("C:\\Users\\Kamil\\source\\repos\\Testy2023_2024\\API\\propertiesBackup.xml");
        protected internal static byte[] Encrypt(string code)
        {
            System.Security.Cryptography.DESCryptoServiceProvider crypt = new System.Security.Cryptography.DESCryptoServiceProvider();
            crypt.IV = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            crypt.Key = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            byte[] buf = Encoding.ASCII.GetBytes(code);
            using (MemoryStream ms = new MemoryStream())
            {
                System.Security.Cryptography.CryptoStream cs = new System.Security.Cryptography.CryptoStream(ms, crypt.CreateEncryptor(), System.Security.Cryptography.CryptoStreamMode.Write);
                cs.Write(buf, 0, buf.Length);
                cs.Close();
                buf = ms.ToArray();
            }
            return buf;
        }
        protected internal static string Decrypt(byte[] buf)
        {
            System.Security.Cryptography.DESCryptoServiceProvider crypt = new System.Security.Cryptography.DESCryptoServiceProvider();
            crypt.IV = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            crypt.Key = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            string kod = string.Empty;
            using (MemoryStream ms = new MemoryStream())
            {
                System.Security.Cryptography.CryptoStream cs = new System.Security.Cryptography.CryptoStream(ms, crypt.CreateDecryptor(), System.Security.Cryptography.CryptoStreamMode.Write);
                cs.Write(buf, 0, buf.Length);
                cs.Close();
                kod = Encoding.ASCII.GetString(ms.ToArray());

            }
            return kod;
        }
        public static object Load()
        {
            object o = null;
            Exception error = null;
            try
            {
                if (File.Exists(path))
                {
                    using (StreamReader sr = new StreamReader(path))
                    {
                        Properties = JsonConvert.DeserializeObject<PropertiesSingleton>(sr.ReadToEnd());
                    }
                }
            }
            catch (Exception se)
            {
                error = se;
            }
            try
            {
                if (error != null)
                {
                    if (File.Exists(path))
                    {
                        using (StreamReader sr = new StreamReader(pathBackup))
                        {
                            Properties = JsonConvert.DeserializeObject<PropertiesSingleton>(sr.ReadToEnd());
                        }
                    }
                }
            }
            catch (Exception se)
            {
            }

            return Properties;
        }
        public static void Save(object obj)
        {
            if (!Directory.Exists(Path.GetDirectoryName(path)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            }

            if (!Directory.Exists(Path.GetDirectoryName(pathBackup)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(pathBackup));
            }

            using (Stream fs = new FileStream(pathBackup, FileMode.Create, FileAccess.Write, FileShare.None, 32756, FileOptions.WriteThrough))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.Write(JsonConvert.SerializeObject(obj, Formatting.Indented));
                }
            }
            Thread.Sleep(1000);
            using (Stream fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, 32756, FileOptions.WriteThrough))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.Write(JsonConvert.SerializeObject(obj, Formatting.Indented));
                }
            }
        }
    }
}
