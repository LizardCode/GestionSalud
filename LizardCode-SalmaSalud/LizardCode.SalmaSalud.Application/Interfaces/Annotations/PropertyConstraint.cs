using System;
using System.Collections.Generic;

namespace LizardCode.SalmaSalud.Application.Interfaces.Annotations
{
    public abstract class PropertyConstraint : Attribute
    {
        public abstract Dictionary<string, string> HtmlAttributes { get; }
    }
}
