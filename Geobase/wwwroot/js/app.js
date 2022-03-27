'use strict';

const ipLocationUrl = 'ip/location';
const cityLocationsUrl = 'city/locations';

(function() {
    function init() {
        var router = new Router([
            new Route('cityLocations', 'cityLocations.html', true),
            new Route('ipLocation', 'ipLocation.html')
        ]);
    }

    init();
}());

function getIpLocation() {
    var ipAddressTextBox = document.getElementById('ipAddressInput');
    var ipAddress = ipAddressTextBox.value.trim();

    if (!ipAddress) {
        displayInputError('Please provide a value to find');
        return;
    }
    if (!validateIPaddress(ipAddress)) {
        displayInputError('Invalid ip address format');
        return;
	}

    var requestUrl = ipLocationUrl + "?ip=" + ipAddress;

    fetch(requestUrl).then(r => processResponse(r));
}

function getCityLocations() {
    var cityNameTextBox = document.getElementById('cityNameInput');
    var cityName = cityNameTextBox.value.trim();

    if (!cityName) {
        displayInputError('Please provide a value to find');
        return;
	}

    var requestUrl = cityLocationsUrl + "?city=" + cityName;

    fetch(requestUrl).then(r => processResponse(r));        
}

function processResponse(response) {
    console.log(response.status);

    if (response.status == 200) {
        response.json().then(data => displayResult(data));
    }
    else if (response.status == 400) {
        response.text().then(text => displayBadRequest(text));
    }
    else if (response.status == 404) {
        displayNotFound();
    }
    else if (response.status == 500) {
        response.text().then(text => displayOops(text));
    }
}

function validateIPaddress(ipaddress) {
    if (/^(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$/.test(ipaddress)) {
        return (true)
    }
    
    return (false)
}

function displayInputError(errorText) {
    var tBody = document.getElementById('results');
    tBody.innerHTML = '<p class="errorMessage">' + errorText + '</p>';
}

function displayBadRequest(responseText) {
    var tBody = document.getElementById('results');
    tBody.innerHTML = '<p class="errorMessage"> Bad request: ' + responseText + ' </p>';
}

function displayNotFound() {
    var tBody = document.getElementById('results');
    tBody.innerHTML = '<p class="errorMessage"> Results not found </p>';
}

function displayOops(responseText) {   
    console.log(responseText);

    var tBody = document.getElementById('results');
    tBody.innerHTML = '<p class="errorMessage"> Oops! Something went wrong :\'( </p>';
}

function displayResult(results) {
    var tBody = document.getElementById('results');
    tBody.innerHTML = '';

    var resultsCollection = [].concat(results);
    tBody.innerHTML = drawTable(resultsCollection);
}

function drawTable(locations) {
    var html = [];
    html.push("<table border=\"1\">");

    html.push("<tr>");
    html.push("<th scope=\"col\">Country</th>");
    html.push("<th>Region</th>");
    html.push("<th>Postal</th>");
    html.push("<th>City</th>");
    html.push("<th>Organization</th>");
    html.push("<th>Coordinates</th>");
    html.push("<tr>");

    locations.forEach((location) => {
        html.push("<tr>");
        html.push("<td scope=\"row\">" + location.country + "</td>")
        html.push("<td>" + location.region + "</td>")
        html.push("<td>" + location.postal + "</td>")
        html.push("<td>" + location.city + "</td>")
        html.push("<td>" + location.organization + "</td>")
        html.push("<td>" + location.coordinates.latitude + "/" + location.coordinates.longitude + "</td>")
        html.push("</tr>");
    });

    html.push("</table>");

    return html.join("");
}