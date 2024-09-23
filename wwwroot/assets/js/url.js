//======================YAHOO FINANCE API  ===================================//


const API_KEY = "69cdd731d7mshb69784c2a6313b4p1c5444jsnb4f2c4a294ac";
const API_KEY_2 = "8f6042bc97msha003fe69e6bb4f4p105aabjsna64480f53e4c";


//FOR PRESENTATION - EXAMINERS
const API_KEY_3 = "61c602f9f8msha34f143fa2b1bb7p1315ccjsnee07f0f6c596";


//trending tickers
const trendURL = `https://apidojo-yahoo-finance-v1.p.rapidapi.com/market/get-trending-tickers?region=US`;

//summary 
const url = `https://apidojo-yahoo-finance-v1.p.rapidapi.com/market/v2/get-summary?region=US"`;

//news 
const urlNews = {
    method: 'GET',
    url: 'https://apidojo-yahoo-finance-v1.p.rapidapi.com/news/v2/get-details',
    params: { uuid: '9803606d-a324-3864-83a8-2bd621e6ccbd', region: 'US' },
    headers: {
        'x-rapidapi-key': `${API_KEY_2}`,
        'x-rapidapi-host': 'apidojo-yahoo-finance-v1.p.rapidapi.com'
    }
};


//charts
const urlCharts = "https://apidojo-yahoo-finance-v1.p.rapidapi.com/stock/v2/get-chart?interval=1d&symbol=%5EGSPC&range=1mo&region=US";
const nasdaqUrl = "https://apidojo-yahoo-finance-v1.p.rapidapi.com/stock/v2/get-chart?interval=1d&symbol=%5EIXIC&range=1mo&region=US";
const dowUrl = "https://apidojo-yahoo-finance-v1.p.rapidapi.com/stock/v2/get-chart?interval=1d&symbol=%5EDJI&range=1mo&region=US";

const urlMovers = "https://apidojo-yahoo-finance-v1.p.rapidapi.com/market/v2/get-movers?region=US&lang=en-US&start=0";



//generic header
const headers = {
    "method": "GET",
    "headers": {
        "x-rapidapi-key": `${API_KEY_2}`,
        "x-rapidapi-host": "apidojo-yahoo-finance-v1.p.rapidapi.com"
    }
};


//post Header
//generic header
const headersPost = {
    "method": "POST"
};




//======================MARKETSTACK ===================================//

const marketURL = `https://api.marketstack.com/v1/eod?access_key=8ab519ff412561125ca0729e24df2b3c&symbols=MSFT`;


//===================NEWS API ======================================//


const NEWS_API = `https://newsapi.org/v2/top-headlines?sources=business-insider&apiKey=3d286b585e694be39fcdfd24d4856f2e`;




//===================ALPHA ADVANTAGE API ======================================//



const ALPHA_API_KEY = 'W3975TFRO13G7RVG';

const ALPHA_DAILY_URL = `https://www.alphavantage.co/query?function=TIME_SERIES_DAILY&symbol=MSFT&apikey=${ALPHA_API_KEY}`;




export {

    API_KEY,
    API_KEY_2,
    API_KEY_3,
    trendURL,
    url,
    urlNews,
    urlCharts,
    nasdaqUrl,
    dowUrl,
    headers,
    marketURL,
    NEWS_API,
    ALPHA_API_KEY,
    ALPHA_DAILY_URL,
    urlMovers,
    headersPost
}
