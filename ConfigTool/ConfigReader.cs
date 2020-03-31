using System;
using System.IO;
using System.Runtime.Serialization.Json;

namespace ConfigTool
{
    public class ConfigReader<T>
    {
        T config;
        public T Config => config;
        public ConfigReader(string jsonfile, T taget)
        {
            FileInfo info = new FileInfo($"{AppContext.BaseDirectory}\\{jsonfile}");
            if (info.Exists)
            {
                StreamReader reader = null;
                try
                {
                    using (reader = info.OpenText())
                    {
                        var ser = new DataContractJsonSerializer(taget.GetType());
                        config = (T)(ser.ReadObject(reader.BaseStream));
                    }
                }
                catch
                {

                }
                finally
                {
                    if (reader != null)
                        reader.Close();
                }
            }
        }
    }
}