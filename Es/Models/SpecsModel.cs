using System;
using System.Collections.Generic;
using System.Text;
using Nest;

namespace Es.Models
{
   
    public class SpecsModel
    {
        [PropertyName("spec"), Nested]
        public SpecModel[] Spec { get; set; }
    }

    public class SpecModel
    {
        [Number(Name = "id", Index = true)]
        public int Id { get; set; }
    }
}
