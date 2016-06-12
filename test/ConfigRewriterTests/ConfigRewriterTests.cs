using Xunit;
using ConfigRewriter;
using System.Collections.Generic;
using System;
using System.Xml;
using System.Xml.Linq;
using System.Linq;

namespace ConfigRewriterTests
{
    public class ConfigRewriterTests
    {
        [Fact]
        public void it_produce_valid_utf8_xml()
        {
            var xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<configuration></configuration>";
            var rewriter = new ConfigFileRewriter(xml);

            Assert.Equal(xml, rewriter.Result());
        } 

                [Fact]
        public void it_produce_valid_utf16_xml()
        {
            var xml = @"<?xml version=""1.0"" encoding=""utf-16""?>
<configuration></configuration>";
            var rewriter = new ConfigFileRewriter(xml);

            Assert.Equal(xml, rewriter.Result());
        }

        [Fact]
        public void it_throws_exception_if_configuration_element_is_missing()
        {
            var xml = @"<?xml version=""1.0"" encoding=""utf-16""?>";

            
            Assert.Throws<XmlException>(() =>
            {
                var rewriter = new ConfigFileRewriter(xml);

                rewriter.RewriteAppSettings(new List<AppSetting>
                {
                    new AppSetting
                    {
                        Key = "key",
                        Value = "value"
                    }
                });
            });
        }

        [Fact]
        public void it_adds_new_app_settings_element()
        {
            var xml = @"<?xml version=""1.0"" encoding=""utf-16""?>
<configuration></configuration>";

            var rewriter = new ConfigFileRewriter(xml);

            rewriter.RewriteAppSettings(new List<AppSetting>
            {
                new AppSetting
                {
                    Key = "key",
                    Value = "value"
                }
            });

            var result = rewriter.Result();

            var xdoc = XDocument.Parse(result);

            Assert.Single(xdoc.Root.Elements(xdoc.Root.GetDefaultNamespace() + "appSettings"));
        }

        [Fact]
        public void it_does_not_add_new_app_settings_element_if_there_is_one()
        {
            var xml = @"<?xml version=""1.0"" encoding=""utf-16""?>
<configuration><appSettings></appSettings></configuration>";

            var rewriter = new ConfigFileRewriter(xml);

            rewriter.RewriteAppSettings(new List<AppSetting>
            {
                new AppSetting
                {
                    Key = "key",
                    Value = "value"
                }
            });

            var result = rewriter.Result();

            var xdoc = XDocument.Parse(result);

            Assert.Single(xdoc.Root.Elements(xdoc.Root.GetDefaultNamespace() + "appSettings"));
        }

        [Fact]
        public void it_adds_new_app_setting_add_element()
        {
            var xml = @"<?xml version=""1.0"" encoding=""utf-16""?>
<configuration><appSettings></appSettings></configuration>";

            var rewriter = new ConfigFileRewriter(xml);

            rewriter.RewriteAppSettings(new List<AppSetting>
            {
                new AppSetting
                {
                    Key = "key",
                    Value = "value"
                },
                new AppSetting
                {
                    Key = "key1",
                    Value = "value1"
                },
            });

            var result = rewriter.Result();

            var xdoc = XDocument.Parse(result);
            var ns = xdoc.Root.GetDefaultNamespace();

            var appSettings = xdoc.Root.Elements(ns + "appSettings");

            var items = appSettings.Elements();

            var item = Assert.Single(items.Where(x => x.Attribute(ns + "key").Value == "key"));

            Assert.Equal("value", item.Attribute(ns + "value").Value);

            var item1 = Assert.Single(items.Where(x => x.Attribute(ns + "key").Value == "key1"));

            Assert.Equal("value1", item1.Attribute(ns + "value").Value);
        }

        [Fact]
        public void it_overwrites_existing_app_setting()
        {
            var xml = @"<?xml version=""1.0"" encoding=""utf-16""?>
<configuration>
    <appSettings>
        <add key=""key"" value=""old_value"" />
    </appSettings>
</configuration>";

            var rewriter = new ConfigFileRewriter(xml);

            rewriter.RewriteAppSettings(new List<AppSetting>
            {
                new AppSetting
                {
                    Key = "key",
                    Value = "value"
                }
            });

            var result = rewriter.Result();

            var xdoc = XDocument.Parse(result);
            var ns = xdoc.Root.GetDefaultNamespace();

            var appSettings = xdoc.Root.Elements(ns + "appSettings");

            var items = appSettings.Elements();

            var item = Assert.Single(items.Where(x => x.Attribute(ns + "key").Value == "key"));

            Assert.Equal("value", item.Attribute(ns + "value").Value);
        }
    }
}