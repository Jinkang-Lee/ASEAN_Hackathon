using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using Amazon.Lambda.Core;
using Amazon.Rekognition;
using Amazon.Rekognition.Model;


// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace AWSLambda_RekogTest1
{
    public class Function
    {
        
        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        /// 



        public async Task<int> FunctionHandler(string input, ILambdaContext context)
        {
            //AWS rekog client
            var rekognitionClient = new AmazonRekognitionClient();

            //using await keyword because its an async method and save result to local var
            var detectResponse = await rekognitionClient.DetectLabelsAsync(
                new DetectLabelsRequest
                {
                    //new image
                    Image = new Image
                    {
                        //S3Object class will point the system to the bucket we want to use
                        S3Object = new S3Object
                        {
                            Bucket = "rekog-test-bucket1",
                            Name = input
                        }
                    }
                }) ;

            int count = 0;
            int failed = 1000;

            //foreach to scan through all he labels
            foreach (var label in detectResponse.Labels)
            {
                if(label.Confidence > 90)
                {
                    if(label.Name == "Person")
                    {
                        count = label.Instances.Count;
                        return count;

                        //Ask help for this part
                        //Patient Database cannot be updated
                        //string sql = @"UPDATE Patient SET visitorsIN = '{0}' WHERE ward_num = 2 AND bed_num = 7";
                        //string insert = String.Format(sql, count);
                        //int count1 = DBUtl.ExecSQL(insert);

                        //if (count1 == 1)
                        //{
                        //    return count;
                        //}
                    }
                    
                }
            }

            return failed;

            
        }
    }
}
