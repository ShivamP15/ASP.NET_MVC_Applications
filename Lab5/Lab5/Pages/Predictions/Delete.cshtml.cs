using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Lab5.Data;
using Lab5.Models;
using Azure.Storage.Blobs;
using Azure;

namespace Lab5.Pages.Predictions
{
    public class DeleteModel : PageModel
    {
        private readonly Lab5.Data.PredictionDataContext _context;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string earthContainerName = "earthimages";
        private readonly string computerContainerName = "computerimages";

        public DeleteModel(Lab5.Data.PredictionDataContext context, BlobServiceClient blobServiceClient)
        {
            _context = context;
            _blobServiceClient = blobServiceClient;
        }

        [BindProperty]
      public Prediction Prediction { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Predictions == null)
            {
                return NotFound();
            }

            var prediction = await _context.Predictions.FirstOrDefaultAsync(m => m.PredictionId == id);

            if (prediction == null)
            {
                return NotFound();
            }
            else 
            {
                Prediction = prediction;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            BlobContainerClient ContainerClient;

            try
            {
                var containerName = Prediction.Question.Equals(Question.Earth) ? earthContainerName : computerContainerName;
                ContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            }
            catch (RequestFailedException)
            {
                return RedirectToPage("Error");
            }

            try
            {
                var blockBlob = ContainerClient.GetBlobClient(Prediction.FileName);

                if (await blockBlob.ExistsAsync())
                {
                    await blockBlob.DeleteAsync();
                }
            }
            catch (RequestFailedException)
            {
                return RedirectToPage("Error");
            }

            Prediction = await _context.Predictions.FindAsync(id);

            if (Prediction != null)
            {
                _context.Predictions.Remove(Prediction);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage("./Index");
        }
    }
}
