import subprocess

cs_source = r"""using System;
using System.IO;
using System.Reflection;

class Program {
    static void Main(string[] args) {
        foreach (var dll in args) {
            Console.WriteLine("========================================");
            Console.WriteLine("DLL: " + dll);
            Console.WriteLine("========================================");
            try {
                byte[] b = File.ReadAllBytes(dll);
                Assembly a = Assembly.Load(b);
                Console.WriteLine("Assembly FullName: " + a.FullName);
                Console.WriteLine("--- Referenced Assemblies ---");
                foreach (var r in a.GetReferencedAssemblies()) {
                    Console.WriteLine("  Ref: " + r.FullName);
                }
                
                Type[] types = null;
                try {
                    types = a.GetTypes();
                } catch (ReflectionTypeLoadException rtlex) {
                    Console.WriteLine("--- Loader Exceptions ---");
                    foreach (var lex in rtlex.LoaderExceptions) {
                        Console.WriteLine("  LoaderEx: " + (lex != null ? lex.Message : "null"));
                    }
                    types = rtlex.Types;
                }
                
                Console.WriteLine("--- Loaded Types ---");
                if (types != null) {
                    foreach (Type t in types) {
                        if (t != null) {
                            Console.WriteLine("Type: " + t.FullName);
                        }
                    }
                }
            } catch (Exception ex) {
                Console.WriteLine("Error: " + ex.ToString());
            }
        }
    }
}
"""

with open(r"D:\Git\WorksErgoRPro\tools\Inspector.cs", "w", encoding="utf-8") as f:
    f.write(cs_source)

csc = r"C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe"
exe = r"D:\Git\WorksErgoRPro\tools\Inspector.exe"
subprocess.run([csc, f"/out:{exe}", r"D:\Git\WorksErgoRPro\tools\Inspector.cs"], check=True)

import os
user_dir = r"C:\Users\Jojo\Documents"
target_dir = None
for item in os.listdir(user_dir):
    if "work(s) ergo" in item.lower():
        target_dir = os.path.join(user_dir, item)
        break

dll1 = os.path.join(target_dir, "Plugin.WorksErgoAPI_.dll")
dll2 = os.path.join(target_dir, "Plugin.WorksErgoAPI.dll")

res = subprocess.run([exe, dll1, dll2], capture_output=True, text=True, encoding="utf-8", errors="ignore")
with open(r"D:\Git\WorksErgoRPro\tools\dll_inspection_output.txt", "w", encoding="utf-8") as f:
    f.write(res.stdout)
print("Updated inspection output:")
for line in res.stdout.splitlines()[:60]:
    print(line)
