using System;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using System.Text;

namespace AIDMS.Repositories
{
    public class PDFManagementRepository : IPDFManagementRepository
    {
        public async Task<string> ReadPDFContent(IFormFile file)
        {
            StringBuilder text = new StringBuilder();

            using (var stream = file.OpenReadStream())
            using (PdfDocument document = PdfDocument.Open(stream))
            {
                foreach (Page page in document.GetPages())
                {
                    text.Append(page.Text);
                }
            }

            return text.ToString();
        }
    }
}
