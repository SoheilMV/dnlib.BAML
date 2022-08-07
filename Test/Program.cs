using System;
using dnlib.DotNet;
using dnlib.DotNet.Resources;
using dnlib.DotNet.Writer;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "dnlib.BAML";

            try
            {
                using (ModuleDefMD module = ModuleDefMD.Load("WpfApp1.exe"))
                {
                    if (module.HasGResources())
                    {
                        var gResources = module.gResources();
                        foreach (var element in gResources.Elements)
                        {
                            var em = gResources.Read(element.Name);
                            foreach (var record in em.Document)
                            {
                                if (record.Type == BamlRecordType.PropertyWithConverter)
                                {
                                    PropertyWithConverterRecord pwcr = record as PropertyWithConverterRecord;
                                    if (pwcr.Value.ToLower().Contains("window title"))
                                    {
                                        pwcr.Value = "dnlib.BAML";
                                    }
                                    else if (pwcr.Value.ToLower().Contains("change my text"))
                                    {
                                        pwcr.Value = "Coded By Soheil MV.";
                                    }
                                }
                            }
                            gResources.Write(em);
                        }
                        module.RemoveGResources();
                        module.Resources.Add(gResources.GetAsEmbeddedResource());
                    }

                    module.Write("WpfApp2.exe", new ModuleWriterOptions(module)
                    {
                        MetadataOptions = { Flags = MetadataFlags.PreserveAll },
                        PEHeadersOptions = { NumberOfRvaAndSizes = 0x10 },
                        Logger = DummyLogger.ThrowModuleWriterExceptionOnErrorInstance,
                        WritePdb = true
                    });

                    Success("Changed the Window Title and TextBlock Text!");
                }
            }
            catch (ModuleWriterException ex)
            {
                Error(ex.Message);
            }
            Console.ReadKey();
        }

        private static void Success(string text)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(text);
        }

        private static void Error(string text)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(text);
        }
    }
}
