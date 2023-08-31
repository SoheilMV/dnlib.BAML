# dnlib.BAML [![Nuget](https://img.shields.io/nuget/v/dnlib.BAML)](https://www.nuget.org/packages/dnlib.BAML/)
BAML Editor with dnlib in .NET

# Tutorial
To understand correctly, first pay attention to the photo below :

![g.Resources](Images/bamls.png)

In the Resources, there are resource called `*.g.resources`, where there are `*.baml` files. 

From now on, we will call this resource by the name `g.resources`, and we will call the files inside it as its `elements`.

> Now first install the package from [Nuget](https://www.nuget.org/packages/dnlib.BAML).

Then you can use the library, below I changed the title of the Window and the text of a TextBlock:
```csharp
using dnlib.DotNet;
using dnlib.DotNet.Resources;

using (ModuleDefMD module = ModuleDefMD.Load("WpfApp1.exe"))
{
    if (module.HasGResources()) //Check if exist g.resources
    {
        var gResources = module.gResources(); //Get the g.resources
        foreach (var element in gResources.Elements) //Get the Elements
        {
            var em = gResources.Read(element.Name); //Read the element
            foreach (var record in em.Document) //Edit...
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
            gResources.Write(em); //Write the element
        }
        module.RemoveGResources(); //Delete the g.resources in the module
        module.Resources.Add(gResources.GetAsEmbeddedResource()); //Add the edited g.resources to the module
    }
}
```
![Editor](Images/baml_editor.gif)

# List of other open source libraries used
- [dnlib](https://github.com/0xd4d/dnlib) (Reads and writes .NET assemblies and modules)