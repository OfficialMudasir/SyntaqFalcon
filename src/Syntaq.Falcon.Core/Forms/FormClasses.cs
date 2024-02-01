using System.Collections.Generic;

namespace Syntaq.Falcon.Forms.Models
{
    public class FormIOSchemaComponentClass
    {
        public string rows { get; set; }
        public string Action { get; set; }
        public string AddAnother { get; set; }
        public List<AttrsClass> Attrs { get; set; }
        public string Breadcrumb { get; set; }
        public bool? BreadcrumbClickable { get; set; }
        public bool? Collapsed { get; set; }
        public bool? Collapsible { get; set; }
        public List<FormIOSchemaComponentClass> Components { get; set; }
        public string Content { get; set; }
        public string Custom { get; set; }
        public string CustomClass { get; set; }
        public FormDataClass Data { get; set; }
        public string DataSrc { get; set; }
        public List<FormValuesClass> DataTemp { get; set; }
        public DatePickerClass DatePicker { get; set; }
        public dynamic DefaultValue { get; set; }
        public bool? Disabled { get; set; }
        public bool? DisableLimit { get; set; }
        public string Dividerstyle { get; set; }
        public string Dividertitle { get; set; }
        public bool? DoNotLoadFromRecord { get; set; }
        public bool? EnableDate { get; set; }
        public bool? EnableTime { get; set; }
        //[JsonProperty("event")]
        public string Event { get; set; }
        public bool? FieldasRecordName { get; set; }
        public List<FormSubstitutionClass> FNSubstitute { get; set; } //= new List<FormSubstitutionClass>();
        public string Format { get; set; }
        public string FormId { get; set; }
        public string Heading { get; set; }
        public bool? Hidden { get; set; }
        public bool? Hidecc { get; set; }
        public bool? Hidecontent { get; set; }
        public bool? Hidecountry { get; set; }
        public bool? Hidefullname { get; set; }
        public bool? HideLabel { get; set; }
        public bool? ApplyLabelFirstRow { get; set; }
        public bool? Hidelvl { get; set; }
        public bool? Hidepostcode { get; set; }
        public bool? Hidemidname { get; set; }
        public bool? Hidestate { get; set; }
        public bool? Hidestreet { get; set; }
        public bool? Hidesuburb { get; set; }
        public bool? Hidetitle { get; set; }
        public bool? Hidefulladdress { get; set; }
        public string Htmlcontent { get; set; }
        public string ImageSiz { get; set; }
        public bool? Input { get; set; }
        public string InputMask { get; set; }
        public string Key { get; set; }
        public string Label { get; set; }
        public string LabelPosition { get; set; }
        public string Labelvalue { get; set; }
        public bool? LazyLoad { get; set; }
        public string LeftIcon { get; set; }
        public List<FormlogicClass> Logic { get; set; }
        public FormMapClass Map { get; set; }
        public bool? Mask { get; set; }
        public int MaxLength { get; set; }
        public int Minrows { get; set; }
        public int Initialrows { get; set; }
        public string Offsetslider { get; set; }
        public bool? Popuphelp { get; set; }
        public string Row { get; set; }
        public bool? Important { get; set; }
        public bool? Importantdisplay { get; set; }
        public bool? ShowCharCount { get; set; }
        public bool? ShowSummary { get; set; }
        public bool? ShowValidations { get; set; }
        public bool? ShowWordCount { get; set; }
        public string Sizeslider { get; set; }
        public string Tabindex { get; set; }
        public bool? TableView { get; set; }
        public string Tag { get; set; }
        public string Theme { get; set; }
        public string Title { get; set; }
        public string Tooltip { get; set; }
        public string Type { get; set; }
        public List<FileUploadClass> Uploadfile { get; set; }

        public string Url { get; set; }
        public ValidationClass Validate { get; set; }
        public List<FormValuesClass> Value { get; set; }
        public List<FormValuesClass> Values { get; set; }
        public string WebcamSize { get; set; }
        public FormDateWidgetClass Widget { get; set; }
        public string Widthslider { get; set; }

        // Border and Border Radius
        public int Border { get; set; }
        public int BorderR { get; set; }

        //Options Label Position and Inline
        public string OptionsLabelPosition { get; set; }
        public bool? Inline { get; set; }

        //placeholder
        public string Placeholder { get; set; }
        public string FirstNamePlaceholder { get; set; }
        public string MiddleNamePlaceholder { get; set; }
        public string LastNamePlaceholder { get; set; }

        //Template
        public string Template { get; set; }

        //prefix
        public string Prefix { get; set; }
        public string Suffix { get; set; }

        //delimiter
        public bool? Delimiter { get; set; }
        //decimalLimit
        public int DecimalLimit { get; set; }
        //requireDecimal
        public bool? RequireDecimal { get; set; }

        //Address component label
        public string Cclabel { get; set; }
        public string Lvllabel { get; set; }
        public string Streetlabel { get; set; }
        public string Suburblabel { get; set; }
        public string Statelabel { get; set; }
        public string Postcodelabel { get; set; }
        public string Countrylabel { get; set; }
        //Address component required fields
        public bool? Requiredstreet { get; set; }
        public bool? Requiredsuburb { get; set; }
        public bool? Requiredstate { get; set; }
        public bool? Requiredpostcode { get; set; }
        public bool? Requiredcountry { get; set; }

        //First&Last name  component required fields
        public bool? Requiredfirstname { get; set; }
        public bool? Requiredlastname { get; set; }

        //errorLabel
        public string ErrorLabel { get; set; }

        //defaultDate
        public string DefaultDate { get; set; }

        //clearOnHide
        public bool? ClearOnHide { get; set; }

        //inputFormat
        public string InputFormat { get; set; }

        //labelWidth labelMargin
        public int LabelWidth { get; set; }
        public int LabelMargin { get; set; }

        //validateOn
        public string ValidateOn { get; set; }

        //selectValues
        public string SelectValues { get; set; }

        //refreshOn
        public string RefreshOn { get; set; }

        //timezone
        public string Timezone { get; set; }

        //italic underline
        public bool? Italic { get; set; }
        public bool? Underline { get; set; }

        //fontcolour backcolour fontsize
        public string Fontcolour { get; set; }
        public string Backcolour { get; set; }
        public int Fontsize { get; set; }


        //conditional
        public dynamic Conditional { get; set; }

        //editor
        public string Editor { get; set; }

        //maxValue step
        public int MaxValue { get; set; }
        public int Step { get; set; }


        //header youtubeid
        public string Header { get; set; }
        public string Youtubeid { get; set; }

        //width height ** size imageSize
        public string Width { get; set; }
        public string Height { get; set; }
        public string Size { get; set; }
        public string ImageSize { get; set; }

        //rightIcon
        public string RightIcon { get; set; }

        //disableOnInvalid multiple webcam
        public bool? DisableOnInvalid { get; set; }
        public bool? Multiple { get; set; }
        public bool? Webcam { get; set; }


        //filePattern fileMinSize fileMaxSize
        public string FilePattern { get; set; }
        public string FileMinSize { get; set; }
        public string FileMaxSize { get; set; }

        //backgroundColor penColor
        public string BackgroundColor { get; set; }
        public string PenColor { get; set; }

        //AddressGroup Placeholder
        public string CcPlacehoder { get; set; }
        public string LvlPlacehoder { get; set; }
        public string StreetPlacehoder { get; set; }
        public string SuburbPlacehoder { get; set; }
        public string StatePlacehoder { get; set; }
        public string PostcodePlacehoder { get; set; }
    }

	public class AttrsClass
	{
		public string attr { get; set; }
		public string value { get; set; }

	}

	public class FileUploadClass
	{
		public string storage { get; set; }
		public string name { get; set; }
		public string url { get; set; }

		public string size { get; set; }
		public string type { get; set; }
		public string originalname { get; set; }

	}

	public class FormMapClass
	{
		public string Key { get; set; }
	}

	public class DatePickerClass
	{
		public string MinDate { get; set; }
		public string MaxDate { get; set; }
	}

	public class ValidationClass
	{
		public bool? Required { get; set; }

        public string CustomMessage { get; set; }

        //pattern
        public string Pattern { get; set; }
        //minLength maxLength minWords maxWords
        public int MinLength { get; set; }
        public int MaxLength { get; set; }
        public int MinWords { get; set; }
        public int MaxWords { get; set; }

        public int? Min { get; set; }
        public int? Max { get; set; } 

    }


    public class FormDataClass
    {
        public List<FormValuesClass> Value { get; set; }
        public List<FormValuesClass> Values { get; set; }
        public dynamic Json { get; set; }
        public string Custom { get; set; }
        public string Url { get; set; }
        public dynamic Headers { get; set; }
    }

	public class FormSubstitutionClass
	{
		public bool Repeat { get; set; }
		public string FNPattern { get; set; }
		public string FNReplacement { get; set; }
	}

	public class FormDateWidgetClass
	{
		public bool AllowInput { get; set; }
		public string DefaultDate { get; set; }
		public string DisplayInTimezone { get; set; }
		public bool EnableTime { get; set; }
		public string Format { get; set; }
		public int HourIncrement { get; set; }
		public string Language { get; set; }
		public string MaxDate { get; set; }
		public string MinDate { get; set; }
		public int MinuteIncrement { get; set; }
		public string Mode { get; set; }
		public bool NoCalendar { get; set; }
		public bool Time_24hr { get; set; }
		public string Type { get; set; }
		public bool UseLocaleSettings { get; set; }
	}

	public class FormValuesClass
	{
		public string Label { get; set; }
		public string Value { get; set; }
		public string Mtext { get; set; }
	}

	public class FormlogicClass
	{
		public string Name { get; set; }
		public FormlogicTriggerClass Trigger { get; set; }
		public List<FormlogicActionsClass> Actions { get; set; }
	}

	public class FormlogicTriggerClass
	{
		public string Type { get; set; }
		public string Javascript { get; set; }
		public string Json { get; set; }
		//[JsonProperty("event")]
		public string Event { get; set; }
		public FormlogicTriggerSimpleClass Simple { get; set; }
	}

	public class FormlogicTriggerSimpleClass
	{
		public bool Show { get; set; }
		public string When { get; set; }
		public string Eq { get; set; }
	}


	public class FormlogicActionsClass
	{
		public string Name { get; set; }
		public string Type { get; set; }
		public FormlogicActionsPropertyClass Property { get; set; }
		public string Value { get; set; }
		public string State { get; set; }
		public string Text { get; set; }

        public string Content { get; set; }
    }

	public class FormlogicActionsPropertyClass
	{
		public string Label { get; set; }
		public string Value { get; set; }
		public string Type { get; set; }

        public string Component { get; set; }
    }

	public class PageSettingsClass
	{
		public string Breadcrumb { get; set; }
		public bool BreadcrumbClickable { get; set; } = true;
		public bool Collapsed { get; set; } = false;
		public bool Collapsible { get; set; } = false;
		public string Key { get; set; }
		public string Label { get; set; }
		public bool IsWizard { get; set; }
	}

    public class SchemaButtonsClass
    {
        public string Clear_label { get; set; } = "Clear";
        public string Clear_disable { get; set; } = "false";
        public string Clear_hide { get; set; } = "false";
        public string Next_label { get; set; } = "Next";
        public string Next_disable { get; set; } = "false";
        public string Next_hide { get; set; } = "false";
        public string Previous_label { get; set; } = "Previous";
        public string Previous_disable { get; set; } = "false";
        public string Previous_hide { get; set; } = "false";
        public string Save_label { get; set; } = "Save";
        public string Save_disable { get; set; } = "false";
        public string Save_hide { get; set; } = "false";
        public string Submit_label { get; set; } = "Submit";
        public string Submit_disable { get; set; } = "false";
        public string Submit_hide { get; set; } = "false";
    }


    public class ButtonClass
    {
        public string key { get; set; } = "";
        public string label { get; set; } = "";
        public string action { get; set; } = "custom";
        public bool showValidations { get; set; } = false;
        public string url { get; set; } = "";
        public string custom { get; set; } = "alert('hi');";
        public string theme { get; set; } = "success";
        public string widthslider { get; set; } = "1";
        public string offsetslider { get; set; } = "0";
        public string type { get; set; } = "sfabutton";
        public bool input { get; set; } = true;
        public bool tableView { get; set; } = true;

    }
}