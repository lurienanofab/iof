using System.Text;

namespace IOF
{
    public class Accordion
    {
        public static int SelectedIndex = -1;

        public static string Script()
        {
            var sb = new StringBuilder();
            sb.AppendLine("<script>");
            sb.AppendLine("  $(document).ready(function(){");
            sb.AppendLine("    $('#accordion-nav').accordion({");
            sb.AppendLine("      active: " + (SelectedIndex < 0 ? "false" : SelectedIndex.ToString()) + ",");
            sb.AppendLine("      clearStyle: false,");
            sb.AppendLine("      collapsible: true");
            sb.AppendLine("    });");
            sb.AppendLine("    $('#accordion-nav h3:first').unbind('click');");
            sb.AppendLine("  });");
            sb.AppendLine("</script>");
            return sb.ToString();
        }
    }
}
