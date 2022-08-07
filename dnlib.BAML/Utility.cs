using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using dnlib.IO;

namespace dnlib.DotNet.Resources
{
    public static class Utility
    {
        private static readonly Regex ResourceNamePattern = new Regex("^.*\\.g\\.resources$");

        public static IList<T> RemoveWhere<T>(this IList<T> self, Predicate<T> match)
        {
            for (int i = self.Count - 1; i >= 0; i--)
            {
                bool flag = match(self[i]);
                if (flag)
                {
                    self.RemoveAt(i);
                }
            }
            return self;
        }

        public static gResources gResources(this ModuleDefMD module)
        {
            gResources g = null;
            foreach (Resource resource in module.Resources)
            {
                EmbeddedResource er = resource as EmbeddedResource;
                if (ResourceNamePattern.IsMatch(er.Name))
                {
                    DataReader cr = er.CreateReader();
                    g = new gResources(er.Name, er.Attributes, cr.AsStream());
                    break;
                }
            }
            return g;
        }

        public static gResources gResources(this ModuleDef module)
        {
            gResources g = null;
            foreach (Resource resource in module.Resources)
            {
                EmbeddedResource er = resource as EmbeddedResource;
                if (ResourceNamePattern.IsMatch(er.Name))
                {
                    DataReader cr = er.CreateReader();
                    g = new gResources(er.Name, er.Attributes, cr.AsStream());
                    break;
                }
            }
            return g;
        }

        public static void RemoveGResources(this ModuleDefMD module)
        {
            foreach (Resource resource in module.Resources)
            {
                EmbeddedResource er = resource as EmbeddedResource;
                if (ResourceNamePattern.IsMatch(er.Name))
                {
                    module.Resources.Remove(resource);
                    break;
                }
            }
        }

        public static void RemoveGResources(this ModuleDef module)
        {
            foreach (Resource resource in module.Resources)
            {
                EmbeddedResource er = resource as EmbeddedResource;
                if (ResourceNamePattern.IsMatch(er.Name))
                {
                    module.Resources.Remove(resource);
                    break;
                }
            }
        }

        public static bool HasGResources(this ModuleDefMD module)
        {
            bool result = false;
            foreach (Resource resource in module.Resources)
            {
                EmbeddedResource er = resource as EmbeddedResource;
                if (ResourceNamePattern.IsMatch(er.Name))
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

        public static bool HasGResources(this ModuleDef module)
        {
            bool result = false;
            foreach (Resource resource in module.Resources)
            {
                EmbeddedResource er = resource as EmbeddedResource;
                if (ResourceNamePattern.IsMatch(er.Name))
                {
                    result = true;
                    break;
                }
            }
            return result;
        }
    }
}
