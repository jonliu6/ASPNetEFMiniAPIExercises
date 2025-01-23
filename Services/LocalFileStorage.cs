
namespace MinimalAPIsWithASPNetEF.Services
{
    /// <summary>
    /// need to register this service in Program.cs - Service section
    /// </summary>
    /// <param name="env"></param>
    /// <param name="ctx"></param>
    public class LocalFileStorage(IWebHostEnvironment env, IHttpContextAccessor ctx) : IFileStorage
    {
        public Task Delete(string? route, string container)
        {
            if (string.IsNullOrEmpty(route))
            {
                return Task.CompletedTask;
            }

            var newFilename = Path.GetFileName(route);
            var fileDirectory = Path.Combine(env.WebRootPath, container, newFilename);
            if (File.Exists(fileDirectory))
            {
                File.Delete(fileDirectory);
            }

            return Task.CompletedTask;
        }

        public async Task<string> Store(string container, IFormFile file)
        {
            var extName = Path.GetExtension(file.FileName);
            var newFileName = $"{Guid.NewGuid()}{extName}";
            string folder = Path.Combine(env.WebRootPath, container);

            if (!File.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            string route = Path.Combine(folder, newFileName);
            using (var ms = new MemoryStream())
            {
                await file.CopyToAsync(ms);
                var content = ms.ToArray();
                await File.WriteAllBytesAsync(route, content);
            }

            var schema = ctx.HttpContext!.Request.Scheme;
            var host = ctx.HttpContext!.Request.Host;
            var url = $"{schema}://{host}";
            var urlFile = Path.Combine(url, container, newFileName).Replace("\\", "/");

            return urlFile;

        }
    }
}
