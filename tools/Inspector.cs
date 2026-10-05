using System;
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
