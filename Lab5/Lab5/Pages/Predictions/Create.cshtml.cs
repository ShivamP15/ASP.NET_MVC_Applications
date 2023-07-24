using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Lab5.Data;
using Lab5.Models;
using Azure.Storage.Blobs;
using Azure;

namespace Lab5.Pages.Predictions
{
    public class CreateModel : PageModel
    {
        private readonly Lab5.Data.PredictionDataContext _context;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string earthContainerName = "earthimages";
        private readonly string computerContainerName = "computerimages";

        public CreateModel(Lab5.Data.PredictionDataContext context, BlobServiceClient blobServiceClient)
        {
            _context = context;
            _blobServiceClient = blobServiceClient;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Prediction Prediction { get; set; }

        private string containerChoice;

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync(IFormFile file)
        {

            if (file == null)
            {
                return RedirectToPage("./Index");
            }

            BlobContainerClient containerClient;

            var containerName = Request.Form["Question"];
            containerName = containerName == "Computer" ? computerContainerName : earthContainerName;

            try
            {
                containerClient = await _blobServiceClient.CreateBlobContainerAsync(containerName);
                containerClient.SetAccessPolicy(Azure.Storage.Blobs.Models.PublicAccessType.BlobContainer);
            }
            catch (RequestFailedException)
            {
                containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            }

            try
            {
                string randomFileName = Path.GetRandomFileName();

                var blockBlob = containerClient.GetBlobClient(randomFileName);

                if (await blockBlob.ExistsAsync())
                {
                    await blockBlob.DeleteAsync();
                }

                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    memoryStream.Position = 0;
                    await blockBlob.UploadAsync(memoryStream);
                    memoryStream.Close();
                }

                var PredictedImage = new Prediction
                {
                    Url = blockBlob.Uri.AbsoluteUri,
                    FileName = randomFileName,
                    Question = containerName.Equals(computerContainerName) ? Question.Computer: Question.Earth
                };
                _context.Predictions.Add(PredictedImage);
                _context.SaveChanges();
            }
            catch (RequestFailedException)
            {
                return RedirectToPage("Error");
            }

            return RedirectToPage("./Index");
        }
    }
}
