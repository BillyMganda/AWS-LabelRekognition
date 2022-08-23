using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.Rekognition;
using Amazon.Rekognition.Model;


namespace ImageRecognition
{
#pragma warning disable CA1416

    public class RekognitionHelper
    {
        private string _bucketname { get; set; }
        private RegionEndpoint _region { get; set; }
        private AmazonRekognitionClient _rekognitionClient { get; set; }
        private AmazonS3Client _s3Client { get; set; }

        public RekognitionHelper(string bucketname, RegionEndpoint region)
        {
            _bucketname = bucketname;
            _region = region;
            _rekognitionClient = new AmazonRekognitionClient(_region);
            _s3Client = new AmazonS3Client(_region);
        }

        /// <summary>
        /// Upload local file to S3 bucket.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns>Amazon Rekognition Image object.</returns>
        private async Task<Amazon.Rekognition.Model.Image> UploadFileToBucket(string filename)
        {
            Console.WriteLine($"Upload {filename} to bucket {_bucketname}");
            var putRequest = new PutObjectRequest
            {
                BucketName = _bucketname,
                FilePath = filename,
                Key = Path.GetFileName(filename)
            };
            await _s3Client.PutObjectAsync(putRequest);

            Console.WriteLine("image upload starting............");

            return new Amazon.Rekognition.Model.Image
            {
                S3Object = new Amazon.Rekognition.Model.S3Object
                {
                    Bucket = _bucketname,
                    Name = putRequest.Key
                }

            };

        }

        /// <summary>
        /// Detect labels.
        /// </summary>
        /// <param name="filename"></param>

        public async Task DetectLabels(string filename)
        {
            Console.WriteLine("image upload was successful............");

            var image = await UploadFileToBucket(filename);

            DetectLabelsRequest detectlabelsRequest = new DetectLabelsRequest()
            {
                Image = image,
                MaxLabels = 10,
                MinConfidence = 75F
            };

            var detectLabelsResponse = await _rekognitionClient.DetectLabelsAsync(detectlabelsRequest);
            Console.WriteLine("Detected labels for " + filename);
            foreach (var label in detectLabelsResponse.Labels)
                Console.WriteLine($"{label.Name}, {label.Confidence}");

            await DeleteFileFromBucket(filename);
        }

        /// <summary>
        /// Detect moderation labels.
        /// </summary>
        /// <param name="filename"></param>

        //public async Task DetectModerationLabels(string filename)
        //{
        //    var image = await UploadFileToBucket(filename);

        //    var detectModerationLabelsRequest = new DetectModerationLabelsRequest()
        //    {
        //        Image = image,
        //        MinConfidence = 75F
        //    };

        //    var detectModerationLabelsResponse = await _rekognitionClient.DetectModerationLabelsAsync(detectModerationLabelsRequest);
        //    Console.WriteLine("Detected labels for " + filename);
        //    foreach (var label in detectModerationLabelsResponse.ModerationLabels)
        //    {
        //        Console.WriteLine($"{label.Name}, {label.Confidence}");
        //    }

        //    await DeleteFileFromBucket(filename);
        //}

        /// <summary>
        /// Recognize celebrities.
        /// </summary>
        /// <param name="filename"></param>

        //public async Task RecognizeCelebrities(string filename)
        //{
        //    var image = await UploadFileToBucket(filename);

        //    var recognizeCelebritiesRequest = new RecognizeCelebritiesRequest()
        //    {
        //        Image = image
        //    };

        //    var recognizeCelebritiesResponse = await _rekognitionClient.RecognizeCelebritiesAsync(recognizeCelebritiesRequest);
        //    Console.WriteLine("Detected celebrities for " + filename);
        //    foreach (var celebrity in recognizeCelebritiesResponse.CelebrityFaces)
        //    {
        //        Console.WriteLine($"{celebrity.Name}, {celebrity.MatchConfidence}");
        //    }

        //    await DeleteFileFromBucket(filename);
        //}

   
        

        /// <summary>
        /// Delete file from S3 bucket.
        /// </summary>
        /// <param name="filename"></param>
        private async Task DeleteFileFromBucket(string filename)
        {
            Console.WriteLine($"Delete {filename} from bucket {_bucketname}");
            var deleteRequest = new DeleteObjectRequest
            {
                BucketName = _bucketname,
                Key = Path.GetFileName(filename)
            };
            await _s3Client.DeleteObjectAsync(deleteRequest);
        }
    }
}

