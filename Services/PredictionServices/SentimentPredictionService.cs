using Microsoft.ML;
using static Microsoft.ML.DataOperationsCatalog;
using webapi.Models.PredictModels;
using Microsoft.ML.Data;

namespace webapi.Services.PredictionServices
{
    public class SentimentPredictionService
    {
        
        string _dataPath = Path.Combine(Environment.CurrentDirectory, "Assets", "yelp_labelled.txt");
        string _modelPath = Path.Combine(Environment.CurrentDirectory, "Assets", "yelp_Model.zip");
        MLContext mlContext;
        public bool isModelLoaded { get; set; } 

        public SentimentPredictionService(MLContext _mlContext)
        {
            mlContext = _mlContext;
            isModelLoaded = File.Exists(_modelPath);

        }

        public TrainTestData LoadData(MLContext mlContext)
        {

            IDataView dataView = mlContext.Data.LoadFromTextFile<SentimentData>(_dataPath, hasHeader: true);
            TrainTestData splitDataView = mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2);

            return splitDataView;
        }

        public ITransformer LoadModel(MLContext mLContext)
        {
            DataViewSchema modelSchema;
            ITransformer model = mlContext.Model.Load(_modelPath, out modelSchema);
            return model;
            
        }

        public ITransformer BuildAndTrainModel(MLContext mlContext, IDataView splitTrainSet)
        {

            var estimator = mlContext.Transforms.Text.FeaturizeText(outputColumnName: "Features", inputColumnName: nameof(SentimentData.SentimentText))
                .Append(mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(labelColumnName: "Label", featureColumnName: "Features"));
            ITransformer model = estimator.Fit(splitTrainSet);

            mlContext.Model.Save(model, splitTrainSet.Schema, Path.Combine(Environment.CurrentDirectory, "Assets", "Model.zip"));

            return model;
        }

        public void Evaluate(MLContext mLContext, ITransformer model, IDataView splitTestSet)
        {
            IDataView predictions = model.Transform(splitTestSet);
            CalibratedBinaryClassificationMetrics metrics = mLContext.BinaryClassification.Evaluate(predictions, "Label");
            Console.WriteLine();
            Console.WriteLine("Model quality metrics evaluation");
            Console.WriteLine("--------------------------------");
            Console.WriteLine($"Accuracy: {metrics.Accuracy:P2}");
            Console.WriteLine($"Auc: {metrics.AreaUnderRocCurve:P2}");
            Console.WriteLine($"F1Score: {metrics.F1Score:P2}");
            Console.WriteLine("=============== End of model evaluation ===============");
        }

        public SentimentPrediction UseModelWithSingleItem(MLContext mLContext, ITransformer model, SentimentData sample)
        {
            PredictionEngine<SentimentData, SentimentPrediction> predictionFunction = mLContext.Model.CreatePredictionEngine<SentimentData, SentimentPrediction>(model);
            //SentimentData sampleStatement = new SentimentData { SentimentText = "This was a very bad steak" };
            var resultPrediction = predictionFunction.Predict(sample);
            Console.WriteLine();
            Console.WriteLine("=============== Prediction Test of model with a single sample and test dataset ===============");

            Console.WriteLine();
            Console.WriteLine($"Sentiment: {resultPrediction.SentimentText} | Prediction: {(Convert.ToBoolean(resultPrediction.Prediction) ? "Positive" : "Negative")} | Probability: {resultPrediction.Probability} ");

            Console.WriteLine("=============== End of Predictions ===============");
            Console.WriteLine();

            return resultPrediction;
        }

        public IEnumerable<SentimentPrediction> UseModelWithBatchItems(MLContext mLContext, ITransformer model, IEnumerable<SentimentData> sample)
        {
            //IEnumerable<SentimentData> sentiments = new[]
            //{
            //        new SentimentData
            //        {
            //            SentimentText = "This was a horrible meal"
            //        },
            //        new SentimentData
            //        {
            //            SentimentText = "I love this spaghetti"
            //        }
            //    };

            IDataView batchComments = mLContext.Data.LoadFromEnumerable(sample);

            IDataView predictions = model.Transform(batchComments);

            //Use the model to predict sentiment
            IEnumerable<SentimentPrediction> predictedResults = mLContext.Data.CreateEnumerable<SentimentPrediction>(predictions, reuseRowObject: false);

            Console.WriteLine();

            Console.WriteLine("=============== Prediction Test of loaded model with multiple samples ===============");

            foreach (SentimentPrediction prediction in predictedResults)
            {
                Console.WriteLine($"Sentiment: {prediction.SentimentText} | Prediction: {(Convert.ToBoolean(prediction.Prediction) ? "Positive" : "Negative")} | Probability: {prediction.Probability} ");
            }
            Console.WriteLine("=============== End of predictions ===============");

            return predictedResults;
        }

    }
}
