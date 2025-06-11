import React, { useState } from 'react';
import { BrowserRouter as Router, Routes, Route, Link, useLocation } from 'react-router-dom';
import { Button } from '@/components/ui/button.jsx';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card.jsx';
import { Badge } from '@/components/ui/badge.jsx';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs.jsx';
import { 
  BookOpen, 
  Code, 
  Download, 
  Github, 
  Menu, 
  X, 
  Zap, 
  Shield, 
  Settings, 
  Gamepad2,
  ChevronRight,
  Star,
  Users,
  Rocket,
  FileText,
  Terminal,
  Play
} from 'lucide-react';
import './App.css';

// Navigation Component
const Navigation = ({ isOpen, setIsOpen }) => {
  const location = useLocation();
  
  const navItems = [
    { path: '/', label: 'الرئيسية', icon: BookOpen },
    { path: '/docs', label: 'الوثائق', icon: FileText },
    { path: '/quickstart', label: 'البدء السريع', icon: Play },
    { path: '/examples', label: 'الأمثلة', icon: Code },
    { path: '/download', label: 'التحميل', icon: Download },
  ];

  return (
    <nav className="bg-white dark:bg-gray-900 border-b border-gray-200 dark:border-gray-700 sticky top-0 z-50">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="flex justify-between h-16">
          <div className="flex items-center">
            <Link to="/" className="flex items-center space-x-2">
              <div className="w-8 h-8 bg-blue-600 rounded-lg flex items-center justify-center">
                <Zap className="w-5 h-5 text-white" />
              </div>
              <span className="text-xl font-bold text-gray-900 dark:text-white">LabFramework</span>
            </Link>
          </div>
          
          {/* Desktop Navigation */}
          <div className="hidden md:flex items-center space-x-8">
            {navItems.map((item) => {
              const Icon = item.icon;
              return (
                <Link
                  key={item.path}
                  to={item.path}
                  className={`flex items-center space-x-1 px-3 py-2 rounded-md text-sm font-medium transition-colors ${
                    location.pathname === item.path
                      ? 'text-blue-600 bg-blue-50 dark:bg-blue-900/20'
                      : 'text-gray-700 dark:text-gray-300 hover:text-blue-600 dark:hover:text-blue-400'
                  }`}
                >
                  <Icon className="w-4 h-4" />
                  <span>{item.label}</span>
                </Link>
              );
            })}
            <Button asChild>
              <a href="https://github.com/labframework/labframework" target="_blank" rel="noopener noreferrer">
                <Github className="w-4 h-4 mr-2" />
                GitHub
              </a>
            </Button>
          </div>

          {/* Mobile menu button */}
          <div className="md:hidden flex items-center">
            <Button
              variant="ghost"
              size="sm"
              onClick={() => setIsOpen(!isOpen)}
            >
              {isOpen ? <X className="w-5 h-5" /> : <Menu className="w-5 h-5" />}
            </Button>
          </div>
        </div>
      </div>

      {/* Mobile Navigation */}
      {isOpen && (
        <div className="md:hidden">
          <div className="px-2 pt-2 pb-3 space-y-1 sm:px-3 bg-white dark:bg-gray-900 border-t border-gray-200 dark:border-gray-700">
            {navItems.map((item) => {
              const Icon = item.icon;
              return (
                <Link
                  key={item.path}
                  to={item.path}
                  className={`flex items-center space-x-2 px-3 py-2 rounded-md text-base font-medium transition-colors ${
                    location.pathname === item.path
                      ? 'text-blue-600 bg-blue-50 dark:bg-blue-900/20'
                      : 'text-gray-700 dark:text-gray-300 hover:text-blue-600 dark:hover:text-blue-400'
                  }`}
                  onClick={() => setIsOpen(false)}
                >
                  <Icon className="w-4 h-4" />
                  <span>{item.label}</span>
                </Link>
              );
            })}
          </div>
        </div>
      )}
    </nav>
  );
};

// Home Page Component
const HomePage = () => {
  return (
    <div className="min-h-screen">
      {/* Hero Section */}
      <section className="bg-gradient-to-br from-blue-50 to-indigo-100 dark:from-gray-900 dark:to-gray-800 py-20">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="text-center">
            <h1 className="text-4xl md:text-6xl font-bold text-gray-900 dark:text-white mb-6">
              LabFramework
            </h1>
            <p className="text-xl md:text-2xl text-gray-600 dark:text-gray-300 mb-8 max-w-3xl mx-auto">
              إطار عمل متقدم وأفضل من EXILED لتطوير البرامج المساعدة لخوادم SCP: Secret Laboratory
            </p>
            <div className="flex flex-col sm:flex-row gap-4 justify-center">
              <Button size="lg" asChild>
                <Link to="/quickstart">
                  <Play className="w-5 h-5 mr-2" />
                  البدء السريع
                </Link>
              </Button>
              <Button size="lg" variant="outline" asChild>
                <Link to="/docs">
                  <BookOpen className="w-5 h-5 mr-2" />
                  الوثائق
                </Link>
              </Button>
            </div>
          </div>
        </div>
      </section>

      {/* Features Section */}
      <section className="py-20">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="text-center mb-16">
            <h2 className="text-3xl md:text-4xl font-bold text-gray-900 dark:text-white mb-4">
              لماذا LabFramework؟
            </h2>
            <p className="text-lg text-gray-600 dark:text-gray-300 max-w-2xl mx-auto">
              مصمم ليكون أسرع وأسهل وأكثر قوة من EXILED مع تكامل مثالي مع LabAPI
            </p>
          </div>

          <div className="grid md:grid-cols-2 lg:grid-cols-3 gap-8">
            <Card>
              <CardHeader>
                <Rocket className="w-8 h-8 text-blue-600 mb-2" />
                <CardTitle>أداء فائق</CardTitle>
                <CardDescription>
                  محسّن للسرعة وقلة استهلاك الذاكرة مع معالجة غير متزامنة للأحداث
                </CardDescription>
              </CardHeader>
            </Card>

            <Card>
              <CardHeader>
                <Code className="w-8 h-8 text-green-600 mb-2" />
                <CardTitle>سهولة التطوير</CardTitle>
                <CardDescription>
                  واجهات برمجية بسيطة ونظام أوامر متقدم مع تحليل تلقائي للمعاملات
                </CardDescription>
              </CardHeader>
            </Card>

            <Card>
              <CardHeader>
                <Shield className="w-8 h-8 text-purple-600 mb-2" />
                <CardTitle>نظام صلاحيات متقدم</CardTitle>
                <CardDescription>
                  إدارة هرمية للمجموعات والصلاحيات مع دعم الوراثة والصلاحيات المتداخلة
                </CardDescription>
              </CardHeader>
            </Card>

            <Card>
              <CardHeader>
                <Gamepad2 className="w-8 h-8 text-red-600 mb-2" />
                <CardTitle>عناصر مخصصة</CardTitle>
                <CardDescription>
                  إنشاء عناصر مخصصة بسلوكيات معقدة ونظام المتانة والتراكم
                </CardDescription>
              </CardHeader>
            </Card>

            <Card>
              <CardHeader>
                <Settings className="w-8 h-8 text-orange-600 mb-2" />
                <CardTitle>إدارة تكوين مرنة</CardTitle>
                <CardDescription>
                  تحديثات فورية للإعدادات مع دعم الأنواع المختلفة والتحقق التلقائي
                </CardDescription>
              </CardHeader>
            </Card>

            <Card>
              <CardHeader>
                <BookOpen className="w-8 h-8 text-indigo-600 mb-2" />
                <CardTitle>وثائق شاملة</CardTitle>
                <CardDescription>
                  أدلة مفصلة وأمثلة عملية ومرجع شامل للواجهات البرمجية
                </CardDescription>
              </CardHeader>
            </Card>
          </div>
        </div>
      </section>

      {/* Comparison Section */}
      <section className="bg-gray-50 dark:bg-gray-800 py-20">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="text-center mb-16">
            <h2 className="text-3xl md:text-4xl font-bold text-gray-900 dark:text-white mb-4">
              مقارنة مع EXILED
            </h2>
          </div>

          <div className="overflow-x-auto">
            <table className="w-full bg-white dark:bg-gray-900 rounded-lg shadow-lg">
              <thead className="bg-gray-50 dark:bg-gray-800">
                <tr>
                  <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">
                    الميزة
                  </th>
                  <th className="px-6 py-3 text-center text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">
                    LabFramework
                  </th>
                  <th className="px-6 py-3 text-center text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">
                    EXILED
                  </th>
                </tr>
              </thead>
              <tbody className="divide-y divide-gray-200 dark:divide-gray-700">
                <tr>
                  <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900 dark:text-white">
                    الأداء
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-center">
                    <Badge className="bg-green-100 text-green-800">محسّن للأداء العالي</Badge>
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-center">
                    <Badge variant="secondary">أداء متوسط</Badge>
                  </td>
                </tr>
                <tr>
                  <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900 dark:text-white">
                    سهولة الاستخدام
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-center">
                    <Badge className="bg-green-100 text-green-800">واجهات بسيطة وواضحة</Badge>
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-center">
                    <Badge variant="secondary">معقد نسبياً</Badge>
                  </td>
                </tr>
                <tr>
                  <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900 dark:text-white">
                    التكامل مع LabAPI
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-center">
                    <Badge className="bg-green-100 text-green-800">تكامل مباشر ومحسّن</Badge>
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-center">
                    <Badge variant="secondary">يتطلب طبقات إضافية</Badge>
                  </td>
                </tr>
                <tr>
                  <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900 dark:text-white">
                    نظام الأوامر
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-center">
                    <Badge className="bg-green-100 text-green-800">تحليل تلقائي متقدم</Badge>
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-center">
                    <Badge variant="secondary">تحليل يدوي</Badge>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </section>

      {/* CTA Section */}
      <section className="py-20">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 text-center">
          <h2 className="text-3xl md:text-4xl font-bold text-gray-900 dark:text-white mb-4">
            جاهز للبدء؟
          </h2>
          <p className="text-lg text-gray-600 dark:text-gray-300 mb-8 max-w-2xl mx-auto">
            ابدأ في تطوير البرامج المساعدة الخاصة بك باستخدام LabFramework اليوم
          </p>
          <div className="flex flex-col sm:flex-row gap-4 justify-center">
            <Button size="lg" asChild>
              <Link to="/download">
                <Download className="w-5 h-5 mr-2" />
                تحميل LabFramework
              </Link>
            </Button>
            <Button size="lg" variant="outline" asChild>
              <Link to="/examples">
                <Code className="w-5 h-5 mr-2" />
                عرض الأمثلة
              </Link>
            </Button>
          </div>
        </div>
      </section>
    </div>
  );
};

// Documentation Page Component
const DocsPage = () => {
  return (
    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
      <div className="mb-8">
        <h1 className="text-4xl font-bold text-gray-900 dark:text-white mb-4">الوثائق</h1>
        <p className="text-lg text-gray-600 dark:text-gray-300">
          دليل شامل لاستخدام وتطوير LabFramework
        </p>
      </div>

      <Tabs defaultValue="overview" className="w-full">
        <TabsList className="grid w-full grid-cols-4">
          <TabsTrigger value="overview">نظرة عامة</TabsTrigger>
          <TabsTrigger value="installation">التثبيت</TabsTrigger>
          <TabsTrigger value="api">مرجع API</TabsTrigger>
          <TabsTrigger value="advanced">متقدم</TabsTrigger>
        </TabsList>

        <TabsContent value="overview" className="mt-8">
          <Card>
            <CardHeader>
              <CardTitle>نظرة عامة على LabFramework</CardTitle>
              <CardDescription>
                تعرف على المفاهيم الأساسية والميزات الرئيسية
              </CardDescription>
            </CardHeader>
            <CardContent className="prose dark:prose-invert max-w-none">
              <h3>ما هو LabFramework؟</h3>
              <p>
                LabFramework هو إطار عمل متقدم لتطوير البرامج المساعدة (Plugins) لخوادم SCP: Secret Laboratory. 
                تم تصميمه ليكون أسرع وأسهل وأكثر قوة من EXILED مع تكامل مثالي مع LabAPI.
              </p>
              
              <h3>المميزات الرئيسية</h3>
              <ul>
                <li><strong>أداء عالي:</strong> محسّن للسرعة وقلة استهلاك الذاكرة</li>
                <li><strong>سهولة التطوير:</strong> واجهات برمجية بسيطة وواضحة</li>
                <li><strong>نظام صلاحيات متقدم:</strong> إدارة هرمية للمجموعات والصلاحيات</li>
                <li><strong>عناصر مخصصة:</strong> إنشاء عناصر بسلوكيات معقدة</li>
                <li><strong>إدارة تكوين مرنة:</strong> تحديثات فورية للإعدادات</li>
              </ul>

              <h3>البنية المعمارية</h3>
              <p>
                يتكون LabFramework من عدة مكونات أساسية:
              </p>
              <ul>
                <li><strong>Core:</strong> المكونات الأساسية للإطار</li>
                <li><strong>LabAPI:</strong> طبقة التكامل مع LabAPI</li>
                <li><strong>Commands:</strong> نظام الأوامر المتقدم</li>
                <li><strong>Permissions:</strong> نظام الصلاحيات</li>
                <li><strong>CustomItems:</strong> العناصر المخصصة</li>
                <li><strong>Loader:</strong> نظام تحميل البرامج المساعدة</li>
              </ul>
            </CardContent>
          </Card>
        </TabsContent>

        <TabsContent value="installation" className="mt-8">
          <Card>
            <CardHeader>
              <CardTitle>دليل التثبيت</CardTitle>
              <CardDescription>
                خطوات تثبيت وإعداد LabFramework
              </CardDescription>
            </CardHeader>
            <CardContent className="prose dark:prose-invert max-w-none">
              <h3>متطلبات النظام</h3>
              <ul>
                <li>.NET 8.0 SDK أو أحدث</li>
                <li>خادم SCP: Secret Laboratory مع LabAPI مثبت</li>
                <li>نظام تشغيل Linux/Windows</li>
              </ul>

              <h3>التثبيت السريع</h3>
              <div className="bg-gray-100 dark:bg-gray-800 p-4 rounded-lg">
                <pre><code>{`# استخراج الملفات
tar -xzf LabFramework-Complete.tar.gz

# الانتقال إلى مجلد الإطار
cd LabFramework

# تشغيل سكريبت التثبيت
chmod +x install.sh
./install.sh`}</code></pre>
              </div>

              <h3>البناء من المصدر</h3>
              <div className="bg-gray-100 dark:bg-gray-800 p-4 rounded-lg">
                <pre><code>{`# بناء جميع المشاريع
dotnet build --configuration Release

# تشغيل الاختبارات
cd tests/LabFramework.Tests
dotnet run`}</code></pre>
              </div>

              <h3>التحقق من التثبيت</h3>
              <p>
                بعد التثبيت، يمكنك التحقق من عمل الإطار بتشغيل:
              </p>
              <div className="bg-gray-100 dark:bg-gray-800 p-4 rounded-lg">
                <pre><code>{`cd src/LabFramework.Console
dotnet run`}</code></pre>
              </div>
            </CardContent>
          </Card>
        </TabsContent>

        <TabsContent value="api" className="mt-8">
          <Card>
            <CardHeader>
              <CardTitle>مرجع API</CardTitle>
              <CardDescription>
                مرجع شامل للواجهات البرمجية
              </CardDescription>
            </CardHeader>
            <CardContent className="prose dark:prose-invert max-w-none">
              <h3>الفئات الأساسية</h3>
              
              <h4>BasePlugin</h4>
              <p>الفئة الأساسية لجميع البرامج المساعدة:</p>
              <div className="bg-gray-100 dark:bg-gray-800 p-4 rounded-lg">
                <pre><code>{`public abstract class BasePlugin : IPlugin
{
    public abstract string Name { get; }
    public abstract string Version { get; }
    public abstract string Author { get; }
    public abstract string Description { get; }
    
    public virtual async Task OnLoadAsync() { }
    public virtual async Task OnUnloadAsync() { }
}`}</code></pre>
              </div>

              <h4>CommandAttribute</h4>
              <p>لتعريف الأوامر:</p>
              <div className="bg-gray-100 dark:bg-gray-800 p-4 rounded-lg">
                <pre><code>{`[Command("hello", "Say hello to a player")]
public CommandResult HelloCommand(CommandContext context,
    [CommandParameter("name", "Player name")] string name)
{
    return CommandResult.Successful($"Hello, {name}!");
}`}</code></pre>
              </div>

              <h4>EventBus</h4>
              <p>نظام الأحداث:</p>
              <div className="bg-gray-100 dark:bg-gray-800 p-4 rounded-lg">
                <pre><code>{`// الاشتراك في حدث
EventBus.Subscribe<PlayerJoinedEvent>(OnPlayerJoined);

// إلغاء الاشتراك
EventBus.Unsubscribe<PlayerJoinedEvent>(OnPlayerJoined);

// إطلاق حدث
await EventBus.PublishAsync(new PlayerJoinedEvent { ... });`}</code></pre>
              </div>

              <h4>ServiceContainer</h4>
              <p>حاوي حقن التبعيات:</p>
              <div className="bg-gray-100 dark:bg-gray-800 p-4 rounded-lg">
                <pre><code>{`// تسجيل خدمة
ServiceContainer.Register<IMyService, MyService>();

// الحصول على خدمة
var service = ServiceContainer.Resolve<IMyService>();`}</code></pre>
              </div>
            </CardContent>
          </Card>
        </TabsContent>

        <TabsContent value="advanced" className="mt-8">
          <Card>
            <CardHeader>
              <CardTitle>المواضيع المتقدمة</CardTitle>
              <CardDescription>
                مواضيع متقدمة لتطوير البرامج المساعدة
              </CardDescription>
            </CardHeader>
            <CardContent className="prose dark:prose-invert max-w-none">
              <h3>العناصر المخصصة</h3>
              <p>إنشاء عناصر مخصصة بسلوكيات معقدة:</p>
              <div className="bg-gray-100 dark:bg-gray-800 p-4 rounded-lg">
                <pre><code>{`var customItem = new CustomItemDefinition
{
    Id = "super_weapon",
    Name = "Super Weapon",
    Description = "A powerful weapon",
    IsStackable = false,
    MaxDurability = 100
};

_customItemService.RegisterItem(customItem, new SuperWeaponBehavior());`}</code></pre>
              </div>

              <h3>نظام الصلاحيات</h3>
              <p>إدارة الصلاحيات والمجموعات:</p>
              <div className="bg-gray-100 dark:bg-gray-800 p-4 rounded-lg">
                <pre><code>{`// إنشاء مجموعة
await permissionService.CreateGroupAsync("moderators", "Moderators");

// إضافة صلاحية لمجموعة
await permissionService.SetGroupPermissionAsync("moderators", "kick.players", true);

// إضافة مستخدم لمجموعة
await permissionService.AddUserToGroupAsync("user123", "moderators");`}</code></pre>
              </div>

              <h3>التكوين المتقدم</h3>
              <p>إدارة الإعدادات والتكوين:</p>
              <div className="bg-gray-100 dark:bg-gray-800 p-4 rounded-lg">
                <pre><code>{`// قراءة إعداد
var maxPlayers = await configService.GetAsync<int>("server.max_players", 20);

// كتابة إعداد
await configService.SetAsync("server.max_players", 30);

// الاستماع لتغييرات الإعدادات
configService.OnConfigChanged += (key, value) => {
    Logger.LogInformation($"Setting {key} changed to {value}");
};`}</code></pre>
              </div>

              <h3>الأداء والتحسين</h3>
              <ul>
                <li>استخدم البرمجة غير المتزامنة (async/await) دائماً</li>
                <li>تجنب العمليات المكلفة في معالجات الأحداث</li>
                <li>استخدم التخزين المؤقت للبيانات المتكررة</li>
                <li>قم بتنظيف الموارد في OnUnloadAsync</li>
              </ul>
            </CardContent>
          </Card>
        </TabsContent>
      </Tabs>
    </div>
  );
};

// Quick Start Page Component
const QuickStartPage = () => {
  return (
    <div className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
      <div className="mb-8">
        <h1 className="text-4xl font-bold text-gray-900 dark:text-white mb-4">البدء السريع</h1>
        <p className="text-lg text-gray-600 dark:text-gray-300">
          ابدأ في تطوير أول برنامج مساعد خلال دقائق
        </p>
      </div>

      <div className="space-y-8">
        <Card>
          <CardHeader>
            <CardTitle className="flex items-center">
              <span className="bg-blue-600 text-white rounded-full w-6 h-6 flex items-center justify-center text-sm mr-3">1</span>
              التثبيت
            </CardTitle>
          </CardHeader>
          <CardContent>
            <p className="mb-4">قم بتحميل واستخراج LabFramework:</p>
            <div className="bg-gray-100 dark:bg-gray-800 p-4 rounded-lg">
              <pre><code>{`tar -xzf LabFramework-Complete.tar.gz
cd LabFramework
chmod +x install.sh
./install.sh`}</code></pre>
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle className="flex items-center">
              <span className="bg-blue-600 text-white rounded-full w-6 h-6 flex items-center justify-center text-sm mr-3">2</span>
              إنشاء مشروع جديد
            </CardTitle>
          </CardHeader>
          <CardContent>
            <p className="mb-4">أنشئ مجلد جديد لبرنامجك المساعد:</p>
            <div className="bg-gray-100 dark:bg-gray-800 p-4 rounded-lg">
              <pre><code>{`mkdir MyFirstPlugin
cd MyFirstPlugin
dotnet new classlib`}</code></pre>
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle className="flex items-center">
              <span className="bg-blue-600 text-white rounded-full w-6 h-6 flex items-center justify-center text-sm mr-3">3</span>
              إضافة المراجع
            </CardTitle>
          </CardHeader>
          <CardContent>
            <p className="mb-4">أضف مراجع LabFramework:</p>
            <div className="bg-gray-100 dark:bg-gray-800 p-4 rounded-lg">
              <pre><code>{`dotnet add reference ../LabFramework/LabFramework.Core.dll
dotnet add reference ../LabFramework/LabFramework.Commands.dll`}</code></pre>
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle className="flex items-center">
              <span className="bg-blue-600 text-white rounded-full w-6 h-6 flex items-center justify-center text-sm mr-3">4</span>
              كتابة الكود
            </CardTitle>
          </CardHeader>
          <CardContent>
            <p className="mb-4">استبدل محتوى Class1.cs بالكود التالي:</p>
            <div className="bg-gray-100 dark:bg-gray-800 p-4 rounded-lg overflow-x-auto">
              <pre><code>{`using LabFramework.Core;
using LabFramework.Commands;

namespace MyFirstPlugin
{
    public class Plugin : BasePlugin
    {
        public override string Name => "My First Plugin";
        public override string Version => "1.0.0";
        public override string Author => "Your Name";
        public override string Description => "My first LabFramework plugin";

        public override async Task OnLoadAsync()
        {
            await base.OnLoadAsync();
            
            var commandService = ServiceContainer.Resolve<ICommandService>();
            commandService.RegisterCommands(this);
            
            Logger.LogInformation("My First Plugin loaded successfully!");
        }

        [Command("hello", "Say hello to a player")]
        public CommandResult HelloCommand(CommandContext context,
            [CommandParameter("name", "Player name")] string name)
        {
            return CommandResult.Successful($"Hello, {name}!");
        }
    }
}`}</code></pre>
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle className="flex items-center">
              <span className="bg-blue-600 text-white rounded-full w-6 h-6 flex items-center justify-center text-sm mr-3">5</span>
              البناء والنشر
            </CardTitle>
          </CardHeader>
          <CardContent>
            <p className="mb-4">ابن ونشر البرنامج المساعد:</p>
            <div className="bg-gray-100 dark:bg-gray-800 p-4 rounded-lg">
              <pre><code>{`dotnet build --configuration Release
cp bin/Release/net8.0/MyFirstPlugin.dll ../LabFramework/plugins/`}</code></pre>
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle className="flex items-center">
              <span className="bg-blue-600 text-white rounded-full w-6 h-6 flex items-center justify-center text-sm mr-3">6</span>
              الاختبار
            </CardTitle>
          </CardHeader>
          <CardContent>
            <p className="mb-4">شغّل الخادم واختبر البرنامج المساعد:</p>
            <div className="bg-gray-100 dark:bg-gray-800 p-4 rounded-lg">
              <pre><code>{`cd ../LabFramework/src/LabFramework.Console
dotnet run`}</code></pre>
            </div>
            <p className="mt-4">اختبر الأوامر:</p>
            <ul className="list-disc list-inside mt-2 space-y-1">
              <li><code>hello John</code> - يجب أن يرد بـ "Hello, John!"</li>
              <li><code>status</code> - لعرض حالة الإطار</li>
              <li><code>plugins</code> - لعرض البرامج المساعدة المحملة</li>
            </ul>
          </CardContent>
        </Card>

        <div className="bg-green-50 dark:bg-green-900/20 border border-green-200 dark:border-green-800 rounded-lg p-6">
          <h3 className="text-lg font-semibold text-green-800 dark:text-green-200 mb-2">
            🎉 تهانينا!
          </h3>
          <p className="text-green-700 dark:text-green-300">
            لقد أنشأت أول برنامج مساعد باستخدام LabFramework بنجاح. الآن يمكنك استكشاف المزيد من الميزات المتقدمة.
          </p>
        </div>

        <div className="flex gap-4">
          <Button asChild>
            <Link to="/examples">
              <Code className="w-4 h-4 mr-2" />
              عرض المزيد من الأمثلة
            </Link>
          </Button>
          <Button variant="outline" asChild>
            <Link to="/docs">
              <BookOpen className="w-4 h-4 mr-2" />
              قراءة الوثائق الكاملة
            </Link>
          </Button>
        </div>
      </div>
    </div>
  );
};

// Examples Page Component
const ExamplesPage = () => {
  return (
    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
      <div className="mb-8">
        <h1 className="text-4xl font-bold text-gray-900 dark:text-white mb-4">الأمثلة</h1>
        <p className="text-lg text-gray-600 dark:text-gray-300">
          أمثلة عملية لتطوير البرامج المساعدة باستخدام LabFramework
        </p>
      </div>

      <div className="grid md:grid-cols-2 gap-8">
        <Card>
          <CardHeader>
            <CardTitle>مثال أساسي</CardTitle>
            <CardDescription>
              برنامج مساعد بسيط يوضح المفاهيم الأساسية
            </CardDescription>
          </CardHeader>
          <CardContent>
            <div className="bg-gray-100 dark:bg-gray-800 p-4 rounded-lg overflow-x-auto">
              <pre className="text-sm"><code>{`public class BasicPlugin : BasePlugin
{
    public override string Name => "Basic Plugin";
    public override string Version => "1.0.0";
    public override string Author => "LabFramework";
    
    [Command("info", "Show server info")]
    public CommandResult InfoCommand(CommandContext context)
    {
        return CommandResult.Successful(
            $"Server: {Environment.MachineName}\\n" +
            $"Players: {GetPlayerCount()}/20"
        );
    }
}`}</code></pre>
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>إدارة اللاعبين</CardTitle>
            <CardDescription>
              أوامر متقدمة لإدارة اللاعبين والمراقبة
            </CardDescription>
          </CardHeader>
          <CardContent>
            <div className="bg-gray-100 dark:bg-gray-800 p-4 rounded-lg overflow-x-auto">
              <pre className="text-sm"><code>{`[Command("kick", "Kick a player", permission: "admin.kick")]
public async Task<CommandResult> KickCommand(
    CommandContext context,
    [CommandParameter("player")] string playerName,
    [CommandParameter("reason", isOptional: true)] string reason = "No reason")
{
    var player = await FindPlayerAsync(playerName);
    if (player == null)
        return CommandResult.Failed("Player not found");
    
    // player.Kick(reason);
    Logger.LogInformation($"{context.SenderName} kicked {player.Name}");
    
    return CommandResult.Successful($"Kicked {player.Name}");
}`}</code></pre>
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>العناصر المخصصة</CardTitle>
            <CardDescription>
              إنشاء عناصر مخصصة بسلوكيات تفاعلية
            </CardDescription>
          </CardHeader>
          <CardContent>
            <div className="bg-gray-100 dark:bg-gray-800 p-4 rounded-lg overflow-x-auto">
              <pre className="text-sm"><code>{`var superMedkit = new CustomItemDefinition
{
    Id = "super_medkit",
    Name = "Super Medkit",
    Description = "Advanced healing item",
    BaseItemType = "Medkit",
    IsStackable = true,
    MaxStackSize = 5
};

_customItemService.RegisterItem(superMedkit, 
    new SuperMedkitBehavior());

public class SuperMedkitBehavior : ICustomItemBehavior
{
    public async Task OnUseAsync(PlayerWrapper player, CustomItem item)
    {
        // player.Health = player.MaxHealth;
        item.CurrentDurability = 0;
    }
}`}</code></pre>
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>نظام الأحداث</CardTitle>
            <CardDescription>
              التعامل مع أحداث اللعبة والاستجابة لها
            </CardDescription>
          </CardHeader>
          <CardContent>
            <div className="bg-gray-100 dark:bg-gray-800 p-4 rounded-lg overflow-x-auto">
              <pre className="text-sm"><code>{`public override async Task OnLoadAsync()
{
    await base.OnLoadAsync();
    
    EventBus.Subscribe<PlayerJoinedEvent>(OnPlayerJoined);
    EventBus.Subscribe<PlayerLeftEvent>(OnPlayerLeft);
}

private async Task OnPlayerJoined(PlayerJoinedEvent eventArgs)
{
    Logger.LogInformation($"Player {eventArgs.PlayerName} joined");
    
    // Send welcome message
    var playerCount = await GetPlayerCountAsync();
    // eventArgs.Player.SendMessage($"Welcome! ({playerCount} online)");
}

private async Task OnPlayerLeft(PlayerLeftEvent eventArgs)
{
    var sessionTime = DateTime.UtcNow - eventArgs.JoinTime;
    Logger.LogInformation($"Player {eventArgs.PlayerName} left after {sessionTime}");
}`}</code></pre>
            </div>
          </CardContent>
        </Card>
      </div>

      <div className="mt-12">
        <Card>
          <CardHeader>
            <CardTitle>مثال شامل: برنامج إدارة الخادم</CardTitle>
            <CardDescription>
              برنامج مساعد متكامل يجمع عدة ميزات
            </CardDescription>
          </CardHeader>
          <CardContent>
            <div className="bg-gray-100 dark:bg-gray-800 p-4 rounded-lg overflow-x-auto">
              <pre className="text-sm"><code>{`public class ServerManagementPlugin : BasePlugin
{
    private readonly Dictionary<string, DateTime> _playerJoinTimes = new();
    private readonly Dictionary<string, int> _playerWarnings = new();
    
    public override string Name => "Server Management";
    public override string Version => "2.0.0";
    public override string Author => "LabFramework Team";
    public override string Description => "Comprehensive server management tools";

    public override async Task OnLoadAsync()
    {
        await base.OnLoadAsync();
        
        var commandService = ServiceContainer.Resolve<ICommandService>();
        commandService.RegisterCommands(this);
        
        EventBus.Subscribe<PlayerJoinedEvent>(OnPlayerJoined);
        EventBus.Subscribe<PlayerLeftEvent>(OnPlayerLeft);
        
        Logger.LogInformation("Server Management Plugin loaded");
    }

    [Command("warn", "Warn a player", permission: "moderator.warn")]
    public async Task<CommandResult> WarnCommand(CommandContext context,
        [CommandParameter("player", "Player to warn")] string playerName,
        [CommandParameter("reason", "Warning reason")] string reason)
    {
        var player = await FindPlayerAsync(playerName);
        if (player == null)
            return CommandResult.Failed($"Player '{playerName}' not found");

        var playerId = player.Id;
        _playerWarnings[playerId] = _playerWarnings.GetValueOrDefault(playerId, 0) + 1;
        var warningCount = _playerWarnings[playerId];

        Logger.LogInformation($"{context.SenderName} warned {player.Name} ({warningCount} warnings) for: {reason}");
        
        // Auto-kick after 3 warnings
        if (warningCount >= 3)
        {
            // player.Kick("Too many warnings");
            return CommandResult.Successful($"Warned {player.Name} for: {reason}. Player auto-kicked for excessive warnings.");
        }

        return CommandResult.Successful($"Warned {player.Name} for: {reason} (Warning {warningCount}/3)");
    }

    [Command("playtime", "Show player's session time")]
    public async Task<CommandResult> PlaytimeCommand(CommandContext context,
        [CommandParameter("player", "Player name", isOptional: true)] string playerName = null)
    {
        if (string.IsNullOrEmpty(playerName) && !context.IsConsole)
            playerName = context.SenderName;

        if (string.IsNullOrEmpty(playerName))
            return CommandResult.Failed("Player name required when executed from console");

        var player = await FindPlayerAsync(playerName);
        if (player == null)
            return CommandResult.Failed($"Player '{playerName}' not found");

        if (_playerJoinTimes.TryGetValue(player.Id, out var joinTime))
        {
            var sessionTime = DateTime.UtcNow - joinTime;
            return CommandResult.Successful($"{player.Name} has been playing for {sessionTime:hh\\:mm\\:ss}");
        }

        return CommandResult.Failed($"No session data found for {player.Name}");
    }

    private async Task OnPlayerJoined(PlayerJoinedEvent eventArgs)
    {
        _playerJoinTimes[eventArgs.PlayerId] = DateTime.UtcNow;
        Logger.LogInformation($"Player {eventArgs.PlayerName} joined the server");
        
        var playerCount = await GetPlayerCountAsync();
        // eventArgs.Player.SendMessage($"Welcome to the server! ({playerCount} players online)");
    }

    private async Task OnPlayerLeft(PlayerLeftEvent eventArgs)
    {
        if (_playerJoinTimes.TryGetValue(eventArgs.PlayerId, out var joinTime))
        {
            var sessionTime = DateTime.UtcNow - joinTime;
            Logger.LogInformation($"Player {eventArgs.PlayerName} left after {sessionTime:hh\\:mm\\:ss}");
            _playerJoinTimes.Remove(eventArgs.PlayerId);
        }
    }
}`}</code></pre>
            </div>
          </CardContent>
        </Card>
      </div>

      <div className="mt-8 flex gap-4">
        <Button asChild>
          <Link to="/download">
            <Download className="w-4 h-4 mr-2" />
            تحميل الأمثلة الكاملة
          </Link>
        </Button>
        <Button variant="outline" asChild>
          <Link to="/docs">
            <BookOpen className="w-4 h-4 mr-2" />
            قراءة الوثائق
          </Link>
        </Button>
      </div>
    </div>
  );
};

// Download Page Component
const DownloadPage = () => {
  return (
    <div className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
      <div className="text-center mb-12">
        <h1 className="text-4xl font-bold text-gray-900 dark:text-white mb-4">تحميل LabFramework</h1>
        <p className="text-lg text-gray-600 dark:text-gray-300">
          احصل على أحدث إصدار من LabFramework وابدأ التطوير اليوم
        </p>
      </div>

      <div className="grid md:grid-cols-2 gap-8 mb-12">
        <Card className="border-2 border-blue-200 dark:border-blue-800">
          <CardHeader>
            <div className="flex items-center justify-between">
              <CardTitle className="text-xl">الإصدار الكامل</CardTitle>
              <Badge className="bg-blue-100 text-blue-800">موصى به</Badge>
            </div>
            <CardDescription>
              الحزمة الكاملة مع جميع المكونات والوثائق والأمثلة
            </CardDescription>
          </CardHeader>
          <CardContent>
            <div className="space-y-4">
              <div className="text-3xl font-bold text-blue-600">مجاني</div>
              <ul className="space-y-2 text-sm">
                <li className="flex items-center">
                  <Star className="w-4 h-4 text-green-500 mr-2" />
                  جميع المكونات الأساسية
                </li>
                <li className="flex items-center">
                  <Star className="w-4 h-4 text-green-500 mr-2" />
                  وثائق شاملة وأمثلة
                </li>
                <li className="flex items-center">
                  <Star className="w-4 h-4 text-green-500 mr-2" />
                  سكريبت تثبيت تلقائي
                </li>
                <li className="flex items-center">
                  <Star className="w-4 h-4 text-green-500 mr-2" />
                  دعم مجتمعي
                </li>
              </ul>
              <Button className="w-full" size="lg">
                <Download className="w-4 h-4 mr-2" />
                تحميل الإصدار الكامل
              </Button>
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle className="text-xl">الكود المصدري</CardTitle>
            <CardDescription>
              للمطورين الذين يريدون البناء من المصدر أو المساهمة
            </CardDescription>
          </CardHeader>
          <CardContent>
            <div className="space-y-4">
              <div className="text-3xl font-bold text-gray-600">مفتوح المصدر</div>
              <ul className="space-y-2 text-sm">
                <li className="flex items-center">
                  <Github className="w-4 h-4 text-gray-500 mr-2" />
                  كود مصدري كامل
                </li>
                <li className="flex items-center">
                  <Github className="w-4 h-4 text-gray-500 mr-2" />
                  إمكانية التخصيص
                </li>
                <li className="flex items-center">
                  <Github className="w-4 h-4 text-gray-500 mr-2" />
                  المساهمة في التطوير
                </li>
                <li className="flex items-center">
                  <Github className="w-4 h-4 text-gray-500 mr-2" />
                  تتبع المشاكل والطلبات
                </li>
              </ul>
              <Button variant="outline" className="w-full" size="lg" asChild>
                <a href="https://github.com/labframework/labframework" target="_blank" rel="noopener noreferrer">
                  <Github className="w-4 h-4 mr-2" />
                  عرض على GitHub
                </a>
              </Button>
            </div>
          </CardContent>
        </Card>
      </div>

      <Card className="mb-8">
        <CardHeader>
          <CardTitle>متطلبات النظام</CardTitle>
        </CardHeader>
        <CardContent>
          <div className="grid md:grid-cols-3 gap-6">
            <div>
              <h4 className="font-semibold mb-2">نظام التشغيل</h4>
              <ul className="text-sm space-y-1">
                <li>• Windows 10/11</li>
                <li>• Linux (Ubuntu 20.04+)</li>
                <li>• macOS 10.15+</li>
              </ul>
            </div>
            <div>
              <h4 className="font-semibold mb-2">البرمجيات المطلوبة</h4>
              <ul className="text-sm space-y-1">
                <li>• .NET 8.0 SDK</li>
                <li>• SCP:SL Server</li>
                <li>• LabAPI</li>
              </ul>
            </div>
            <div>
              <h4 className="font-semibold mb-2">الأجهزة الموصى بها</h4>
              <ul className="text-sm space-y-1">
                <li>• 4GB RAM أو أكثر</li>
                <li>• 1GB مساحة تخزين</li>
                <li>• اتصال إنترنت</li>
              </ul>
            </div>
          </div>
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle>سجل الإصدارات</CardTitle>
        </CardHeader>
        <CardContent>
          <div className="space-y-4">
            <div className="border-l-4 border-blue-500 pl-4">
              <div className="flex items-center justify-between mb-1">
                <h4 className="font-semibold">v1.0.0</h4>
                <Badge>أحدث إصدار</Badge>
              </div>
              <p className="text-sm text-gray-600 dark:text-gray-400 mb-2">2024-12-10</p>
              <ul className="text-sm space-y-1">
                <li>• الإصدار الأول من LabFramework</li>
                <li>• نظام البرامج المساعدة الكامل</li>
                <li>• تكامل مع LabAPI</li>
                <li>• نظام الأوامر والصلاحيات</li>
                <li>• العناصر المخصصة</li>
                <li>• وثائق شاملة وأمثلة</li>
              </ul>
            </div>
          </div>
        </CardContent>
      </Card>

      <div className="mt-8 text-center">
        <p className="text-gray-600 dark:text-gray-400 mb-4">
          هل تحتاج مساعدة في التثبيت؟
        </p>
        <div className="flex gap-4 justify-center">
          <Button variant="outline" asChild>
            <Link to="/quickstart">
              <Play className="w-4 h-4 mr-2" />
              دليل البدء السريع
            </Link>
          </Button>
          <Button variant="outline" asChild>
            <Link to="/docs">
              <BookOpen className="w-4 h-4 mr-2" />
              الوثائق الكاملة
            </Link>
          </Button>
        </div>
      </div>
    </div>
  );
};

// Main App Component
function App() {
  const [isMenuOpen, setIsMenuOpen] = useState(false);

  return (
    <Router>
      <div className="min-h-screen bg-white dark:bg-gray-900">
        <Navigation isOpen={isMenuOpen} setIsOpen={setIsMenuOpen} />
        
        <main>
          <Routes>
            <Route path="/" element={<HomePage />} />
            <Route path="/docs" element={<DocsPage />} />
            <Route path="/quickstart" element={<QuickStartPage />} />
            <Route path="/examples" element={<ExamplesPage />} />
            <Route path="/download" element={<DownloadPage />} />
          </Routes>
        </main>

        <footer className="bg-gray-50 dark:bg-gray-800 border-t border-gray-200 dark:border-gray-700">
          <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
            <div className="grid md:grid-cols-4 gap-8">
              <div>
                <div className="flex items-center space-x-2 mb-4">
                  <div className="w-8 h-8 bg-blue-600 rounded-lg flex items-center justify-center">
                    <Zap className="w-5 h-5 text-white" />
                  </div>
                  <span className="text-xl font-bold text-gray-900 dark:text-white">LabFramework</span>
                </div>
                <p className="text-gray-600 dark:text-gray-400 text-sm">
                  إطار عمل متقدم لتطوير البرامج المساعدة لخوادم SCP: Secret Laboratory
                </p>
              </div>
              
              <div>
                <h3 className="font-semibold text-gray-900 dark:text-white mb-4">الوثائق</h3>
                <ul className="space-y-2 text-sm">
                  <li><Link to="/quickstart" className="text-gray-600 dark:text-gray-400 hover:text-blue-600">البدء السريع</Link></li>
                  <li><Link to="/docs" className="text-gray-600 dark:text-gray-400 hover:text-blue-600">دليل المطور</Link></li>
                  <li><Link to="/examples" className="text-gray-600 dark:text-gray-400 hover:text-blue-600">الأمثلة</Link></li>
                </ul>
              </div>
              
              <div>
                <h3 className="font-semibold text-gray-900 dark:text-white mb-4">المجتمع</h3>
                <ul className="space-y-2 text-sm">
                  <li><a href="#" className="text-gray-600 dark:text-gray-400 hover:text-blue-600">Discord</a></li>
                  <li><a href="#" className="text-gray-600 dark:text-gray-400 hover:text-blue-600">المنتدى</a></li>
                  <li><a href="#" className="text-gray-600 dark:text-gray-400 hover:text-blue-600">الدعم</a></li>
                </ul>
              </div>
              
              <div>
                <h3 className="font-semibold text-gray-900 dark:text-white mb-4">المشروع</h3>
                <ul className="space-y-2 text-sm">
                  <li><a href="https://github.com/labframework/labframework" className="text-gray-600 dark:text-gray-400 hover:text-blue-600">GitHub</a></li>
                  <li><Link to="/download" className="text-gray-600 dark:text-gray-400 hover:text-blue-600">التحميل</Link></li>
                  <li><a href="#" className="text-gray-600 dark:text-gray-400 hover:text-blue-600">الترخيص</a></li>
                </ul>
              </div>
            </div>
            
            <div className="border-t border-gray-200 dark:border-gray-700 mt-8 pt-8 text-center">
              <p className="text-gray-600 dark:text-gray-400 text-sm">
                © 2024 LabFramework Team. جميع الحقوق محفوظة.
              </p>
            </div>
          </div>
        </footer>
      </div>
    </Router>
  );
}

export default App;

