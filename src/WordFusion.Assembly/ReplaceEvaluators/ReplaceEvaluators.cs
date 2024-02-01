using Aspose.Words;
using System.Collections.ObjectModel;
using Aspose.Words.Fields;
 
namespace WordFusion.Assembly {

    public class TestReplaceHandler : IReplacingCallback
    {
        public Collection<CompositeNode> nodecol = new Collection<CompositeNode>();
        ReplaceAction IReplacingCallback.Replacing(ReplacingArgs args)
        {
            return ReplaceAction.Replace;
        }
    }

    public class ReplaceDelEmptyPara : IReplacingCallback {
        public Collection<CompositeNode> nodecol = new Collection<CompositeNode>();
        ReplaceAction IReplacingCallback.Replacing(ReplacingArgs e) {

            //try {
            //    // Clean empty Word IF fields so can del empty para
            //    NodeList fields = e.MatchNode.ParentNode.SelectNodes("//FieldStart");
            //    foreach (FieldStart field in fields) {

            //        if (field.FieldType == FieldType.FieldIf) {

            //            Collection<Node> col = new Collection<Node>();
            //            Node node = field; col.Add(node);
            //            while (node.NodeType != NodeType.FieldEnd) {
            //                node = node.NextSibling;
            //                col.Add(node);
            //            }

            //            string val = string.Empty;
            //            foreach (Node node2 in col) {
            //                if (node2.NodeType == NodeType.FieldSeparator) {
            //                    Node ndrun = node2.NextSibling;
            //                    while (ndrun.NodeType == NodeType.Run) {
            //                        val += ndrun.Range.Text;
            //                        ndrun = ndrun.NextSibling;
            //                    }
            //                }
            //            }

            //            if (string.IsNullOrEmpty(val)) {
            //                foreach (Node node3 in col) {
            //                    node3.Remove();
            //                }
            //            }
            //        }
            //    }
            //}
            //catch {
            //    // Dont catch
            //}
            //finally {

            //}

            if (e.MatchNode.ParentNode.Range.Text.ToLower() == ControlChar.NonBreakingSpaceChar.ToString() + "wf:delemptypara\f" || 
                    e.MatchNode.ParentNode.Range.Text.ToLower() == ControlChar.NonBreakingSpaceChar.ToString() + "wf:delemptypara\r" || 
                    e.MatchNode.ParentNode.Range.Text.ToLower() == "wf:delemptypara\r" || e.MatchNode.ParentNode.Range.Text.ToLower() == "wf:delemptypara\f" || 
                    e.MatchNode.ParentNode.Range.Text.ToLower() == "wf:delemptypara\a") {

                nodecol.Add(e.MatchNode.ParentNode);            
                return Aspose.Words.ReplaceAction.Replace;
            }

            if (e.MatchNode.ParentNode.Range.Text.ToLower() == ControlChar.NonBreakingSpaceChar.ToString() + "wf:delemptypara \f" ||
                    e.MatchNode.ParentNode.Range.Text.ToLower() == ControlChar.NonBreakingSpaceChar.ToString() + "wf:delemptypara \r" ||
                    e.MatchNode.ParentNode.Range.Text.ToLower() == "wf:delemptypara \r" || e.MatchNode.ParentNode.Range.Text.ToLower() == "wf:delemptypara \f" ||
                    e.MatchNode.ParentNode.Range.Text.ToLower() == "wf:delemptypara \a") {

                nodecol.Add(e.MatchNode.ParentNode);
                return Aspose.Words.ReplaceAction.Replace;
            }

            if (e.MatchNode.ParentNode.Range.Text.ToLower() == ControlChar.NonBreakingSpaceChar.ToString() + " wf:delemptypara\f" ||
                    e.MatchNode.ParentNode.Range.Text.ToLower() == ControlChar.NonBreakingSpaceChar.ToString() + " wf:delemptypara\r" ||
                    e.MatchNode.ParentNode.Range.Text.ToLower() == " wf:delemptypara\r" || e.MatchNode.ParentNode.Range.Text.ToLower() == " wf:delemptypara\f" ||
                    e.MatchNode.ParentNode.Range.Text.ToLower() == " wf:delemptypara\a") {

                nodecol.Add(e.MatchNode.ParentNode);
                return Aspose.Words.ReplaceAction.Replace;
            }

            return Aspose.Words.ReplaceAction.Replace;
        }
    }

    public class ReplaceSectionStartContinuous : IReplacingCallback {

        public Collection<CompositeNode> nodecol = new Collection<CompositeNode>();
        ReplaceAction IReplacingCallback.Replacing(ReplacingArgs e) {

            if (e.MatchNode.ParentNode.Range.Text.ToLower() == ControlChar.NonBreakingSpaceChar.ToString() + "wf:SectionStart.Continuous\f" ||
                    e.MatchNode.ParentNode.Range.Text.ToLower() == ControlChar.NonBreakingSpaceChar.ToString() + "wf:SectionStart.Continuous\r" ||
                    e.MatchNode.ParentNode.Range.Text.ToLower() == "wf:SectionStart.Continuous\r" || e.MatchNode.ParentNode.Range.Text.ToLower() == "wf:SectionStart.Continuous\f" ||
                    e.MatchNode.ParentNode.Range.Text.ToLower() == "wf:SectionStart.Continuous\a") {

                nodecol.Add(e.MatchNode.ParentNode);
                return Aspose.Words.ReplaceAction.Replace;
            }

            if (e.MatchNode.ParentNode.Range.Text.ToLower() == ControlChar.NonBreakingSpaceChar.ToString() + "wf:SectionStart.Continuous \f" ||
                    e.MatchNode.ParentNode.Range.Text.ToLower() == ControlChar.NonBreakingSpaceChar.ToString() + "wf:SectionStart.Continuous \r" ||
                    e.MatchNode.ParentNode.Range.Text.ToLower() == "wf:SectionStart.Continuous \r" || e.MatchNode.ParentNode.Range.Text.ToLower() == "wf:SectionStart.Continuous \f" ||
                    e.MatchNode.ParentNode.Range.Text.ToLower() == "wf:SectionStart.Continuous \a") {

                nodecol.Add(e.MatchNode.ParentNode);
                return Aspose.Words.ReplaceAction.Replace;
            }

            if (e.MatchNode.ParentNode.Range.Text.ToLower() == ControlChar.NonBreakingSpaceChar.ToString() + " wf:SectionStart.Continuous\f" ||
                    e.MatchNode.ParentNode.Range.Text.ToLower() == ControlChar.NonBreakingSpaceChar.ToString() + " wf:SectionStart.Continuous\r" ||
                    e.MatchNode.ParentNode.Range.Text.ToLower() == " wf:SectionStart.Continuous\r" || e.MatchNode.ParentNode.Range.Text.ToLower() == " wf:SectionStart.Continuous\f" ||
                    e.MatchNode.ParentNode.Range.Text.ToLower() == " wf:SectionStart.Continuous\a") {

                nodecol.Add(e.MatchNode.ParentNode);
                return Aspose.Words.ReplaceAction.Replace;
            }

            return Aspose.Words.ReplaceAction.Replace;
        }
    }

    public class ReplaceSectionDoNotAppend : IReplacingCallback {

        public Collection<CompositeNode> nodecol = new Collection<CompositeNode>();
        ReplaceAction IReplacingCallback.Replacing(ReplacingArgs e) {

            if (e.MatchNode.ParentNode.Range.Text.ToLower() == ControlChar.NonBreakingSpaceChar.ToString() + "wf:DoNotAppend\f" ||
                    e.MatchNode.ParentNode.Range.Text.ToLower() == ControlChar.NonBreakingSpaceChar.ToString() + "wf:DoNotAppend\r" ||
                    e.MatchNode.ParentNode.Range.Text.ToLower() == "wf:DoNotAppend\r" || e.MatchNode.ParentNode.Range.Text.ToLower() == "wf:DoNotAppend\f" ||
                    e.MatchNode.ParentNode.Range.Text.ToLower() == "wf:DoNotAppend\a") {

                nodecol.Add(e.MatchNode.ParentNode);
                return Aspose.Words.ReplaceAction.Replace;
            }

            if (e.MatchNode.ParentNode.Range.Text.ToLower() == ControlChar.NonBreakingSpaceChar.ToString() + "wf:DoNotAppend \f" ||
                    e.MatchNode.ParentNode.Range.Text.ToLower() == ControlChar.NonBreakingSpaceChar.ToString() + "wf:DoNotAppend \r" ||
                    e.MatchNode.ParentNode.Range.Text.ToLower() == "wf:DoNotAppend \r" || e.MatchNode.ParentNode.Range.Text.ToLower() == "wf:DoNotAppend \f" ||
                    e.MatchNode.ParentNode.Range.Text.ToLower() == "wf:DoNotAppend \a") {

                nodecol.Add(e.MatchNode.ParentNode);
                return Aspose.Words.ReplaceAction.Replace;
            }

            if (e.MatchNode.ParentNode.Range.Text.ToLower() == ControlChar.NonBreakingSpaceChar.ToString() + " wf:DoNotAppend\f" ||
                    e.MatchNode.ParentNode.Range.Text.ToLower() == ControlChar.NonBreakingSpaceChar.ToString() + " wf:DoNotAppend\r" ||
                    e.MatchNode.ParentNode.Range.Text.ToLower() == " wf:DoNotAppend\r" || e.MatchNode.ParentNode.Range.Text.ToLower() == " wf:DoNotAppend\f" ||
                    e.MatchNode.ParentNode.Range.Text.ToLower() == " wf:DoNotAppend\a") {

                nodecol.Add(e.MatchNode.ParentNode);
                return Aspose.Words.ReplaceAction.Replace;
            }

            return Aspose.Words.ReplaceAction.Replace;
        }
    }

    public class ReplaceSectionInsert : IReplacingCallback {

        public Collection<CompositeNode> nodecol = new Collection<CompositeNode>();
        ReplaceAction IReplacingCallback.Replacing(ReplacingArgs e) {

            if (e.MatchNode.ParentNode.Range.Text.ToLower() == ControlChar.NonBreakingSpaceChar.ToString() + "wf:sectioninsert\f" ||
                    e.MatchNode.ParentNode.Range.Text.ToLower() == ControlChar.NonBreakingSpaceChar.ToString() + "wf:sectioninsert\r" ||
                    e.MatchNode.ParentNode.Range.Text.ToLower() == "wf:sectioninsert\r" || e.MatchNode.ParentNode.Range.Text.ToLower() == "wf:sectioninsert\f" ||
                    e.MatchNode.ParentNode.Range.Text.ToLower() == "wf:sectioninsert\a") {

                nodecol.Add(e.MatchNode.ParentNode);
                return Aspose.Words.ReplaceAction.Replace;
            }

            if (e.MatchNode.ParentNode.Range.Text.ToLower() == ControlChar.NonBreakingSpaceChar.ToString() + "wf:sectioninsert \f" ||
                    e.MatchNode.ParentNode.Range.Text.ToLower() == ControlChar.NonBreakingSpaceChar.ToString() + "wf:sectioninsert \r" ||
                    e.MatchNode.ParentNode.Range.Text.ToLower() == "wf:sectioninsert \r" || e.MatchNode.ParentNode.Range.Text.ToLower() == "wf:sectioninsert \f" ||
                    e.MatchNode.ParentNode.Range.Text.ToLower() == "wf:sectioninsert \a") {

                nodecol.Add(e.MatchNode.ParentNode);
                return Aspose.Words.ReplaceAction.Replace;
            }

            if (e.MatchNode.ParentNode.Range.Text.ToLower() == ControlChar.NonBreakingSpaceChar.ToString() + " wf:sectioninsert\f" ||
                    e.MatchNode.ParentNode.Range.Text.ToLower() == ControlChar.NonBreakingSpaceChar.ToString() + " wf:sectioninsert\r" ||
                    e.MatchNode.ParentNode.Range.Text.ToLower() == " wf:sectioninsert\r" || e.MatchNode.ParentNode.Range.Text.ToLower() == " wf:sectioninsert\f" ||
                    e.MatchNode.ParentNode.Range.Text.ToLower() == " wf:sectioninsert\a") {

                nodecol.Add(e.MatchNode.ParentNode);
                return Aspose.Words.ReplaceAction.Replace;
            }

            return Aspose.Words.ReplaceAction.Replace;
        }
    }

    public class ReplaceSectionInsertNotLast : IReplacingCallback {

        public Collection<CompositeNode> nodecol = new Collection<CompositeNode>();
        ReplaceAction IReplacingCallback.Replacing(ReplacingArgs e) {

            if (e.MatchNode.ParentNode.Range.Text.ToLower() == ControlChar.NonBreakingSpaceChar.ToString() + "wf:sectioninsertnotlast\f" ||
                  e.MatchNode.ParentNode.Range.Text.ToLower() == ControlChar.NonBreakingSpaceChar.ToString() + "wf:sectioninsertnotlast\r" ||
                  e.MatchNode.ParentNode.Range.Text.ToLower() == "wf:sectioninsertnotlast\r" || e.MatchNode.ParentNode.Range.Text.ToLower() == "wf:sectioninsertnotlast\f" ||
                  e.MatchNode.ParentNode.Range.Text.ToLower() == "wf:sectioninsertnotlast\a") {

                nodecol.Add(e.MatchNode.ParentNode);
                return Aspose.Words.ReplaceAction.Replace;
            }

            if (e.MatchNode.ParentNode.Range.Text.ToLower() == ControlChar.NonBreakingSpaceChar.ToString() + "wf:sectioninsertnotlast \f" ||
                    e.MatchNode.ParentNode.Range.Text.ToLower() == ControlChar.NonBreakingSpaceChar.ToString() + "wf:sectioninsertnotlast \r" ||
                    e.MatchNode.ParentNode.Range.Text.ToLower() == "wf:sectioninsertnotlast \r" || e.MatchNode.ParentNode.Range.Text.ToLower() == "wf:sectioninsertnotlast \f" ||
                    e.MatchNode.ParentNode.Range.Text.ToLower() == "wf:sectioninsertnotlast \a") {

                nodecol.Add(e.MatchNode.ParentNode);
                return Aspose.Words.ReplaceAction.Replace;
            }

            if (e.MatchNode.ParentNode.Range.Text.ToLower() == ControlChar.NonBreakingSpaceChar.ToString() + " wf:sectioninsertnotlast\f" ||
                    e.MatchNode.ParentNode.Range.Text.ToLower() == ControlChar.NonBreakingSpaceChar.ToString() + " wf:sectioninsertnotlast\r" ||
                    e.MatchNode.ParentNode.Range.Text.ToLower() == " wf:sectioninsertnotlast\r" || e.MatchNode.ParentNode.Range.Text.ToLower() == " wf:sectioninsertnotlast\f" ||
                    e.MatchNode.ParentNode.Range.Text.ToLower() == " wf:sectioninsertnotlast\a") {

                nodecol.Add(e.MatchNode.ParentNode);
                return Aspose.Words.ReplaceAction.Replace;
            }

            return Aspose.Words.ReplaceAction.Replace;
        }
    }

    public class ReplaceBold : IReplacingCallback
    {

        public Collection<CompositeNode> nodecol = new Collection<CompositeNode>();
        ReplaceAction IReplacingCallback.Replacing(ReplacingArgs e)
        {

            if (e.MatchNode.ParentNode.Range.Text.ToLower() == ControlChar.NonBreakingSpaceChar.ToString() + "wf:Bold\f" ||
                  e.MatchNode.ParentNode.Range.Text.ToLower() == ControlChar.NonBreakingSpaceChar.ToString() + "wf:Bold\r" ||
                  e.MatchNode.ParentNode.Range.Text.ToLower() == "wf:Bold\r" || e.MatchNode.ParentNode.Range.Text.ToLower() == "wf:Bold\f" ||
                  e.MatchNode.ParentNode.Range.Text.ToLower() == "wf:Bold\a")
            {

                nodecol.Add(e.MatchNode.ParentNode);
                return Aspose.Words.ReplaceAction.Replace;
            }

            if (e.MatchNode.ParentNode.Range.Text.ToLower() == ControlChar.NonBreakingSpaceChar.ToString() + "wf:Bold \f" ||
                    e.MatchNode.ParentNode.Range.Text.ToLower() == ControlChar.NonBreakingSpaceChar.ToString() + "wf:Bold \r" ||
                    e.MatchNode.ParentNode.Range.Text.ToLower() == "wf:Bold \r" || e.MatchNode.ParentNode.Range.Text.ToLower() == "wf:Bold \f" ||
                    e.MatchNode.ParentNode.Range.Text.ToLower() == "wf:Bold \a")
            {

                nodecol.Add(e.MatchNode.ParentNode);
                return Aspose.Words.ReplaceAction.Replace;
            }

            if (e.MatchNode.ParentNode.Range.Text.ToLower() == ControlChar.NonBreakingSpaceChar.ToString() + " wf:Bold\f" ||
                    e.MatchNode.ParentNode.Range.Text.ToLower() == ControlChar.NonBreakingSpaceChar.ToString() + " wf:Bold\r" ||
                    e.MatchNode.ParentNode.Range.Text.ToLower() == " wf:Bold\r" || e.MatchNode.ParentNode.Range.Text.ToLower() == " wf:Bold\f" ||
                    e.MatchNode.ParentNode.Range.Text.ToLower() == " wf:Bold\a")
            {

                nodecol.Add(e.MatchNode.ParentNode);
                return Aspose.Words.ReplaceAction.Replace;
            }

            return Aspose.Words.ReplaceAction.Replace;
        }
    }
}

