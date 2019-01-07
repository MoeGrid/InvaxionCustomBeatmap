using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace InvaxionCustomSpectrumInstall
{
    public partial class InstallForm : Form
    {
        private static readonly string CurrentGamePath = "F:\\Program Files (x86)\\Steam\\steamapps\\common\\音灵 INVAXION";
        private static readonly string CurrentManagedPath = $"{CurrentGamePath}\\INVAXION_Data\\Managed";
        private static readonly string AssemblyName = "Assembly-CSharp.dll";
        private static readonly string PatchClass = "Aquatrax.GameController";
        private static readonly string PatchMethod = "Awake";

        private static readonly string AssemblyFile = Path.Combine(CurrentManagedPath, AssemblyName);
        private static readonly string BackupAssemblyName = $"{AssemblyFile}.backup_";
        private static readonly string TmpAssemblyName = $"{AssemblyFile}.tmp_";
        private static readonly string AssemblyKey = "Aquatrax_wearshoes";
        
        public InstallForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Inject();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (File.Exists(BackupAssemblyName))
            {
                File.Delete(AssemblyFile);
                File.Move(BackupAssemblyName, AssemblyFile);
                Log($"卸载完成！");
            }
            else
            {
                Log($"找不到备份文件，无法完成卸载！");
            }
        }

        /******************************/

        // 安装
        private void Inject()
        {
            try
            {
                // 验证文件
                if (!File.Exists(AssemblyFile))
                {
                    Log($"找不到 {AssemblyFile} 文件！");
                    return;
                }

                // 解密DLL
                var bytes = File.ReadAllBytes(AssemblyFile);
                bytes = XXTEA.Decrypt(bytes, AssemblyKey);

                // 加载DLL
                var assembly = ModuleDefMD.Load(bytes);

                // 获得要注入的类
                if (IsInstalled(assembly))
                {
                    Log("您已经安装过，请卸载后再次安装！");
                    return;
                }
                
                // 获得被注入的类
                var targetClass = assembly.Types.FirstOrDefault(x => x.FullName == PatchClass);
                if (targetClass == null)
                {
                    Log($"找不到 '{PatchClass}' 类！");
                    return;
                }

                // 获得被注入方法
                var targetMethod = targetClass.Methods.FirstOrDefault(x => x.Name == PatchMethod);
                if (targetMethod == null)
                {
                    Log($"找不到 '{PatchMethod}' 方法！");
                    return;
                }

                // 备份游戏DLL
                Log($"备份文件 {AssemblyName} ...");
                File.Copy(AssemblyFile, BackupAssemblyName, true);

                

                Log("安装补丁中...");

                // 复制用到的类库
                var libs = new string[] { "0Harmony12.dll", "InvaxionCustomSpectrumPlugin.dll" };
                foreach (var i in libs)
                {
                    var path = Path.Combine(CurrentManagedPath, i);
                    File.Copy(i, path, true);
                }

                // 添加MOD加载类
                var modLoaderType = typeof(ModLoader);
                var modLoaderDef = ModuleDefMD.Load(modLoaderType.Module);
                var modLoader = modLoaderDef.Types.First(x => x.Name == modLoaderType.Name);
                modLoaderDef.Types.Remove(modLoader);
                assembly.Types.Add(modLoader);

                // 织入IL代码
                var instr = OpCodes.Call.ToInstruction(modLoader.Methods.First(x => x.Name == nameof(ModLoader.Load)));
                targetMethod.Body.Instructions.Insert(targetMethod.Body.Instructions.Count - 1, instr);

                // 保存
                assembly.Write(TmpAssemblyName);

                // 加密
                bytes = File.ReadAllBytes(TmpAssemblyName);
                bytes = XXTEA.Encrypt(bytes, AssemblyKey);
                File.WriteAllBytes(AssemblyFile, bytes);
                //File.Delete(TmpAssemblyName);

                Log("安装成功!");

            }
            catch (Exception e)
            {
                Log("安装发生错误：" + e.Message);
            }
        }

        // 是否已经安装
        private bool IsInstalled(ModuleDefMD assembly)
        {
            return assembly.Types.FirstOrDefault(x => x.Name == typeof(ModLoader).Name) != null;
        }

        // 输出日志
        private void Log(string msg)
        {
            listBox1.Items.Add(msg);
            listBox1.TopIndex = listBox1.Items.Count - 1;
        }

        private void InstallForm_Load(object sender, EventArgs e)
        {

        }
    }
}
