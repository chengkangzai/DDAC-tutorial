﻿using Amazon;
using Amazon.Internal;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace NewNewTry.Controllers
{
    public class UploadFilesController : Controller
    {
        //First Function: User creating form for adding the image to s3

        private const string bucketName = "mvcflowershop-lab-cck";

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadImage(List<IFormFile> images)
        {
            List<string> keylists = getAWSCredentialInfo();

            //2. setup

            var s3Client = new AmazonS3Client(keylists[0], keylists[1], keylists[2], RegionEndpoint.USEast1);

            var fileName = "";

            foreach (var image in images)
            {
                //3.1 input file validation
                if (image.Length <= 0)
                {
                    return BadRequest(image.FileName + " is empty! not allow to upload!");
                }

                if (image.Length > 1048576)
                {
                    return BadRequest(image.FileName + " more than 1MB not allow to upload");
                }

                if (image.ContentType.ToLower() != "image/png" &&
                    image.ContentType.ToLower() != "image/jpg" &&
                    image.ContentType.ToLower() != "image/gif")
                {
                    return BadRequest(image.FileName + " is not valid image for uploading");
                }

                try
                {
                    //create upload request for s3
                    var request = new PutObjectRequest
                    {
                        InputStream = image.OpenReadStream(),//source file
                        BucketName = bucketName + "/images",// bucket name and path for folder
                        Key = image.FileName,
                        CannedACL = S3CannedACL.PublicRead// public can read it
                    };
                    await s3Client.PutObjectAsync(request);
                    fileName = fileName + " " + image.FileName;
                }
                catch (AmazonS3Exception e)
                {
                    return BadRequest("error file of " + image.FileName + " : " + e.Message);
                }
                catch (Exception e)
                {
                    return BadRequest("error file of " + image.FileName + " : " + e.Message);
                }
            }

            return RedirectToAction("Index", "UploadFiles", new { msg = "Images of " + fileName + "already uploaded to S3" });
        }

        private List<string> getAWSCredentialInfo()
        {
            //1 set up the appsetting.json file path 
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            IConfiguration configuration = builder.Build();

            //2 read the key info from the json using configure instance 
            var keyList = new List<string>();
            keyList.Add(configuration["AWSCredential:key1"]);// access key
            keyList.Add(configuration["AWSCredential:key2"]);// session key
            keyList.Add(configuration["AWSCredential:key3"]);// token key

            return keyList;
        }

        public async Task<IActionResult> displayAsGallery(string msg = "")
        {
            var keylists = getAWSCredentialInfo();

            //2. setup

            var s3Client = new AmazonS3Client(keylists[0], keylists[1], keylists[2], RegionEndpoint.USEast1);

            var imageList = new List<S3Object>();

            //steps to get all object list from s3

            try
            {
                string token;

                do
                {
                    var request = new ListObjectsRequest()
                    {
                        BucketName = bucketName
                    };

                    //return image back from S3 to application
                    var response = await s3Client.ListObjectsAsync(request).ConfigureAwait(false);
                    imageList.AddRange(response.S3Objects);
                    token = response.NextMarker;
                } while (token != null);

                ViewBag.PreSignedURLList = GetPreSignedUrl(imageList, s3Client);
            }
            catch (AmazonS3Exception e)
            {
                return BadRequest("Error in displaying the S3 objects" + e.Message);
            }
            catch (Exception e)
            {
                return BadRequest("Error in displaying the S3 objects" + e.Message);
            }

            ViewBag.msg = msg;
            return View(imageList);
        }

        public List<string> GetPreSignedUrl(List<S3Object> imageListFromS3, AmazonS3Client amazonS3Client)
        {
            var preSignedUrl = new List<string>();
            foreach (var image in imageListFromS3)
            {
                var request = new GetPreSignedUrlRequest()
                {
                    BucketName = image.BucketName,
                    Key = image.Key,
                    Expires = DateTime.Now.AddMinutes(1)
                };
                preSignedUrl.Add(amazonS3Client.GetPreSignedURL(request));
            }
            return preSignedUrl;
        }

        public async Task<IActionResult> DeleteImage(string filename)
        {
            List<string> keylists = getAWSCredentialInfo();
            var s3Client = new AmazonS3Client(keylists[0], keylists[1], keylists[2], RegionEndpoint.USEast1);

            try
            {
                if (string.IsNullOrEmpty(filename))
                {
                    return RedirectToAction("displayAsGallery", "UploadFiles", new { msg = filename + "is not valid" });
                }

                var deleteObjectRequest = new DeleteObjectRequest()
                {
                    BucketName = bucketName,
                    Key = filename,
                };
                await s3Client.DeleteObjectAsync(deleteObjectRequest);
            }
            catch (AmazonS3Exception e)
            {
                return BadRequest("Error in deleting the S3 objects" + e.Message);
            }
            catch (Exception e)
            {
                return BadRequest("Error in deleting the S3 objects" + e.Message);
            }

            return RedirectToAction("displayAsGallery", "UploadFiles", new { msg = filename + "file deleted" });
        }

        public async Task<IActionResult> DownloadImage(string filename)
        {
            var keylists = getAWSCredentialInfo();
            var s3Client = new AmazonS3Client(keylists[0], keylists[1], keylists[2], RegionEndpoint.USEast1);

            try
            {
                if (string.IsNullOrEmpty(filename))
                {
                    return RedirectToAction("displayAsGallery", "UploadFiles", new { msg = filename + "is not valid" });
                }

                var folderName = Path.GetDirectoryName(filename);
                filename = Path.GetFileName(filename);
                folderName = !string.IsNullOrEmpty(filename) ? bucketName + "/" + folderName : bucketName;

                var downloadPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\" + filename;

                var transferUtility = new TransferUtility(s3Client);

                await transferUtility.DownloadAsync(downloadPath, folderName, filename);
                // var deleteObjectRequest = new GetObjectRequest()
                // {
                //     BucketName = bucketName,
                //     Key = filename,
                // };
                // var s3Object = await s3Client.GetObjectAsync(deleteObjectRequest);
                // //stream the s3 object to the response
                // return File(s3Object.ResponseStream, s3Object.Headers.ContentType);
                //stream the s3 object to the response

                // var request = new GetObjectRequest()
                // {
                //     BucketName = bucketName,
                //     Key = filename,
                // };
                // using (var response = await s3Client.GetObjectAsync(request))
                // {
                //     //stream the s3 object to the response
                //     using (var responseStream = response.ResponseStream)
                //     {
                //         var stream = new MemoryStream();
                //         await responseStream.CopyToAsync(stream);
                //         stream.Position = 0;
                //         return stream;
                //     }
                // }
            }
            catch (AmazonS3Exception e)
            {
                return BadRequest("Error in downloading the S3 objects" + e.Message);
            }
            catch (Exception e)
            {
                return BadRequest("Error in downloading the S3 objects" + e.Message);
            }

            return RedirectToAction("displayAsGallery", "UploadFiles", new { msg = filename + "is downloading from the S3" });
        }

        public async Task<Stream> GetStream(string filename)
        {
            var keylists = getAWSCredentialInfo();
            var s3Client = new AmazonS3Client(keylists[0], keylists[1], keylists[2], RegionEndpoint.USEast1);

            try
            {
                var request = new GetObjectRequest()
                {
                    BucketName = bucketName,
                    Key = filename,
                };
                using (var response = await s3Client.GetObjectAsync(request))
                {
                    //stream the s3 object to the response
                    using (var responseStream = response.ResponseStream)
                    {
                        var stream = new MemoryStream();
                        await responseStream.CopyToAsync(stream);
                        stream.Position = 0;
                        return stream;
                    }
                }
            }
            catch (AmazonS3Exception e)
            {
                throw new Exception("Error in downloading the S3 objects" + e.Message);
            }
            catch (Exception e)
            {
                throw new Exception("Error in downloading the S3 objects" + e.Message);
            }
        }

        public async Task<IActionResult> SecondDownloadImage(string filename)
        {
            var imageStream = await GetStream(filename);
            var imageName = Path.GetFileName(filename);

            Response.Headers.Add("Content-Disposition", "attachment; filename=" + imageName);
            return File(imageStream, "image/jpeg");
        }
    }
}
