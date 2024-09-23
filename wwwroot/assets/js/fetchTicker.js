import * as url from './url'
async function getDataAsync() {
    try {

        // const news =  fetch(url.NEWS_API);
        const table = fetch(url.trendURL, url.headers);
        // const trending =  fetch(url.trendURL, url.headers);
        //const spChart = fetch(url.urlCharts, url.headers);
        //const nasdaqCharts = fetch(url.nasdaqUrl, url.headers);
        //const dowCharts = fetch(url.dowUrl, url.headers);
        //const movers = fetch(url.urlMovers, url.headers);
        const results = Promise.all([table]).then(async ([table]) => {
            const tableJson = await table.json();
            // const trendingJson = await trending.json();
           
            return [tableJson];
        }).then(dataApi => {
            // console.log(dataApi);
            // localStorage.setItem('slider', JSON.stringify(dataApi[1].finance.result[0].quotes));
            // localStorage.setItem('news', JSON.stringify(dataApi[0].articles));
            localStorage.setItem('table', JSON.stringify(dataApi[0].finance.result[0].quotes));
            loadData();
        })


    } catch (err) {
        console.log(err);
    }
   
    
}


let loadData = () => {

    let tableLocal = JSON.parse(localStorage.getItem('table'));
    

    populateSlider(tableLocal);
    negativeNumber();
}


let populateSlider = (trending) => {


    const items = trending.map(trend => {

        let item = `<div class="ticker-item">
                        <div class="ticker-title mx-2" id="ticker-title">${trend.symbol.toUpperCase()}</div>
                        <div id="inner-display">
                            <div class="ticker-price mx-2">$${trend.regularMarketPrice.toFixed(2)}</div>
                            <div class="ticker-change mx-2" id="changePercentage">${trend.regularMarketChangePercent.toFixed(1)}<span>%</span></div>
                        </div>
                    </div>`


        return item;

    })


    document.getElementById('sliderDisplay').innerHTML = items.join('');

    let dailyChange = document.getElementsByClassName('ticker-change');

    sliderNegativeChanges(dailyChange);

}

let sliderNegativeChanges = (slider) => {

    for (let i = 0; i < slider.length; i++) {
        
            //    console.log(Number(dailyChange[i].innerText.replace('%','')));
            if (Number(slider[i].innerText.replace('%', '')) < 0) {
                slider[i].style.color = "red";
            }
        
    }
}

async function negativeNumber() {

    let array = [];

    let table = document.querySelectorAll(`#tableBody > tr`);

    for (let i = 1; i <= table.length; i++) {
        let row = document.querySelectorAll(`#tableBody > tr:nth-child(${i})`);
        array.push(row[0]);
    }


    // console.log(array);
    for (let j = 0; j <= array.length - 1; j++) {
        let percentage = array[j].cells[4];
        let change = array[j].cells[5];
        let previousClose = array[j].cells[2];
        let dayClose = array[j].cells[3];
        // console.log(percentage.innerText);
        if (Number(percentage.innerText).toFixed(2) < 0) {
            percentage.style.color = "red";
            change.style.color = "red";

            // console.log(percentage.innerText);
        } else if (Number(percentage.innerText).toFixed(2) > 0) {
            percentage.style.color = "green";
            change.style.color = "green";

        }

        if (Number(dayClose.innerText) < Number(previousClose.innerText)) {
            dayClose.style.color = "red"
        } else if (Number(dayClose.innerText) > Number(previousClose.innerText)) {
            dayClose.style.color = "green"
        }
    }
} 


export {
    getDataAsync
}