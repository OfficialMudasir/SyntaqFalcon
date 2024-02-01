using System;
using System.IO;
using System.Xml.Linq;

namespace WordFusion.Assembly {
    public class AssemblePDF {

        public Byte[] AssemblePDFItext(string documentURL, XElement xdata, XElement xdataparent) {

            byte[] retval;

            try {
                // Retrieve document template as URL

                iTextSharp.text.pdf.PdfReader pdfReader;
                using (System.Net.WebClient client = new System.Net.WebClient()) {
                    // Download data.
                    byte[] docbyte = new System.Net.WebClient().DownloadData(documentURL);
                    pdfReader = new iTextSharp.text.pdf.PdfReader(docbyte);
                }
                
                MemoryStream filestream = new MemoryStream();
                iTextSharp.text.pdf.PdfStamper pdfStamper = new iTextSharp.text.pdf.PdfStamper(pdfReader, filestream, '\0', true);
                iTextSharp.text.pdf.AcroFields pdfFormFields = pdfStamper.AcroFields;

                // set form pdfFormFields
                // The first worksheet and W-4 form
                //foreach (KeyValuePair<string, iTextSharp.text.pdf.AcroFields.Item> fld in pdfFormFields.Fields) {
                foreach (System.Collections.DictionaryEntry fld in pdfFormFields.Fields) {

                    string fldname = fld.Key.ToString().Replace("File[0]", "");
                    fldname = fldname.Replace("[", "_%0112%_").Replace("]", "_%0113%_");

                    fldname = System.Xml.XmlConvert.EncodeLocalName(fldname);

                    var value = TryGetElementValue(xdataparent, fldname, string.Empty);
                    value = string.IsNullOrEmpty(value) ? TryGetElementValue(xdata, fldname, string.Empty) : value;

                    //TextField :4//CheckBox :2//RadioButton :3//ComboBox :6//ListBox :5//Button :1
                    int field_type = pdfFormFields.GetFieldType((string)fld.Key);

                    if (field_type == 2) {

                        // Vesion 6 ItextSharp
                        // iTextSharp.text.pdf.AcroFields.Item itm1 = pdfFormFields.GetFieldItem((string)fld.Key).GetValue(0).GetAsDict(iTextSharp.text.pdf.PdfName.AP).GetAsDict(iTextSharp.text.pdf.PdfName.N).Keys.First().ToString().TrimStart('/');

                        string chkvalue = "true";
                        iTextSharp.text.pdf.AcroFields.Item pdfitm = pdfFormFields.GetFieldItem((string)fld.Key);
                        iTextSharp.text.pdf.PdfDictionary pdfdic = (iTextSharp.text.pdf.PdfDictionary)pdfitm.values[0];

                        if (pdfdic != null) {
                            pdfdic = (iTextSharp.text.pdf.PdfDictionary)pdfdic.Get(new iTextSharp.text.pdf.PdfName("AP"));
                            if (pdfdic != null) {
                                pdfdic = (iTextSharp.text.pdf.PdfDictionary)pdfdic.Get(new iTextSharp.text.pdf.PdfName("N"));

                                // get the first item in the keys
                                if (pdfdic.Keys.Count > 0) {
                                    int cnt = 0;
                                    while (cnt == 0) {
                                        System.Collections.IDictionaryEnumerator enums = pdfdic.GetEnumerator();
                                        enums.MoveNext();
                                        chkvalue = enums.Key.ToString().TrimStart('/');
                                        cnt++;
                                    }
                                }
                            }
                        }

                        if (value.ToLower() == "true" || value.ToLower() == "yes" || value.ToLower() == "on" || value.ToLower() == chkvalue.ToLower()) {
                            pdfFormFields.SetField((string)fld.Key, chkvalue);
                        }
                        else {
                            pdfFormFields.SetField((string)fld.Key, "");
                        }

                    }
                    if (field_type == 3) {

                        pdfFormFields.SetField((string)fld.Key, value);

                    }
                    else {

                        if (value.ToLower().StartsWith("image/png;base64")) {

                            value = TryGetElementValue(xdataparent, fldname.ToLower().Replace("img:", ""), string.Empty);
                            value = string.IsNullOrEmpty(value) ? TryGetElementValue(xdata, fldname.ToLower().Replace("img:", ""), string.Empty) : value;

                            value = value.Replace("image/png;base64,", "");

                            byte[] data = Convert.FromBase64String(value);
                            using (var stream = new MemoryStream(data, 0, data.Length)) {

                                var pdfContentByte = pdfStamper.GetOverContent(1);
                                iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(stream);

                                float[] rec = pdfFormFields.GetFieldPositions((string)fld.Key);

                                float left = rec[1]; float right = rec[3]; float top = rec[4]; float bottom = rec[2];
                                image.ScaleToFit(right - left, top - bottom);

                                image.SetAbsolutePosition(left, bottom);
                                pdfContentByte.AddImage(image);

                            }
                        }
                        else {
                            pdfFormFields.SetField((string)fld.Key, value);
                        }

                    }

                }

                // flatten the form to remove editting options, set it to false
                // to leave the form open to subsequent manual edits
                pdfStamper.FormFlattening = false;

                // close the pdf
                pdfStamper.Close();
                pdfReader.Close();

                byte[] docBytes = filestream.ToArray();
                return retval = docBytes;
            }
            catch {
                return null;
            }

        }

        public static String TryGetElementValue(System.Xml.Linq.XElement parentEl, string elementname, string defaultValue = "") {

            if (string.IsNullOrEmpty(elementname)) {
                return defaultValue;
            }
            else {

                if (parentEl != null) {

                    System.Xml.Linq.XElement foundele = parentEl.Element(elementname);
                    if (foundele != null) {
                        return foundele.Value;
                    }
                    else {
                        return defaultValue;
                    }
                }
                else {
                    return defaultValue;
                }
            }
        }

    }
}
