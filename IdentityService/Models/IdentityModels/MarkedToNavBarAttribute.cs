using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharedServices.Models.IdentityModels
{

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]

    public class MarkedToNavBarAttribute : Attribute
    {
        public readonly string name;

        public MarkedToNavBarAttribute(string name)
        {
            this.name = name;
        }
    }
}
