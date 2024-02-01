using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace WordFusion.Assembly {
     static public  class FormProgress {

         static public XmlDocument xmlDoc = new XmlDocument();
         static private String fname = System.Windows.Forms.Application.StartupPath + "\\Data\\FormProgress.xml";

         static public void Initialise() {
            String xml = "<Forms></Forms>";
            xmlDoc.XmlResolver = null;
            xmlDoc.LoadXml(xml);
        }

         static public void LoadFormProgress() {
             xmlDoc.XmlResolver = null;
             xmlDoc.Load(fname);
         }

         static public XmlNodeList Forms {
             get { return xmlDoc.DocumentElement.ChildNodes; }
         }

         static  public int FormCount {
             get { return xmlDoc.DocumentElement.ChildNodes.Count; }
         }

 
         static public void AddStep(String name) {
             try{

                 if (xmlDoc.DocumentElement != null) {
                     String nd = "<Step Loaded='false'></Step>";
                     XmlElement nodeform = xmlDoc.CreateElement("Step");

                     XmlAttribute attform = xmlDoc.CreateAttribute("Loaded");
                     attform.Value = "false";
                     nodeform.Attributes.Append(attform);

                     attform = xmlDoc.CreateAttribute("Name");
                     attform.Value = name;
                     nodeform.Attributes.Append(attform);

                     xmlDoc.DocumentElement.AppendChild(nodeform);

                 }

             }
             catch {
                 // Do Nothing
             } 
         }

         static public void SetLoaded(String name, String value) {
             try {

                 if (xmlDoc.DocumentElement != null) {
                     XmlNode formnd = xmlDoc.DocumentElement.SelectSingleNode("Step[@Name='" + name + "']");
                    formnd.Attributes["Loaded"].Value = value;
                 }
             }
             catch {
                 // Do Nothing
             }            
        }


         static public void Save() {
             try{
                 if( xmlDoc.DocumentElement !=null) xmlDoc.Save(fname);
             }
             catch {
                 // Do Nothing
             } 
        }

    }
}
