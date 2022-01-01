using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlobSampleApp.Services
{
	public class BlobService : IBlobService
	{
		private readonly BlobServiceClient _blobServiceClient;

		public BlobService(BlobServiceClient blobServiceClient)
		{
			_blobServiceClient = blobServiceClient;
		}

		public async Task<IEnumerable<string>> AllBlobs(string containerName)
		{
			// allow us to access the data inside the container
			var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

			var files = new List<string>();

			var blobs = containerClient.GetBlobsAsync();

			await foreach(var item in blobs)
			{
				files.Add(item.Name);
			}

			return files;
		}

		public async Task<bool> DeleteBlob(string name, string containerName)
		{
			// this will allow us access to the storage container
			var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

			// this will allow us access to the file inside the container via the file name
			var blobClient = containerClient.GetBlobClient(name);

			return await blobClient.DeleteIfExistsAsync();
		}

		public Task<string> GetBlob(string name, string containerName)
		{
			// this will allow us access to the storage container
			var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

			// this will allow us access to the file inside the container via the file name
			var blobClient = containerClient.GetBlobClient(name);

			return Task.FromResult(blobClient.Uri.AbsoluteUri);
		}

		public async Task<bool> UploadBlob(string name, IFormFile file, string containerName)
		{
			// this will allow us access to the storage container
			var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

			// checking if the file exist 
			// if the file exist it will be replaced
			// if it doesn't exist it will create a temp space until its uploaded
			var blobClient = containerClient.GetBlobClient(name);

			var httpHeaders = new BlobHttpHeaders()
			{
				ContentType = file.ContentType
			};

			var res = await blobClient.UploadAsync(file.OpenReadStream(), httpHeaders);

			if(res != null)
			{
				return true;
			}

			return false;
		}
	}
}
