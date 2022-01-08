const http = require('http');
const https = require('https');

/**
 * getJSON:  RESTful GET request returning JSON object(s)
 * @param options: http options object
 * @param callback: callback to pass the results JSON object(s) back
 */

module.exports.getJSON = (options, onResult) => {
    //console.log('rest::getJSON');
    const port = options.port == 443 ? https : http;

    let output = '';
    //console.log(options);
    const req = port.request(options, (res) => {
        //console.log('response geldi');
        //console.log(`${options.host} : ${res.statusCode}`);
        res.setEncoding('utf8');

        res.on('data', (chunk) => {
            output += chunk;
        });

        res.on('end', () => {
            let obj = JSON.parse(output);
            //console.dir(obj);
            onResult(res.statusCode, obj);
        });
    });

    req.on('error', (err) => {
        console.dir(err.message);
        onResult(res.statusCode, err);
    });

    req.end();
};