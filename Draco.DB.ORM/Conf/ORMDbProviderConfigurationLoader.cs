using System;
using System.Collections.Generic;
using System.Text;
using Draco.DB.QuickDataBase.Configuration;
using System.IO;
using System.Xml;

namespace Draco.DB.ORM.Conf
{
    internal class ORMDbProviderConfigurationLoader : DbProviderConfigurationLoader
    {
        public override DbProviderFactoriesConfiguration Load()
        {
            String filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DefaultConfigurationFileName);
            if (File.Exists(filePath))
                return Load(filePath);
            Stream stream = this.GetType().Assembly.GetManifestResourceStream("Draco.DB.ORM.Conf.DbAdapter.config");
            if (stream != null && stream.Length > 0)
            {
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(stream);
                return Load(xDoc);
            }
            return null;
        }
    }
}
