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
    
    public partial class Curriculum
    {
        public int Curriculum_CurriculumID { get; set; }
        public int Curriculum_GroupID { get; set; }
        public int Curriculum_SubjectID { get; set; }
        public int Curriculum_Hours { get; set; }
        public int Curriculum_Load { get; set; }
        public Nullable<int> Curriculum_RemainingHours { get; set; }
    
        public virtual Groups Groups { get; set; }
        public virtual Subjects Subjects { get; set; }
    }
}
