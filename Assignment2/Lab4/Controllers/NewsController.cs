using Azure;
using Azure.Storage.Blobs;
using Lab4.Data;
using Lab4.Models;
using Lab4.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lab4.Controllers
{
    public class NewsController : Controller
    {
        private readonly SportsDbContext _context;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string containerName = "news";

        public NewsController(SportsDbContext context, BlobServiceClient blobServiceClient)
        {
            _context = context;
            _blobServiceClient = blobServiceClient;
        }

        // GET: News
        public async Task<IActionResult> Index(string id)
        {
            var viewModel = new NewsViewModel()
            {
                SportClub = await _context.SportClubs.FindAsync(id),
                News = await _context.News.Where(s => s.SportClubId == id).ToListAsync()
            };
            return View(viewModel);
        }

        // GET: News/Create
        public async Task<IActionResult> Create(string id)
        {
            SportClub viewModel = await _context.SportClubs.FindAsync(id);
            return View(viewModel);
        }

        // Upload Images on News
        public async Task<IActionResult> Upload(string SportClubId, IFormFile file)
        {
            if (file == null)
            {
                return View("Error");
            }
            BlobContainerClient containerClient;

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

                var img = new News() { };
                img.Url = blockBlob.Uri.AbsoluteUri;
                img.FileName = randomFileName;
                img.SportClubId = SportClubId;

                _context.News.Add(img);
                _context.SaveChanges();
            }
            catch (RequestFailedException)
            {
                View("Error");
            }

            return RedirectToAction("Index", new { id = SportClubId });
        }
        // POST: News/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        /*[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SportClubId,FileName,Url")] News news)
        {
            if (ModelState.IsValid)
            {
                _context.Add(news);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(news);
        }*/

        // GET: News/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) 
            { 
                return NotFound(); 
            }

            var news = await _context.News.Where(s => s.Id == id).ToListAsync();
            var sportclub = await _context.SportClubs
                .FirstOrDefaultAsync(s => s.Id == news.FirstOrDefault().SportClubId);
            if (news == null || sportclub == null) 
            { 
                return NotFound(); 
            }

            var viewModel = new NewsViewModel()
            {
                News = news,
                SportClub = sportclub
            };

            return View(viewModel);
            /*if (id == null || _context.News == null)
            {
                return NotFound();
            }

            var news = await _context.News
                .FirstOrDefaultAsync(m => m.Id == id);
            if (news == null)
            {
                return NotFound();
            }

            return View(news);*/
        }

        // POST: News/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var image = await _context.News.FindAsync(id);
            string SportClubId = _context.SportClubs
                .Where(m => m.Id == image.SportClubId).First().Id;

            BlobContainerClient containerClient;

            try
            {
                containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            }
            catch (RequestFailedException)
            {
                return View("Error");
            }

            try
            {
                var blockBlob = containerClient.GetBlobClient(image.FileName);
                if (await blockBlob.ExistsAsync())
                {
                    await blockBlob.DeleteAsync();
                }

                _context.News.Remove(image);
                await _context.SaveChangesAsync();

            }
            catch (RequestFailedException)
            {
                return View("Error");
            }

            return RedirectToAction("Index", new { id = SportClubId });
            /*if (_context.News == null)
            {
                return Problem("Entity set 'SportsDbContext.News'  is null.");
            }
            var news = await _context.News.FindAsync(id);
            if (news != null)
            {
                _context.News.Remove(news);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));*/
        }

        /*private bool NewsExists(int id)
        {
            return _context.News.Any(e => e.Id == id);
        }*/
    }
}
