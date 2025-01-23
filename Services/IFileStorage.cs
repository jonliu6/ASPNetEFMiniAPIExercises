namespace MinimalAPIsWithASPNetEF.Services
{
    public interface IFileStorage
    {
        public Task<string> Store(string container, IFormFile file);
        /// <summary>
        /// Delete a file
        /// </summary>
        /// <param name="route"></param> where the file is stored
        /// <param name="container"></param>
        /// <returns></returns>
        Task Delete(string? route, string container);

        async Task<string> Edit(string? route, string container, IFormFile file)
        {
            await Delete(route, container); // delete the old file
            return await Store(container, file); // replace with the new file
        }
    }
}
