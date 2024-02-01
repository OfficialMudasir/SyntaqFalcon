namespace Syntaq.Falcon.Forms.Dtos
{
    public class GetFormFeedbackForViewDto
    {
		public FormFeedbackDto FormFeedback { get; set; }

		public string FormName { get; set;}

        public string UserName { get; set; }

        public string Email { get; set; }

        public int Rating { get; set; }

        public string Comment { get; set; }

        public string FeedbackFormSchema { get; set; }
        public string FeedbackFormData { get; set; }
    }
}