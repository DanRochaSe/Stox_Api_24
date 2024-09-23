using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.ML;
using Microsoft.ML.Transforms.Onnx;
using webapi.Models.PredictModels;


namespace webapi.Services.PredictionServices
{

    
    public class TaxiFarePredictionService
    {
        string _trainDataPath = Path.Combine(Environment.CurrentDirectory, "Assets", "taxi-fare-train.csv");
        string _testDataPath = Path.Combine(Environment.CurrentDirectory, "Assets", "taxi-fare-test.csv");
        //string _modelPath = Path.Combine(Environment.CurrentDirectory, "Assets", "AzureML_Model.zip");
        string _modelPath = Path.Combine(Environment.CurrentDirectory, "Assets", "Model.zip");
        string _onnxModelPath = Path.Combine(Environment.CurrentDirectory, "Assets", "Onnx_Model.onnx");

        public bool isModelLoaded { get; set; }

        MLContext mlContext;
        public TaxiFarePredictionService(MLContext _mlContext)
        {
            mlContext = _mlContext;
            isModelLoaded = File.Exists(_modelPath);
        }

        public ITransformer LoadModel(MLContext mlContext)
        {
            DataViewSchema modelSchema;
            ITransformer model = mlContext.Model.Load(_modelPath, out modelSchema);

            return model;
        }
        public ITransformer Train(MLContext mlContext)
        {
            ITransformer model;

            //if (File.Exists(Path.Combine(Environment.CurrentDirectory, "Assets", "Model.zip")))
            //{
            //    DataViewSchema modelSchema;
            //    model = mlContext.Model.Load(_modelPath, out modelSchema);
            //    isModelLoaded = true;
            //    return model;

            //}

            
            IDataView dataView = mlContext.Data.LoadFromTextFile<TaxiTrip>(_trainDataPath, hasHeader: true, separatorChar: ',');
            var pipeline = mlContext.Transforms.CopyColumns(outputColumnName: "Label", inputColumnName: nameof(TaxiTrip.FareAmount))
                .Append(mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "VendorIdEncoded", inputColumnName: "VendorId"))
                .Append(mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "RateCodeEncoded", inputColumnName: "RateCode"))
                .Append(mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "PaymentTypeEncoded", inputColumnName: "PaymentType"))
                .Append(mlContext.Transforms.Concatenate("Features", "VendorIdEncoded", "RateCodeEncoded", "PassengerCount", "TripDistance", "PaymentTypeEncoded"))
                .Append(mlContext.Regression.Trainers.FastTree());

            model =  pipeline.Fit(dataView);

            //Save .Zip model 
            mlContext.Model.Save(model, dataView.Schema, Path.Combine(Environment.CurrentDirectory, "Assets", "Model.zip"));

          

            return model;

        } 

        public void Evaluate(MLContext mlContext, ITransformer model)
        {
            IDataView dataView = mlContext.Data.LoadFromTextFile<TaxiTrip>(_testDataPath, hasHeader: true, separatorChar: ',');
            var predictions = model.Transform(dataView);
            var metrics = mlContext.Regression.Evaluate(predictions, labelColumnName: "Label", scoreColumnName: "Score");
            Console.WriteLine();
            Console.WriteLine($"*************************************************");
            Console.WriteLine($"*       Model quality metrics evaluation         ");
            Console.WriteLine($"*------------------------------------------------");
            Console.WriteLine($"*       RSquared Score:      {metrics.RSquared:0.##}");
            Console.WriteLine($"*       Root Mean Squared Error:      {metrics.RootMeanSquaredError:#.##}");

        }


        public async Task<TaxiTripFarePrediction> SinglePrediction(MLContext mlContext, ITransformer model, TaxiTrip sample)
        {

            var predictionFunction = mlContext.Model.CreatePredictionEngine<TaxiTrip, TaxiTripFarePrediction>(model);              

            var prediction = predictionFunction.Predict(sample);

            Console.WriteLine($"**********************************************************************");
            Console.WriteLine($"Predicted fare: {prediction.FareAmount:$0.##}");
            Console.WriteLine($"**********************************************************************");


            return prediction;


        }


        public ITransformer TrainOnnxModel(MLContext mlContext)
        {
            ITransformer model;


            OnnxScoringEstimator estimator = mlContext.Transforms.ApplyOnnxModel(_onnxModelPath);
            IDataView dataView = mlContext.Data.LoadFromTextFile<TaxiTrip>(_trainDataPath, hasHeader: true, separatorChar: ',');

            model = estimator.Fit(dataView);

            //Save .ONNX model
            using FileStream stream = File.Create(Path.Combine(Environment.CurrentDirectory, "Assets", "Onnx_Model.onnx"));
            mlContext.Model.ConvertToOnnx(model, dataView, stream);

            return model;

        }




    }
}
