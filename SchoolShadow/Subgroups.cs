//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SchoolShadow
{
    using System;
    using System.Collections.Generic;
    
    public partial class Subgroups
    {
        public int Subgroups_SubgroupID { get; set; }
        public int Subgroups_GroupID { get; set; }
        public string Subgroups_Name { get; set; }
        public int Subgroups_StudentsCount { get; set; }
    
        public virtual Groups Groups { get; set; }
    }
}
