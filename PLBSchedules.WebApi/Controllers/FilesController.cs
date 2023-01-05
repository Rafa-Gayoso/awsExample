using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Mvc;

namespace PLBSchedules.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FilesController : ControllerBase
{
    private readonly IAmazonS3 _amazonS3;

    public FilesController(IAmazonS3 amazonS3)
    {
        _amazonS3 = amazonS3;
    }

     [HttpPost("/uploadFile")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            //la equest lleva el nombre del bucket, el tipo de contenido del fihcero, el stram y la key que es el indentificador que le das en el bucket
            //cada archivo debe terner un identificador unico. yo uso el nombre del fichero como Key
            var putRequest = new PutObjectRequest
            {
                BucketName = "myawstestbicket",
                ContentType = file.ContentType,
                InputStream = file.OpenReadStream(),
                Key = file.FileName
            };
            //aqui subes el objeto al bucket
            var putResponse = await _amazonS3.PutObjectAsync(putRequest);
            
            //aqui pides los obejtos que estan en el bucket
            var s3Response = await _amazonS3.ListObjectsAsync("myawstestbicket");
    
            StringWriter objects = new StringWriter();
            s3Response.S3Objects.ForEach(s3 => objects.WriteLine(s3.Key));
            
            return Ok(objects.ToString());
        }
        
        [HttpGet("/getFile/{fileName}")]
        public async Task<IActionResult> GetFile(string fileName)
        {
            
            var request = new GetObjectRequest
            {
                BucketName = "myawstestbicket",
                Key = fileName
            };
    
            //aqui pides el objeto especifico
    
            var response = await _amazonS3.GetObjectAsync(request);
    
            using var reader = new StreamReader(response.ResponseStream);
    
            var fileContent = await reader.ReadToEndAsync();
    
            var file = File(response.ResponseStream, response.Headers.ContentType, response.Key);
            
            
            return Ok(fileContent);
            
        }
        
        [HttpDelete("/deleteFile/{fileName}")]
        public async Task<IActionResult> DeleteFile(string fileName)
        {
            
            var deleteRequest = new DeleteObjectRequest()
            {
                BucketName = "myawstestbicket",
                Key = fileName
            };
            var objectDeleted = await _amazonS3.DeleteObjectAsync(deleteRequest);
            //aqui subes el objeto al bucket
           
            
            return Ok(objectDeleted.ToString());
        }
}