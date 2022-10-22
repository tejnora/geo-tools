using System;
using System.Collections.Generic;
using System.Windows.Documents;

namespace GeoBase.Extensions
{
    public static class FlowDocummentExtensions
    {
        public static string[] GetParagraphs(this FlowDocument document)
        {
            List<string> lines = new List<string>();
            foreach (TextElement el in GetRunsAndParagraphs(document))
            {
                if(el is Run)
                    lines.Add(((Run)el).Text);
                else if(el is Paragraph)
                    continue;
                else
                {
                    throw new ArgumentException();
                }
            }
            return lines.ToArray();
        }
        private static IEnumerable<TextElement> GetRunsAndParagraphs(FlowDocument doc)
        {
            // use the GetNextContextPosition method to iterate through the  
            // FlowDocument  
            for (TextPointer position = doc.ContentStart;
                position != null && position.CompareTo(doc.ContentEnd) <= 0;
                position = position.GetNextContextPosition(LogicalDirection.Forward))
            {
                if (position.GetPointerContext(LogicalDirection.Forward) ==
                    TextPointerContext.ElementEnd)
                {
                    // return solely the Runs and Paragraphs. all other elements are   
                    // ignored since they aren't supported by FormattedText.  
                    Run run = position.Parent as Run;
                    if (run != null)
                    {
                        yield return run;
                    }
                    else
                    {
                        Paragraph para = position.Parent as Paragraph;
                        if (para != null)
                        {
                            yield return para;
                        }
                    }
                }
            }
        }
    }
}
