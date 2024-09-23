using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ML.Data;
using Microsoft.ML;
using static Microsoft.ML.DataOperationsCatalog;
using webapi.Models.PredictModels;
using webapi.Services.PredictionServices;
using Microsoft.EntityFrameworkCore;


namespace webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PredictController : ControllerBase
    {

        [HttpPost("PredictSingleItem", Name = "PredictSingleItem")]
        public async Task<IActionResult> PredictSentiment([FromBody] SentimentData sample)
        {
            //var _dataPath = Path.Combine(Environment.CurrentDirectory, "Assets", "yelp_labelled.txt");
            SentimentPrediction resultPrediction;
            MLContext mlContext = new MLContext();

            SentimentPredictionService clsSentimentPrediction = new SentimentPredictionService(mlContext);
            ITransformer model;
            if (!clsSentimentPrediction.isModelLoaded)
            {
                TrainTestData splitDataView = clsSentimentPrediction.LoadData(mlContext);
                model = clsSentimentPrediction.BuildAndTrainModel(mlContext, splitDataView.TrainSet);
                clsSentimentPrediction.Evaluate(mlContext, model, splitDataView.TestSet);
            }
            else
            {
                model = clsSentimentPrediction.LoadModel(mlContext);
            }

            //TrainTestData LoadData(MLContext mlContext)
            //{

            //    IDataView dataView = mlContext.Data.LoadFromTextFile<SentimentData>(_dataPath, hasHeader: true);
            //    TrainTestData splitDataView = mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2);
            //    return splitDataView;
            //}


            //ITransformer BuildAndTrainModel(MLContext mLContext, IDataView splitTrainSet)
            //{
            //    var estimator = mLContext.Transforms.Text.FeaturizeText(outputColumnName: "Features", inputColumnName: nameof(SentimentData.SentimentText))
            //        .Append(mLContext.BinaryClassification.Trainers.SdcaLogisticRegression(labelColumnName: "Label", featureColumnName: "Features"));
            //    var model = estimator.Fit(splitTrainSet);

            //    return model;
            //}

            

            //void Evaluate(MLContext mLContext, ITransformer model, IDataView splitTestSet)
            //{
            //    IDataView predictions = model.Transform(splitTestSet);
            //    CalibratedBinaryClassificationMetrics metrics = mLContext.BinaryClassification.Evaluate(predictions, "Label");
            //    Console.WriteLine();
            //    Console.WriteLine("Model quality metrics evaluation");
            //    Console.WriteLine("--------------------------------");
            //    Console.WriteLine($"Accuracy: {metrics.Accuracy:P2}");
            //    Console.WriteLine($"Auc: {metrics.AreaUnderRocCurve:P2}");
            //    Console.WriteLine($"F1Score: {metrics.F1Score:P2}");
            //    Console.WriteLine("=============== End of model evaluation ===============");
            //}



            resultPrediction = clsSentimentPrediction.UseModelWithSingleItem(mlContext, model, sample);

            //void UseModelWithSingleItem(MLContext mLContext, ITransformer model)
            //{
            //    PredictionEngine<SentimentData, SentimentPrediction> predictionFunction = mLContext.Model.CreatePredictionEngine<SentimentData, SentimentPrediction>(model);
            //    SentimentData sampleStatement = new SentimentData { SentimentText = "This was a very bad steak" };
            //    resultPrediction = predictionFunction.Predict(sampleStatement);
            //    Console.WriteLine();
            //    Console.WriteLine("=============== Prediction Test of model with a single sample and test dataset ===============");

            //    Console.WriteLine();
            //    Console.WriteLine($"Sentiment: {resultPrediction.SentimentText} | Prediction: {(Convert.ToBoolean(resultPrediction.Prediction) ? "Positive" : "Negative")} | Probability: {resultPrediction.Probability} ");

            //    Console.WriteLine("=============== End of Predictions ===============");
            //    Console.WriteLine();
            //}


            //UseModelWithSingleItem(mLContext, model);
            return Ok(resultPrediction);
        }


        [HttpPost("PredictBatch", Name ="PredictBatch")]
        public async Task<IActionResult> PredictBatch([FromBody] IEnumerable<SentimentData> sample)
        {
            //File path
            var _dataPath = Path.Combine(Environment.CurrentDirectory, "Assets", "yelp_labelled.txt");
            IEnumerable<SentimentPrediction> predictedResults;
            //Create mlContext
            MLContext mLContext = new MLContext();
            SentimentPredictionService clsSentimentPrediction = new SentimentPredictionService(mLContext);
            ITransformer model;

            //Get Split Data View for testing

            //TrainTestData LoadData(MLContext mlContext)
            //{

            //    IDataView dataView = mlContext.Data.LoadFromTextFile<SentimentData>(_dataPath, hasHeader: true);
            //    TrainTestData splitDataView = mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2);
            //    return splitDataView;
            //}

            if (!clsSentimentPrediction.isModelLoaded)
            {
                TrainTestData splitDataViewBatch = clsSentimentPrediction.LoadData(mLContext);
                model = clsSentimentPrediction.BuildAndTrainModel(mLContext, splitDataViewBatch.TrainSet);
                clsSentimentPrediction.Evaluate(mLContext, model, splitDataViewBatch.TestSet);

            }
            else
            {
                model = clsSentimentPrediction.LoadModel(mLContext);
            }

            //TrainTestData splitDataView = LoadData(mLContext);


            //Build and Train Model
            //ITransformer BuildAndTrainModel(MLContext mLContext, IDataView splitTrainSet)
            //{
            //    var estimator = mLContext.Transforms.Text.FeaturizeText(outputColumnName: "Features", inputColumnName: nameof(SentimentData.SentimentText))
            //        .Append(mLContext.BinaryClassification.Trainers.SdcaLogisticRegression(labelColumnName: "Label", featureColumnName: "Features"));
            //    var model = estimator.Fit(splitTrainSet);

            //    return model;
            //}

            //ITransformer model = BuildAndTrainModel(mLContext, splitDataView.TrainSet);

            //void Evaluate(MLContext mLContext, ITransformer model, IDataView splitTestSet)
            //{
            //    IDataView predictions = model.Transform(splitTestSet);
            //    CalibratedBinaryClassificationMetrics metrics = mLContext.BinaryClassification.Evaluate(predictions, "Label");
            //    Console.WriteLine();
            //    Console.WriteLine("Model quality metrics evaluation");
            //    Console.WriteLine("--------------------------------");
            //    Console.WriteLine($"Accuracy: {metrics.Accuracy:P2}");
            //    Console.WriteLine($"Auc: {metrics.AreaUnderRocCurve:P2}");
            //    Console.WriteLine($"F1Score: {metrics.F1Score:P2}");
            //    Console.WriteLine("=============== End of model evaluation ===============");
            //}


            //Evaluate(mLContext, model, splitDataView.TestSet);


            //void UseModelWithBatchItems(MLContext mLContext, ITransformer model)
            //{
            //    IEnumerable<SentimentData> sentiments = new[]
            //    {
            //        new SentimentData
            //        {
            //            SentimentText = "This was a horrible meal"
            //        },
            //        new SentimentData
            //        {
            //            SentimentText = "I love this spaghetti"
            //        }
            //    };

            //    IDataView batchComments = mLContext.Data.LoadFromEnumerable(sentiments);

            //    IDataView predictions = model.Transform(batchComments);

            //    //Use the model to predict sentiment
            //    predictedResults = mLContext.Data.CreateEnumerable<SentimentPrediction>(predictions, reuseRowObject: false);

            //    Console.WriteLine();

            //    Console.WriteLine("=============== Prediction Test of loaded model with multiple samples ===============");

            //    foreach (SentimentPrediction prediction in predictedResults)
            //    {
            //        Console.WriteLine($"Sentiment: {prediction.SentimentText} | Prediction: {(Convert.ToBoolean(prediction.Prediction) ? "Positive" : "Negative")} | Probability: {prediction.Probability} ");
            //    }
            //    Console.WriteLine("=============== End of predictions ===============");
            //}

            predictedResults = clsSentimentPrediction.UseModelWithBatchItems(mLContext, model, sample);

            return Ok(predictedResults);
        }


        [HttpPost("PredictTaxiFare", Name ="PredictTaxiFare")]
        public async Task<IActionResult> PredictTaxiFare([FromBody] TaxiTrip sample)
        {

            MLContext mlContext = new MLContext(seed: 0);
            TaxiFarePredictionService taxiFarePrediction = new TaxiFarePredictionService(mlContext);
            ITransformer model;

            if (!taxiFarePrediction.isModelLoaded)
            {
                model = taxiFarePrediction.Train(mlContext);

                taxiFarePrediction.Evaluate(mlContext, model);
            }
            else
            {
                model = taxiFarePrediction.LoadModel(mlContext);
            }
            
            var predictionResult = await taxiFarePrediction.SinglePrediction(mlContext, model, sample);

            return Ok(string.Format("${0:0.00}", predictionResult.FareAmount));
        }


        [HttpPost("PredictTaxiFareOnnx", Name = "PredictTaxiFareOnnx")]
        public async Task<IActionResult> PredictTaxiFareWithOnnx([FromBody] IEnumerable<TaxiTrip> samples)
        {

            MLContext mlContext = new MLContext(seed: 0);
            TaxiFarePredictionService taxiFarePrediction = new TaxiFarePredictionService(mlContext);
            ITransformer model;
            IEnumerable<TaxiTripFarePrediction> result = [];
           
            model = taxiFarePrediction.TrainOnnxModel(mlContext);

            taxiFarePrediction.Evaluate(mlContext, model);
            foreach(TaxiTrip sample in samples)
            {
                var predictionResult = await taxiFarePrediction.SinglePrediction(mlContext, model, sample);
                result.Append(predictionResult);
            }



            return Ok(result);
        }



    }


    


}
