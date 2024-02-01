using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Syntaq.Falcon.Forms
{
	[Table("SfaFormTypes")]
	public class FormType
	{
		public int Id { get; set; }

		[StringLength(FormConsts.MaxNameLength, MinimumLength = FormConsts.MinNameLength)]
		public virtual string Name { get; set; }

		[StringLength(FormConsts.MaxDescriptionLength, MinimumLength = FormConsts.MinDescriptionLength)]
		public virtual string Description { get; set; }
	}
}
