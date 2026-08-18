using UnityEngine;
using UnityModManagerNet;
using System.Diagnostics;
using System.Reflection;
using System.Linq;

namespace Death99TauntMod
{
    public class Main
    {
        public static UnityModManager.ModEntry.ModLogger Logger;
        public static Settings settings;
        private static object harmonyInstance;

        // Unity Mod Manager 入口点
        public static bool Load(UnityModManager.ModEntry modEntry)
        {
            Logger = modEntry.Logger;
            settings = UnityModManager.ModSettings.Load<Settings>(modEntry);

            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
            modEntry.OnToggle = OnToggle;

            Logger.Log("死亡95%嘲讽Mod 已加载！");

            return true;
        }

        // 启用/禁用mod
        static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            try
            {
                if (value)
                {
                    ApplyPatches(modEntry.Info.Id);
                    Logger.Log("Mod已启用");
                }
                else
                {
                    Logger.Log("Mod已禁用");
                }
                return true;
            }
            catch (System.Exception ex)
            {
                Logger.Error($"OnToggle出错: {ex.Message}\n{ex.StackTrace}");
                return false;
            }
        }

        // 使用反射应用Harmony补丁
        static void ApplyPatches(string modId)
        {
            try
            {
                // 尝试加载HarmonyLib (新版本)
                var harmonyType = System.Type.GetType("HarmonyLib.Harmony, 0Harmony");
                if (harmonyType == null)
                {
                    // 尝试旧版本
                    harmonyType = System.Type.GetType("Harmony.HarmonyInstance, 0Harmony");
                }

                if (harmonyType == null)
                {
                    Logger.Error("找不到Harmony类型！");
                    return;
                }

                // 创建Harmony实例
                var constructor = harmonyType.GetConstructor(new[] { typeof(string) });
                if (constructor != null)
                {
                    harmonyInstance = constructor.Invoke(new object[] { modId });
                }
                else
                {
                    var createMethod = harmonyType.GetMethod("Create", new[] { typeof(string) });
                    if (createMethod != null)
                    {
                        harmonyInstance = createMethod.Invoke(null, new object[] { modId });
                    }
                }

                if (harmonyInstance == null)
                {
                    Logger.Error("无法创建Harmony实例！");
                    return;
                }

                // 手动patch scrController.Fail方法
                PatchFailMethod();

                Logger.Log("Harmony补丁应用成功");
            }
            catch (System.Exception ex)
            {
                Logger.Error($"应用补丁时出错: {ex.Message}\n{ex.StackTrace}");
            }
        }

        static void PatchFailMethod()
        {
            try
            {
                // 查找scrController类
                var controllerType = FindType("scrController");
                if (controllerType == null)
                {
                    Logger.Error("找不到scrController类！");
                    return;
                }

                Logger.Log($"找到scrController类: {controllerType.FullName}");

                // 直接使用FailAction方法
                var failMethod = controllerType.GetMethod("FailAction", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (failMethod == null)
                {
                    Logger.Error("找不到FailAction方法！");
                    return;
                }

                Logger.Log($"找到FailAction方法");

                // 获取Prefix方法（在FailAction执行前调用）
                var prefixMethod = typeof(Main).GetMethod("FailPrefix", BindingFlags.Public | BindingFlags.Static);
                if (prefixMethod == null)
                {
                    Logger.Error("找不到FailPrefix方法！");
                    return;
                }

                // 从0Harmony.dll加载HarmonyLib命名空间
                var harmonyAssembly = System.AppDomain.CurrentDomain.GetAssemblies()
                    .FirstOrDefault(a => a.GetName().Name == "0Harmony");

                if (harmonyAssembly == null)
                {
                    Logger.Error("找不到0Harmony程序集！");
                    return;
                }

                // 获取HarmonyMethod类型
                var harmonyMethodType = harmonyAssembly.GetType("HarmonyLib.HarmonyMethod");
                if (harmonyMethodType == null)
                {
                    Logger.Error("找不到HarmonyLib.HarmonyMethod类型！");
                    return;
                }

                var harmonyMethodCtor = harmonyMethodType.GetConstructor(new[] { typeof(MethodInfo) });
                if (harmonyMethodCtor == null)
                {
                    Logger.Error("找不到HarmonyMethod构造函数！");
                    return;
                }

                var prefixWrapper = harmonyMethodCtor.Invoke(new object[] { prefixMethod });

                // 获取Patch方法
                var patchMethod = harmonyInstance.GetType().GetMethod("Patch",
                    BindingFlags.Public | BindingFlags.Instance);

                if (patchMethod == null)
                {
                    Logger.Error("找不到Patch方法！");
                    return;
                }

                // 应用patch - 使用Prefix而不是Postfix
                patchMethod.Invoke(harmonyInstance, new object[] { failMethod, prefixWrapper, null, null, null });

                Logger.Log($"成功patch FailAction方法");
            }
            catch (System.Exception ex)
            {
                Logger.Error($"Patch Fail方法时出错: {ex.Message}\n{ex.StackTrace}");
            }
        }

        // Fail方法的Prefix（在方法执行前调用）
        public static void FailPrefix(object __instance)
        {
            try
            {
                if (__instance == null)
                {
                    Logger.Error("__instance为null");
                    return;
                }

                var controllerType = __instance.GetType();

                // 尝试使用percentComplete属性
                var percentProperty = controllerType.GetProperty("percentComplete", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                if (percentProperty != null)
                {
                    var percentValue = percentProperty.GetValue(__instance, null);
                    float progress = System.Convert.ToSingle(percentValue) * 100f; // 转换为百分比
                    Logger.Log($"死亡时进度: {progress:F2}%");

                    if (progress >= settings.progressThreshold)
                    {
                        Logger.Log($"进度 {progress:F2}% >= {settings.progressThreshold:F1}%，触发嘲讽！");

                        // 获取判定窗口设置
                        string url = GetUrlByDifficulty();
                        Logger.Log($"使用URL: {url}");
                        OpenBrowser(url);
                    }
                    return;
                }

                // 如果属性不存在，尝试字段
                var percentField = controllerType.GetField("oldPercentComplete", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (percentField != null)
                {
                    var percentValue = percentField.GetValue(__instance);
                    float progress = System.Convert.ToSingle(percentValue);
                    Logger.Log($"死亡时进度: {progress:F2}%");

                    if (progress >= settings.progressThreshold)
                    {
                        Logger.Log($"进度 {progress:F2}% >= {settings.progressThreshold:F1}%，触发嘲讽！");

                        // 获取判定窗口设置
                        string url = GetUrlByDifficulty();
                        Logger.Log($"使用URL: {url}");
                        OpenBrowser(url);
                    }
                    return;
                }

                Logger.Error("找不到percentComplete属性或oldPercentComplete字段");
            }
            catch (System.Exception ex)
            {
                Logger.Error($"FailPrefix出错: {ex.Message}\n{ex.StackTrace}");
            }
        }

        // 根据判定窗口获取URL
        static string GetUrlByDifficulty()
        {
            try
            {
                if (!settings.useCustomUrlByDifficulty)
                {
                    Logger.Log("未启用判定窗口自定义URL，使用默认URL");
                    return settings.defaultUrl;
                }

                // 查找GCS类（游戏设置）
                var gcsType = FindType("GCS");
                if (gcsType == null)
                {
                    Logger.Error("找不到GCS类型，使用默认URL");
                    return settings.defaultUrl;
                }

                Logger.Log($"找到GCS类型: {gcsType.FullName}");

                // 列出所有包含difficulty的字段
                var allFields = gcsType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
                foreach (var field in allFields)
                {
                    if (field.Name.ToLower().Contains("difficult") || field.Name.ToLower().Contains("timing"))
                    {
                        Logger.Log($"找到可能的判定字段: {field.Name} ({field.FieldType.Name})");
                    }
                }

                // 尝试多个可能的字段名
                string[] possibleFieldNames = { "difficulty", "currentDifficulty", "timingDifficulty", "timing" };

                foreach (var fieldName in possibleFieldNames)
                {
                    var difficultyField = gcsType.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
                    if (difficultyField != null)
                    {
                        var difficultyValue = difficultyField.GetValue(null);
                        if (difficultyValue != null)
                        {
                            var difficultyEnum = difficultyValue.ToString();
                            Logger.Log($"通过{fieldName}获取到判定窗口: {difficultyEnum}");

                            // 根据判定窗口返回对应URL
                            switch (difficultyEnum)
                            {
                                case "Strict":
                                case "StrictTiming":
                                case "0":
                                    Logger.Log("使用严格判定URL");
                                    return settings.urlOnStrict;
                                case "Normal":
                                case "NormalTiming":
                                case "1":
                                    Logger.Log("使用标准判定URL");
                                    return settings.urlOnNormal;
                                case "Lenient":
                                case "LenientTiming":
                                case "2":
                                    Logger.Log("使用宽容判定URL");
                                    return settings.urlOnLenient;
                                default:
                                    Logger.Log($"未知判定窗口: {difficultyEnum}，使用默认URL");
                                    return settings.defaultUrl;
                            }
                        }
                    }
                }

                Logger.Error("找不到difficulty相关字段，使用默认URL");
                return settings.defaultUrl;
            }
            catch (System.Exception ex)
            {
                Logger.Error($"获取判定窗口出错: {ex.Message}\n{ex.StackTrace}");
                return settings.defaultUrl;
            }
        }

        // 查找类型
        static System.Type FindType(string typeName)
        {
            foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                var type = assembly.GetType(typeName);
                if (type != null)
                    return type;
            }
            return null;
        }

        // 获取当前进度
        static float GetCurrentProgress()
        {
            try
            {
                var controllerType = FindType("scrController");
                if (controllerType == null)
                {
                    Logger.Error("找不到scrController类型");
                    return 0f;
                }

                // 获取实例
                var instanceField = controllerType.GetField("_instance", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                if (instanceField == null)
                {
                    Logger.Error("找不到_instance字段");
                    return 0f;
                }

                var instance = instanceField.GetValue(null);
                if (instance == null)
                {
                    Logger.Error("instance为null");
                    return 0f;
                }

                // 直接使用oldPercentComplete字段
                var percentField = controllerType.GetField("oldPercentComplete", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (percentField != null)
                {
                    var percent = percentField.GetValue(instance);
                    float result = System.Convert.ToSingle(percent);
                    return result;
                }

                Logger.Error("找不到oldPercentComplete字段");
                return 0f;
            }
            catch (System.Exception ex)
            {
                Logger.Error($"获取进度出错: {ex.Message}");
                return 0f;
            }
        }

        private static bool hasListedFields = false;

        // GUI设置界面
        static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            GUILayout.BeginVertical();

            GUILayout.Label("触发进度阈值 (%):");
            GUILayout.BeginHorizontal();
            settings.progressThreshold = GUILayout.HorizontalSlider(settings.progressThreshold, 50f, 99.9f, GUILayout.Width(300));
            GUILayout.Label($"{settings.progressThreshold:F1}%", GUILayout.Width(50));
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            // 判定窗口自定义URL开关
            settings.useCustomUrlByDifficulty = GUILayout.Toggle(settings.useCustomUrlByDifficulty, "根据判定窗口（严格/标准/宽容）自定义嘲讽URL");

            GUILayout.Space(10);

            if (settings.useCustomUrlByDifficulty)
            {
                GUILayout.Label("=== 根据判定窗口自定义URL ===");

                GUILayout.Label("严格判定 (Strict Timing):");
                settings.urlOnStrict = GUILayout.TextField(settings.urlOnStrict, GUILayout.Width(500));

                GUILayout.Space(5);

                GUILayout.Label("标准判定 (Normal Timing):");
                settings.urlOnNormal = GUILayout.TextField(settings.urlOnNormal, GUILayout.Width(500));

                GUILayout.Space(5);

                GUILayout.Label("宽容判定 (Lenient Timing):");
                settings.urlOnLenient = GUILayout.TextField(settings.urlOnLenient, GUILayout.Width(500));
            }
            else
            {
                GUILayout.Label("默认嘲讽URL:");
                settings.defaultUrl = GUILayout.TextField(settings.defaultUrl, GUILayout.Width(500));
            }

            GUILayout.Space(10);

            if (GUILayout.Button("测试打开浏览器", GUILayout.Width(150)))
            {
                OpenBrowser(settings.defaultUrl);
            }

            GUILayout.Space(10);
            GUILayout.Label("提示：当你在谱面完成度>=设定值时死亡，将自动打开浏览器嘲讽你！", GUILayout.ExpandWidth(false));

            GUILayout.EndVertical();
        }

        // 保存设置
        static void OnSaveGUI(UnityModManager.ModEntry modEntry)
        {
            settings.Save(modEntry);
        }

        // 打开浏览器
        static void OpenBrowser(string url)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
                Logger.Log($"成功打开浏览器: {url}");
            }
            catch (System.Exception ex)
            {
                Logger.Error($"打开浏览器失败: {ex.Message}");
            }
        }
    }

    // 设置类
    public class Settings : UnityModManager.ModSettings
    {
        public float progressThreshold = 95f;

        // 根据判定窗口自定义URL
        public bool useCustomUrlByDifficulty = false;
        public string urlOnStrict = "https://www.bilibili.com/video/BV1zSM46AE7w/"; // 严格判定
        public string urlOnNormal = "https://www.bilibili.com/video/BV1uT4y1P7CX/"; // 标准判定 (Never Gonna Give You Up)
        public string urlOnLenient = "https://www.bilibili.com/video/BV1uN4y1d7Js/"; // 宽容判定
        public string defaultUrl = "https://www.bilibili.com/video/BV1uT4y1P7CX/"; // 默认URL

        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }
    }
}
