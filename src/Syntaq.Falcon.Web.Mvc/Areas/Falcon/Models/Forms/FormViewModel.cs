using System;
using Syntaq.Falcon.Forms.Dtos;

namespace Syntaq.Falcon.Web.Areas.Falcon.Models.Forms
{
    public class FormViewModel : GetFormForView
    {
        public Guid? RecordMatterId { get; set; }
    }
}