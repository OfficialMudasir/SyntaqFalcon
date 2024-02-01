using Aspose.Words;
using Aspose.Words.Drawing;
using Aspose.Words.Saving;
using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using WordFusion.Assembly;

namespace Syntaq.Falcon.Utility
{
    public static class AsposeUtility
    {
        private static Document BytesToDoc(byte[] docBytes)
        {
            License license = new License();
            license.SetLicense("Aspose.Words.lic");
            Document Doc = new Document(new MemoryStream(docBytes));
            return Doc;
        }

        private static byte[] DocToBytes(Document Doc)
        {
            MemoryStream outStream = new MemoryStream();
            Doc.Save(outStream, SaveFormat.Docx);
            byte[] docBytes = outStream.ToArray();
            return docBytes;
        }

        public static byte[] BytesToWord(byte[] docBytes, bool draft = false)
        {
            License license = new License();
            license.SetLicense("Aspose.Words.lic");
            byte[] wordbytes = null;
            if (docBytes != null)
            {
                Document doc = new Document(new MemoryStream(docBytes));
                MemoryStream wordstream = new MemoryStream();

                if (draft)
                {
                    doc = InsertWatermarkText(doc, "Draft");
                }

                doc.Save(wordstream, SaveFormat.Docx);
                wordbytes = wordstream.ToArray();
            }
            return wordbytes;
        }

		public static byte[] BytesToPdf(byte[] docBytes, bool draft = false)
		{
			License license = new License();
			license.SetLicense("Aspose.Words.lic");
			byte[] pdfbytes = null;
			if (docBytes != null)
			{

                if (IsValidPdf(docBytes))
                {
                    pdfbytes = docBytes;
                }
                else
                {
				    Document doc = new Document(new MemoryStream(docBytes));
				    MemoryStream pdfstream = new MemoryStream();

                    if (draft)
                    {
                        doc = InsertWatermarkText(doc, "Draft");
                    }

				    doc.Save(pdfstream, SaveFormat.Pdf);
				    pdfbytes = pdfstream.ToArray();
                }


			}
			return pdfbytes;
		}

        public static bool IsValidPdf(byte[] bytes)
        {
            var header = new[] { bytes[0], bytes[1], bytes[2], bytes[3] };
            var isHeaderValid = header[0] == 0x25 && header[1] == 0x50 && header[2] == 0x44 && header[3] == 0x46; //%PDF
            var trailer = new[] { bytes[bytes.Length - 5], bytes[bytes.Length - 4], bytes[bytes.Length - 3], bytes[bytes.Length - 2], bytes[bytes.Length - 1] };
            var isTrailerValid = trailer[0] == 0x25 && trailer[1] == 0x25 && trailer[2] == 0x45 && trailer[3] == 0x4f && trailer[4] == 0x46; //%%EOF
            return isHeaderValid; // && isTrailerValid;

            //if (bytes != null && bytes.Length > 4 &&
            //         bytes[0] == 0x25 && // %
            //         bytes[1] == 0x50 && // P
            //         bytes[2] == 0x44 && // D
            //         bytes[3] == 0x46 && // F
            //         bytes[4] == 0x2D)
            //{ // -

            //    // version 1.3 file terminator
            //    if (bytes[5] == 0x31 && bytes[6] == 0x2E && bytes[7] == 0x33 &&
            //            bytes[bytes.Length - 7] == 0x25 && // %
            //            bytes[bytes.Length - 6] == 0x25 && // %
            //            bytes[bytes.Length - 5] == 0x45 && // E
            //            bytes[bytes.Length - 4] == 0x4F && // O
            //            bytes[bytes.Length - 3] == 0x46 && // F
            //            bytes[bytes.Length - 2] == 0x20 && // SPACE
            //            bytes[bytes.Length - 1] == 0x0A)
            //    { // EOL
            //        return true;
            //    }

            //    // version 1.3 file terminator
            //    if (bytes[5] == 0x31 && bytes[6] == 0x2E && bytes[7] == 0x34 &&
            //            bytes[bytes.Length - 6] == 0x25 && // %
            //            bytes[bytes.Length - 5] == 0x25 && // %
            //            bytes[bytes.Length - 4] == 0x45 && // E
            //            bytes[bytes.Length - 3] == 0x4F && // O
            //            bytes[bytes.Length - 2] == 0x46 && // F
            //            bytes[bytes.Length - 1] == 0x0A)
            //    { // EOL
            //        return true;
            //    }
            //}
            //return false;

        }

        public static byte[] BytesToHTML(byte[] docBytes)
        {
            License license = new License();
            license.SetLicense("Aspose.Words.lic");
            byte[] htmlbytes = null;
            if (docBytes != null)
            {
                Document doc = new Document(new MemoryStream(docBytes));

                MemoryStream htmlStream = new MemoryStream();
                MemoryStream imagesStream = new MemoryStream();
                try
                {
                    HtmlSaveOptions options = new HtmlSaveOptions(SaveFormat.Html)
                    {
                        ExportImagesAsBase64 = true
                    };
                    var handleImageSaving = new HandleImageSaving(imagesStream);
                    options.ImageSavingCallback = handleImageSaving;

                    doc.Save(htmlStream, options);

                    DocumentBuilder builder = new DocumentBuilder(doc);
                    foreach (Stream imgStream in handleImageSaving.ImageStreams)
                    {
                        builder.InsertImage(imgStream);
                        builder.Writeln();
                    }

                    htmlbytes = htmlStream.ToArray();
                }
                catch (Exception)
                {
                    htmlbytes = null;
                }
            }
            //BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB
            //if (docBytes != null)
            //{
            //	Document doc = new Document(new MemoryStream(docBytes));
            //	MemoryStream htmlstream = new MemoryStream();
            //	try
            //	{
            //		doc.Save(htmlstream, SaveFormat.Html);
            //		htmlbytes = htmlstream.ToArray();
            //	}
            //	catch (Exception)
            //	{
            //		htmlbytes = null;
            //	}				
            //}
 
            return htmlbytes;
        }

        public static byte[] WriteErrorsToDoc(byte[] docBytes, string[] Errors)
        {
            var Doc = BytesToDoc(docBytes);
            DocumentBuilder builder = new DocumentBuilder(Doc);
            builder.Bold = true;
            builder.Font.Color = Color.Red;
            builder.Font.Size = 18;
            builder.Font.Name = "Verdana";
            builder.Writeln("There were one or more errors while assembling your document.");
            builder.Bold = false;
            builder.Font.Size = 12;
            foreach (string Error in Errors)
            {
                builder.Writeln(Error);
                builder.Writeln("");
            }
            var DocBytes = DocToBytes(Doc);
            return DocBytes;
        }

        public static bool ShouldNotAppendDocument(byte[] docBytes)
        {
            var Doc = BytesToDoc(docBytes);
            var rangetext = Doc == null ? string.Empty : Doc.Range.Text;
            return rangetext.ToLower().Contains("wf:donotappend") ? true : false;
        }

        public static bool ContainsDocumentName(byte[] docBytes)
        {
            var Doc = BytesToDoc(docBytes);
            var rangetext = Doc == null ? string.Empty : Doc.Range.Text;
            return rangetext.ToLower().Contains("<docname>") ? true : false;
        }

        public static (byte[], string) DocumentName(byte[] docBytes)
        {
            var Doc = BytesToDoc(docBytes);
            var rangetext = Doc == null ? string.Empty : Doc.Range.Text;
            var matches = new Regex("<docname>(.*?)</docname>", RegexOptions.IgnoreCase).Matches(rangetext);
            if (matches.Count > 0)
            {
                Doc.Range.Replace(new Regex(@"<docname>(.*?)</docname>", RegexOptions.IgnoreCase), "");
                var DocBytes = DocToBytes(Doc);
                return (DocBytes, matches[0].Groups[1].ToString());
            }
            else
            {
                return (null, null);
            }
        }

        public static byte[] RemoveDoNotAppend(byte[] docBytes)
        {
            var Doc = BytesToDoc(docBytes);
            ReplaceSectionDoNotAppend rep2 = new ReplaceSectionDoNotAppend();
            Doc.Range.Replace(new Regex(@"wf:donotappend", RegexOptions.IgnoreCase), rep2, false);
            foreach (Node nd in rep2.nodecol) { try { nd.Remove(); } catch { } }
            var DocBytes = DocToBytes(Doc);
            return DocBytes;
        }

        public static bool ContainsContinuousSections(byte[] docBytes)
        {
            var Doc = BytesToDoc(docBytes);
            var rangetext = Doc == null ? string.Empty : Doc.Range.Text;
            return rangetext.ToLower().Contains("wf:sectionstart.continuous") ? true : false;
        }

        public static byte[] SetContinuousSections(byte[] docBytes)
        {
            var Doc = BytesToDoc(docBytes);
            Doc.FirstSection.PageSetup.SectionStart = SectionStart.Continuous;
            Doc.FirstSection.PageSetup.RestartPageNumbering = false;

            ReplaceSectionStartContinuous rep = new ReplaceSectionStartContinuous();
            Doc.Range.Replace(new Regex(@"wf:sectionstart.continuous", RegexOptions.IgnoreCase), rep, false);
            foreach (Node nd in rep.nodecol) { try { nd.Remove(); } catch { } }
            var DocBytes = DocToBytes(Doc);
            return DocBytes;
        }

        public static byte[] JoinNextSection(byte[] docBytes)
        {

            var Doc = BytesToDoc(docBytes);

            //WordFusion.Assembly.ReplaceDelEmptyPara rep = new ReplaceDelEmptyPara();
            //rep = new ReplaceDelEmptyPara();
            //Doc.Range.Replace(new Regex(@"WF:DelEmptyPara", RegexOptions.IgnoreCase), rep, false);
            //foreach (Node nd in rep.nodecol)
            //{
            //    try
            //    {
            //        nd.Remove();
            //    }
            //    catch { }
            //}


            //FindReplaceOptions options = new FindReplaceOptions();
            //options.Direction = FindReplaceDirection.Backward;
            //options.ReplacingCallback = new TestReplaceHandler();


            FindMatchedNodes repafterjoin = new FindMatchedNodes();
            repafterjoin = new FindMatchedNodes();
            Doc.Range.Replace(new Regex(@"wf:joinnextsection", RegexOptions.IgnoreCase), repafterjoin, false);

            DocumentBuilder builder = new DocumentBuilder(Doc);
            foreach (Node nd in repafterjoin.nodes)
            {
                try
                {
                    builder.MoveTo(nd);
                    var sectionindex = Doc.Sections.IndexOf(builder.CurrentSection);
                    if (Doc.Sections[sectionindex + 1] != null)
                    {
                        var nextsection = Doc.Sections[sectionindex + 1];
                        Doc.Sections[sectionindex].PrependContent(nextsection);
                        nextsection.Remove();
                    }

                    nd.Remove();
                }
                catch { }
            }

            //TestReplaceHandler rep = new TestReplaceHandler();
            //Doc.Range.Replace(new Regex(@"wf:delemptypara", RegexOptions.IgnoreCase), rep, false);
            //foreach (Node nd in rep.nodecol) { try { nd.Remove(); } catch { } }

            var DocBytes = DocToBytes(Doc);
            return DocBytes;

        }
        public static byte[] AppendDocument(byte[] PrimaryBytes, byte[] BytesToAppend)
        {
            var PrimaryDoc = BytesToDoc(PrimaryBytes);
            var DocToAppend = BytesToDoc(BytesToAppend);
            PrimaryDoc.AppendDocument(DocToAppend, ImportFormatMode.UseDestinationStyles);
            return DocToBytes(PrimaryDoc);
        }

		public static byte[] FinalCleandown(byte[] docBytes)
		{
			var Doc = BytesToDoc(docBytes);
			Assembler docassembly = new Assembler();
			docassembly.DocumentCleanDownJoinToDoc(ref Doc);
			docassembly.UpdateDocumentFields(ref Doc);

            cleanMacros(ref Doc);

            var DocBytes = DocToBytes(Doc);
			return DocBytes;
		}

        public static void cleanMacros(ref Aspose.Words.Document doc)
        {
            if(doc != null)
            {
                if (doc.Range != null)
                {
                    if (doc.Range.Text.ToLower().Contains("wf:donotremovemacros") ? false : true)
                    {
                        doc.RemoveMacros();
                    }
                    else
                    {
                        doc.Range.Replace(new Regex(@"wf:donotremovemacros", RegexOptions.IgnoreCase), "");
                    }                    
                }
            }
        }

        /// <summary>
        /// Inserts a watermark into a document.
        /// </summary>
        /// <param name="doc">The input document.</param>
        /// <param name="watermarkText">Text of the watermark.</param>
        public static Aspose.Words.Document InsertWatermarkText(Aspose.Words.Document doc, string watermarkText)
        {
            // Create a watermark shape. This will be a WordArt shape. 
            // You are free to try other shape types as watermarks.
            Aspose.Words.Drawing.Shape watermark = new Aspose.Words.Drawing.Shape(doc, Aspose.Words.Drawing.ShapeType.TextPlainText);

            // Set up the text of the watermark.
            watermark.TextPath.Text = watermarkText;
            watermark.TextPath.FontFamily = "Arial";
            watermark.Width = 500;
            watermark.Height = 100;
            // Text will be directed from the bottom-left to the top-right corner.
            watermark.Rotation = -40;
            // Remove the following two lines if you need a solid black text.
            watermark.Fill.Color =  System.Drawing.Color.FromArgb(222,222,222); // Try LightGray to get more Word-style watermark
            watermark.StrokeColor = System.Drawing.Color.FromArgb(222, 222, 222); // Try LightGray to get more Word-style watermark

            // Place the watermark in the page center.
            watermark.RelativeHorizontalPosition = Aspose.Words.Drawing.RelativeHorizontalPosition.Page;
            watermark.RelativeVerticalPosition = Aspose.Words.Drawing.RelativeVerticalPosition.Page;
            watermark.WrapType = Aspose.Words.Drawing.WrapType.None;
            watermark.VerticalAlignment = Aspose.Words.Drawing.VerticalAlignment.Center;
            watermark.HorizontalAlignment = Aspose.Words.Drawing.HorizontalAlignment.Center;

            // Create a new paragraph and append the watermark to this paragraph.
            Aspose.Words.Paragraph watermarkPara = new Aspose.Words.Paragraph(doc);
            watermarkPara.AppendChild(watermark);

            // Insert the watermark into all headers of each document section.
            foreach (Aspose.Words.Section sect in doc.Sections)
            {
                // There could be up to three different headers in each section, since we want
                // the watermark to appear on all pages, insert into all headers.
                InsertWatermarkIntoHeader(watermarkPara, sect, Aspose.Words.HeaderFooterType.HeaderPrimary);
                InsertWatermarkIntoHeader(watermarkPara, sect, Aspose.Words.HeaderFooterType.HeaderFirst);
                InsertWatermarkIntoHeader(watermarkPara, sect, Aspose.Words.HeaderFooterType.HeaderEven);
            }

            return doc;
        }

        private static void InsertWatermarkIntoHeader(Aspose.Words.Paragraph watermarkPara, Aspose.Words.Section sect, Aspose.Words.HeaderFooterType headerType)
        {

            Aspose.Words.HeaderFooter header = sect.HeadersFooters[headerType];
            if (header == null)
            {
                // There is no header of the specified type in the current section, create it.
                header = new Aspose.Words.HeaderFooter(sect.Document, headerType);
                sect.HeadersFooters.Add(header);
            }

            // Insert a clone of the watermark into the header.
            header.AppendChild(watermarkPara.Clone(true));
        }
    }

    public class HandleImageSaving : IImageSavingCallback
    {
        public ArrayList ImageStreams;
        private MemoryStream m_images;

        public HandleImageSaving(MemoryStream i_images)
        {
            ImageStreams = new ArrayList();
            m_images = i_images;
        }

        void IImageSavingCallback.ImageSaving(ImageSavingArgs args)
        {

            try
            {
                Shape shape = (Shape)args.CurrentShape;
                if (shape.ImageData.ImageBytes == null)
                {
                    m_images = new MemoryStream();
                }
                else
                {
                    m_images = new MemoryStream(shape.ImageData.ImageBytes);
                }
                ImageStreams.Add(m_images);

                args.ImageStream = m_images;
                args.KeepImageStreamOpen = true;
            }
            catch
            {
                // Continue
            }


        }
    }
}
