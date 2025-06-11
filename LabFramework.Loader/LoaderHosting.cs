using System;
using System.IO;
using System.Text;

namespace DZCP.Loader.Hosting
{
    public static class LoaderHosting
    {
        public static readonly string RootPath = Path.Combine(AppContext.BaseDirectory, "DZCP", "LoaderHosting");
        public static readonly string PluginsPath = Path.Combine(RootPath, "plugins");
        public static readonly string LogsPath = Path.Combine(RootPath, "logs");
        public static readonly string ConfigPath = Path.Combine(RootPath, "config.json");
        public static readonly string ReadmePath = Path.Combine(RootPath, "README.txt");

        public static void Initialize()
        {
            try
            {
                Directory.CreateDirectory(RootPath);
                Directory.CreateDirectory(PluginsPath);
                Directory.CreateDirectory(LogsPath);

                // إنشاء ملف config.json إذا لم يكن موجود
                if (!File.Exists(ConfigPath))
                {
                    var defaultConfig = new
                    {
                        LoaderName = "DZCP.Loader",
                        Version = "1.0.0",
                        AutoLoadPlugins = true,
                        DebugMode = false
                    };

                    string json = JsonSerializer.Serialize(defaultConfig, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(ConfigPath, json);
                }

                // إنشاء README.txt
                if (!File.Exists(ReadmePath))
                {
                    string content = "📦 DZCP LoaderHosting\n\n" +
                                     "هذا المجلد يحتوي على الملفات الأساسية لتحميل البلغنات وتشغيل النظام.\n" +
                                     "- ضع بلغناتك داخل مجلد plugins/\n" +
                                     "- يتم إنشاء ملفات السجل داخل logs/\n" +
                                     "- يمكنك تعديل إعدادات loader من config.json\n";

                    File.WriteAllText(ReadmePath, content);
                }

                Console.WriteLine($"[DZCP] تم تهيئة LoaderHosting بنجاح في: {RootPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DZCP] خطأ أثناء تهيئة LoaderHosting: {ex.Message}");
            }
        }
    }
}
