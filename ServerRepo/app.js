'use strict';
//config dosyasindan modulde abstract class'i implement eden dosya adi alinir
//absRep implement eden modül sınıfı, absRepos dosyasini require yapar ve absRep abstract sinifini implement eder
//var config = require("./configs/import-config");

var debug = require('debug');
var express = require('express');
var path = require('path');
var logger = require('morgan');
var bodyParser = require('body-parser');
var webapi = require('./etkilesim/web');
var app = express();
var conn_str = "mongodb+srv://merlingo:MNN123gjj.@working.mr5cg.mongodb.net/VulnerabilityMapping?retryWrites=true&w=majority"  //CONFIG DATA

//var absRep = require("./AbsRepos");

app.use(bodyParser.json({ limit: '5mb' }));
app.use(bodyParser.urlencoded({ limit: '5mb', extended: true }));
// uncomment after placing your favicon in /public
//app.use(favicon(__dirname + '/public/favicon.ico'));
app.use(logger('dev'));
app.use(bodyParser.json());
app.use(bodyParser.urlencoded({ extended: false }));
//absRep.connectDB(conn_str);
var mongoose = require("mongoose");
//var connection_string = ""; //'mongodb://localhost:27017/myapp'
mongoose.connect(conn_str, { useNewUrlParser: true });

app.use('/', webapi);
// catch 404 and forward to error handler
app.use(function (req, res, next) {
    var err = new Error('Not Found');
    err.status = 404;
    next(err);
});

// error handlers

// development error handler
// will print stacktrace
if (app.get('env') === 'development') {
    app.use(function (err, req, res, next) {
        res.status(err.status || 500);
        res.send( {
            message: err.message,
            error: err
        });
    });
}

// production error handler
// no stacktraces leaked to user
app.use(function (err, req, res, next) {
    res.status(err.status || 500);
    res.json( {
        message: err.message,
        error: {}
    });
});

app.set('port', process.env.PORT || 5000);

var server = app.listen(app.get('port'), function () {
    debug('Express server listening on port ' + server.address().port);
});
