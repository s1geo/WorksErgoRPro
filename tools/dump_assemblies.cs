using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Linq;

namespace WorksErgo.Tools
{
    class AssemblyDumper
    {
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, resolveArgs) =>
            {
                string name = new AssemblyName(resolveArgs.Name).Name + ".dll";
                string probe = Path.Combine(@"D:\Apps\RProv222", name);
                if (File.Exists(probe)) return Assembly.LoadFrom(probe);
                return null;
            };

            string outDir = @"D:\Git\WorksErgoRPro\knowledge_base\04_rpro_cad_internals";
            Directory.CreateDirectory(outDir);

            DumpAssembly(@"D:\Apps\RProv222\Plugin.Ergonomics.dll", Path.Combine(outDir, "Plugin_Ergonomics_API_Dump.md"));
            DumpAssembly(@"D:\Apps\RProv222\Plugin.ErgonomicsWPP.dll", Path.Combine(outDir, "Plugin_ErgonomicsWPP_API_Dump.md"));
            Console.WriteLine("[COMPLETE] Assemblies dumped successfully.");
        }

        static void DumpAssembly(string path, string outFile)
        {
            if (!File.Exists(path))
            {
                Console.WriteLine("File not found: " + path);
                return;
            }

            var asm = Assembly.LoadFrom(path);
            var sb = new StringBuilder();
            sb.AppendLine("# Assembly Analysis: " + asm.GetName().Name);
            sb.AppendLine("- **Full Name:** " + asm.FullName);
            sb.AppendLine("- **Location:** " + asm.Location);
            sb.AppendLine("- **Target Framework:** " + asm.ImageRuntimeVersion);
            sb.AppendLine();
            sb.AppendLine("## Types and Signatures");
            sb.AppendLine();

            Type[] types;
            try { types = asm.GetTypes(); }
            catch (ReflectionTypeLoadException ex) { types = ex.Types.Where(t => t != null).ToArray(); }

            foreach (var t in types.OrderBy(t => t.FullName))
            {
                if (t.Name.StartsWith("<>")) continue; // skip compiler closures

                sb.AppendLine("### " + t.FullName);
                sb.AppendLine("- **Base Type:** " + (t.BaseType != null ? t.BaseType.FullName : "none"));
                
                var ifaces = t.GetInterfaces().Select(i => i.Name).ToArray();
                if (ifaces.Length > 0)
                    sb.AppendLine("- **Interfaces:** " + string.Join(", ", ifaces));

                var attrs = t.GetCustomAttributesData().Select(a => a.AttributeType.Name).ToArray();
                if (attrs.Length > 0)
                    sb.AppendLine("- **Attributes:** " + string.Join(", ", attrs));

                var props = t.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);
                if (props.Length > 0)
                {
                    sb.AppendLine();
                    sb.AppendLine("**Properties:**");
                    foreach (var p in props)
                    {
                        try
                        {
                            sb.AppendLine(string.Format("  * `{0} {1} {{ get; set; }}`", p.PropertyType.Name, p.Name));
                        }
                        catch (Exception ex)
                        {
                            sb.AppendLine(string.Format("  * `/* unresolved: {0} */ {1} {{ get; set; }}`", ex.GetType().Name, p.Name));
                        }
                    }
                }

                MethodInfo[] methods = new MethodInfo[0];
                try {
                    methods = t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly)
                               .Where(m => !m.IsSpecialName).ToArray();
                } catch { }

                if (methods.Length > 0)
                {
                    sb.AppendLine();
                    sb.AppendLine("**Methods:**");
                    foreach (var m in methods)
                    {
                        try
                        {
                            var pars = m.GetParameters().Select(p => {
                                try { return p.ParameterType.Name + " " + p.Name; }
                                catch { return "var " + p.Name; }
                            }).ToArray();
                            sb.AppendLine(string.Format("  * `{0} {1}({2})`", m.ReturnType.Name, m.Name, string.Join(", ", pars)));
                        }
                        catch (Exception ex)
                        {
                            sb.AppendLine(string.Format("  * `/* unresolved method: {0} */ {1}()`", ex.GetType().Name, m.Name));
                        }
                    }
                }

                sb.AppendLine();
                sb.AppendLine("---");
                sb.AppendLine();
            }

            File.WriteAllText(outFile, sb.ToString(), Encoding.UTF8);
            Console.WriteLine("Successfully wrote: " + outFile);
        }
    }
}
