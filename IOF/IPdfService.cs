namespace IOF
{
    public interface IPdfService
    {
        /// <summary>
        /// Creates a pdf file for a purchase order and returns the full file path.
        /// </summary>
        string CreatePDF(int poid);
    }
}
