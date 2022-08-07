# dnlib.BAML
(XAML/BAML) Editor with dnlib in .NET

# Tutorial
To understand correctly, first pay attention to the photo below :

![g.Resources][image_baml]

In the Resources, there are resource called `*.g.resources`, where there are `*.baml` files. 

From now on, we will call this resource by the name `g.resources`, and we will call the files inside it as its `elements`.

> Now first install the package from [Nuget](https://www.nuget.org/packages/dnlib.BAML/1.0.0).

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
                    PropertyWithConverterRecord pwcr = recordPropertyWithConverterRecord;
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
![Editor](https://s25.picofile.com/file/8452031242/baml_editor.gif)




[image_baml]: data:image/png;base64,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
