using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Syntaq.Falcon.Documents
{
    public class DocumentAssemblyMessageProvider : FalconDomainServiceBase
    {
        public virtual void RepeatIDsSet(XElement xparent, int parentid, int id)
        {
            var xname = "data";
            if (xparent.Name == "data" && xparent.Attribute("ParentID") == null)
            {
                TrySetAttributeValue(xparent, "ParentID", parentid.ToString());
                TrySetAttributeValue(xparent, "ID", id.ToString());
                RepeatIDsSet(xparent, 1, 1);
            }
            else
            {
                foreach (XElement xitem in xparent.Elements())
                {
                    if (xitem.HasElements)
                    {
                        id = xname == xitem.Name.LocalName ? id : 1;
                        TrySetAttributeValue(xitem, "ParentID", parentid.ToString());
                        TrySetAttributeValue(xitem, "ID", id.ToString());

                        // Check child ID's
                        RepeatIDsSet(xitem, id, 1);

                        xname = xitem.Name.ToString();
                        id++;
                    }
                }
            }
        }

        private static void TrySetAttributeValue(XElement parentEl, string attrName, string value)
        {
            if (string.IsNullOrEmpty(value)) value = string.Empty;

            XAttribute foundAttr = parentEl.Attribute(attrName);
            if (foundAttr != null)
            {
                foundAttr.Value = value;
            }
            else
            {
                parentEl.Add(new XAttribute(attrName, value));
            }
        }

        public virtual void RepeatCountSet(XElement xparent)
        {
            try
            {
                xparent.Elements().ToList().ForEach(q => { });
                foreach (XElement xchild in xparent.Elements())
                {

                    if (xchild.HasElements)
                    {
                        int index = 1;
                        xparent.Elements().Where(i => i.Name == xchild.Name).ToList().ForEach(
                            q => {
                                if (!q.Elements("rpt_Index").Any())
                                {
                                    q.Add(new XElement("rpt_Index", index));
                                }
                                else
                                {
                                    q.Element("rpt_Index").Value = index.ToString();
                                }

                                if (!q.Elements("rpt_Index_Parity").Any())
                                {
                                    q.Add(new XElement("rpt_Index_Parity", index % 2 == 0 ? "even" : "odd"));
                                }
                                else
                                {
                                    q.Element("rpt_Index_Parity").Value = index % 2 == 0 ? "even" : "odd";
                                }

                                index++;
                            }
                        );

                        // get top level node
                        XElement xtop = xparent;
                        while (xtop != null)
                        {
                            if (xtop.Parent == null)
                            {
                                break;
                            }
                            else
                            {
                                xtop = xtop.Parent;
                            }
                        }

                        int cnt = xparent.Elements(xchild.Name).Count();

                        int totalcnt = xtop.Descendants(xchild.Name).Count();

                        // Check if Count exists
                        if (!xparent.Elements(xchild.Name + "_Count").Any())
                        {
                            xparent.Add(new XElement(xchild.Name + "_Count", cnt.ToString()));
                        }
                        else
                        {
                            xparent.Element(xchild.Name + "_Count").Value = cnt.ToString();
                        }

                        if (!xtop.Elements(xchild.Name + "_Count").Any())
                        {
                            xtop.Add(new XElement(xchild.Name + "_Count", totalcnt.ToString()));
                        }
                        else
                        {
                            xtop.Element(xchild.Name + "_Count").Value = totalcnt.ToString();
                        }

                        RepeatCountSet(xchild);
                    }
                }
            }
            catch (Exception)
            {
                //Add Exception Handler
            }
        }
    }
}
