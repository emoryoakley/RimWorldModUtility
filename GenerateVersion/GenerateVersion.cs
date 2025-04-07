using System;
using System.IO;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;

public class GenerateVersion : Task
{
    // 輸出的檔案路徑
    [Required]
    public string OutputFile { get; set; }

    // 單獨輸入：主版本號
    public string MajorVersion { get; set; }

    // 單獨輸入：次版本號
    public string MinorVersion { get; set; }

    // 合併輸入：完整版本號，如 "1.5"
    public string Version { get; set; }

    // 產品版本號
    public string ProductiVersion { get; set; }

    public override bool Execute()
    {
        try
        {
            // 儲存主版本號和次版本號
            string major;
            string minor;

            // 偵測輸入方式
            if (!string.IsNullOrEmpty(Version))
            {
                // 如果使用合併格式，解析版本號
                var versionParts = Version.Split('.');
                if (versionParts.Length != 2)
                {
                    Log.LogError("提供的版本號格式不正確！請使用 'Major.Minor' 格式，例如 '1.5'。");
                    return false;
                }

                major = versionParts[0];
                minor = versionParts[1];
            }
            else if (!string.IsNullOrEmpty(MajorVersion) && !string.IsNullOrEmpty(MinorVersion))
            {
                // 如果單獨提供，直接使用
                major = MajorVersion;
                minor = MinorVersion;
            }
            else
            {
                // 未提供有效的版本號
                Log.LogError("請提供版本號，您可以使用 'Version=\"1.5\"' 或 'MajorVersion=\"1\"' 和 'MinorVersion=\"5\"'。");
                return false;
            }

            string productVersion = "";
            if (!string.IsNullOrEmpty(ProductiVersion))
            {
                // 確保不會多出不必要的引號
                productVersion = $@"[assembly: AssemblyInformationalVersion(""{ProductiVersion.Replace("\"", "")}"")]";
            }

            // 獲取當前日期和時間
            DateTime now = DateTime.UtcNow;

            // 自動生成 build number 和 revision number
            var buildNumber = (now - new DateTime(2000, 1, 1)).Days + 1;
            var secondsToday = now.ToLocalTime().TimeOfDay.TotalSeconds;
            var revisionNumber = (int)(secondsToday / 2);

            Log.LogMessage($"buildNumber: {buildNumber}, revisionNumber: {revisionNumber}");

            // 建立版本資訊的內容
            string versionContent = $@"using System.Reflection;
// GenerateVersion.dll 自動生成 VersionInfo.cs
[assembly: AssemblyVersion(""{major}.{minor}.{buildNumber}.{revisionNumber}"")]
[assembly: AssemblyFileVersion(""{major}.{minor}.{buildNumber}.{revisionNumber}"")]
{productVersion}
";

            // 將內容寫入到指定的檔案
            File.WriteAllText(OutputFile, versionContent);
            return true;
        }
        catch (Exception ex)
        {
            Log.LogErrorFromException(ex, true);
            return false;
        }
    }
}
