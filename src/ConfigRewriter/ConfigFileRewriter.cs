using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace ConfigRewriter
{
    public class ConfigFileRewriter
    {
        private readonly string _config;

        private readonly XDocument _xdoc;
        private readonly XNamespace _ns;

        public ConfigFileRewriter(string config)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));

            _config = config;
            _xdoc = XDocument.Parse(config, LoadOptions.PreserveWhitespace);
            _ns = _xdoc.Root.GetDefaultNamespace();
        }

        public void RewriteAppSettings(List<AppSetting> appSettings)
        {
            var configEl = _xdoc.Root;

            var appSettingsEl = configEl.Element(_ns + "appSettings");
            if (appSettingsEl == null)
            {
                appSettingsEl = new XElement("appSettings");
                configEl.Add(appSettingsEl);
            }

            foreach (var setting in appSettings)
            {
                var el = appSettingsEl.Elements(_ns + "add")
                    .Where(x => x.Attribute("key")?.Value == setting.Key)
                    .FirstOrDefault();
                if (el == null)
                {
                    el = new XElement("add");
                    appSettingsEl.Add(el);
                }

                el.SetAttributeValue("key", setting.Key);
                el.SetAttributeValue("value", setting.Value);
            }
        }

        public string Result()
        {
            var encoding = Encoding.GetEncoding(_xdoc.Declaration.Encoding ?? "utf-8");

            var writer = new CustomEncodingStringWriter(encoding);

            _xdoc.Save(writer, SaveOptions.None);

            return writer.ToString();
        }
    }

    class CustomEncodingStringWriter : StringWriter
    {
        private Encoding _encoding;

        public CustomEncodingStringWriter(Encoding encoding)
        {
            _encoding = encoding;
        }

        public override Encoding Encoding => _encoding;
    }
}