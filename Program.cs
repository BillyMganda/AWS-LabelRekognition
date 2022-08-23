using Amazon;
using Amazon.Rekognition.Model;
using ImageRecognition;

const string bucketname = @"az-rekognition-lambda-bucket";
var region = RegionEndpoint.USEast1;

if (args.Length == 2)
{
    var filename = args[0];
    var analysisType = (args.Length > 1) ? args[1] : "text";

    try
    {
        RekognitionHelper helper = new RekognitionHelper(bucketname, region);       

        switch (analysisType)
        {           
            case "labels":                
                await helper.DetectLabels(filename);
                Environment.Exit(1);
                break;
            //case "moderate":
            //    await helper.DetectModerationLabels(filename);
            //    Environment.Exit(1);
            //    break;
            //case "celebrity":
            //    await helper.RecognizeCelebrities(filename);
            //    Environment.Exit(1);
            //    break;            
        }
    }
    catch (AccessDeniedException e)
    {
        Console.WriteLine($"EXCEPTION {e.Message}");
        Console.WriteLine("There was an exception............");
    }
}

Console.WriteLine("Invalid parameter - command line format: dotnet run -- <file> labels|moderate|celebrity|faces");
